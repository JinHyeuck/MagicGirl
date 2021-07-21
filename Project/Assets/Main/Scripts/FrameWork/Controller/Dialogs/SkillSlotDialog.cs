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

            Message.AddListener<GameBerry.Event.InitializeSkillSlotMsg>(InitializeSkillSlot);
        }
        //------------------------------------------------------------------------------------
        protected override void OnUnload()
        {
            Message.RemoveListener<GameBerry.Event.InitializeSkillSlotMsg>(InitializeSkillSlot);
        }
        //------------------------------------------------------------------------------------
        private void InitializeSkillSlot(GameBerry.Event.InitializeSkillSlotMsg msg)
        {
            foreach (KeyValuePair<int, Managers.SkillSlotData> pair in msg.SkillSlot)
            {
                UISkillSlotElement element = CreateSlot();
                pair.Value.SlotElements.Add(element);
                element.SetSlotID(pair.Value.SlotID);
                element.SetState(pair.Value.CurrSlotState);
                element.SetSlotBG(m_skillSlotAtlas.GetSprite(pair.Value.CurrSlotState.ToString()));
            }
        }
        //------------------------------------------------------------------------------------
        private void InitializeSkillSlotasdfsadfsad(GameBerry.Event.InitializeSkillSlotMsg msg)
        {
            //IEnumerator enumerator = msg.SkillSlot.Keys.GetEnumerator();

            //for (int i = 0; i < msg.SkillSlot.Count; ++i)
            //{
            //    SlotState slotstate = SlotState.None;

            //    UISkillSlotElement element = CreateSlot();

            //    if (enumerator.MoveNext() == true)
            //    {
            //        int slotid = (int)enumerator.Current;
            //        slotstate = SlotState.OpenSlot;
            //        element.SetSlotID(slotid);
            //        element.SetState(slotstate);
            //        element.SetSkill(m_skillLocalChart.GetSkillData(msg.SkillSlot[slotid]));
            //        element.SetLinkSlot();
            //    }
            //    else
            //    {
            //        if (i == msg.SkillSlot.Count)
            //        {
            //            slotstate = SlotState.AddSlot;
            //            m_uICreatedSkillSlot_List[i].SetState(slotstate);
            //        }
            //        else
            //        {
            //            slotstate = SlotState.LockSlot;
            //            m_uICreatedSkillSlot_List[i].SetState(slotstate);
            //        }
            //    }

            //    m_uICreatedSkillSlot_List[i].SetSlotBG(m_skillSlotAtlas.GetSprite(slotstate.ToString()));
            //}
        }
        //------------------------------------------------------------------------------------
        private UISkillSlotElement CreateSlot()
        {
            GameObject clone = Instantiate(m_uISkillSlotElement.gameObject, m_slotRoot.transform);
            UISkillSlotElement slot = clone.GetComponent<UISkillSlotElement>();
            slot.Init(OnClick_SlotBtn);

            m_uICreatedSkillSlot_List.Add(slot);

            return slot;
        }
        //------------------------------------------------------------------------------------
        private void OnClick_SlotBtn(int slotid)
        {
        }
        //------------------------------------------------------------------------------------
    }
}