using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry.Contents
{
    public class MonsterContent : IContent
    {
        protected override void OnLoadComplete()
        {
            Managers.MonsterManager manager = Managers.MonsterManager.Instance;
        }
    }
}