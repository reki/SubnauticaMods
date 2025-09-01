// File: Plugin.cs
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets; // SetRecipe / SetEquipment
using Nautilus.Assets.PrefabTemplates; // CloneTemplate
using Nautilus.Crafting;
using Nautilus.Handlers; // CraftTreeHandler / KnownTechHandler / SpriteHandler
using Nautilus.Utility; // SpriteManager
using UnityEngine;

namespace AshFox.Subnautica
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("com.snmodding.nautilus")] // Nautilus
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "jp.ashfox.uraninitepowercell";
        public const string PluginName = "UraninitePowerCell";
        public const string PluginVersion = "1.0.0";

        internal static ManualLogSource Log;
        internal static TechType UraniniteCellTechType;

        // バニラ Power Cell は 200、イオンは 1000。核燃料棒は閃ウラン鉱２つで20000。なので50倍にして10,000にしたい（参照: Subnautica Wiki）
        private const float BasePowerCellCapacity = 200f;
        private const float CapacityMultiplier = 50f;
        internal static float TargetCapacity => BasePowerCellCapacity * CapacityMultiplier; // = 100000

        private void Awake()
        {
            Log = Logger;

            // ローカライゼーションを初期化
            LocalizationManager.Initialize();

            RegisterItem();
            new Harmony(PluginGuid).PatchAll(); // 互換性パッチ（EnergyMixin / 充電器）
        }

        private static void RegisterItem()
        {
            var info = PrefabInfo.WithTechType(
                classId: "UraninitePowerCell",
                displayName: LocalizationManager.GetLocalizedString(
                    "UraninitePowerCell.DisplayName"
                ),
                description: LocalizationManager.GetLocalizedString(
                    "UraninitePowerCell.Description",
                    new Dictionary<string, string>
                    {
                        { "CapacityMultiplier", CapacityMultiplier.ToString() },
                    }
                )
            );

            var prefab = new CustomPrefab(info);

            // Power Cell をクローン
            var clone = new CloneTemplate(info, TechType.PowerCell);
            prefab.SetGameObject(clone);

            // レシピ：PowerCell + 閃ウラン鉱 + リチウム + シリコンゴム
            var recipe = new RecipeData
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>
                {
                    new Ingredient(TechType.PowerCell, 2),
                    new Ingredient(TechType.UraniniteCrystal, 1),
                    new Ingredient(TechType.Lithium, 1),
                    new Ingredient(TechType.Silicone, 1),
                },
            };
            // WithFabricatorTypeを削除して、手動でタブ指定のみ使用
            prefab.SetRecipe(recipe);

            // PDAグループ/カテゴリを設定（設計図タブに表示するため）
            CraftDataHandler.AddToGroup(
                TechGroup.Resources,
                TechCategory.Electronics,
                info.TechType
            );

            // 設計図を起動時アンロック（改造ステーションに表示するため）
            KnownTechHandler.UnlockOnStart(info.TechType);
            // 設計図表示：UraniniteCrystalをスキャンしてアンロック
            KnownTechHandler.SetAnalysisTechEntry(
                TechType.UraniniteCrystal,
                new TechType[] { info.TechType }
            );
            var pcIcon = SpriteManager.Get(TechType.PowerCell);
            if (pcIcon != null)
                SpriteHandler.RegisterSprite(info.TechType, pcIcon);

            // 登録
            prefab.Register();
            UraniniteCellTechType = info.TechType;

            // 表示タブ（基本素材 → 電子部品）のみに配置
            CraftTreeHandler.AddCraftingNode(
                CraftTree.Type.Fabricator,
                info.TechType,
                "Resources",
                "Electronics"
            );

            // パワーセル充電器での互換性を設定（重要！）
            CraftDataHandler.SetEquipmentType(info.TechType, EquipmentType.PowerCellCharger);
        }
    }

    // ====== 容量設定パッチ ======
    // Pickupableのパッチで容量を設定
    [HarmonyPatch(typeof(Pickupable))]
    internal static class Pickupable_Awake_Patch
    {
        [HarmonyPostfix, HarmonyPatch("Awake")]
        private static void Postfix(Pickupable __instance)
        {
            if (__instance == null)
                return;
            if (__instance.GetTechType() != Plugin.UraniniteCellTechType)
                return;

            var battery = __instance.GetComponent<Battery>();
            if (battery == null)
                return;

            battery._capacity = Plugin.TargetCapacity;
            battery._charge = Plugin.TargetCapacity; // フル充電状態で開始

            // 充電可能フラグを明示（存在する場合）
            var fRechargeable =
                AccessTools.Field(typeof(Battery), "rechargeable")
                ?? AccessTools.Field(typeof(Battery), "_rechargeable");
            if (fRechargeable != null)
            {
                fRechargeable.SetValue(battery, true);
            }
        }
    }

    // ====== 互換性パッチ ======
    // PowerCell を受け入れる機器（EnergyMixin）で新セルも受け入れる
    [HarmonyPatch(typeof(EnergyMixin))]
    internal static class EnergyMixin_Start_Patch
    {
        [HarmonyPostfix, HarmonyPatch("Start")]
        private static void Postfix(EnergyMixin __instance)
        {
            if (__instance == null)
                return;

            var fields = new[]
            {
                "compatibleBatteries",
                "batteryTechTypes",
                "compatibleBatteryTypes",
                "compatibleTech",
            };

            foreach (var name in fields)
            {
                var f = AccessTools.Field(__instance.GetType(), name);
                if (f == null)
                    continue;

                var val = f.GetValue(__instance);
                if (val is List<TechType> list)
                {
                    if (
                        (
                            list.Contains(TechType.PowerCell)
                            || list.Contains(TechType.PrecursorIonPowerCell)
                        ) && !list.Contains(Plugin.UraniniteCellTechType)
                    )
                    {
                        list.Add(Plugin.UraniniteCellTechType);
                    }
                    continue; // returnではなくcontinue
                }
                if (val is TechType[] arr)
                {
                    var set = new HashSet<TechType>(arr);
                    if (
                        (
                            set.Contains(TechType.PowerCell)
                            || set.Contains(TechType.PrecursorIonPowerCell)
                        ) && set.Add(Plugin.UraniniteCellTechType)
                    )
                    {
                        f.SetValue(__instance, set.ToArray());
                    }
                    continue; // returnではなくcontinue
                }
            }
        }
    }

    // 共通Util
    internal static class ChargerCompatUtil
    {
        private static object _originalIsAllowedToAdd = null;

        internal static void TryAllow(Charger charger, TechType tt)
        {
            if (charger == null)
                return;

            // Equipment の isAllowedToAdd デリゲートをパッチ
            var equipmentField = AccessTools.Field(typeof(Charger), "equipment");
            if (equipmentField != null)
            {
                var equipment = equipmentField.GetValue(charger);
                if (equipment != null)
                {
                    // isAllowedToAdd デリゲートを取得
                    var isAllowedToAddField = AccessTools.Field(
                        equipment.GetType(),
                        "isAllowedToAdd"
                    );
                    if (isAllowedToAddField != null)
                    {
                        var originalDelegate = isAllowedToAddField.GetValue(equipment);

                        // IsAllowedToAdd の正しい型を取得
                        var delegateType = isAllowedToAddField.FieldType;

                        // デリゲートのInvokeメソッドを調べてシグネチャを確認
                        var invokeMethod = delegateType.GetMethod("Invoke");
                        if (invokeMethod != null)
                        {
                            var parameters = invokeMethod.GetParameters();

                            // 引数の数に応じて適切なメソッドを選択
                            string methodName = parameters.Length switch
                            {
                                1 => "CustomIsAllowedToAdd1",
                                2 => "CustomIsAllowedToAdd2",
                                3 => "CustomIsAllowedToAdd3",
                                _ => null,
                            };

                            if (methodName != null)
                            {
                                var customMethod = typeof(ChargerCompatUtil).GetMethod(
                                    methodName,
                                    System.Reflection.BindingFlags.Static
                                        | System.Reflection.BindingFlags.NonPublic
                                );

                                if (customMethod != null)
                                {
                                    var customDelegate = System.Delegate.CreateDelegate(
                                        delegateType,
                                        customMethod
                                    );

                                    // 元のデリゲートを保存
                                    _originalIsAllowedToAdd = originalDelegate;

                                    // カスタムデリゲートを設定
                                    isAllowedToAddField.SetValue(equipment, customDelegate);
                                }
                            }
                        }
                    }
                    else
                    {
                        Plugin.Log.LogWarning("Could not find isAllowedToAdd field in Equipment");
                    }
                }
            }
        }

        // 1フレーム遅延で再適用（Startで上書きされたケースを拾う）
        internal static System.Collections.IEnumerator LateAllow(Charger charger, TechType tt)
        {
            yield return null;
            TryAllow(charger, tt);
        }

        // デバッグ用：許可リストの中身を確認
        internal static void DumpAllowed(Charger charger)
        {
            var f =
                AccessTools.Field(typeof(Charger), "allowedTechTypes")
                ?? AccessTools.Field(typeof(Charger), "allowedTech");
            if (f == null)
            {
                return;
            }

            var val = f.GetValue(charger);
            System.Collections.Generic.IEnumerable<TechType> seq = null;
            if (val is List<TechType> list)
                seq = list;
            else if (val is TechType[] arr)
                seq = arr;
            if (seq == null){
                Plugin.Log.LogWarning("Charger allowedTech* type unexpected: " + val?.GetType());
            }
        }

        // カスタム isAllowedToAdd メソッド（2引数版 - PowerCellChargerで使用）
        private static bool CustomIsAllowedToAdd2(Pickupable pickupable, bool verbose)
        {
            if (pickupable != null)
            {
                var techType = pickupable.GetTechType();

                // UraninitePowerCell の場合は許可
                if (techType == Plugin.UraniniteCellTechType)
                {
                    return true;
                }

                // 元のデリゲートがある場合は呼び出し
                if (_originalIsAllowedToAdd != null)
                {
                    try
                    {
                        var method = _originalIsAllowedToAdd.GetType().GetMethod("Invoke");
                        if (method != null)
                        {
                            var result = method.Invoke(
                                _originalIsAllowedToAdd,
                                new object[] { pickupable, verbose }
                            );
                            return (bool)result;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Plugin.Log.LogWarning(
                            $"Error calling original isAllowedToAdd: {ex.Message}"
                        );
                    }
                }

                // デフォルト: PowerCell系を許可
                return techType == TechType.PowerCell || techType == TechType.PrecursorIonPowerCell;
            }
            return false;
        }

        // 1引数版（念のため）
        private static bool CustomIsAllowedToAdd1(Pickupable pickupable)
        {
            return CustomIsAllowedToAdd2(pickupable, false);
        }

        // 3引数版（念のため）
        private static bool CustomIsAllowedToAdd3(Pickupable pickupable, bool verbose, object arg3)
        {
            return CustomIsAllowedToAdd2(pickupable, verbose);
        }
    }

    // PowerCellCharger への互換追加（定期的に全充電器をスキャンして追加）
    internal static class PowerCellChargerCompatibility
    {
        private static HashSet<int> patchedChargers = new HashSet<int>();

        // ゲーム開始後に定期的に実行
        internal static void EnsureCompatibility()
        {
            var chargers = UnityEngine.Object.FindObjectsOfType<PowerCellCharger>();
            if (chargers.Length > 0)
            {
                int newChargers = 0;
                foreach (var charger in chargers)
                {
                    int chargerId = charger.GetInstanceID();
                    if (!patchedChargers.Contains(chargerId))
                    {
                        ChargerCompatUtil.TryAllow(charger, Plugin.UraniniteCellTechType);
                        charger.StartCoroutine(
                            ChargerCompatUtil.LateAllow(charger, Plugin.UraniniteCellTechType)
                        );
                        patchedChargers.Add(chargerId);
                        newChargers++;
                    }
                }
            }
        }
    }

    // ゲームのメインループで定期的にチェック
    [HarmonyPatch(typeof(Player), "Update")]
    internal static class Player_Update_Patch
    {
        private static int frameCounter = 0;

        [HarmonyPostfix]
        private static void Postfix()
        {
            // 5秒ごとにチェック（継続的に新しい充電器を検出）
            if (frameCounter++ % 300 == 0)
            {
                PowerCellChargerCompatibility.EnsureCompatibility();
            }
        }
    }
}
