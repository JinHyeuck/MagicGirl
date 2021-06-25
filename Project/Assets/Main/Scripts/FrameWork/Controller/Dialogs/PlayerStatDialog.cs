using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry.UI
{
    public class PlayerStatDialog : IDialog
    {
        [Header("---------ChangeTab---------")]
        [SerializeField]
        private Button m_changeTab_Upgrade;

        [SerializeField]
        private Button m_changeTab_Promotion;

        [Header("---------PlayerProfile---------")]
        [SerializeField]
        private Image m_playerIcon;

        [SerializeField]
        private Text m_levelText;

        [SerializeField]
        private Image m_expBar;

        [SerializeField]
        private Text m_expText;

        [SerializeField]
        private Text m_expPerText;

        [Header("---------UpGradeContent---------")]
        [SerializeField]
        private Transform m_upGradeContentRoot;




        //------------------------------------------------------------------------------------



    }
}