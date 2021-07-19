using System.Collections.Generic;

namespace GameBerry.Event
{
    public class SetSlotMsg : Message
    {
        public Dictionary<int, int> SkillSlot = new Dictionary<int, int>();
    }
}