using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry.UI
{
    public class InGamePlayMenuDialog : IDialog
    {

        [Header("------------BottomMenu------------")]
        [SerializeField]
        private Button m_characterBtn = null;

        [SerializeField]
        private Button m_skillBtn = null;

        [SerializeField]
        private Button m_equipmentBtn = null;

        [SerializeField]
        private Button m_colleagueBtn = null;

        [SerializeField]
        private Button m_adventureBtn = null;

        [SerializeField]
        private Button m_storeBtn = null;

        [Header("------------RightTopMenu------------")]
        [SerializeField]
        private Button m_questBtn = null;

        [SerializeField]
        private Button m_scrollBtn = null;

        [SerializeField]
        private Button m_bonusBtn = null;

        [SerializeField]
        private Button m_bonusContentBtn = null;

        [SerializeField]
        private Text m_bonusContentText = null;



    }
}