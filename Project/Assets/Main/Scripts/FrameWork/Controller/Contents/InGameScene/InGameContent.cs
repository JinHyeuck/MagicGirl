using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBerry.UI;

namespace GameBerry.Contents
{
    public class InGameContent : IContent
    {
        protected override void OnEnter()
        {
            IDialog.RequestDialogEnter<InGamePlayMenuDialog>();
        }
    }
}