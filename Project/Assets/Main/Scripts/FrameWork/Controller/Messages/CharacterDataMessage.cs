using System.Collections.Generic;

namespace GameBerry.Event
{
    public class RefrashLevelMsg : Message
    {
    }

    public class RefrashExpMsg : Message
    {
    }

    public class RefrashGoldMsg : Message
    {
    }

    public class RefrashDiaMsg : Message
    {
    }

    public class RefrashEquipmentStonMsg : Message
    {
    }

    public class RefrashSkillStonMsg : Message
    {
    }

    public class ChangeEquipElementMsg : Message
    {
        public EquipmentType EquipementType;
        public int BeforeEquipmentID;
        public int AfterEquipmentID;
    }

    public class RefrashEquipmentInfoListMsg : Message
    {
        public List<EquipmentData> infos = new List<EquipmentData>();
    }

    public class DunjeonPharmingRewardMsg : Message
    {
        public DunjeonRewardType RewardType = DunjeonRewardType.None;
        public double RewardCount = 0;
    }
}