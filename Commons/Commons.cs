using BepInEx.Logging;
using UnityEngine;

public static class Commons
{
    internal static ManualLogSource Log;

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
}
