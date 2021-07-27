using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry.Contents
{
    public class SkillContent : IContent
    {
        protected override void OnLoadComplete()
        {
            Managers.SkillSlotManager.Instance.InitializeSkillSlot();
        }
    }
}