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
        private static readonly Dictionary<string, float> SpeedMultipliers = new Dictionary<
            string,
            float
        >
        {
            { "VehicleSpeedUpgradeMK1", 1.5f },
            { "VehicleSpeedUpgradeMK2", 2.0f },
            { "VehicleSpeedUpgradeMK3", 2.5f },
        };

        // TechTypeを保存するための変数
        public static TechType MK1TechType { get; private set; }
        public static TechType MK2TechType { get; private set; }
        public static TechType MK3TechType { get; private set; }

        public static void RegisterModules()
        {
            // 順番に登録（依存関係のため）
            RegisterMK1();
            RegisterMK2();
            RegisterMK3();
        }

        private static void RegisterMK1()
        {
            var info = PrefabInfo.WithTechType(
                classId: "VehicleSpeedUpgradeMK1",
                displayName: LocalizationManager.GetLocalizedString("VehicleSpeedUpgrade.1.Name"),
                description: LocalizationManager.GetLocalizedString(
                    "VehicleSpeedUpgrade.1.Description"
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

            // MK1: 乗物アップグレード装置（SeamothUpgrades + CyclopsUpgrades）で作成
            prefab.SetRecipe(recipe).WithFabricatorType(CraftTree.Type.SeamothUpgrades);

            // 装備可能なモジュールとして設定
            prefab.SetEquipment(EquipmentType.VehicleModule);
            prefab.SetVehicleUpgradeModule(EquipmentType.VehicleModule, QuickSlotType.Passive);

            // PDAグループ/カテゴリを設定（共通モジュールとして表示）
            CraftDataHandler.AddToGroup(
                TechGroup.VehicleUpgrades,
                TechCategory.VehicleUpgrades,
                info.TechType
            );

            // 設計図を起動時アンロック
            KnownTechHandler.UnlockOnStart(info.TechType);

            // 乗物アップグレード装置に追加（共通モジュールカテゴリ）
            CraftTreeHandler.AddCraftingNode(
                CraftTree.Type.SeamothUpgrades,
                info.TechType,
                "SeamothUpgrades",
                "CommonModules"
            );

            prefab.Register();
            MK1TechType = info.TechType;
        }

        private static void RegisterMK2()
        {
            var info = PrefabInfo.WithTechType(
                classId: "VehicleSpeedUpgradeMK2",
                displayName: LocalizationManager.GetLocalizedString("VehicleSpeedUpgrade.2.Name"),
                description: LocalizationManager.GetLocalizedString(
                    "VehicleSpeedUpgrade.2.Description"
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

            // PDAグループ/カテゴリを設定（設計図タブに表示するため）
            CraftDataHandler.AddToGroup(TechGroup.Workbench, TechCategory.Workbench, info.TechType);

            // 設計図表示：MK1をスキャンしてMK2をアンロック
            KnownTechHandler.SetAnalysisTechEntry(MK1TechType, new TechType[] { info.TechType });

            // Workbenchに追加
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Workbench, info.TechType, "Workbench");

            prefab.Register();
            MK2TechType = info.TechType;
        }

        private static void RegisterMK3()
        {
            var info = PrefabInfo.WithTechType(
                classId: "VehicleSpeedUpgradeMK3",
                displayName: LocalizationManager.GetLocalizedString("VehicleSpeedUpgrade.3.Name"),
                description: LocalizationManager.GetLocalizedString(
                    "VehicleSpeedUpgrade.3.Description"
                )
            );

            var prefab = new CustomPrefab(info);

            // アイコンを設定（カスタムアイコンを使用）
            var clone = new CloneTemplate(info, TechType.VehicleHullModule1);
            prefab.SetGameObject(clone);

            // カスタムアイコンを設定
            var customIcon = Commons.LoadCustomIcon("module_icon.png");
            SpriteHandler.RegisterSprite(info.TechType, customIcon);

            // レシピを設定（MK2を素材に含む）
            var recipe = new RecipeData
            {
                craftAmount = 1,
                Ingredients = GetRecipeIngredients("3"),
            };

            // MK3: 改造ステーション（Workbench）で作成
            prefab.SetRecipe(recipe).WithFabricatorType(CraftTree.Type.Workbench);

            // 装備可能なモジュールとして設定
            prefab.SetEquipment(EquipmentType.VehicleModule);
            prefab.SetVehicleUpgradeModule(EquipmentType.VehicleModule, QuickSlotType.Passive);

            // PDAグループ/カテゴリを設定（設計図タブに表示するため）
            CraftDataHandler.AddToGroup(TechGroup.Workbench, TechCategory.Workbench, info.TechType);

            // 設計図表示：MK2をスキャンしてMK3をアンロック
            KnownTechHandler.SetAnalysisTechEntry(MK2TechType, new TechType[] { info.TechType });

            // Workbenchに追加
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Workbench, info.TechType, "Workbench");

            prefab.Register();
            MK3TechType = info.TechType;
        }

        private static List<Ingredient> GetRecipeIngredients(string mkNumber)
        {
            var ingredients = new List<Ingredient>();

            switch (mkNumber)
            {
                case "1":
                    ingredients.Add(new Ingredient(TechType.Titanium, 2));
                    ingredients.Add(new Ingredient(TechType.Copper, 1));
                    ingredients.Add(new Ingredient(TechType.Quartz, 1));
                    break;
                case "2":
                    ingredients.Add(new Ingredient(MK1TechType, 1)); // MK1を素材に
                    ingredients.Add(new Ingredient(TechType.Titanium, 2));
                    ingredients.Add(new Ingredient(TechType.Copper, 1));
                    ingredients.Add(new Ingredient(TechType.Quartz, 1));
                    ingredients.Add(new Ingredient(TechType.Lithium, 1));
                    break;
                case "3":
                    ingredients.Add(new Ingredient(MK2TechType, 1)); // MK2を素材に
                    ingredients.Add(new Ingredient(TechType.Titanium, 2));
                    ingredients.Add(new Ingredient(TechType.Copper, 1));
                    ingredients.Add(new Ingredient(TechType.Quartz, 1));
                    ingredients.Add(new Ingredient(TechType.Lithium, 1));
                    ingredients.Add(new Ingredient(TechType.Nickel, 1));
                    break;
            }

            return ingredients;
        }
    }
}
