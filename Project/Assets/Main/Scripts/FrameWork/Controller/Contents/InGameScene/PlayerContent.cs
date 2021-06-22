using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBerry.UI;

namespace GameBerry.Contents
{
    public class PlayerContent : IContent
    {
        protected override void OnLoadComplete()
        {
            Managers.PlayerManager manager = Managers.PlayerManager.Instance;
        }

        protected override void OnEnter()
        {
            IDialog.RequestDialogEnter<PlayerInfoDialog>();
        }
    }
}