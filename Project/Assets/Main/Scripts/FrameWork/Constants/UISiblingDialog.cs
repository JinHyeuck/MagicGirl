﻿namespace GameBerry.UI
{
	public enum UISibling
	{
        // Screen UI 1 ~ 499
        SceneUI = 0,

        // Login
        LoginDialog,
        // Login

        // InGame
        DunjeonPharmingDialog,

        PlayerStatDialog,
        EquipmentDialog,

        PlayerInfoDialog,
        InGamePlayMenuDialog,
        // Global UI 501 ~ 999
        GlobalUI = 500,

        GlobalFadeDialog,
        GlobalButtonLockDialog,
        GlobalBufferingDialog,
        GlobalPopupDialog,
        GlobalEmergencyNoticeDialog,
    }
}