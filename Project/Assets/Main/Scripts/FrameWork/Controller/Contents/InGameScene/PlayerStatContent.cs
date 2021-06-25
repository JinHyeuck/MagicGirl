using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBerry.UI;

namespace GameBerry.Contents
{
    public class PlayerStatContent : IContent
    {
        //------------------------------------------------------------------------------------
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
        //------------------------------------------------------------------------------------
    }
}