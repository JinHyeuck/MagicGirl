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

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.D))
            {
                IDialog.RequestDialogEnter<PlayerStatDialog>();
            }
            else if (Input.GetKeyUp(KeyCode.F))
            {
                IDialog.RequestDialogExit<PlayerStatDialog>();
            }
        }
    }
}