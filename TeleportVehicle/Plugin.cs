// File: Plugin.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets; // SetRecipe / SetEquipment
using Nautilus.Assets.PrefabTemplates; // CloneTemplate, IPrefabRequest
using Nautilus.Crafting;
using Nautilus.Handlers; // CraftTreeHandler / KnownTechHandler / SpriteHandler
using Nautilus.Utility; // SpriteManager
using UnityEngine;
using UWE;

namespace AshFox.Subnautica
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("com.snmodding.nautilus")] // Nautilus
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "jp.ashfox.teleportvehicle";
        public const string PluginName = "TeleportVehicle";
        public const string PluginVersion = "1.0.1";

        internal static ManualLogSource Log;
        internal static TechType VehicleTeleporterTechType;

        private void Awake()
        {
            Log = Logger;

            // ローカライゼーションを初期化
            LocalizationManager.Initialize();
            Commons.SetLogger(Log);
            RegisterVehicleTeleporter();
            new Harmony(PluginGuid).PatchAll();
        }

        private static void RegisterVehicleTeleporter()
        {
            var info = PrefabInfo.WithTechType(
                classId: "VehicleTeleporter",
                displayName: LocalizationManager.GetLocalizedString(
                    "VehicleTeleporter.DisplayName"
                ),
                description: LocalizationManager.GetLocalizedString("VehicleTeleporter.Description")
            );

            var prefab = new CustomPrefab(info);

            // Scannerのテクスチャとメッシュのみを参照して独自のPrefabを作成
            prefab.SetGameObject(GetVehicleTeleporterPrefabRequest);

            // レシピ：Scanner + イオンキューブ + ダイヤモンド + リチウム
            var recipe = new RecipeData
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>
                {
                    new Ingredient(TechType.Scanner, 1),
                    new Ingredient(TechType.Diamond, 2),
                    new Ingredient(TechType.Lithium, 2),
                    new Ingredient(TechType.PowerCell, 2),
                },
            };
            prefab.SetRecipe(recipe);

            // 装備スロットに設定
            prefab.SetEquipment(EquipmentType.Hand);

            // PDAグループ/カテゴリを設定
            CraftDataHandler.AddToGroup(TechGroup.Personal, TechCategory.Tools, info.TechType);

            // 設計図を起動時アンロック
            KnownTechHandler.UnlockOnStart(info.TechType);

            // Scannerのアイコンを流用
            var scannerIcon = SpriteManager.Get(TechType.Scanner);
            if (scannerIcon != null)
            {
                SpriteHandler.RegisterSprite(info.TechType, scannerIcon);
            }

            // 登録
            prefab.Register();
            VehicleTeleporterTechType = info.TechType;

            // ファブリケータの個人用ツールタブに配置
            CraftTreeHandler.AddCraftingNode(
                CraftTree.Type.Fabricator,
                info.TechType,
                "Personal",
                "Tools"
            );
        }

        // カスタムPrefabリクエスト関数
        private static IEnumerator GetVehicleTeleporterPrefabRequest(IOut<GameObject> result)
        {
            return GetVehicleTeleporterPrefab(result);
        }

        private static IEnumerator GetVehicleTeleporterPrefab(IOut<GameObject> result)
        {
            // Scannerのプレハブを取得してメッシュとテクスチャのみを参照
            var scannerTask = CraftData.GetPrefabForTechTypeAsync(TechType.Scanner, false);
            yield return scannerTask;

            var scannerPrefab = scannerTask.GetResult();
            if (scannerPrefab == null)
            {
                Plugin.Log.LogError("Failed to get Scanner prefab for VehicleTeleporter");
                result.Set(null);
                yield break;
            }

            // 新しいGameObjectを作成
            var teleporterPrefab = new GameObject("VehicleTeleporter");

            // Scannerからメッシュとマテリアルをコピー
            var scannerRenderer = scannerPrefab.GetComponentInChildren<MeshRenderer>();
            var scannerMeshFilter = scannerPrefab.GetComponentInChildren<MeshFilter>();

            if (scannerRenderer != null && scannerMeshFilter != null)
            {
                // MeshRendererとMeshFilterを追加
                var meshRenderer = teleporterPrefab.AddComponent<MeshRenderer>();
                var meshFilter = teleporterPrefab.AddComponent<MeshFilter>();

                // メッシュとマテリアルをコピー
                meshFilter.mesh = scannerMeshFilter.mesh;
                meshRenderer.materials = scannerRenderer.materials;
            }

            // Scannerからコライダーをコピー
            var scannerCollider = scannerPrefab.GetComponent<Collider>();
            if (scannerCollider != null)
            {
                var collider = teleporterPrefab.AddComponent<BoxCollider>();
                if (scannerCollider is BoxCollider scannerBoxCollider)
                {
                    collider.center = scannerBoxCollider.center;
                    collider.size = scannerBoxCollider.size;
                }
            }

            // 必要なコンポーネントのみを追加
            var pickupable = teleporterPrefab.AddComponent<Pickupable>();
            teleporterPrefab.AddComponent<PrefabIdentifier>().ClassId = "VehicleTeleporter";
            teleporterPrefab.AddComponent<TechTag>().type = VehicleTeleporterTechType;
            teleporterPrefab.AddComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity
                .CellLevel
                .Near;

            // カスタムPlayerToolコンポーネントを追加（右クリック動作制御のため）
            var playerTool = teleporterPrefab.AddComponent<VehicleTeleporterTool>();

            // カスタムアクションコンポーネントを追加（右クリック動作を制御）
            var customAction = teleporterPrefab.AddComponent<VehicleTeleporterAction>();

            // WorldForcesを追加（物理挙動のため）
            var worldForces = teleporterPrefab.AddComponent<WorldForces>();
            worldForces.handleGravity = true;
            worldForces.aboveWaterGravity = 9.81f;
            worldForces.underwaterGravity = 1.0f;
            worldForces.handleDrag = true;
            worldForces.aboveWaterDrag = 0.0f;
            worldForces.underwaterDrag = 10.0f;

            // Rigidbodyを追加
            var rigidbody = teleporterPrefab.AddComponent<Rigidbody>();
            rigidbody.mass = 1.0f;
            rigidbody.drag = 1.0f;
            rigidbody.angularDrag = 1.0f;
            result.Set(teleporterPrefab);
        }
    }

    // ====== カスタムPlayerToolクラス ======
    public class VehicleTeleporterTool : PlayerTool
    {
        public override bool OnRightHandDown()
        {
            return false; // ドロップを防ぐ
        }

        public override bool OnRightHandUp()
        {
            // 右クリックでのドロップを防ぐため、falseを返す
            return false;
        }

        public override bool OnLeftHandDown()
        {
            return false;
        }

        public override bool OnLeftHandUp()
        {
            // 左クリック処理完了
            return false;
        }

        private void Update()
        {
            if (
                (
                    GameInput.GetButtonDown(GameInput.Button.AltTool)
                    || GameInput.GetButtonDown(GameInput.Button.LeftHand)
                ) && !Commons.IsSystemUiOpen()
            )
            {
                VehicleTeleporterManager.TeleportSelectedVehicle();
            }
            if (GameInput.GetButtonDown(GameInput.Button.RightHand) && !Commons.IsSystemUiOpen())
            {
                VehicleTeleporterManager.SelectNextVehicle();
            }
        }
    }

    // ====== カスタムアクションコンポーネント ======
    public class VehicleTeleporterAction : MonoBehaviour, IHandTarget
    {
        public void OnHandHover(GUIHand hand)
        {
            // 右クリックでの車両選択のヒント表示
            HandReticle.main.SetText(
                HandReticle.TextType.Hand,
                LocalizationManager.GetLocalizedString(
                    "VehicleTeleporterMessages.RightClickToSelect"
                ),
                false,
                GameInput.Button.RightHand
            );

            // 左クリックでのテレポートのヒント表示
            HandReticle.main.SetText(
                HandReticle.TextType.HandSubscript,
                LocalizationManager.GetLocalizedString(
                    "VehicleTeleporterMessages.LeftClickToTeleport"
                ),
                false,
                GameInput.Button.LeftHand
            );
        }

        public void OnHandClick(GUIHand hand)
        {
            // 左クリック処理は既存のPlayer_Update_Patchで処理
        }
    }
}
