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

        [SerializeField]
        UnityEngine.U2D.SpriteAtlas m_skillSlotAtlas = null;

        private List<UISkillSlotElement> m_uICreatedSkillSlot_List = new List<UISkillSlotElement>();

        private SkillLocalChart m_skillLocalChart = null;

        //------------------------------------------------------------------------------------
        protected override void OnLoad()
        {
            m_skillLocalChart = Managers.TableManager.Instance.GetTableClass<SkillLocalChart>();

            for (int i = 0; i < Define.CharacterDefaultSlotTotalCount; ++i)
            {
                CreateSlot();
            }

            Message.AddListener<GameBerry.Event.SetSkillSlotMsg>(SetSlot);
        }
        //------------------------------------------------------------------------------------
        protected override void OnUnload()
        {
            Message.RemoveListener<GameBerry.Event.SetSkillSlotMsg>(SetSlot);
        }
        //------------------------------------------------------------------------------------
        private void SetSlot(GameBerry.Event.SetSkillSlotMsg msg)
        {
            IEnumerator enumerator = msg.SkillSlot.Keys.GetEnumerator();

            for (int i = 0; i < m_uICreatedSkillSlot_List.Count; ++i)
            {
                SlotState slotstate = SlotState.None;

                if (enumerator.MoveNext() == true)
                {
                    int slotid = (int)enumerator.Current;
                    slotstate = SlotState.OpenSlot;
                    m_uICreatedSkillSlot_List[i].SetSlotID(slotid);
                    m_uICreatedSkillSlot_List[i].SetState(slotstate);
                    m_uICreatedSkillSlot_List[i].SetSkill(m_skillLocalChart.GetSkillData(msg.SkillSlot[slotid]));
                    m_uICreatedSkillSlot_List[i].SetLinkSlot();
                }
                else
                {
                    if (i == msg.SkillSlot.Count)
                    {
                        slotstate = SlotState.AddSlot;
                        m_uICreatedSkillSlot_List[i].SetState(slotstate);
                    }
                    else
                    {
                        slotstate = SlotState.LockSlot;
                        m_uICreatedSkillSlot_List[i].SetState(slotstate);
                    }
                }

                m_uICreatedSkillSlot_List[i].SetSlotBG(m_skillSlotAtlas.GetSprite(slotstate.ToString()));
            }
        }
        //------------------------------------------------------------------------------------
        private void CreateSlot()
        {
            GameObject clone = Instantiate(m_uISkillSlotElement.gameObject, m_slotRoot.transform);
            UISkillSlotElement slot = clone.GetComponent<UISkillSlotElement>();
            slot.Init(OnClick_SlotBtn);

            m_uICreatedSkillSlot_List.Add(slot);
        }
        //------------------------------------------------------------------------------------
        private void OnClick_SlotBtn(int slotid)
        {
        }
        //------------------------------------------------------------------------------------
    }
}