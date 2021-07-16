using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry.UI
{
    public enum SlotState
    { 
        None = 0,
        OpenSlot,
        AddSlot,
        LockSlot,
    }

    public class UISkillSlotElement : MonoBehaviour
    {
        [SerializeField]
        private Image m_slotState;

        [SerializeField]
        private Transform m_skillGroup;

        [SerializeField]
        private Image m_skillIcon;

        [SerializeField]
        private Image m_coolTimeFilled;

        [SerializeField]
        private Image m_buffApplyTime;

        [SerializeField]
        private Text m_coolTimeText;

        [SerializeField]
        private Text m_skillTriggerTypeText;

        [SerializeField]
        private Button m_slotBtn;

        //------------------------------------------------------------------------------------
        private SlotState m_currslotState = SlotState.None;
        [HideInInspector]
        public int m_slotID = -1;

        //------------------------------------------------------------------------------------
        public void Init(int slotid, System.Action<int> action)
        {
            if (m_slotBtn)
                m_slotBtn.onClick.AddListener(OnClick_SlotBtn);


        }
        //------------------------------------------------------------------------------------
        public void SetSkillSlot()
        { 

        }
        //------------------------------------------------------------------------------------
        public void SetState(SlotState state)
        {
            if (m_currslotState == state)
                return;

            m_currslotState = state;


        }
        //------------------------------------------------------------------------------------
        private void OnClick_SlotBtn()
        { 
        }
        //------------------------------------------------------------------------------------
    }
}