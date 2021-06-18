using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry.Contents
{
    public class PlayerContent : IContent
    {
        protected override void OnLoadComplete()
        {
            Managers.PlayerManager manager = Managers.PlayerManager.Instance;
        }
    }
}