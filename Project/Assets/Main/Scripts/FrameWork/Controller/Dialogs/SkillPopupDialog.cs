using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry.UI
{
    public class SkillPopupDialog : IDialog
    {
        [SerializeField]
        private Text m_skillStonCountText;

        [SerializeField]
        private UISkillElement m_uISkillElement;

        [SerializeField]
        private Text m_skillNameText;

        [SerializeField]
        private Text m_skillEquipOptionText;

        [SerializeField]
        private Text m_skillCoolTitleText;

        [SerializeField]
        private Text m_skillCoolTimeText;

        [SerializeField]
        private Text m_skillNeedMPText;

        [SerializeField]
        private Button m_skillLevelUpBtn;

        [SerializeField]
        private Text m_skillLevelUpText;

        [SerializeField]
        private Button m_skillEquipSlotBtn;

        [SerializeField]
        private Button m_exitBtn;

        [Header("-------------ChangeSlotPopup-------------")]
        [SerializeField]
        private Transform m_changeSlotPopup;

        [SerializeField]
        private Button m_changeSlotPopupExitBtn;

        [SerializeField]
        private Transform m_slotElementRoot;

        [SerializeField]
        private UISkillSlotElement m_uiSkillSlotElement;

        private SkillLocalChart m_skillLocalChart = null;

        [SerializeField]
        UnityEngine.U2D.SpriteAtlas m_skillSlotAtlas = null;

        //------------------------------------------------------------------------------------
        private SkillData m_currentSkillData = null;
        private PlayerSkillInfo m_currentSKillInfo = null;

        private List<UISkillSlotElement> m_uICreatedSkillSlot_List = new List<UISkillSlotElement>();

        //------------------------------------------------------------------------------------
        protected override void OnLoad()
        {
            m_skillLocalChart = Managers.TableManager.Instance.GetTableClass<SkillLocalChart>();

            Message.AddListener<GameBerry.Event.RefreshSkillStonMsg>(RefreshSkillSton);
            Message.AddListener<GameBerry.Event.SetSkillPopupMsg>(SetSkillPopup);
            Message.AddListener<GameBerry.Event.SetSkillSlotMsg>(SetSlot);

            for (int i = 0; i < Define.CharacterDefaultSlotTotalCount; ++i)
            {
                CreateSlot();
            }

            if (m_skillLevelUpBtn != null)
                m_skillLevelUpBtn.onClick.AddListener(OnClick_LevelUpBtn);

            if (m_skillEquipSlotBtn != null)
                m_skillEquipSlotBtn.onClick.AddListener(OnClick_EquipSkillSlot);

            if (m_exitBtn != null)
                m_exitBtn.onClick.AddListener(() =>
                {
                    RequestDialogExit<SkillPopupDialog>();
                });

            if (m_changeSlotPopupExitBtn != null)
                m_changeSlotPopupExitBtn.onClick.AddListener(() =>
                {
                    if (m_changeSlotPopup != null)
                        m_changeSlotPopup.gameObject.SetActive(false);
                });

            RefreshSkillSton(null);
        }
        //------------------------------------------------------------------------------------
        protected override void OnUnload()
        {
            Message.RemoveListener<GameBerry.Event.RefreshSkillStonMsg>(RefreshSkillSton);
            Message.RemoveListener<GameBerry.Event.SetSkillPopupMsg>(SetSkillPopup);
            Message.RemoveListener<GameBerry.Event.SetSkillSlotMsg>(SetSlot);
        }
        //------------------------------------------------------------------------------------
        private void RefreshSkillSton(GameBerry.Event.RefreshSkillStonMsg msg)
        {
            if (m_skillStonCountText != null)
                m_skillStonCountText.text = string.Format("{0:#,0}", Managers.PlayerDataManager.Instance.GetSkillSton());
        }
        //------------------------------------------------------------------------------------
        private void SetSkillPopup(GameBerry.Event.SetSkillPopupMsg msg)
        {
            SetSkill(msg.skilldata, msg.skillinfo);
        }
        //------------------------------------------------------------------------------------
        private void SetSkill(SkillData skilldata, PlayerSkillInfo skillinfo)
        {
            m_currentSkillData = skilldata;
            m_currentSKillInfo = skillinfo;

            if (m_uISkillElement != null)
                m_uISkillElement.SetSkillElement(skilldata, skillinfo);

            if (m_skillNameText != null)
                m_skillNameText.text = skilldata.SkillName;

            if (m_skillEquipOptionText != null)
                m_skillEquipOptionText.text = string.Format("optionValue : {0:0.#}", Managers.PlayerDataManager.Instance.GetSkillOptionValue(skilldata));

            if (m_skillCoolTitleText != null)
                m_skillCoolTitleText.text = skilldata.SkillTriggerType == SkillTriggerType.Passive ? "PASSIVE" : "대기시간";

            if (m_skillCoolTimeText != null)
                m_skillCoolTimeText.text = skilldata.CoolTime <= 0.0f ? "-" : string.Format("{0:0.#}s", skilldata.CoolTime);

            if (m_skillNeedMPText != null)
                m_skillNeedMPText.text = skilldata.NeedMP <= 0.0f ? "-" : skilldata.NeedMP.ToString();

            if (m_skillLevelUpText != null)
                m_skillLevelUpText.text = Managers.PlayerDataManager.Instance.GetNeedLevelUPSkillSton(skilldata, skillinfo).ToString();
        }
        //------------------------------------------------------------------------------------
        private void OnClick_LevelUpBtn()
        {
            if (Managers.PlayerDataManager.Instance.SetLevelUpSkill(m_currentSkillData) == true)
                SetSkill(m_currentSkillData, m_currentSKillInfo);
        }
        //------------------------------------------------------------------------------------
        private void OnClick_EquipSkillSlot()
        {
            if (m_currentSKillInfo == null)
                return;

            if (m_changeSlotPopup != null)
                m_changeSlotPopup.gameObject.SetActive(true);
        }
        //------------------------------------------------------------------------------------
        private void CreateSlot()
        {
            GameObject clone = Instantiate(m_uiSkillSlotElement.gameObject, m_slotElementRoot.transform);
            UISkillSlotElement slot = clone.GetComponent<UISkillSlotElement>();
            slot.Init(OnClick_SlotBtn);

            m_uICreatedSkillSlot_List.Add(slot);
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
        private void OnClick_SlotBtn(int slotid)
        {
            Managers.SkillManager.Instance.ChangeSlotSkill(m_currentSkillData, slotid);
        }
        //------------------------------------------------------------------------------------
    }
}