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
        internal static TechType UraniniteBatteryTechType;

        internal const float UraninitePowerCapacity = 10000f;
        internal const float BasePowerCellCapacity = 200f;
        internal const float BaseBatteryCapacity = 100f;

        private void Awake()
        {
            Log = Logger;

            // ローカライゼーションを初期化
            LocalizationManager.Initialize();
            Commons.SetLogger(Log);

            RegisterItems();
            new Harmony(PluginGuid).PatchAll(); // 互換性パッチ（EnergyMixin / 充電器）
        }

        private static void RegisterItems()
        {
            RegisterPowerCell();
            RegisterBattery();
        }

        private static void RegisterPowerCell()
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
                        {
                            "CapacityMultiplier",
                            (UraninitePowerCapacity / BasePowerCellCapacity).ToString()
                        },
                    }
                )
            );

            var prefab = new CustomPrefab(info);

            // Power Cell をクローン
            var clone = new CloneTemplate(info, TechType.PowerCell);
            prefab.SetGameObject(clone);

            // カスタムアイコンを設定
            var customIcon = Commons.LoadCustomIcon("uraninite_power_cell.png");
            if (customIcon != null)
            {
                Log.LogWarning("Custom icon loaded: " + customIcon.name);
                SpriteHandler.RegisterSprite(info.TechType, customIcon);
            }
            else
            {
                Log.LogWarning("Custom icon not loaded");
                var pcIcon = SpriteManager.Get(TechType.PowerCell);
                SpriteHandler.RegisterSprite(info.TechType, pcIcon);
            }

            // レシピ：PowerCell + 閃ウラン鉱 + リチウム + シリコンゴム
            var recipe = new RecipeData
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>
                {
                    new Ingredient(TechType.PowerCell, 1),
                    new Ingredient(TechType.UraniniteCrystal, 2),
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

        private static void RegisterBattery()
        {
            var info = PrefabInfo.WithTechType(
                classId: "UraniniteBattery",
                displayName: LocalizationManager.GetLocalizedString("UraniniteBattery.DisplayName"),
                description: LocalizationManager.GetLocalizedString(
                    "UraniniteBattery.Description",
                    new Dictionary<string, string>
                    {
                        {
                            "CapacityMultiplier",
                            (UraninitePowerCapacity / BaseBatteryCapacity).ToString()
                        },
                    }
                )
            );

            var prefab = new CustomPrefab(info);

            // Battery をクローン
            var clone = new CloneTemplate(info, TechType.Battery);
            prefab.SetGameObject(clone);

            // カスタムアイコンを設定
            var customIcon = Commons.LoadCustomIcon("uranitite_battery.png");
            if (customIcon != null)
            {
                Log.LogWarning("Custom icon loaded: " + customIcon.name);
                SpriteHandler.RegisterSprite(info.TechType, customIcon);
            }
            else
            {
                Log.LogWarning("Custom icon not loaded");
                var batteryIcon = SpriteManager.Get(TechType.Battery);
                SpriteHandler.RegisterSprite(info.TechType, batteryIcon);
            }

            // レシピ：Battery + 閃ウラン鉱 + リチウム + シリコンゴム
            var recipe = new RecipeData
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>
                {
                    new Ingredient(TechType.Battery, 1),
                    new Ingredient(TechType.UraniniteCrystal, 2),
                    new Ingredient(TechType.Lithium, 1),
                    new Ingredient(TechType.Silicone, 1),
                },
            };
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
            // 登録
            prefab.Register();
            UraniniteBatteryTechType = info.TechType;

            // 表示タブ（基本素材 → 電子部品）のみに配置
            CraftTreeHandler.AddCraftingNode(
                CraftTree.Type.Fabricator,
                info.TechType,
                "Resources",
                "Electronics"
            );

            // バッテリー充電器での互換性を設定（重要！）
            CraftDataHandler.SetEquipmentType(info.TechType, EquipmentType.BatteryCharger);
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
            if (
                __instance.GetTechType() != Plugin.UraniniteCellTechType
                && __instance.GetTechType() != Plugin.UraniniteBatteryTechType
            )
                return;

            var battery = __instance.GetComponent<Battery>();
            if (battery == null)
                return;

            if (__instance.GetTechType() == Plugin.UraniniteCellTechType)
            {
                battery._capacity = Plugin.UraninitePowerCapacity;
                battery._charge = Plugin.UraninitePowerCapacity; // フル充電状態で開始
            }
            else if (__instance.GetTechType() == Plugin.UraniniteBatteryTechType)
            {
                battery._capacity = Plugin.UraninitePowerCapacity;
                battery._charge = Plugin.UraninitePowerCapacity; // フル充電状態で開始
            }

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
                    if (
                        list.Contains(TechType.Battery)
                        && !list.Contains(Plugin.UraniniteBatteryTechType)
                    )
                    {
                        list.Add(Plugin.UraniniteBatteryTechType);
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
                    if (set.Contains(TechType.Battery) && set.Add(Plugin.UraniniteBatteryTechType))
                    {
                        f.SetValue(__instance, set.ToArray());
                    }
                    continue; // returnではなくcontinue
                }
            }

            // 可視モデルのマッピングにUraninitePowerCellを追加（PowerCellの見た目を流用）
            EnergyMixinVisualUtil.TryAliasVisualModel(__instance, TechType.PowerCell, Plugin.UraniniteCellTechType);
        }
    }

    // EnergyMixin の可視モデル配列/リストへ TechType の見た目をエイリアスする
    internal static class EnergyMixinVisualUtil
    {
        internal static void TryAliasVisualModel(EnergyMixin mixin, TechType from, TechType to)
        {
            if (mixin == null)
                return;

            // EnergyMixin 内の全フィールドを走査し、要素が techType と model を持つ配列/リストを探す
            var allFields = AccessTools.GetDeclaredFields(mixin.GetType());
            foreach (var f in allFields)
            {
                var val = f.GetValue(mixin);
                if (val == null)
                    continue;

                System.Type enumerableType = null;
                System.Collections.IList asList = null;
                bool isArray = false;

                if (val is System.Array arr)
                {
                    enumerableType = f.FieldType.GetElementType();
                    asList = new System.Collections.ArrayList(arr);
                    isArray = true;
                }
                else if (val is System.Collections.IList list)
                {
                    // List<T> など
                    var gargs = f.FieldType.IsGenericType ? f.FieldType.GetGenericArguments() : null;
                    enumerableType = gargs != null && gargs.Length == 1 ? gargs[0] : null;
                    asList = list;
                }
                else
                {
                    continue;
                }

                if (enumerableType == null)
                    continue;

                // 要素が techType と model を持つ型か判定
                var techField = AccessTools.Field(enumerableType, "techType")
                    ?? AccessTools.Field(enumerableType, "techTypeId")
                    ?? AccessTools.Field(enumerableType, "batteryType");
                var modelField = AccessTools.Field(enumerableType, "model")
                    ?? AccessTools.Field(enumerableType, "prefab")
                    ?? AccessTools.Field(enumerableType, "gameObject");
                if (techField == null || modelField == null)
                    continue;

                // from のモデルを取得
                object fromModelObj = null;
                bool hasToEntry = false;
                foreach (var item in asList)
                {
                    if (item == null) continue;
                    var tt = (TechType)techField.GetValue(item);
                    if (tt == to)
                    {
                        hasToEntry = true;
                    }
                    if (tt == from && fromModelObj == null)
                    {
                        fromModelObj = modelField.GetValue(item);
                    }
                }

                if (fromModelObj == null || hasToEntry)
                    continue;

                // 新規要素を作成して to を追加
                var newElem = System.Activator.CreateInstance(enumerableType);
                techField.SetValue(newElem, to);
                modelField.SetValue(newElem, fromModelObj);

                if (isArray)
                {
                    var arrList = (System.Collections.ArrayList)asList;
                    arrList.Add(newElem);
                    var newArray = System.Array.CreateInstance(enumerableType, arrList.Count);
                    arrList.CopyTo(newArray);
                    f.SetValue(mixin, newArray);
                }
                else
                {
                    asList.Add(newElem);
                }
            }
        }
    }

    // 共通Util
    internal static class ChargerCompatUtil
    {
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

                                    // 元のデリゲートは保存しない（デフォルト動作のみを使用）

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
            if (seq == null)
            {
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

                // UraniniteBattery の場合は許可
                if (techType == Plugin.UraniniteBatteryTechType)
                {
                    return true;
                }

                // デフォルト: PowerCell系とBattery系を許可
                return techType == TechType.PowerCell
                    || techType == TechType.PrecursorIonPowerCell
                    || techType == TechType.Battery;
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

    // BatteryCharger への互換追加（定期的に全充電器をスキャンして追加）
    internal static class BatteryChargerCompatibility
    {
        private static HashSet<int> patchedChargers = new HashSet<int>();

        // ゲーム開始後に定期的に実行
        internal static void EnsureCompatibility()
        {
            var chargers = UnityEngine.Object.FindObjectsOfType<BatteryCharger>();
            if (chargers.Length > 0)
            {
                int newChargers = 0;
                foreach (var charger in chargers)
                {
                    int chargerId = charger.GetInstanceID();
                    if (!patchedChargers.Contains(chargerId))
                    {
                        ChargerCompatUtil.TryAllow(charger, Plugin.UraniniteBatteryTechType);
                        charger.StartCoroutine(
                            ChargerCompatUtil.LateAllow(charger, Plugin.UraniniteBatteryTechType)
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
                BatteryChargerCompatibility.EnsureCompatibility();
            }
        }
    }
}
