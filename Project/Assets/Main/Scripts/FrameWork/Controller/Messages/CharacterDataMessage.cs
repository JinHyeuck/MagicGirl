using System.Collections.Generic;

namespace GameBerry.Event
{
    public class RefreshLevelMsg : Message
    {
    }

    public class RefreshExpMsg : Message
    {
    }

    public class RefreshGoldMsg : Message
    {
    }

    public class RefreshDiaMsg : Message
    {
    }

    public class RefreshEquipmentStonMsg : Message
    {
    }

    public class RefreshSkillStonMsg : Message
    {
    }

    public class ChangeEquipElementMsg : Message
    {
        public EquipmentType EquipementType;
        public int BeforeEquipmentID;
        public int AfterEquipmentID;
    }

    public class RefreshEquipmentInfoListMsg : Message
    {
        public List<EquipmentData> datas = new List<EquipmentData>();
    }

    public class SetEquipmentPopupMsg : Message
    {
        public EquipmentData equipmentdata;
        public PlayerEquipmentInfo equipmentinfo;
    }

    public class RefreshSkillInfoListMsg : Message
    {
        public List<SkillData> datas = new List<SkillData>();
    }

    public class InitializeSkillSlotMsg : Message
    {
        public Dictionary<int, Managers.SkillSlotData> SkillSlot = new Dictionary<int, Managers.SkillSlotData>();
    }

    public class ChangeEquipSkillMsg : Message
    {
        public List<int> EquipSkillList = new List<int>();
    }

    public class SetSkillPopupMsg : Message
    {
        public SkillData skilldata;
        public PlayerSkillInfo skillinfo;
    }

    public class ChangeSkillSlotPageMsg : Message
    {
        public int SkillPageID = -1;
    }

    public class SetAutoSkillModeMsg : Message
    {
        public bool AutoSkillMode = false;
    }

    public class ChangeSlotStateMsg : Message
    {
        public List<Managers.SkillSlotData> SkillSlotData = null;
    }

    public class DunjeonPharmingRewardMsg : Message
    {
        public DunjeonRewardType RewardType = DunjeonRewardType.None;
        public double RewardCount = 0;
    }
}