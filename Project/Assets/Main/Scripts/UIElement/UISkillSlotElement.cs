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
        private System.Action<int> m_callBack = null;

        private int m_slotID = -1;

        private SkillData m_currentSkillData = null;

        //------------------------------------------------------------------------------------
        public void Init(System.Action<int> action)
        {
            if (m_slotBtn)
                m_slotBtn.onClick.AddListener(OnClick_SlotBtn);

            m_callBack = action;
        }
        //------------------------------------------------------------------------------------
        public void SetSlotID(int id)
        {
            m_slotID = id;
        }
        //------------------------------------------------------------------------------------
        public void SetSkill(SkillData data)
        {
            if (data == null)
            {
                if (m_skillGroup != null)
                    m_skillGroup.gameObject.SetActive(false);
            }
            else
            {
                if (m_skillGroup != null)
                    m_skillGroup.gameObject.SetActive(true);

                if (m_skillIcon != null)
                    m_skillIcon.sprite = data.SkillSprite;

                if (m_skillTriggerTypeText != null)
                    m_skillTriggerTypeText.text = data.SkillTriggerType.ToString();
            }

            m_currentSkillData = data;
        }
        //------------------------------------------------------------------------------------
        public void SetState(SlotState state)
        {
            if (m_currslotState == state)
                return;

            m_currslotState = state;

            if (m_skillGroup != null)
                m_skillGroup.gameObject.SetActive(false);
        }
        //------------------------------------------------------------------------------------
        public void SetLinkSlot()
        {
            if (m_currentSkillData != null)
                Managers.SkillManager.Instance.LinkSlotElement(m_currentSkillData.SkillID, this);
        }
        //------------------------------------------------------------------------------------
        public void SetSlotBG(Sprite sp)
        {
            if (m_slotState != null)
                m_slotState.sprite = sp;
        }
        //------------------------------------------------------------------------------------
        private void OnClick_SlotBtn()
        {
            if (m_callBack != null)
                m_callBack(m_slotID);
        }
        //------------------------------------------------------------------------------------
        public void SetCoolTime(float currtime, float totaltime)
        {
            if (m_coolTimeText != null)
                m_coolTimeText.text = string.Format("{0 : 0.#}", totaltime - currtime);

            if (m_coolTimeFilled != null && totaltime > 0.0f)
                m_coolTimeFilled.fillAmount = currtime / totaltime;
        }
        //------------------------------------------------------------------------------------
        public void EndCoolTime()
        {
            if (m_coolTimeText != null)
                m_coolTimeText.text = string.Empty;

            if (m_coolTimeFilled != null)
                m_coolTimeFilled.fillAmount = 0.0f;
        }
        //------------------------------------------------------------------------------------
        public void SetBuffApplyTime(float currtime, float totaltime)
        {
            if (m_buffApplyTime != null && totaltime > 0.0f)
                m_buffApplyTime.fillAmount = (totaltime - currtime) / totaltime;
        }
        //------------------------------------------------------------------------------------
        public void EndBuffApplyTime()
        {
            if (m_buffApplyTime != null)
                m_buffApplyTime.fillAmount = 0.0f;
        }
        //------------------------------------------------------------------------------------
    }
}