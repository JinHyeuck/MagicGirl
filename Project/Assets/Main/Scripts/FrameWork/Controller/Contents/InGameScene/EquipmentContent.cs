using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBerry.UI;

namespace GameBerry.Contents
{
    public class EquipmentContent : IContent
    {
        //------------------------------------------------------------------------------------
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.D))
            {
                IDialog.RequestDialogEnter<EquipmentDialog>();
            }
            else if (Input.GetKeyUp(KeyCode.F))
            {
                IDialog.RequestDialogExit<EquipmentDialog>();
            }
        }
        //------------------------------------------------------------------------------------
    }
}