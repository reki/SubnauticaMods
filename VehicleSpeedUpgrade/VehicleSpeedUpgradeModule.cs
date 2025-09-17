using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using UnityEngine;

namespace AshFox.Subnautica
{
    public static class VehicleSpeedUpgradeModule
    {
        // SeaMoth/Exosuit用モジュール
        public static TechType MK1TechType { get; private set; }
        public static TechType MK2TechType { get; private set; }

        // Cyclops専用モジュール
        public static TechType CyclopsMK1TechType { get; private set; }
        public static TechType CyclopsMK2TechType { get; private set; }

        public static void RegisterModules()
        {
            // SeaMoth/Exosuit用モジュール
            RegisterMK1();
            RegisterMK2();

            // Cyclops専用モジュール
            RegisterCyclopsMK1();
            RegisterCyclopsMK2();
        }

        private static void RegisterMK1()
        {
            var info = PrefabInfo.WithTechType(
                classId: "VehicleSpeedUpgradeMK1",
                displayName: LocalizationManager.GetLocalizedString("VehicleSpeedUpgrade.MK1.Name"),
                description: LocalizationManager.GetLocalizedString(
                    "VehicleSpeedUpgrade.MK1.Description"
                )
            );

            var prefab = new CustomPrefab(info);

            // アイコンを設定（カスタムアイコンを使用）
            var clone = new CloneTemplate(info, TechType.VehicleHullModule1);
            prefab.SetGameObject(clone);

            // カスタムアイコンを設定
            var customIcon = Commons.LoadCustomIcon("module_icon.png");
            SpriteHandler.RegisterSprite(info.TechType, customIcon);

            // レシピを設定
            var recipe = new RecipeData
            {
                craftAmount = 1,
                Ingredients = GetRecipeIngredients("1"),
            };

            // MK1: レシピのみ設定（ファブリケータータイプは手動で追加）
            prefab.SetRecipe(recipe);

            // 装備可能なモジュールとして設定
            prefab.SetEquipment(EquipmentType.VehicleModule);
            prefab.SetVehicleUpgradeModule(EquipmentType.VehicleModule, QuickSlotType.Passive);

            // PDAグループ/カテゴリを設定（乗り物モジュールとして表示）
            CraftDataHandler.AddToGroup(
                TechGroup.VehicleUpgrades,
                TechCategory.VehicleUpgrades,
                info.TechType
            );

            // 設計図を起動時アンロック
            KnownTechHandler.UnlockOnStart(info.TechType);

            // 乗物アップグレード装置のCommonModulesカテゴリのみに追加
            CraftTreeHandler.AddCraftingNode(
                CraftTree.Type.SeamothUpgrades,
                info.TechType,
                "CommonModules"
            );

            prefab.Register();
            MK1TechType = info.TechType;
        }

        private static void RegisterMK2()
        {
            var info = PrefabInfo.WithTechType(
                classId: "VehicleSpeedUpgradeMK2",
                displayName: LocalizationManager.GetLocalizedString("VehicleSpeedUpgrade.MK2.Name"),
                description: LocalizationManager.GetLocalizedString(
                    "VehicleSpeedUpgradeMK2.Description"
                )
            );

            var prefab = new CustomPrefab(info);

            // アイコンを設定（カスタムアイコンを使用）
            var clone = new CloneTemplate(info, TechType.VehicleHullModule1);
            prefab.SetGameObject(clone);

            // カスタムアイコンを設定
            var customIcon = Commons.LoadCustomIcon("module_icon.png");
            SpriteHandler.RegisterSprite(info.TechType, customIcon);

            // レシピを設定（MK1を素材に含む）
            var recipe = new RecipeData
            {
                craftAmount = 1,
                Ingredients = GetRecipeIngredients("2"),
            };

            // MK2: 改造ステーション（Workbench）で作成
            prefab.SetRecipe(recipe).WithFabricatorType(CraftTree.Type.Workbench);

            // 装備可能なモジュールとして設定
            prefab.SetEquipment(EquipmentType.VehicleModule);
            prefab.SetVehicleUpgradeModule(EquipmentType.VehicleModule, QuickSlotType.Passive);

            // PDAグループ/カテゴリを設定（乗り物モジュールとして表示）
            CraftDataHandler.AddToGroup(TechGroup.Workbench, TechCategory.Workbench, info.TechType);

            // 設計図表示：MK1をスキャンしてMK2をアンロック
            KnownTechHandler.SetAnalysisTechEntry(MK1TechType, new TechType[] { info.TechType });

            // Workbenchに追加
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Workbench, info.TechType);
            prefab.Register();
            MK2TechType = info.TechType;
        }

        private static List<Ingredient> GetRecipeIngredients(string mkNumber)
        {
            var ingredients = new List<Ingredient>();

            switch (mkNumber)
            {
                case "1":
                    ingredients.Add(new Ingredient(TechType.Titanium, 2));
                    ingredients.Add(new Ingredient(TechType.Quartz, 1));
                    break;
                case "2":
                    ingredients.Add(new Ingredient(MK1TechType, 1)); // MK1を素材に
                    ingredients.Add(new Ingredient(TechType.Titanium, 2));
                    ingredients.Add(new Ingredient(TechType.Quartz, 2));
                    ingredients.Add(new Ingredient(TechType.Nickel, 1));
                    ingredients.Add(new Ingredient(TechType.Diamond, 1));
                    break;
            }

            return ingredients;
        }

        // ====== Cyclops専用モジュール ======
        private static void RegisterCyclopsMK1()
        {
            var info = PrefabInfo.WithTechType(
                classId: "CyclopsSpeedUpgradeMK1",
                displayName: LocalizationManager.GetLocalizedString("CyclopsSpeedUpgrade.MK1.Name"),
                description: LocalizationManager.GetLocalizedString(
                    "CyclopsSpeedUpgrade.MK1.Description"
                )
            );

            var prefab = new CustomPrefab(info);

            // アイコンを設定（Cyclopsソナーモジュールと同じ）
            var clone = new CloneTemplate(info, TechType.CyclopsSonarModule);
            prefab.SetGameObject(clone);

            // カスタムアイコンを設定
            var customIcon = Commons.LoadCustomIcon("module_icon.png");
            SpriteHandler.RegisterSprite(info.TechType, customIcon);

            // レシピを設定
            var recipe = new RecipeData
            {
                craftAmount = 1,
                Ingredients = GetCyclopsRecipeIngredients("1"),
            };

            // Cyclopsファブリケーターで作成
            prefab.SetRecipe(recipe).WithFabricatorType(CraftTree.Type.CyclopsFabricator);

            // Cyclops専用モジュールとして設定
            prefab.SetEquipment(EquipmentType.CyclopsModule);

            // PDAグループ/カテゴリを設定（乗り物モジュールとして表示）
            CraftDataHandler.AddToGroup(
                TechGroup.VehicleUpgrades,
                TechCategory.VehicleUpgrades,
                info.TechType
            );

            // 設計図を起動時アンロック
            KnownTechHandler.UnlockOnStart(info.TechType);

            // Cyclopsファブリケーター > Modules に追加
            CraftTreeHandler.AddCraftingNode(
                CraftTree.Type.CyclopsFabricator,
                info.TechType,
                "Modules"
            );

            prefab.Register();
            CyclopsMK1TechType = info.TechType;
        }

        private static void RegisterCyclopsMK2()
        {
            var info = PrefabInfo.WithTechType(
                classId: "CyclopsSpeedUpgradeMK2",
                displayName: LocalizationManager.GetLocalizedString("CyclopsSpeedUpgrade.MK2.Name"),
                description: LocalizationManager.GetLocalizedString(
                    "CyclopsSpeedUpgrade.MK2.Description"
                )
            );

            var prefab = new CustomPrefab(info);

            // アイコンを設定（Cyclopsソナーモジュールと同じ）
            var clone = new CloneTemplate(info, TechType.CyclopsSonarModule);
            prefab.SetGameObject(clone);

            // カスタムアイコンを設定
            var customIcon = Commons.LoadCustomIcon("module_icon.png");
            SpriteHandler.RegisterSprite(info.TechType, customIcon);

            // レシピを設定（MK1を素材に含む）
            var recipe = new RecipeData
            {
                craftAmount = 1,
                Ingredients = GetCyclopsRecipeIngredients("2"),
            };

            // Cyclopsファブリケーターで作成
            prefab.SetRecipe(recipe).WithFabricatorType(CraftTree.Type.Workbench);

            // Cyclops専用モジュールとして設定
            prefab.SetEquipment(EquipmentType.CyclopsModule);

            // PDAグループ/カテゴリを設定（乗り物モジュールとして表示）
            CraftDataHandler.AddToGroup(TechGroup.Workbench, TechCategory.Workbench, info.TechType);

            // 設計図表示：MK1をスキャンしてMK2をアンロック
            KnownTechHandler.SetAnalysisTechEntry(
                CyclopsMK1TechType,
                new TechType[] { info.TechType }
            );
            prefab.Register();
            CyclopsMK2TechType = info.TechType;
        }

        private static List<Ingredient> GetCyclopsRecipeIngredients(string mkNumber)
        {
            var ingredients = new List<Ingredient>();

            switch (mkNumber)
            {
                case "1":
                    ingredients.Add(new Ingredient(TechType.Titanium, 3));
                    ingredients.Add(new Ingredient(TechType.Quartz, 2));
                    ingredients.Add(new Ingredient(TechType.Copper, 2));
                    break;
                case "2":
                    ingredients.Add(new Ingredient(CyclopsMK1TechType, 1)); // MK1を素材に
                    ingredients.Add(new Ingredient(TechType.Titanium, 3));
                    ingredients.Add(new Ingredient(TechType.Quartz, 3));
                    ingredients.Add(new Ingredient(TechType.Nickel, 2));
                    ingredients.Add(new Ingredient(TechType.Diamond, 1));
                    break;
            }

            return ingredients;
        }
    }
}
