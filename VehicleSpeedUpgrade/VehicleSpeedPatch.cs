using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace AshFox.Subnautica
{
    // 乗り物の速度をアップグレードするパッチ
    internal static class VehicleSpeedPatch
    {
        // VehicleDefaults
        private static readonly float DEFAULT_VECHICLE_UPDATE_DISTANCE = 2.0f;
        private static readonly float DEFAULT_MK1_FORCE = 2.0f;
        private static readonly float DEFAULT_MK2_FORCE = 2.5f;
        private static readonly float DEFAULT_MK1_BATTERY_ENERGY_CONSUMPTION_VALUE = 0.02f;
        private static readonly float DEFAULT_MK2_BATTERY_ENERGY_CONSUMPTION_VALUE = 0.03f;

        // CyclopsDefaults
        private static readonly float DEFAULT_CYCLOPS_UPDATE_DISTANCE = 2.0f;
        private static readonly float DEFAULT_CYCLOPS_MK1_SLOW_FORCE = 2.5f;
        private static readonly float DEFAULT_CYCLOPS_MK1_STANDARD_FORCE = 2.5f;
        private static readonly float DEFAULT_CYCLOPS_MK1_FAST_FORCE = 2.5f;
        private static readonly float DEFAULT_CYCLOPS_MK1_SLOW_BATTERY_ENERGY_CONSUMPTION_VALUE =
            0.05f;
        private static readonly float DEFAULT_CYCLOPS_MK1_STANDARD_BATTERY_ENERGY_CONSUMPTION_VALUE =
            0.1f;
        private static readonly float DEFAULT_CYCLOPS_MK1_FAST_BATTERY_ENERGY_CONSUMPTION_VALUE =
            0.2f;
        private static readonly float DEFAULT_CYCLOPS_MK2_SLOW_FORCE = 4.0f;
        private static readonly float DEFAULT_CYCLOPS_MK2_STANDARD_FORCE = 4.0f;
        private static readonly float DEFAULT_CYCLOPS_MK2_FAST_FORCE = 4.0f;
        private static readonly float DEFAULT_CYCLOPS_MK2_SLOW_BATTERY_ENERGY_CONSUMPTION_VALUE =
            0.08f;
        private static readonly float DEFAULT_CYCLOPS_MK2_STANDARD_BATTERY_ENERGY_CONSUMPTION_VALUE =
            0.12f;
        private static readonly float DEFAULT_CYCLOPS_MK2_FAST_BATTERY_ENERGY_CONSUMPTION_VALUE =
            0.25f;

        private static readonly Func<float, bool> CHECK_POSITIVE_FLOAT = (value) => value > 0.0f;
        private static readonly Func<float, bool> CHECK_GREATER_THAN_OR_EQUAL_TO_1_FLOAT = (
            value
        ) => value >= 1.0f;
        private static readonly ConfigTemplate Config = new ConfigTemplate("config.json");
        private static readonly bool DuplicateEffect = Config.GetBool("DuplicateEffect", false);
        public static readonly bool WriteDebugLog = Config.GetBool("WriteDebugLog", false);
        public static readonly bool EnableEnergyConsumptionRate = Config.GetBool(
            "EnableEnergyConsumptionRate",
            true
        );
        public static readonly float VehicleUpdateDistance = Config.GetFloat(
            "VehicleUpdateDistance",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_VECHICLE_UPDATE_DISTANCE
        );
        public static readonly float CyclopsUpdateDistance = Config.GetFloat(
            "CyclopsUpdateDistance",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_CYCLOPS_UPDATE_DISTANCE
        );

        // SeamothMK1
        public static readonly float SeamothMK1ForwardForceMultiplier = Config.GetFloat(
            "Multiplier.Seamoth.MK1.ForwardForce",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_MK1_FORCE
        );
        public static readonly float SeamothMK1BackwardForceMultiplier = Config.GetFloat(
            "Multiplier.Seamoth.MK1.BackwardForce",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_MK1_FORCE
        );
        public static readonly float SeamothMK1SidewardForceMultiplier = Config.GetFloat(
            "Multiplier.Seamoth.MK1.SidewardForce",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_MK1_FORCE
        );
        public static readonly float SeamothMK1VerticalForceMultiplier = Config.GetFloat(
            "Multiplier.Seamoth.MK1.VerticalForce",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_MK1_FORCE
        );
        public static readonly float SeamothMK1BatteryEnergyConsumptionValue = Config.GetFloat(
            "Multiplier.Seamoth.MK1.BatteryEnergyConsumption",
            CHECK_GREATER_THAN_OR_EQUAL_TO_1_FLOAT,
            DEFAULT_MK1_BATTERY_ENERGY_CONSUMPTION_VALUE
        );

        // SeamothMK2
        public static readonly float SeamothMK2ForwardForceMultiplier = Config.GetFloat(
            "Multiplier.Seamoth.MK2.ForwardForce",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_MK2_FORCE
        );
        public static readonly float SeamothMK2BackwardForceMultiplier = Config.GetFloat(
            "Multiplier.Seamoth.MK2.BackwardForce",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_MK2_FORCE
        );
        public static readonly float SeamothMK2SidewardForceMultiplier = Config.GetFloat(
            "Multiplier.Seamoth.MK2.SidewardForce",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_MK2_FORCE
        );
        public static readonly float SeamothMK2VerticalForceMultiplier = Config.GetFloat(
            "Multiplier.Seamoth.MK2.VerticalForce",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_MK2_FORCE
        );
        public static readonly float SeamothMK2BatteryEnergyConsumptionValue = Config.GetFloat(
            "Multiplier.Seamoth.MK2.BatteryEnergyConsumption",
            CHECK_GREATER_THAN_OR_EQUAL_TO_1_FLOAT,
            DEFAULT_MK2_BATTERY_ENERGY_CONSUMPTION_VALUE
        );

        // ExosuitMK1
        public static readonly float ExosuitMK1ForwardForceMultiplier = Config.GetFloat(
            "Multiplier.Exosuit.MK1.ForwardForce",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_MK1_FORCE
        );
        public static readonly float ExosuitMK1BackwardForceMultiplier = Config.GetFloat(
            "Multiplier.Exosuit.MK1.BackwardForce",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_MK1_FORCE
        );
        public static readonly float ExosuitMK1SidewardForceMultiplier = Config.GetFloat(
            "Multiplier.Exosuit.MK1.SidewardForce",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_MK1_FORCE
        );
        public static readonly float ExosuitMK1VerticalForceMultiplier = Config.GetFloat(
            "Multiplier.Exosuit.MK1.VerticalForce",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_MK1_FORCE
        );
        public static readonly float ExosuitMK1BatteryEnergyConsumptionValue = Config.GetFloat(
            "Multiplier.Exosuit.MK1.BatteryEnergyConsumption",
            CHECK_GREATER_THAN_OR_EQUAL_TO_1_FLOAT,
            DEFAULT_MK1_BATTERY_ENERGY_CONSUMPTION_VALUE
        );

        // ExosuitMK2
        public static readonly float ExosuitMK2ForwardForceMultiplier = Config.GetFloat(
            "Multiplier.Exosuit.MK2.ForwardForce",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_MK2_FORCE
        );
        public static readonly float ExosuitMK2BackwardForceMultiplier = Config.GetFloat(
            "Multiplier.Exosuit.MK2.BackwardForce",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_MK2_FORCE
        );
        public static readonly float ExosuitMK2SidewardForceMultiplier = Config.GetFloat(
            "Multiplier.Exosuit.MK2.SidewardForce",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_MK2_FORCE
        );
        public static readonly float ExosuitMK2VerticalForceMultiplier = Config.GetFloat(
            "Multiplier.Exosuit.MK2.VerticalForce",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_MK2_FORCE
        );
        public static readonly float ExosuitMK2BatteryEnergyConsumptionValue = Config.GetFloat(
            "Multiplier.Exosuit.MK2.BatteryEnergyConsumption",
            CHECK_GREATER_THAN_OR_EQUAL_TO_1_FLOAT,
            DEFAULT_MK2_BATTERY_ENERGY_CONSUMPTION_VALUE
        );

        // CyclopsMK1(Slow)
        public static readonly float CyclopsMK1SlowForwardAccelMultiplier = Config.GetFloat(
            "Multiplier.Cyclops.MK1.Slow.ForwardAccel",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_CYCLOPS_MK1_SLOW_FORCE
        );
        public static readonly float CyclopsMK1SlowVerticalAccelMultiplier = Config.GetFloat(
            "Multiplier.Cyclops.MK1.Slow.VerticalAccel",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_CYCLOPS_MK1_SLOW_FORCE
        );
        public static readonly float CyclopsMK1SlowTurningTorqueMultiplier = Config.GetFloat(
            "Multiplier.Cyclops.MK1.Slow.TurningTorque",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_CYCLOPS_MK1_SLOW_FORCE
        );
        public static readonly float CyclopsMK1SlowBatteryEnergyConsumptionValue = Config.GetFloat(
            "Multiplier.Cyclops.MK1.Slow.BatteryEnergyConsumption",
            CHECK_GREATER_THAN_OR_EQUAL_TO_1_FLOAT,
            DEFAULT_CYCLOPS_MK1_SLOW_BATTERY_ENERGY_CONSUMPTION_VALUE
        );

        // CyclopsMK1(Standard)
        public static readonly float CyclopsMK1StandardForwardAccelMultiplier = Config.GetFloat(
            "Multiplier.Cyclops.MK1.Standard.ForwardAccel",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_CYCLOPS_MK1_STANDARD_FORCE
        );
        public static readonly float CyclopsMK1StandardVerticalAccelMultiplier = Config.GetFloat(
            "Multiplier.Cyclops.MK1.Standard.VerticalAccel",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_CYCLOPS_MK1_STANDARD_FORCE
        );
        public static readonly float CyclopsMK1StandardTurningTorqueMultiplier = Config.GetFloat(
            "Multiplier.Cyclops.MK1.Standard.TurningTorque",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_CYCLOPS_MK1_STANDARD_FORCE
        );
        public static readonly float CyclopsMK1StandardBatteryEnergyConsumptionValue =
            Config.GetFloat(
                "Multiplier.Cyclops.MK1.Standard.BatteryEnergyConsumption",
                CHECK_GREATER_THAN_OR_EQUAL_TO_1_FLOAT,
                DEFAULT_CYCLOPS_MK1_STANDARD_BATTERY_ENERGY_CONSUMPTION_VALUE
            );

        // CyclopsMK1(Fast)
        public static readonly float CyclopsMK1FastForwardAccelMultiplier = Config.GetFloat(
            "Multiplier.Cyclops.MK1.Fast.ForwardAccel",
            CHECK_POSITIVE_FLOAT,
            () =>
                Config.GetFloat(
                    "Multiplier.Cyclops.MK1.Flank.ForwardAccel",
                    CHECK_POSITIVE_FLOAT,
                    DEFAULT_CYCLOPS_MK1_FAST_FORCE
                )
        );
        public static readonly float CyclopsMK1FastVerticalAccelMultiplier = Config.GetFloat(
            "Multiplier.Cyclops.MK1.Fast.VerticalAccel",
            CHECK_POSITIVE_FLOAT,
            () =>
                Config.GetFloat(
                    "Multiplier.Cyclops.MK1.Flank.VerticalAccel",
                    CHECK_POSITIVE_FLOAT,
                    DEFAULT_CYCLOPS_MK1_FAST_FORCE
                )
        );
        public static readonly float CyclopsMK1FastTurningTorqueMultiplier = Config.GetFloat(
            "Multiplier.Cyclops.MK1.Fast.TurningTorque",
            CHECK_POSITIVE_FLOAT,
            () =>
                Config.GetFloat(
                    "Multiplier.Cyclops.MK1.Flank.TurningTorque",
                    CHECK_POSITIVE_FLOAT,
                    DEFAULT_CYCLOPS_MK1_FAST_FORCE
                )
        );
        public static readonly float CyclopsMK1FastBatteryEnergyConsumptionValue = Config.GetFloat(
            "Multiplier.Cyclops.MK1.Fast.BatteryEnergyConsumption",
            CHECK_GREATER_THAN_OR_EQUAL_TO_1_FLOAT,
            () =>
                Config.GetFloat(
                    "Multiplier.Cyclops.MK1.Flank.BatteryEnergyConsumption",
                    CHECK_GREATER_THAN_OR_EQUAL_TO_1_FLOAT,
                    DEFAULT_CYCLOPS_MK1_FAST_BATTERY_ENERGY_CONSUMPTION_VALUE
                )
        );

        // CyclopsMK2(Slow)
        public static readonly float CyclopsMK2SlowForwardAccelMultiplier = Config.GetFloat(
            "Multiplier.Cyclops.MK2.Slow.ForwardAccel",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_CYCLOPS_MK2_SLOW_FORCE
        );
        public static readonly float CyclopsMK2SlowVerticalAccelMultiplier = Config.GetFloat(
            "Multiplier.Cyclops.MK2.Slow.VerticalAccel",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_CYCLOPS_MK2_SLOW_FORCE
        );
        public static readonly float CyclopsMK2SlowTurningTorqueMultiplier = Config.GetFloat(
            "Multiplier.Cyclops.MK2.Slow.TurningTorque",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_CYCLOPS_MK2_SLOW_FORCE
        );
        public static readonly float CyclopsMK2SlowBatteryEnergyConsumptionValue = Config.GetFloat(
            "Multiplier.Cyclops.MK2.Slow.BatteryEnergyConsumption",
            CHECK_GREATER_THAN_OR_EQUAL_TO_1_FLOAT,
            DEFAULT_CYCLOPS_MK2_SLOW_BATTERY_ENERGY_CONSUMPTION_VALUE
        );

        // CyclopsMK2(Standard)
        public static readonly float CyclopsMK2StandardForwardAccelMultiplier = Config.GetFloat(
            "Multiplier.Cyclops.MK2.Standard.ForwardAccel",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_CYCLOPS_MK2_STANDARD_FORCE
        );
        public static readonly float CyclopsMK2StandardVerticalAccelMultiplier = Config.GetFloat(
            "Multiplier.Cyclops.MK2.Standard.VerticalAccel",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_CYCLOPS_MK2_STANDARD_FORCE
        );
        public static readonly float CyclopsMK2StandardTurningTorqueMultiplier = Config.GetFloat(
            "Multiplier.Cyclops.MK2.Standard.TurningTorque",
            CHECK_POSITIVE_FLOAT,
            DEFAULT_CYCLOPS_MK2_STANDARD_FORCE
        );
        public static readonly float CyclopsMK2StandardBatteryEnergyConsumptionValue =
            Config.GetFloat(
                "Multiplier.Cyclops.MK2.Standard.BatteryEnergyConsumption",
                CHECK_GREATER_THAN_OR_EQUAL_TO_1_FLOAT,
                DEFAULT_CYCLOPS_MK2_STANDARD_BATTERY_ENERGY_CONSUMPTION_VALUE
            );

        // CyclopsMK2(Fast)
        public static readonly float CyclopsMK2FastForwardAccelMultiplier = Config.GetFloat(
            "Multiplier.Cyclops.MK2.Fast.ForwardAccel",
            CHECK_POSITIVE_FLOAT,
            () =>
                Config.GetFloat(
                    "Multiplier.Cyclops.MK2.Flank.ForwardAccel",
                    CHECK_POSITIVE_FLOAT,
                    DEFAULT_CYCLOPS_MK2_FAST_FORCE
                )
        );
        public static readonly float CyclopsMK2FastVerticalAccelMultiplier = Config.GetFloat(
            "Multiplier.Cyclops.MK2.Fast.VerticalAccel",
            CHECK_POSITIVE_FLOAT,
            () =>
                Config.GetFloat(
                    "Multiplier.Cyclops.MK2.Flank.VerticalAccel",
                    CHECK_POSITIVE_FLOAT,
                    DEFAULT_CYCLOPS_MK2_FAST_FORCE
                )
        );
        public static readonly float CyclopsMK2FastTurningTorqueMultiplier = Config.GetFloat(
            "Multiplier.Cyclops.MK2.Fast.TurningTorque",
            CHECK_POSITIVE_FLOAT,
            () =>
                Config.GetFloat(
                    "Multiplier.Cyclops.MK2.Flank.TurningTorque",
                    CHECK_POSITIVE_FLOAT,
                    DEFAULT_CYCLOPS_MK2_FAST_FORCE
                )
        );
        public static readonly float CyclopsMK2FastBatteryEnergyConsumptionValue = Config.GetFloat(
            "Multiplier.Cyclops.MK2.Fast.BatteryEnergyConsumption",
            CHECK_GREATER_THAN_OR_EQUAL_TO_1_FLOAT,
            () =>
                Config.GetFloat(
                    "Multiplier.Cyclops.MK2.Flank.BatteryEnergyConsumption",
                    CHECK_GREATER_THAN_OR_EQUAL_TO_1_FLOAT,
                    DEFAULT_CYCLOPS_MK2_FAST_BATTERY_ENERGY_CONSUMPTION_VALUE
                )
        );

        private static float lastLogTime = 0f;

        public static void LogMultipliers()
        {
            if (!WriteDebugLog)
                return;

            Plugin.Log.LogInfo("UpdateDistance: ");
            Plugin.Log.LogInfo("VehicleUpdateDistance: " + VehicleUpdateDistance);
            Plugin.Log.LogInfo("CyclopsUpdateDistance: " + CyclopsUpdateDistance);
            Plugin.Log.LogInfo("EnableEnergyConsumptionRate: " + EnableEnergyConsumptionRate);

            Plugin.Log.LogInfo("SeamothMK1: ");
            Plugin.Log.LogInfo("ForwardForceMultiplier: " + SeamothMK1ForwardForceMultiplier);
            Plugin.Log.LogInfo("BackwardForceMultiplier: " + SeamothMK1BackwardForceMultiplier);
            Plugin.Log.LogInfo("SidewardForceMultiplier: " + SeamothMK1SidewardForceMultiplier);
            Plugin.Log.LogInfo("VerticalForceMultiplier: " + SeamothMK1VerticalForceMultiplier);
            Plugin.Log.LogInfo(
                "BatteryEnergyConsumptionValue: " + SeamothMK1BatteryEnergyConsumptionValue
            );

            Plugin.Log.LogInfo("SeamothMK2: ");
            Plugin.Log.LogInfo("ForwardForceMultiplier: " + SeamothMK2ForwardForceMultiplier);
            Plugin.Log.LogInfo("BackwardForceMultiplier: " + SeamothMK2BackwardForceMultiplier);
            Plugin.Log.LogInfo("SidewardForceMultiplier: " + SeamothMK2SidewardForceMultiplier);
            Plugin.Log.LogInfo("VerticalForceMultiplier: " + SeamothMK2VerticalForceMultiplier);
            Plugin.Log.LogInfo(
                "BatteryEnergyConsumptionValue: " + SeamothMK2BatteryEnergyConsumptionValue
            );

            Plugin.Log.LogInfo("PrawnSuitMK1: ");
            Plugin.Log.LogInfo("ForwardForceMultiplier: " + ExosuitMK1ForwardForceMultiplier);
            Plugin.Log.LogInfo("BackwardForceMultiplier: " + ExosuitMK1BackwardForceMultiplier);
            Plugin.Log.LogInfo("SidewardForceMultiplier: " + ExosuitMK1SidewardForceMultiplier);
            Plugin.Log.LogInfo("VerticalForceMultiplier: " + ExosuitMK1VerticalForceMultiplier);
            Plugin.Log.LogInfo(
                "BatteryEnergyConsumptionValue: " + ExosuitMK1BatteryEnergyConsumptionValue
            );

            Plugin.Log.LogInfo("PrawnSuitMK2: ");
            Plugin.Log.LogInfo("ForwardForceMultiplier: " + ExosuitMK2ForwardForceMultiplier);
            Plugin.Log.LogInfo("BackwardForceMultiplier: " + ExosuitMK2BackwardForceMultiplier);
            Plugin.Log.LogInfo("SidewardForceMultiplier: " + ExosuitMK2SidewardForceMultiplier);
            Plugin.Log.LogInfo("VerticalForceMultiplier: " + ExosuitMK2VerticalForceMultiplier);
            Plugin.Log.LogInfo(
                "BatteryEnergyConsumptionValue: " + ExosuitMK2BatteryEnergyConsumptionValue
            );

            Plugin.Log.LogInfo("CyclopsMK1: ");
            Plugin.Log.LogInfo(
                "SlowForwardAccelMultiplier: " + CyclopsMK1SlowForwardAccelMultiplier
            );
            Plugin.Log.LogInfo(
                "SlowVerticalAccelMultiplier: " + CyclopsMK1SlowVerticalAccelMultiplier
            );
            Plugin.Log.LogInfo(
                "SlowTurningTorqueMultiplier: " + CyclopsMK1SlowTurningTorqueMultiplier
            );
            Plugin.Log.LogInfo(
                "SlowBatteryEnergyConsumptionValue: " + CyclopsMK1SlowBatteryEnergyConsumptionValue
            );

            Plugin.Log.LogInfo(
                "StandardForwardAccelMultiplier: " + CyclopsMK1StandardForwardAccelMultiplier
            );
            Plugin.Log.LogInfo(
                "StandardVerticalAccelMultiplier: " + CyclopsMK1StandardVerticalAccelMultiplier
            );
            Plugin.Log.LogInfo(
                "StandardTurningTorqueMultiplier: " + CyclopsMK1StandardTurningTorqueMultiplier
            );
            Plugin.Log.LogInfo(
                "StandardBatteryEnergyConsumptionValue: "
                    + CyclopsMK1StandardBatteryEnergyConsumptionValue
            );

            Plugin.Log.LogInfo(
                "FastForwardAccelMultiplier: " + CyclopsMK1FastForwardAccelMultiplier
            );
            Plugin.Log.LogInfo(
                "FastVerticalAccelMultiplier: " + CyclopsMK1FastVerticalAccelMultiplier
            );
            Plugin.Log.LogInfo(
                "FastTurningTorqueMultiplier: " + CyclopsMK1FastTurningTorqueMultiplier
            );
            Plugin.Log.LogInfo(
                "FastBatteryEnergyConsumptionValue: " + CyclopsMK1FastBatteryEnergyConsumptionValue
            );

            Plugin.Log.LogInfo("CyclopsMK2: ");
            Plugin.Log.LogInfo(
                "SlowForwardAccelMultiplier: " + CyclopsMK2SlowForwardAccelMultiplier
            );
            Plugin.Log.LogInfo(
                "SlowVerticalAccelMultiplier: " + CyclopsMK2SlowVerticalAccelMultiplier
            );
            Plugin.Log.LogInfo(
                "SlowTurningTorqueMultiplier: " + CyclopsMK2SlowTurningTorqueMultiplier
            );
            Plugin.Log.LogInfo(
                "SlowBatteryEnergyConsumptionValue: " + CyclopsMK2SlowBatteryEnergyConsumptionValue
            );

            Plugin.Log.LogInfo(
                "StandardForwardAccelMultiplier: " + CyclopsMK2StandardForwardAccelMultiplier
            );
            Plugin.Log.LogInfo(
                "StandardVerticalAccelMultiplier: " + CyclopsMK2StandardVerticalAccelMultiplier
            );
            Plugin.Log.LogInfo(
                "StandardTurningTorqueMultiplier: " + CyclopsMK2StandardTurningTorqueMultiplier
            );
            Plugin.Log.LogInfo(
                "StandardBatteryEnergyConsumptionValue: "
                    + CyclopsMK2StandardBatteryEnergyConsumptionValue
            );

            Plugin.Log.LogInfo(
                "FastForwardAccelMultiplier: " + CyclopsMK2FastForwardAccelMultiplier
            );
            Plugin.Log.LogInfo(
                "FastVerticalAccelMultiplier: " + CyclopsMK2FastVerticalAccelMultiplier
            );
            Plugin.Log.LogInfo(
                "FastTurningTorqueMultiplier: " + CyclopsMK2FastTurningTorqueMultiplier
            );
            Plugin.Log.LogInfo(
                "FastBatteryEnergyConsumptionValue: " + CyclopsMK2FastBatteryEnergyConsumptionValue
            );

            Plugin.Log.LogInfo("HiddenOptions: ");
            Plugin.Log.LogInfo("DuplicateEffect: " + DuplicateEffect);
        }

        // 速度乗数を計算(Seamoth,Exosuit)
        internal static (float, float, float, float) CalculateSpeedMultiplier(
            Equipment modules,
            bool isSeamoth
        )
        {
            if (modules == null)
            {
                if (Time.time - lastLogTime > 5.0f)
                {
                    Plugin.Log.LogWarning("Equipment modules is null");
                    lastLogTime = Time.time;
                }
                return (1.0f, 1.0f, 1.0f, 1.0f);
            }

            float frontTotalMultiplier = 1.0f;
            float backTotalMultiplier = 1.0f;
            float sideTotalMultiplier = 1.0f;
            float verticalTotalMultiplier = 1.0f;

            float MK1ForwardForceMultiplier = SeamothMK1ForwardForceMultiplier;
            float MK1BackwardForceMultiplier = SeamothMK1BackwardForceMultiplier;
            float MK1SidewardForceMultiplier = SeamothMK1SidewardForceMultiplier;
            float MK1VerticalForceMultiplier = SeamothMK1VerticalForceMultiplier;
            float MK2ForwardForceMultiplier = SeamothMK2ForwardForceMultiplier;
            float MK2BackwardForceMultiplier = SeamothMK2BackwardForceMultiplier;
            float MK2SidewardForceMultiplier = SeamothMK2SidewardForceMultiplier;
            float MK2VerticalForceMultiplier = SeamothMK2VerticalForceMultiplier;
            if (!isSeamoth)
            {
                MK1ForwardForceMultiplier = ExosuitMK1ForwardForceMultiplier;
                MK1BackwardForceMultiplier = ExosuitMK1BackwardForceMultiplier;
                MK1SidewardForceMultiplier = ExosuitMK1SidewardForceMultiplier;
                MK1VerticalForceMultiplier = ExosuitMK1VerticalForceMultiplier;
                MK2ForwardForceMultiplier = ExosuitMK2ForwardForceMultiplier;
                MK2BackwardForceMultiplier = ExosuitMK2BackwardForceMultiplier;
                MK2SidewardForceMultiplier = ExosuitMK2SidewardForceMultiplier;
                MK2VerticalForceMultiplier = ExosuitMK2VerticalForceMultiplier;
            }

            if (DuplicateEffect)
            {
                int count = modules.GetCount(VehicleSpeedUpgradeModule.MK1TechType);
                for (int i = 0; i < count; i++)
                {
                    frontTotalMultiplier *= MK1ForwardForceMultiplier;
                    backTotalMultiplier *= MK1BackwardForceMultiplier;
                    sideTotalMultiplier *= MK1SidewardForceMultiplier;
                    verticalTotalMultiplier *= MK1VerticalForceMultiplier;
                }
                count = modules.GetCount(VehicleSpeedUpgradeModule.MK2TechType);
                for (int i = 0; i < count; i++)
                {
                    frontTotalMultiplier *= MK2ForwardForceMultiplier;
                    backTotalMultiplier *= MK2BackwardForceMultiplier;
                    sideTotalMultiplier *= MK2SidewardForceMultiplier;
                    verticalTotalMultiplier *= MK2VerticalForceMultiplier;
                }
            }
            else
            {
                if (modules.GetCount(VehicleSpeedUpgradeModule.MK2TechType) > 0)
                {
                    frontTotalMultiplier *= MK2ForwardForceMultiplier;
                    backTotalMultiplier *= MK2BackwardForceMultiplier;
                    sideTotalMultiplier *= MK2SidewardForceMultiplier;
                    verticalTotalMultiplier *= MK2VerticalForceMultiplier;
                }
                else if (modules.GetCount(VehicleSpeedUpgradeModule.MK1TechType) > 0)
                {
                    frontTotalMultiplier *= MK1ForwardForceMultiplier;
                    backTotalMultiplier *= MK1BackwardForceMultiplier;
                    sideTotalMultiplier *= MK1SidewardForceMultiplier;
                    verticalTotalMultiplier *= MK1VerticalForceMultiplier;
                }
            }
            return (
                frontTotalMultiplier,
                backTotalMultiplier,
                sideTotalMultiplier,
                verticalTotalMultiplier
            );
        }

        // エネルギー消費率を計算(Seamoth,Exosuit)
        internal static float CalculateEnergyConsumption(Equipment modules, bool isSeamoth)
        {
            if (modules == null)
            {
                if (Time.time - lastLogTime > 5.0f)
                {
                    Plugin.Log.LogWarning("Equipment modules is null");
                    lastLogTime = Time.time;
                }
                return 0.0f;
            }

            float energyConsumptionTotal = 0.0f;
            float MK1BatteryEnergyConsumptionValue = SeamothMK1BatteryEnergyConsumptionValue;
            float MK2BatteryEnergyConsumptionValue = SeamothMK2BatteryEnergyConsumptionValue;

            if (!isSeamoth)
            {
                MK1BatteryEnergyConsumptionValue = ExosuitMK1BatteryEnergyConsumptionValue;
                MK2BatteryEnergyConsumptionValue = ExosuitMK2BatteryEnergyConsumptionValue;
            }

            if (DuplicateEffect)
            {
                int count = modules.GetCount(VehicleSpeedUpgradeModule.MK1TechType);
                for (int i = 0; i < count; i++)
                {
                    energyConsumptionTotal += MK1BatteryEnergyConsumptionValue;
                }
                count = modules.GetCount(VehicleSpeedUpgradeModule.MK2TechType);
                for (int i = 0; i < count; i++)
                {
                    energyConsumptionTotal += MK2BatteryEnergyConsumptionValue;
                }
            }
            else
            {
                if (modules.GetCount(VehicleSpeedUpgradeModule.MK2TechType) > 0)
                {
                    energyConsumptionTotal += MK2BatteryEnergyConsumptionValue;
                }
                else if (modules.GetCount(VehicleSpeedUpgradeModule.MK1TechType) > 0)
                {
                    energyConsumptionTotal += MK1BatteryEnergyConsumptionValue;
                }
            }
            return energyConsumptionTotal;
        }

        // 速度乗数を計算(Cyclops)
        internal static (float, float, float) CalculateCyclopsSpeedMultiplier(
            Equipment modules,
            CyclopsMotorMode.CyclopsMotorModes currentEngineLevel
        )
        {
            if (modules == null)
            {
                if (Time.time - lastLogTime > 5.0f)
                {
                    Plugin.Log.LogWarning("Equipment modules is null");
                    lastLogTime = Time.time;
                }
                return (1.0f, 1.0f, 1.0f);
            }

            float forwardAccelTotalMultiplier = 1.0f;
            float verticalAccelTotalMultiplier = 1.0f;
            float turningTorqueTotalMultiplier = 1.0f;
            float cyclopsMK1ForwardAccelMultiplier = DEFAULT_CYCLOPS_MK1_STANDARD_FORCE;
            float cyclopsMK1VerticalAccelMultiplier = DEFAULT_CYCLOPS_MK1_STANDARD_FORCE;
            float cyclopsMK1TurningTorqueMultiplier = DEFAULT_CYCLOPS_MK1_STANDARD_FORCE;
            float cyclopsMK2ForwardAccelMultiplier = DEFAULT_CYCLOPS_MK2_STANDARD_FORCE;
            float cyclopsMK2VerticalAccelMultiplier = DEFAULT_CYCLOPS_MK2_STANDARD_FORCE;
            float cyclopsMK2TurningTorqueMultiplier = DEFAULT_CYCLOPS_MK2_STANDARD_FORCE;

            if (currentEngineLevel == CyclopsMotorMode.CyclopsMotorModes.Slow)
            {
                cyclopsMK1ForwardAccelMultiplier = CyclopsMK1SlowForwardAccelMultiplier;
                cyclopsMK1VerticalAccelMultiplier = CyclopsMK1SlowVerticalAccelMultiplier;
                cyclopsMK1TurningTorqueMultiplier = CyclopsMK1SlowTurningTorqueMultiplier;
                cyclopsMK2ForwardAccelMultiplier = CyclopsMK2SlowForwardAccelMultiplier;
                cyclopsMK2VerticalAccelMultiplier = CyclopsMK2SlowVerticalAccelMultiplier;
                cyclopsMK2TurningTorqueMultiplier = CyclopsMK2SlowTurningTorqueMultiplier;
            }
            else if (currentEngineLevel == CyclopsMotorMode.CyclopsMotorModes.Standard)
            {
                cyclopsMK1ForwardAccelMultiplier = CyclopsMK1StandardForwardAccelMultiplier;
                cyclopsMK1VerticalAccelMultiplier = CyclopsMK1StandardVerticalAccelMultiplier;
                cyclopsMK1TurningTorqueMultiplier = CyclopsMK1StandardTurningTorqueMultiplier;
                cyclopsMK2ForwardAccelMultiplier = CyclopsMK2StandardForwardAccelMultiplier;
                cyclopsMK2VerticalAccelMultiplier = CyclopsMK2StandardVerticalAccelMultiplier;
                cyclopsMK2TurningTorqueMultiplier = CyclopsMK2StandardTurningTorqueMultiplier;
            }
            else if (currentEngineLevel == CyclopsMotorMode.CyclopsMotorModes.Flank)
            {
                cyclopsMK1ForwardAccelMultiplier = CyclopsMK1FastForwardAccelMultiplier;
                cyclopsMK1VerticalAccelMultiplier = CyclopsMK1FastVerticalAccelMultiplier;
                cyclopsMK1TurningTorqueMultiplier = CyclopsMK1FastTurningTorqueMultiplier;
                cyclopsMK2ForwardAccelMultiplier = CyclopsMK2FastForwardAccelMultiplier;
                cyclopsMK2VerticalAccelMultiplier = CyclopsMK2FastVerticalAccelMultiplier;
                cyclopsMK2TurningTorqueMultiplier = CyclopsMK2FastTurningTorqueMultiplier;
            }

            if (DuplicateEffect)
            {
                int count = modules.GetCount(VehicleSpeedUpgradeModule.CyclopsMK1TechType);
                for (int i = 0; i < count; i++)
                {
                    forwardAccelTotalMultiplier *= cyclopsMK1ForwardAccelMultiplier;
                    verticalAccelTotalMultiplier *= cyclopsMK1VerticalAccelMultiplier;
                    turningTorqueTotalMultiplier *= cyclopsMK1TurningTorqueMultiplier;
                }
                count = modules.GetCount(VehicleSpeedUpgradeModule.CyclopsMK2TechType);
                for (int i = 0; i < count; i++)
                {
                    forwardAccelTotalMultiplier *= cyclopsMK2ForwardAccelMultiplier;
                    verticalAccelTotalMultiplier *= cyclopsMK2VerticalAccelMultiplier;
                    turningTorqueTotalMultiplier *= cyclopsMK2TurningTorqueMultiplier;
                }
            }
            else
            {
                if (modules.GetCount(VehicleSpeedUpgradeModule.CyclopsMK2TechType) > 0)
                {
                    forwardAccelTotalMultiplier *= cyclopsMK2ForwardAccelMultiplier;
                    verticalAccelTotalMultiplier *= cyclopsMK2VerticalAccelMultiplier;
                    turningTorqueTotalMultiplier *= cyclopsMK2TurningTorqueMultiplier;
                }
                else if (modules.GetCount(VehicleSpeedUpgradeModule.CyclopsMK1TechType) > 0)
                {
                    forwardAccelTotalMultiplier *= cyclopsMK1ForwardAccelMultiplier;
                    verticalAccelTotalMultiplier *= cyclopsMK1VerticalAccelMultiplier;
                    turningTorqueTotalMultiplier *= cyclopsMK1TurningTorqueMultiplier;
                }
            }
            return (
                forwardAccelTotalMultiplier,
                verticalAccelTotalMultiplier,
                turningTorqueTotalMultiplier
            );
        }

        // エネルギー消費率乗数を計算(Cyclops)
        internal static float CalculateCyclopsEnergyConsumption(
            Equipment modules,
            CyclopsMotorMode.CyclopsMotorModes currentEngineLevel
        )
        {
            if (modules == null)
            {
                if (Time.time - lastLogTime > 5.0f)
                {
                    Plugin.Log.LogWarning("Equipment modules is null");
                    lastLogTime = Time.time;
                }
                return 0.0f;
            }

            float energyConsumptionTotal = 0.0f;
            float cyclopsMK1EnergyConsumptionValue =
                DEFAULT_CYCLOPS_MK1_STANDARD_BATTERY_ENERGY_CONSUMPTION_VALUE;
            float cyclopsMK2EnergyConsumptionValue =
                DEFAULT_CYCLOPS_MK2_STANDARD_BATTERY_ENERGY_CONSUMPTION_VALUE;

            if (currentEngineLevel == CyclopsMotorMode.CyclopsMotorModes.Slow)
            {
                cyclopsMK1EnergyConsumptionValue = CyclopsMK1SlowBatteryEnergyConsumptionValue;
                cyclopsMK2EnergyConsumptionValue = CyclopsMK2SlowBatteryEnergyConsumptionValue;
            }
            else if (currentEngineLevel == CyclopsMotorMode.CyclopsMotorModes.Standard)
            {
                cyclopsMK1EnergyConsumptionValue = CyclopsMK1StandardBatteryEnergyConsumptionValue;
                cyclopsMK2EnergyConsumptionValue = CyclopsMK2StandardBatteryEnergyConsumptionValue;
            }
            else if (currentEngineLevel == CyclopsMotorMode.CyclopsMotorModes.Flank)
            {
                cyclopsMK1EnergyConsumptionValue = CyclopsMK1FastBatteryEnergyConsumptionValue;
                cyclopsMK2EnergyConsumptionValue = CyclopsMK2FastBatteryEnergyConsumptionValue;
            }

            if (DuplicateEffect)
            {
                int count = modules.GetCount(VehicleSpeedUpgradeModule.CyclopsMK1TechType);
                for (int i = 0; i < count; i++)
                {
                    energyConsumptionTotal += cyclopsMK1EnergyConsumptionValue;
                }
                count = modules.GetCount(VehicleSpeedUpgradeModule.CyclopsMK2TechType);
                for (int i = 0; i < count; i++)
                {
                    energyConsumptionTotal += cyclopsMK2EnergyConsumptionValue;
                }
            }
            else
            {
                if (modules.GetCount(VehicleSpeedUpgradeModule.CyclopsMK2TechType) > 0)
                {
                    energyConsumptionTotal += cyclopsMK2EnergyConsumptionValue;
                }
                else if (modules.GetCount(VehicleSpeedUpgradeModule.CyclopsMK1TechType) > 0)
                {
                    energyConsumptionTotal += cyclopsMK1EnergyConsumptionValue;
                }
            }
            return energyConsumptionTotal;
        }
    }

    // SeaMothの速度を変更するパッチ
    [HarmonyPatch(typeof(SeaMoth))]
    internal static class SeaMothSpeedPatch
    {
        private static readonly Dictionary<SeaMoth, float> OriginalForwardForce =
            new Dictionary<SeaMoth, float>();
        private static readonly Dictionary<SeaMoth, float> OriginalBackwardForce =
            new Dictionary<SeaMoth, float>();
        private static readonly Dictionary<SeaMoth, float> OriginalSidewardForce =
            new Dictionary<SeaMoth, float>();
        private static readonly Dictionary<SeaMoth, float> OriginalVerticalForce =
            new Dictionary<SeaMoth, float>();

        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        private static void Start_Postfix(SeaMoth __instance)
        {
            // 元の推進力を保存
            OriginalForwardForce[__instance] = __instance.forwardForce;
            OriginalBackwardForce[__instance] = __instance.backwardForce;
            OriginalSidewardForce[__instance] = __instance.sidewardForce;
            OriginalVerticalForce[__instance] = __instance.verticalForce;
        }

        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        private static void Update_Postfix(SeaMoth __instance)
        {
            // プレイヤーが乗車している時のみ処理
            if (__instance.GetPilotingMode())
            {
                ApplySpeedMultiplier(__instance);
            }
        }

        private static float lastCalculationTime = 0f;

        private static void ApplySpeedMultiplier(SeaMoth seamoth)
        {
            // 計算頻度を下げる（2秒ごと）
            if (Time.time - lastCalculationTime < VehicleSpeedPatch.VehicleUpdateDistance)
                return;

            (
                float frontMultiplier,
                float backMultiplier,
                float sideMultiplier,
                float verticalMultiplier
            ) = VehicleSpeedPatch.CalculateSpeedMultiplier(seamoth.modules, true);
            lastCalculationTime = Time.time;

            // 元の推進力が保存されていない場合は保存
            if (!OriginalForwardForce.ContainsKey(seamoth))
            {
                OriginalForwardForce[seamoth] = seamoth.forwardForce;
                OriginalBackwardForce[seamoth] = seamoth.backwardForce;
                OriginalSidewardForce[seamoth] = seamoth.sidewardForce;
                OriginalVerticalForce[seamoth] = seamoth.verticalForce;
            }

            // 推進力を乗数で変更
            seamoth.forwardForce = OriginalForwardForce[seamoth] * frontMultiplier;
            seamoth.backwardForce = OriginalBackwardForce[seamoth] * backMultiplier;
            seamoth.sidewardForce = OriginalSidewardForce[seamoth] * sideMultiplier;
            seamoth.verticalForce = OriginalVerticalForce[seamoth] * verticalMultiplier;

            if (VehicleSpeedPatch.WriteDebugLog)
            {
                Plugin.Log.LogInfo("Seamoth: ");
                Plugin.Log.LogInfo(
                    "VehicleSpeedUpgradeMK1: "
                        + seamoth.modules.GetCount(VehicleSpeedUpgradeModule.MK1TechType)
                );
                Plugin.Log.LogInfo(
                    "VehicleSpeedUpgradeMK2: "
                        + seamoth.modules.GetCount(VehicleSpeedUpgradeModule.MK2TechType)
                );
                Plugin.Log.LogInfo("Seamoth multiplier forwardForce: " + frontMultiplier);
                Plugin.Log.LogInfo("Seamoth multiplier backwardForce: " + backMultiplier);
                Plugin.Log.LogInfo("Seamoth multiplier sideForce: " + sideMultiplier);
                Plugin.Log.LogInfo("Seamoth multiplier verticalForce: " + verticalMultiplier);
                Plugin.Log.LogInfo("Seamoth forwardForce: " + seamoth.forwardForce);
                Plugin.Log.LogInfo("Seamoth backwardForce: " + seamoth.backwardForce);
                Plugin.Log.LogInfo("Seamoth sidewardForce: " + seamoth.sidewardForce);
                Plugin.Log.LogInfo("Seamoth verticalForce: " + seamoth.verticalForce);
            }
        }

        // 辞書のクリーンアップは別の方法で処理（メモリリーク対策は後で実装）
    }

    // Exosuitの速度を変更するパッチ
    [HarmonyPatch(typeof(Exosuit))]
    internal static class ExosuitSpeedPatch
    {
        private static readonly Dictionary<Exosuit, float> OriginalForwardForce =
            new Dictionary<Exosuit, float>();
        private static readonly Dictionary<Exosuit, float> OriginalBackwardForce =
            new Dictionary<Exosuit, float>();
        private static readonly Dictionary<Exosuit, float> OriginalSidewardForce =
            new Dictionary<Exosuit, float>();
        private static readonly Dictionary<Exosuit, float> OriginalVerticalForce =
            new Dictionary<Exosuit, float>();

        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        private static void Start_Postfix(Exosuit __instance)
        {
            // 元の推進力を保存
            OriginalForwardForce[__instance] = __instance.forwardForce;
            OriginalBackwardForce[__instance] = __instance.backwardForce;
            OriginalSidewardForce[__instance] = __instance.sidewardForce;
            OriginalVerticalForce[__instance] = __instance.verticalForce;
        }

        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        private static void Update_Postfix(Exosuit __instance)
        {
            // プレイヤーが乗車している時のみ処理
            if (__instance.GetPilotingMode())
            {
                ApplySpeedMultiplier(__instance);
            }
        }

        private static float lastCalculationTimeExosuit = 0f;

        private static void ApplySpeedMultiplier(Exosuit exosuit)
        {
            // 計算頻度を下げる（2秒ごと）
            if (Time.time - lastCalculationTimeExosuit < VehicleSpeedPatch.VehicleUpdateDistance)
                return;

            (
                float frontMultiplier,
                float backMultiplier,
                float sideMultiplier,
                float verticalMultiplier
            ) = VehicleSpeedPatch.CalculateSpeedMultiplier(exosuit.modules, false);
            lastCalculationTimeExosuit = Time.time;

            // 元の推進力が保存されていない場合は保存
            if (!OriginalForwardForce.ContainsKey(exosuit))
            {
                OriginalForwardForce[exosuit] = exosuit.forwardForce;
                OriginalBackwardForce[exosuit] = exosuit.backwardForce;
                OriginalSidewardForce[exosuit] = exosuit.sidewardForce;
                OriginalVerticalForce[exosuit] = exosuit.verticalForce;
            }

            // 推進力を乗数で変更
            exosuit.forwardForce = OriginalForwardForce[exosuit] * frontMultiplier;
            exosuit.backwardForce = OriginalBackwardForce[exosuit] * backMultiplier;
            exosuit.sidewardForce = OriginalSidewardForce[exosuit] * sideMultiplier;
            exosuit.verticalForce = OriginalVerticalForce[exosuit] * verticalMultiplier;

            if (VehicleSpeedPatch.WriteDebugLog)
            {
                Plugin.Log.LogInfo("Exosuit: ");
                Plugin.Log.LogInfo(
                    "VehicleSpeedUpgradeMK1: "
                        + exosuit.modules.GetCount(VehicleSpeedUpgradeModule.MK1TechType)
                );
                Plugin.Log.LogInfo(
                    "VehicleSpeedUpgradeMK2: "
                        + exosuit.modules.GetCount(VehicleSpeedUpgradeModule.MK2TechType)
                );
                Plugin.Log.LogInfo("Seamoth multiplier forwardForce: " + frontMultiplier);
                Plugin.Log.LogInfo("Seamoth multiplier backwardForce: " + backMultiplier);
                Plugin.Log.LogInfo("Seamoth multiplier sideForce: " + sideMultiplier);
                Plugin.Log.LogInfo("Seamoth multiplier verticalForce: " + verticalMultiplier);
                Plugin.Log.LogInfo("Seamoth forwardForce: " + exosuit.forwardForce);
                Plugin.Log.LogInfo("Seamoth backwardForce: " + exosuit.backwardForce);
                Plugin.Log.LogInfo("Seamoth sidewardForce: " + exosuit.sidewardForce);
                Plugin.Log.LogInfo("Seamoth verticalForce: " + exosuit.verticalForce);
            }
        }
    }

    // Cyclopsの速度を変更するパッチ（エンジンレベル対応版）
    [HarmonyPatch(typeof(SubControl))]
    internal static class CyclopsSpeedPatch
    {
        private static readonly Dictionary<SubControl, Vector3> OriginalCyclopsAccel =
            new Dictionary<SubControl, Vector3>();
        private static readonly Dictionary<SubControl, float> OriginalTurningTorque =
            new Dictionary<SubControl, float>();
        private static readonly Dictionary<
            SubControl,
            CyclopsMotorMode.CyclopsMotorModes
        > LastEngineLevel = new Dictionary<SubControl, CyclopsMotorMode.CyclopsMotorModes>();

        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        private static void Update_Postfix(SubControl __instance)
        {
            // Cyclopsかどうかチェックし、プレイヤーが操縦中のみ処理
            if (
                __instance.sub != null
                && __instance.sub.isCyclops
                && __instance.sub.upgradeConsole != null
                && __instance.canAccel
            )
            {
                ApplySpeedMultiplierWithEngineLevel(__instance);
            }
        }

        private static float lastCalculationTimeCyclops = 0f;

        private static void ApplySpeedMultiplierWithEngineLevel(SubControl subControl)
        {
            var motorMode = subControl.sub.GetComponentInChildren<CyclopsMotorMode>();
            if (motorMode == null)
                return;

            // エンジンレベル変更検出
            var currentEngineLevel = motorMode.cyclopsMotorMode;
            bool engineLevelChanged = false;

            if (!LastEngineLevel.ContainsKey(subControl))
            {
                LastEngineLevel[subControl] = currentEngineLevel;
                engineLevelChanged = true;
            }
            else if (LastEngineLevel[subControl] != currentEngineLevel)
            {
                LastEngineLevel[subControl] = currentEngineLevel;
                engineLevelChanged = true;
            }

            // エンジンレベル変更時または定期的に処理
            bool shouldUpdate =
                engineLevelChanged
                || (
                    Time.time - lastCalculationTimeCyclops > VehicleSpeedPatch.CyclopsUpdateDistance
                );
            if (!shouldUpdate)
                return;

            (
                float forwardAccelMultiplier,
                float verticalAccelMultiplier,
                float turningTorqueMultiplier
            ) = VehicleSpeedPatch.CalculateCyclopsSpeedMultiplier(
                subControl.sub.upgradeConsole.modules,
                currentEngineLevel
            );
            lastCalculationTimeCyclops = Time.time;

            // 倍率が1.0以下なら何もしない
            if (forwardAccelMultiplier <= 1.0f)
                return;

            // 現在のエンジンレベルに対応する基本速度を取得
            int currentModeIndex = (int)currentEngineLevel;
            float baseSpeed = motorMode.motorModeSpeeds[currentModeIndex];

            // 推進力を直接変更
            subControl.BaseForwardAccel = baseSpeed * forwardAccelMultiplier;
            subControl.BaseVerticalAccel = baseSpeed * verticalAccelMultiplier;

            // 旋回力を保存・変更
            if (!OriginalTurningTorque.ContainsKey(subControl))
            {
                OriginalTurningTorque[subControl] = subControl.BaseTurningTorque;
            }
            subControl.BaseTurningTorque =
                OriginalTurningTorque[subControl] * turningTorqueMultiplier;

            if (VehicleSpeedPatch.WriteDebugLog)
            {
                Plugin.Log.LogInfo("Cyclops: ");
                Plugin.Log.LogInfo(
                    "VehicleSpeedUpgradeCyclopsMK1: "
                        + subControl.sub.upgradeConsole.modules.GetCount(
                            VehicleSpeedUpgradeModule.CyclopsMK1TechType
                        )
                );
                Plugin.Log.LogInfo(
                    "VehicleSpeedUpgradeCyclopsMK2: "
                        + subControl.sub.upgradeConsole.modules.GetCount(
                            VehicleSpeedUpgradeModule.CyclopsMK2TechType
                        )
                );
                Plugin.Log.LogInfo("Cyclops engine level: " + GetEngineLevelName(currentModeIndex));
                Plugin.Log.LogInfo("Cyclops multiplier forwardAccel: " + forwardAccelMultiplier);
                Plugin.Log.LogInfo("Cyclops multiplier verticalAccel: " + verticalAccelMultiplier);
                Plugin.Log.LogInfo("Cyclops multiplier turningTorque: " + turningTorqueMultiplier);
                Plugin.Log.LogInfo("Cyclops forwardAccel: " + subControl.BaseForwardAccel);
                Plugin.Log.LogInfo("Cyclops verticalAccel: " + subControl.BaseVerticalAccel);
                Plugin.Log.LogInfo("Cyclops turningTorque: " + subControl.BaseTurningTorque);
            }
        }

        private static string GetEngineLevelName(int index)
        {
            return index switch
            {
                0 => "Slow",
                1 => "Standard",
                2 => "Fast",
                _ => "Unknown",
            };
        }
    }

    // バッテリー消費率を制御するパッチ(Seamoth, Exosuit)
    [HarmonyPatch(typeof(EnergyMixin))]
    internal static class VehicleEnergyConsumptionPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("ConsumeEnergy", new Type[] { typeof(float) })]
        private static void ConsumeEnergy_Prefix(EnergyMixin __instance, ref float amount)
        {
            if (!VehicleSpeedPatch.EnableEnergyConsumptionRate)
            {
                return;
            }

            // 乗り物のコンポーネントを取得
            var vehicle = __instance.GetComponent<Vehicle>();
            if (vehicle == null)
            {
                if (VehicleSpeedPatch.WriteDebugLog)
                {
                    Plugin.Log.LogWarning("Vehicle component not found");
                }
                return;
            }

            float batteryValue = 0.0f;
            if (vehicle is SeaMoth seamoth)
            {
                batteryValue = VehicleSpeedPatch.CalculateEnergyConsumption(seamoth.modules, true);
            }
            else if (vehicle is Exosuit exosuit)
            {
                batteryValue = VehicleSpeedPatch.CalculateEnergyConsumption(exosuit.modules, false);
            }

            __instance.ConsumeEnergy(batteryValue);
            if (VehicleSpeedPatch.WriteDebugLog)
            {
                Plugin.Log.LogInfo(
                    $"EnergyMixin.ConsumeEnergy called for {vehicle.GetType().Name} with amount: {amount:F2}"
                );
                Plugin.Log.LogInfo(
                    $"Vehicle energy consumption increased: {amount:F2} (batteryValue: {batteryValue:F1}x)"
                );
            }
        }
    }
}
