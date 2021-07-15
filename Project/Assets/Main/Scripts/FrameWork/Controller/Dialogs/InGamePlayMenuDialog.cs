using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry.UI
{
    public enum BottomMenuID
    { 
        None = 0,
        Character,
        Skill,
        Equipment,
        Colleague,
        Adventure,
        Store,
    }

    [System.Serializable]
    public class BottomData
    {
        public BottomMenuID MenuID = BottomMenuID.None;

        public Button btn;

        public System.Action<BottomMenuID> CallBack;
        public void OnClick()
        {
            if (CallBack != null)
                CallBack(MenuID);
        }
    }

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

        [SerializeField]
        private List<BottomData> m_bottomMenuBtnDatas;

        [SerializeField]
        private Color m_bottomBtnColor_Click;

        [SerializeField]
        private Color m_bottomBtnColor_Release;

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

        //------------------------------------------------------------------------------------
        private Dictionary<BottomMenuID, Button> m_bottomMenuBtn_Dic = new Dictionary<BottomMenuID, Button>();
        private BottomMenuID m_clickedBottomID = BottomMenuID.None;

        //------------------------------------------------------------------------------------
        protected override void OnLoad()
        {
            for (int i = 0; i < m_bottomMenuBtnDatas.Count; ++i)
            {
                if (m_bottomMenuBtnDatas[i] != null)
                {
                    m_bottomMenuBtnDatas[i].CallBack = OnClick_BottomBtn;
                    m_bottomMenuBtnDatas[i].btn.onClick.AddListener(m_bottomMenuBtnDatas[i].OnClick);

                    if (m_bottomMenuBtn_Dic.ContainsKey(m_bottomMenuBtnDatas[i].MenuID) == false)
                    {
                        m_bottomMenuBtn_Dic.Add(m_bottomMenuBtnDatas[i].MenuID, m_bottomMenuBtnDatas[i].btn);
                    }
                }
            }
        }
        //------------------------------------------------------------------------------------
        protected override void OnUnload()
        {

        }
        //------------------------------------------------------------------------------------
        private void OnClick_BottomBtn(BottomMenuID callbackID)
        {
            HideBottomDialog(m_clickedBottomID);

            if (m_clickedBottomID == callbackID)
            {
                m_clickedBottomID = BottomMenuID.None;
            }
            else
            {
                ShowBottomDialog(callbackID);
                m_clickedBottomID = callbackID;

            }
        }
        //------------------------------------------------------------------------------------
        private void ShowBottomDialog(BottomMenuID callbackID)
        {
            Button btn = null;
            m_bottomMenuBtn_Dic.TryGetValue(callbackID, out btn);
            if (btn != null)
                btn.image.color = m_bottomBtnColor_Click;

            switch (callbackID)
            {
                case BottomMenuID.Character:
                    {
                        RequestDialogEnter<PlayerStatDialog>();
                        break;
                    }
                case BottomMenuID.Skill:
                    {
                        RequestDialogEnter<SkillDialog>();
                        break;
                    }
                case BottomMenuID.Equipment:
                    {
                        RequestDialogEnter<EquipmentDialog>();
                        break;
                    }
                case BottomMenuID.Colleague:
                    {
                        break;
                    }
                case BottomMenuID.Adventure:
                    {
                        break;
                    }
                case BottomMenuID.Store:
                    {
                        RequestDialogEnter<GachaDialog>();
                        break;
                    }
            }
        }
        //------------------------------------------------------------------------------------
        private void HideBottomDialog(BottomMenuID callbackID)
        {
            Button btn = null;
            m_bottomMenuBtn_Dic.TryGetValue(callbackID, out btn);
            if (btn != null)
                btn.image.color = m_bottomBtnColor_Release;

            switch (callbackID)
            {
                case BottomMenuID.Character:
                    {
                        RequestDialogExit<PlayerStatDialog>();
                        break;
                    }
                case BottomMenuID.Skill:
                    {
                        RequestDialogExit<SkillDialog>();
                        break;
                    }
                case BottomMenuID.Equipment:
                    {
                        RequestDialogExit<EquipmentDialog>();
                        break;
                    }
                case BottomMenuID.Colleague:
                    {
                        break;
                    }
                case BottomMenuID.Adventure:
                    {
                        break;
                    }
                case BottomMenuID.Store:
                    {
                        RequestDialogExit<GachaDialog>();
                        break;
                    }
            }
        }
        ////------------------------------------------------------------------------------------
    }
}