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
        private static readonly Dictionary<TechType, float> SpeedMultipliers = new Dictionary<TechType, float>();

        // 速度乗数を計算
        internal static float CalculateSpeedMultiplier(Equipment modules)
        {
            if (modules == null)
            {
                Plugin.Log.LogWarning("Equipment modules is null");
                return 1.0f;
            }

            float totalMultiplier = 1.0f;
            int foundModules = 0;

            // 直接TechTypeで検索
            foreach (var kvp in SpeedMultipliers)
            {
                int count = modules.GetCount(kvp.Key);
                
                if (count > 0)
                {
                    Plugin.Log.LogInfo($"Found speed module: {kvp.Key} (count: {count})");
                    for (int i = 0; i < count; i++)
                    {
                        totalMultiplier *= kvp.Value;
                        foundModules++;
                    }
                }
            }

            if (foundModules > 0)
            {
                Plugin.Log.LogInfo($"Total speed multiplier: {totalMultiplier}x from {foundModules} modules");
            }
            else
            {
                Plugin.Log.LogWarning("No speed modules found!");
            }

            return totalMultiplier;
        }

        // SpeedMultipliersを初期化するメソッド
        internal static void InitializeSpeedMultipliers()
        {
            SpeedMultipliers.Clear();
            SpeedMultipliers[VehicleSpeedUpgradeModule.MK1TechType] = 1.5f;
            SpeedMultipliers[VehicleSpeedUpgradeModule.MK2TechType] = 2.0f;
            SpeedMultipliers[VehicleSpeedUpgradeModule.MK3TechType] = 2.5f;
            Plugin.Log.LogInfo("Speed multipliers initialized");
        }
    }

    // SeaMothの速度を変更するパッチ
    [HarmonyPatch(typeof(SeaMoth))]
    internal static class SeaMothSpeedPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        private static void Start_Postfix(SeaMoth __instance)
        {
            Plugin.Log.LogInfo($"SeaMoth Start patch called");
        }

        [HarmonyPostfix]
        [HarmonyPatch("FixedUpdate")]
        private static void FixedUpdate_Postfix(SeaMoth __instance)
        {
            // ログの頻度を下げる（0.5秒ごと）
            if (Time.time % 0.5f < Time.fixedDeltaTime)
            {
                Plugin.Log.LogInfo("SeaMoth FixedUpdate patch called");
            }
            ApplySpeedMultiplier(__instance);
        }

        private static void ApplySpeedMultiplier(SeaMoth seamoth)
        {
            float multiplier = VehicleSpeedPatch.CalculateSpeedMultiplier(seamoth.modules);

            if (multiplier <= 1.0f)
            {
                return;
            }

            // Rigidbodyの速度を直接変更（ブレーキは元のゲームロジックに任せる）
            var rigidbody = seamoth.GetComponent<Rigidbody>();
            if (rigidbody != null && rigidbody.velocity.magnitude > 0.1f)
            {
                Vector3 currentVel = rigidbody.velocity;
                Vector3 enhancedVel = currentVel * multiplier;

                // 最大速度制限
                if (enhancedVel.magnitude > 30f)
                {
                    enhancedVel = enhancedVel.normalized * 30f;
                }

                rigidbody.velocity = Vector3.Lerp(
                    currentVel,
                    enhancedVel,
                    Time.fixedDeltaTime * 2f
                );

                // ログの頻度を下げる（1秒ごと）
                if (Time.time % 1.0f < Time.fixedDeltaTime)
                {
                    Plugin.Log.LogInfo($"SeaMoth speed: {currentVel.magnitude:F2} -> {rigidbody.velocity.magnitude:F2} (multiplier: {multiplier}x)");
                }
            }
        }
    }

    // Exosuitの速度を変更するパッチ
    [HarmonyPatch(typeof(Exosuit))]
    internal static class ExosuitSpeedPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        private static void Start_Postfix(Exosuit __instance)
        {
            Plugin.Log.LogInfo($"Exosuit Start patch called");
        }

        [HarmonyPostfix]
        [HarmonyPatch("FixedUpdate")]
        private static void FixedUpdate_Postfix(Exosuit __instance)
        {
            // ログの頻度を下げる（0.5秒ごと）
            if (Time.time % 0.5f < Time.fixedDeltaTime)
            {
                Plugin.Log.LogInfo("Exosuit FixedUpdate patch called");
            }
            ApplySpeedMultiplier(__instance);
        }

        private static void ApplySpeedMultiplier(Exosuit exosuit)
        {
            float multiplier = VehicleSpeedPatch.CalculateSpeedMultiplier(exosuit.modules);

            if (multiplier <= 1.0f)
            {
                return;
            }

            // Rigidbodyの速度を直接変更（ブレーキは元のゲームロジックに任せる）
            var rigidbody = exosuit.GetComponent<Rigidbody>();
            if (rigidbody != null && rigidbody.velocity.magnitude > 0.1f)
            {
                Vector3 currentVel = rigidbody.velocity;
                Vector3 enhancedVel = currentVel * multiplier;

                // 最大速度制限
                if (enhancedVel.magnitude > 25f)
                {
                    enhancedVel = enhancedVel.normalized * 25f;
                }

                rigidbody.velocity = Vector3.Lerp(
                    currentVel,
                    enhancedVel,
                    Time.fixedDeltaTime * 2f
                );

                // ログの頻度を下げる（1秒ごと）
                if (Time.time % 1.0f < Time.fixedDeltaTime)
                {
                    Plugin.Log.LogInfo($"Exosuit speed: {currentVel.magnitude:F2} -> {rigidbody.velocity.magnitude:F2} (multiplier: {multiplier}x)");
                }
            }
        }
    }

    // TODO: Cyclopsの速度を変更するパッチ（正しいクラス名を確認後に追加）
    // 注: Cyclopsクラスが見つからないため、一時的に無効化

}
