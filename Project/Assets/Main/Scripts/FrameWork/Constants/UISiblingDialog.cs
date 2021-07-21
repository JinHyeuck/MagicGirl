namespace GameBerry.UI
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

        SkillSlotDialog,

        PlayerStatDialog,
        EquipmentDialog,
        SkillDialog,
        GachaDialog,

        PlayerInfoDialog,
        InGamePlayMenuDialog,

        EquipmentPopupDialog,
        SkillPopupDialog,
        GachaResultDialog,

        // Global UI 501 ~ 999
        GlobalUI = 500,

        GlobalFadeDialog,
        GlobalButtonLockDialog,
        GlobalBufferingDialog,
        GlobalPopupDialog,
        GlobalEmergencyNoticeDialog,
    }
}