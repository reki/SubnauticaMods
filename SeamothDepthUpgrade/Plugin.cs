// File: Plugin.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets; // SetRecipe / SetEquipment
using Nautilus.Assets.PrefabTemplates; // CloneTemplate
using Nautilus.Crafting;
using Nautilus.Handlers; // CraftTreeHandler / KnownTechHandler / CraftDataHandler / SpriteHandler
using Nautilus.Utility; // SpriteManager
using UnityEngine;

namespace AshFox.Subnautica
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("com.snmodding.nautilus")] // Nautilus
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "jp.ashfox.seamoth.depthmodule";
        public const string PluginName = "SeamothDepthModule";
        public const string PluginVersion = "1.0.0";

        internal static ManualLogSource Log;

        public static TechType Mk4;
        public static TechType Mk5;

        public const int DepthMk4 = 1300;
        public const int DepthMk5 = 1700;

        private void Awake()
        {
            Log = Logger;

            // ローカライゼーションを初期化
            LocalizationManager.Initialize();

            RegisterMk4();
            RegisterMk5();

            // var harmony = new Harmony(PluginGuid);
            // harmony.PatchAll();                 // Vehicle.Update / LateUpdate の Postfix
            // TryPatchVehicleDepthAPIs(harmony);  // UIが呼ぶVehicle系の深度APIへPostfix
        }

        // ====== アイテム登録（Workbench ルート、MK3アイコン流用） ======
        private static void RegisterMk4()
        {
            var info = PrefabInfo.WithTechType(
                classId: "SeamothDepthModule4",
                displayName: LocalizationManager.GetLocalizedString(
                    "SeamothDepthModule4.DisplayName"
                ),
                description: LocalizationManager.GetLocalizedString(
                    "SeamothDepthModule4.Description"
                )
            );
            var prefab = new CustomPrefab(info);

            // 見た目は MK3 をクローン
            prefab.SetGameObject(new CloneTemplate(info, TechType.VehicleHullModule3));

            // 装備種別（シーモス用）
            prefab.SetEquipment(EquipmentType.SeamothModule);

            // 深度モジュールとして機能させる
            prefab
                .SetVehicleUpgradeModule(EquipmentType.SeamothModule, QuickSlotType.Passive)
                .WithDepthUpgrade(1100f);

            // レシピ（Workbench ルート）
            var recipe = new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>
                {
                    new Ingredient(TechType.VehicleHullModule3, 1),
                    new Ingredient(TechType.Nickel, 2),
                    new Ingredient(TechType.PlasteelIngot, 1),
                },
            };
            prefab.SetRecipe(recipe).WithFabricatorType(CraftTree.Type.Workbench);

            // PDAグループ/カテゴリを設定（設計図タブに表示するため）
            CraftDataHandler.AddToGroup(TechGroup.Workbench, TechCategory.Workbench, info.TechType);

            // 設計図表示：VehicleHullModule3をスキャンしてMk4をアンロック
            KnownTechHandler.SetAnalysisTechEntry(
                TechType.VehicleHullModule3,
                new TechType[] { info.TechType }
            );
            // デバッグ用：ゲーム開始時に既知扱い（表示確認用）
            KnownTechHandler.UnlockOnStart(info.TechType);
            CraftDataHandler.SetQuickSlotType(info.TechType, QuickSlotType.Passive);

            // Workbench ルートに確実に追加
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Workbench, info.TechType, "Workbench");

            // MK3 と同じアイコンを流用
            var mk3Icon = SpriteManager.Get(TechType.VehicleHullModule3);
            if (mk3Icon != null)
                SpriteHandler.RegisterSprite(info.TechType, mk3Icon);

            prefab.Register();
            Mk4 = info.TechType;
        }

        private static void RegisterMk5()
        {
            var info = PrefabInfo.WithTechType(
                classId: "SeamothDepthModule5",
                displayName: LocalizationManager.GetLocalizedString(
                    "SeamothDepthModule5.DisplayName"
                ),
                description: LocalizationManager.GetLocalizedString(
                    "SeamothDepthModule5.Description"
                )
            );
            var prefab = new CustomPrefab(info);

            prefab.SetGameObject(new CloneTemplate(info, TechType.VehicleHullModule3));
            prefab.SetEquipment(EquipmentType.SeamothModule);

            prefab
                .SetVehicleUpgradeModule(EquipmentType.SeamothModule, QuickSlotType.Passive)
                .WithDepthUpgrade(1500f);

            var recipe = new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>
                {
                    new Ingredient(Mk4, 1),
                    new Ingredient(TechType.Kyanite, 2),
                    new Ingredient(TechType.PlasteelIngot, 1),
                },
            };
            prefab.SetRecipe(recipe).WithFabricatorType(CraftTree.Type.Workbench);

            // PDAグループ/カテゴリを設定（設計図タブに表示するため）
            CraftDataHandler.AddToGroup(TechGroup.Workbench, TechCategory.Workbench, info.TechType);

            // 設計図表示：Mk4をスキャンしてMk5をアンロック
            KnownTechHandler.SetAnalysisTechEntry(Mk4, new TechType[] { info.TechType });
            // デバッグ用：ゲーム開始時に既知扱い（表示確認用）
            KnownTechHandler.UnlockOnStart(info.TechType);
            CraftDataHandler.SetQuickSlotType(info.TechType, QuickSlotType.Passive);
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Workbench, info.TechType, "Workbench");

            var mk3Icon = SpriteManager.Get(TechType.VehicleHullModule3);
            if (mk3Icon != null)
                SpriteHandler.RegisterSprite(info.TechType, mk3Icon);

            prefab.Register();
            Mk5 = info.TechType;
        }
    }
}
