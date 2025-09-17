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
        public const string PluginGuid = "jp.ashfox.vehicle.module";
        public const string PluginName = "VehicleSpeedUpgrade";
        public const string PluginVersion = "1.0.0";

        public static ManualLogSource Log { get; private set; }

        private void Awake()
        {
            Log = Logger;
            Commons.SetLogger(Log);
            // ローカライゼーションを初期化
            LocalizationManager.Initialize();

            // 速度アップグレードモジュールを登録
            VehicleSpeedUpgradeModule.RegisterModules();

            // ログを出力
            VehicleSpeedPatch.LogMultipliers();

            // Harmonyパッチを適用
            new Harmony(PluginGuid).PatchAll();

            Log.LogInfo("VehicleSpeedUpgrade loaded successfully!");
        }
    }
}
