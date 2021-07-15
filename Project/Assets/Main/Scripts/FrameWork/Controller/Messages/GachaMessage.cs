using System.Collections.Generic;

namespace GameBerry.Event
{
    public class PlayGachaMsg : Message
    {
        public GaChaType Type = GaChaType.None;
    }

    public class ResultGachaMsg : Message
    {
        public List<EquipmentData> GachaEquipmentList = new List<EquipmentData>();
        public List<SkillData> GachaSkillList = new List<SkillData>();
    }
}