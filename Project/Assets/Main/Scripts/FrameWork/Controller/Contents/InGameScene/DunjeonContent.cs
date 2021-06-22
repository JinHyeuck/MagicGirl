using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBerry.UI;

namespace GameBerry.Contents
{
    public class DunjeonContent : IContent
    {

        //------------------------------------------------------------------------------------
        protected override void OnLoadComplete()
        {
            Managers.DunjeonManager manager = Managers.DunjeonManager.Instance;
        }
        //------------------------------------------------------------------------------------
        protected override void OnEnter()
        {
            IDialog.RequestDialogEnter<DunjeonPharmingDialog>();
        }
        //------------------------------------------------------------------------------------
    }
}