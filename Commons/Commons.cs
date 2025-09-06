using System;
using System.IO;
using System.Reflection;
using BepInEx.Logging;
using Nautilus.Utility;
using UnityEngine;

namespace AshFox.Subnautica
{
    public static class Commons
    {
        private static ManualLogSource Log;

        internal static void SetLogger(ManualLogSource logger)
        {
            Log = logger;
        }

        public static bool IsSystemUiOpen()
        {
            // ポーズ/設定/Mods など
            bool menuOpen =
                IngameMenu.main != null
                && IngameMenu.main.isActiveAndEnabled
                && IngameMenu.main.gameObject.activeInHierarchy;
            // マウスカーソルが解放されている＝UI操作中のヒューリスティック
            bool cursorFree = Cursor.visible || Cursor.lockState != CursorLockMode.Locked;
            return menuOpen || cursorFree;
        }

        public static Sprite LoadCustomIcon(string fileName, string assetDirectory = "Assets")
        {
            try
            {
                // Modのディレクトリからアイコンファイルを読み込み
                string modDirectory = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location
                );
                string iconPath = Path.Combine(modDirectory, assetDirectory, fileName);

                if (File.Exists(iconPath))
                {
                    // ファイルパスを直接使用してSpriteを読み込み
                    return ImageUtils.LoadSpriteFromFile(iconPath);
                }
                else
                {
                    Log?.LogWarning($"Custom icon file not found: {iconPath}");
                }
            }
            catch (Exception ex)
            {
                Log?.LogError($"Failed to load custom icon: {ex.Message}");
            }

            // フォールバック: 既存のアイコンを使用
            return SpriteManager.Get(TechType.VehicleHullModule1);
        }
    }
}
