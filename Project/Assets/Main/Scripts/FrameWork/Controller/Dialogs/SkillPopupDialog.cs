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

        private Dictionary<int, UISkillSlotElement> m_uICreatedSkillSlot_Dic = new Dictionary<int, UISkillSlotElement>();

        //------------------------------------------------------------------------------------
        protected override void OnLoad()
        {
            m_skillLocalChart = Managers.TableManager.Instance.GetTableClass<SkillLocalChart>();

            Message.AddListener<GameBerry.Event.RefreshSkillStonMsg>(RefreshSkillSton);
            Message.AddListener<GameBerry.Event.SetSkillPopupMsg>(SetSkillPopup);

            Message.AddListener<GameBerry.Event.InitializeSkillSlotMsg>(InitializeSkillSlot);
            Message.AddListener<GameBerry.Event.ChangeSlotStateMsg>(ChangeSlotState);

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

            Message.RemoveListener<GameBerry.Event.InitializeSkillSlotMsg>(InitializeSkillSlot);
            Message.RemoveListener<GameBerry.Event.ChangeSlotStateMsg>(ChangeSlotState);
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
                m_skillEquipOptionText.text = string.Format("optionValue : {0:0.#}", Managers.SkillDataManager.Instance.GetSkillOptionValue(skilldata));

            if (m_skillCoolTitleText != null)
                m_skillCoolTitleText.text = skilldata.SkillTriggerType == SkillTriggerType.Passive ? "PASSIVE" : "대기시간";

            if (m_skillCoolTimeText != null)
                m_skillCoolTimeText.text = skilldata.CoolTime <= 0.0f ? "-" : string.Format("{0:0.#}s", skilldata.CoolTime);

            if (m_skillNeedMPText != null)
                m_skillNeedMPText.text = skilldata.NeedMP <= 0.0f ? "-" : skilldata.NeedMP.ToString();

            if (m_skillLevelUpText != null)
                m_skillLevelUpText.text = Managers.SkillDataManager.Instance.GetNeedLevelUPSkillSton(skilldata, skillinfo).ToString();
        }
        //------------------------------------------------------------------------------------
        private void OnClick_LevelUpBtn()
        {
            if (Managers.SkillDataManager.Instance.SetLevelUpSkill(m_currentSkillData) == true)
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
        private UISkillSlotElement CreateSlot()
        {
            GameObject clone = Instantiate(m_uiSkillSlotElement.gameObject, m_slotElementRoot.transform);
            UISkillSlotElement slot = clone.GetComponent<UISkillSlotElement>();
            slot.Init(OnClick_SlotBtn);

            return slot;
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

                m_uICreatedSkillSlot_Dic.Add(pair.Value.SlotID, element);
            }
        }
        //------------------------------------------------------------------------------------
        private void OnClick_SlotBtn(int slotid)
        {
            Managers.SkillSlotManager.Instance.ChangeSlotSkill(slotid, m_currentSkillData);
        }
        //------------------------------------------------------------------------------------
        private void ChangeSlotState(GameBerry.Event.ChangeSlotStateMsg msg)
        {
            if (msg.SkillSlotData == null)
                return;

            for (int i = 0; i < msg.SkillSlotData.Count; ++i)
            {
                Managers.SkillSlotData slotdata = msg.SkillSlotData[i];

                if (slotdata == null)
                    continue;

                if (m_uICreatedSkillSlot_Dic.ContainsKey(slotdata.SlotID) == false)
                    continue;

                UISkillSlotElement element = null;

                if (m_uICreatedSkillSlot_Dic.TryGetValue(slotdata.SlotID, out element) == false)
                    continue;

                element.SetState(slotdata.CurrSlotState);
                element.SetSlotBG(m_skillSlotAtlas.GetSprite(slotdata.CurrSlotState.ToString()));
            }
        }
        //------------------------------------------------------------------------------------
    }
}