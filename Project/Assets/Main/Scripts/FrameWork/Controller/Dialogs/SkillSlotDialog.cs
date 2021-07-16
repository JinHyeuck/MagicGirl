using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry.UI
{
    public class SkillSlotDialog : IDialog
    {
        [SerializeField]
        private Button m_autoSkillBtn;

        [SerializeField]
        private Image m_autoSkillInteractionImage;

        [Header("------------------------------------")]
        [SerializeField]
        private Transform m_slotRoot;

        [SerializeField]
        private UISkillSlotElement m_uISkillSlotElement;

        private Dictionary<int, UISkillSlotElement> m_uISkillSlotList_Dic = new Dictionary<int, UISkillSlotElement>();
        private Dictionary<int, UISkillSlotElement> m_createdSlot_Dic = new Dictionary<int, UISkillSlotElement>();

        private int m_addSlotIndex = -1;

        //------------------------------------------------------------------------------------
        protected override void OnLoad()
        {
            for (int i = 0; i < Define.CharacterDefaultSlotTotalCount; ++i)
            {
                CreateSlot();
            }
        }
        //------------------------------------------------------------------------------------
        private void CreateSlot()
        {
            GameObject clone = Instantiate(m_uISkillSlotElement.gameObject, m_slotRoot.transform);
            UISkillSlotElement slot = clone.GetComponent<UISkillSlotElement>();

            int slotid = m_createdSlot_Dic.Count;

            slot.Init(slotid, OnClick_SlotBtn);

            m_createdSlot_Dic.Add(slotid, slot);
        }
        //------------------------------------------------------------------------------------
        private void OpenSlot(int count)
        {
            foreach (KeyValuePair<int, UISkillSlotElement> pair in m_createdSlot_Dic)
            {
                if (pair.Value != null)
                {
                    if (pair.Value.m_slotID < count)
                        pair.Value.SetState(SlotState.OpenSlot);
                    else if (pair.Value.m_slotID == count)
                    { 
                        pair.Value.SetState(SlotState.AddSlot);
                        m_addSlotIndex = pair.Value.m_slotID;
                    }
                    else
                        pair.Value.SetState(SlotState.LockSlot);
                }
            }
        }
        //------------------------------------------------------------------------------------
        private void OnClick_SlotBtn(int slotid)
        {
        }
        //------------------------------------------------------------------------------------
    }
}