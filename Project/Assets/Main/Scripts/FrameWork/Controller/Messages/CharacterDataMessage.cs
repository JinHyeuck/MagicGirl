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

    public class DunjeonPharmingRewardMsg : Message
    {
        public DunjeonRewardType RewardType = DunjeonRewardType.None;
        public int RewardCount = 0;
    }
}