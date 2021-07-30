using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry.UI
{
    public enum EquipmentPopupPageType
    {
        LevelUP = 0,
        Combine,
    }

    [System.Serializable]
    public class EquipmentPopupChangeTab
    {
        public Button ChangeTabBtn;
        public EquipmentPopupPageType ButtonType;

        private System.Action<EquipmentPopupPageType> CallBack;

        public void SetCallBack(System.Action<EquipmentPopupPageType> callback)
        {
            CallBack = callback;
        }

        public void OnClick()
        {
            if (CallBack != null)
                CallBack(ButtonType);
        }
    }

    public class EquipmentPopupDialog : IDialog
    {
        [SerializeField]
        private Text m_equipmentStonCountText;

        [SerializeField]
        private List<EquipmentPopupChangeTab> m_changeTabBtnList;

        [SerializeField]
        private Button m_beforeEquipment;

        [SerializeField]
        private Button m_afterEquipment;

        [SerializeField]
        private Button m_exitBtn;

        [Header("--------------LevelUpGroup--------------")]
        [SerializeField]
        private Transform m_levelUpGroup;

        [SerializeField]
        private Text m_equipmentGrade;

        [SerializeField]
        private Text m_equipmentName;

        [SerializeField]
        private UIEquipmentElement m_levelUpEquipmentElement;

        [SerializeField]
        private Text m_equipmentOptionText;

        [SerializeField]
        private Text m_enableOptionText;

        [SerializeField]
        private Button m_levelUPBtn;

        [SerializeField]
        private Text m_levelUPBtnText;

        [SerializeField]
        private Button m_equipBtn;

        [SerializeField]
        private Text m_equipBtnText;

        [Header("--------------CombineGroup--------------")]
        [SerializeField]
        private Transform m_combineGroup;

        [SerializeField]
        private Text m_combine_CurrentEquipmentName;

        [SerializeField]
        UIEquipmentElement m_combine_CurrentEquipmentElement;

        [SerializeField]
        private Text m_combine_CurrentEquipmentDecreaseCount;

        [SerializeField]
        private Transform m_enableCombineGroup;

        [SerializeField]
        private Transform m_combineArrow;

        [SerializeField]
        private Text m_combine_NextEquipmentName;

        [SerializeField]
        UIEquipmentElement m_combine_NextEquipmentElement;

        [SerializeField]
        private Text m_combine_NextEquipmentIncreaseCount;

        [SerializeField]
        private Button m_decreaseCountCombineBtn;

        [SerializeField]
        private Button m_increaseCountCombineBtn;

        [SerializeField]
        private Text m_combineCountText;

        [SerializeField]
        private Button m_doCombineBtn;

        //------------------------------------------------------------------------------------
        private EquipmentPopupPageType m_equipmentPopupPageType = EquipmentPopupPageType.LevelUP;

        private EquipmentLocalChart m_equipmentLocalChart = null;

        private EquipmentData m_currentEquipmentData = null;
        private PlayerEquipmentInfo m_currentEquipmentInfo = null;

        private int m_currentMaxCombineCount = 0;
        private int m_combineCount = 0;

        //------------------------------------------------------------------------------------
        protected override void OnLoad()
        {
            Message.AddListener<GameBerry.Event.RefreshEquipmentStonMsg>(RefrashEquipmentSton);
            Message.AddListener<GameBerry.Event.SetEquipmentPopupMsg>(SetEquipmentPopup);

            m_equipmentLocalChart = Managers.TableManager.Instance.GetTableClass<EquipmentLocalChart>();

            for (int i = 0; i < m_changeTabBtnList.Count; ++i)
            {
                if (m_changeTabBtnList[i].ChangeTabBtn != null)
                {
                    m_changeTabBtnList[i].SetCallBack(OnClick_ChangePopupPageBtn);
                    m_changeTabBtnList[i].ChangeTabBtn.onClick.AddListener(m_changeTabBtnList[i].OnClick);
                }
            }

            if (m_beforeEquipment != null)
                m_beforeEquipment.onClick.AddListener(OnClick_BeforeEquipment);

            if (m_afterEquipment != null)
                m_afterEquipment.onClick.AddListener(OnClick_AfterEquipment);

            if (m_exitBtn != null)
                m_exitBtn.onClick.AddListener(() =>
                {
                    RequestDialogExit<EquipmentPopupDialog>();
                });

            if (m_levelUPBtn != null)
                m_levelUPBtn.onClick.AddListener(OnClick_LevelUPBtn);

            if (m_equipBtn != null)
                m_equipBtn.onClick.AddListener(OnClick_equipBtn);

            if (m_decreaseCountCombineBtn != null)
                m_decreaseCountCombineBtn.onClick.AddListener(OnClick_DecreaseCountCombine);

            if (m_increaseCountCombineBtn != null)
                m_increaseCountCombineBtn.onClick.AddListener(OnClick_IncreaseCountCombine);

            if (m_doCombineBtn != null)
                m_doCombineBtn.onClick.AddListener(OnClick_DoCombine);
        }
        //------------------------------------------------------------------------------------
        protected override void OnUnload()
        {
            Message.RemoveListener<GameBerry.Event.RefreshEquipmentStonMsg>(RefrashEquipmentSton);
            Message.RemoveListener<GameBerry.Event.SetEquipmentPopupMsg>(SetEquipmentPopup);
        }
        //------------------------------------------------------------------------------------
        private void RefrashEquipmentSton(GameBerry.Event.RefreshEquipmentStonMsg msg)
        {
            if (m_equipmentStonCountText != null)
                m_equipmentStonCountText.text = string.Format("{0:#,0}", Managers.PlayerDataManager.Instance.GetEquipmentSton());
        }
        //------------------------------------------------------------------------------------
        private void SetEquipmentPopup(GameBerry.Event.SetEquipmentPopupMsg msg)
        {
            SetEquipment(msg.equipmentdata, msg.equipmentinfo);
        }
        //------------------------------------------------------------------------------------
        private void SetEquipment(EquipmentData equipmentdata, PlayerEquipmentInfo equipmentinfo)
        {
            if (m_levelUpGroup != null)
                m_levelUpGroup.gameObject.SetActive(m_equipmentPopupPageType == EquipmentPopupPageType.LevelUP);

            if (m_combineGroup != null)
                m_combineGroup.gameObject.SetActive(m_equipmentPopupPageType == EquipmentPopupPageType.Combine);

            m_currentEquipmentData = equipmentdata;
            m_currentEquipmentInfo = equipmentinfo;

            if (m_equipmentPopupPageType == EquipmentPopupPageType.LevelUP)
            {
                SetLevelUPUI(equipmentdata, equipmentinfo);
            }
            else
            {
                SetCombineUI(equipmentdata, equipmentinfo);
            }
        }
        //------------------------------------------------------------------------------------
        private void SetLevelUPUI(EquipmentData equipmentdata, PlayerEquipmentInfo equipmentinfo)
        {
            if (m_equipmentGrade != null)
                m_equipmentGrade.text = equipmentdata.Grade.ToString();

            if (m_equipmentName != null)
                m_equipmentName.text = equipmentdata.EquipmentName;

            if (m_levelUpEquipmentElement != null)
                m_levelUpEquipmentElement.SetEquipmentElement(equipmentdata, equipmentinfo);

            if (m_equipmentOptionText != null)
            {
                List<StatType> options = equipmentdata.ApplyOption[EquipmentApplyOption.EquipmentOption];

                string contenttext = string.Empty;

                for (int i = 0; i < options.Count; ++i)
                {
                    if (equipmentdata.Option.ContainsKey(options[i]) == false)
                        continue;

                    double curr = Managers.EquipmentDataManager.Instance.GetEquipmentOptionValue(equipmentdata, options[i]);
                    double next = Managers.EquipmentDataManager.Instance.GetEquipmentNextLevelOptionValue(equipmentdata, options[i]);

                    contenttext += string.Format("{0} {1}->{2}", options[i].ToString(), curr, next);

                    if (i <= options.Count - 2)
                        contenttext += "\n";
                }

                m_equipmentOptionText.text = contenttext;
            }

            if (m_enableOptionText != null)
            {
                if (equipmentdata.ApplyOption.ContainsKey(EquipmentApplyOption.EnableOption) == true)
                {
                    List<StatType> options = equipmentdata.ApplyOption[EquipmentApplyOption.EnableOption];

                    string contenttext = string.Empty;

                    for (int i = 0; i < options.Count; ++i)
                    {
                        if (equipmentdata.Option.ContainsKey(options[i]) == false)
                            continue;

                        double curr = 0.0;
                        equipmentdata.Option.TryGetValue(options[i], out curr);

                        contenttext += string.Format("{0} {1}", options[i].ToString(), curr);

                        if (i <= options.Count - 2)
                            contenttext += "\n";
                    }

                    m_enableOptionText.text = contenttext;
                }
                else
                    m_enableOptionText.text = string.Empty;
            }

            if (m_levelUPBtn != null)
                m_levelUPBtn.gameObject.SetActive(equipmentinfo != null);

            if (m_levelUPBtnText != null)
                m_levelUPBtnText.gameObject.SetActive(equipmentinfo != null);

            if (m_equipBtn != null)
                m_equipBtn.gameObject.SetActive(equipmentinfo != null);

            if (m_equipBtnText != null)
                m_equipBtnText.gameObject.SetActive(equipmentinfo != null);

            if (m_enableCombineGroup != null)
                m_enableCombineGroup.gameObject.SetActive(equipmentinfo != null);

            if (equipmentinfo != null)
            {
                if (m_levelUPBtnText != null)
                    m_levelUPBtnText.text = Managers.EquipmentDataManager.Instance.GetNeedLevelUPEquipmentSton(equipmentdata, equipmentinfo).ToString();

                bool equipstate = Managers.EquipmentDataManager.Instance.IsEquipElement(equipmentdata);

                if (m_equipBtn != null)
                {
                    m_equipBtn.image.color = equipstate == true ? Color.gray : Color.white;
                    m_equipBtn.interactable = equipstate == false;
                }

                if (m_equipBtnText != null)
                    m_equipBtnText.text = equipstate == true ? "������" : "����";
            }
        }
        //------------------------------------------------------------------------------------
        private void SetCombineUI(EquipmentData equipmentdata, PlayerEquipmentInfo equipmentinfo)
        {
            EquipmentData NextEquipmentData = m_equipmentLocalChart.GetNextEquipmentData(equipmentdata.Id);
            PlayerEquipmentInfo NextEquipmentInfo = null;

            if (NextEquipmentData != null)
                NextEquipmentInfo = Managers.EquipmentDataManager.Instance.GetPlayerEquipmentInfo(NextEquipmentData.Type, NextEquipmentData.Id);

            if (m_combine_CurrentEquipmentName != null)
                m_combine_CurrentEquipmentName.text = equipmentdata.EquipmentName;

            if (m_combine_CurrentEquipmentElement != null)
                m_combine_CurrentEquipmentElement.SetEquipmentElement(equipmentdata, equipmentinfo);

            int currentequipcount = 0;

            if (equipmentinfo != null)
                currentequipcount = equipmentinfo.Count;

            int maxcombinecount = currentequipcount / Define.EquipmentComposeAmount;

            if (m_combine_CurrentEquipmentDecreaseCount != null)
            {
                if (maxcombinecount > 0)
                    m_combine_CurrentEquipmentDecreaseCount.text = string.Format("-{0}", maxcombinecount * Define.EquipmentComposeAmount);
                else
                    m_combine_CurrentEquipmentDecreaseCount.text = maxcombinecount.ToString();
            }

            if (m_enableCombineGroup != null)
                m_enableCombineGroup.gameObject.SetActive(NextEquipmentData != null);

            if (NextEquipmentData != null)
            {
                m_currentMaxCombineCount = maxcombinecount;
                m_combineCount = maxcombinecount;

                if (m_combine_NextEquipmentName != null)
                    m_combine_NextEquipmentName.text = NextEquipmentData.EquipmentName;

                if (m_combine_NextEquipmentElement != null)
                    m_combine_NextEquipmentElement.SetEquipmentElement(NextEquipmentData, NextEquipmentInfo);

                if (m_combine_NextEquipmentIncreaseCount != null)
                {
                    m_combine_NextEquipmentIncreaseCount.gameObject.SetActive(true);
                    if (maxcombinecount > 0)
                        m_combine_NextEquipmentIncreaseCount.text = string.Format("+{0}", maxcombinecount);
                    else
                        m_combine_NextEquipmentIncreaseCount.text = maxcombinecount.ToString();
                }

                if (m_combineCountText != null)
                    m_combineCountText.text = maxcombinecount.ToString();
            }
        }
        //------------------------------------------------------------------------------------
        private void OnClick_ChangePopupPageBtn(EquipmentPopupPageType type)
        {
            if (m_equipmentPopupPageType == type)
                return;

            m_equipmentPopupPageType = type;

            SetEquipment(m_currentEquipmentData, m_currentEquipmentInfo);
        }
        //------------------------------------------------------------------------------------
        private void OnClick_BeforeEquipment()
        {
            EquipmentData beforedata = m_currentEquipmentData.PrevData;
            PlayerEquipmentInfo beforeinfo = Managers.EquipmentDataManager.Instance.GetPlayerEquipmentInfo(beforedata.Type, beforedata.Id);
            SetEquipment(beforedata, beforeinfo);
        }
        //------------------------------------------------------------------------------------
        private void OnClick_AfterEquipment()
        {
            EquipmentData afterdata = m_currentEquipmentData.NextData;
            PlayerEquipmentInfo afterinfo = Managers.EquipmentDataManager.Instance.GetPlayerEquipmentInfo(afterdata.Type, afterdata.Id);
            SetEquipment(afterdata, afterinfo);
        }
        //------------------------------------------------------------------------------------
        private void OnClick_LevelUPBtn()
        {
            if (Managers.EquipmentDataManager.Instance.SetLevelUpEquipment(m_currentEquipmentData) == true)
                SetEquipment(m_currentEquipmentData, m_currentEquipmentInfo);
        }
        //------------------------------------------------------------------------------------
        private void OnClick_equipBtn()
        {
            if (Managers.EquipmentDataManager.Instance.SetEquipElement(m_currentEquipmentData) == true)
            {
                if (m_equipBtn != null)
                {
                    m_equipBtn.image.color = Color.gray;
                    m_equipBtn.interactable = false;
                }

                if (m_equipBtnText != null)
                    m_equipBtnText.text = "������";
            }
        }
        //------------------------------------------------------------------------------------
        private void OnClick_DecreaseCountCombine()
        {
            if (m_combineCount <= 0)
                return;

            m_combineCount--;

            if (m_combineCountText != null)
                m_combineCountText.text = m_combineCount.ToString();
        }
        //------------------------------------------------------------------------------------
        private void OnClick_IncreaseCountCombine()
        {
            if (m_combineCount >= m_currentMaxCombineCount)
                return;

            m_combineCount++;

            if (m_combineCountText != null)
                m_combineCountText.text = m_combineCount.ToString();
        }
        //------------------------------------------------------------------------------------
        private void OnClick_DoCombine()
        {
            if (m_combineCount > 0)
            {
                if (Managers.EquipmentDataManager.Instance.CombineEquipment(m_currentEquipmentData, m_equipmentLocalChart.GetNextEquipmentData(m_currentEquipmentData.Id), m_combineCount) == true)
                    SetCombineUI(m_currentEquipmentData, Managers.EquipmentDataManager.Instance.GetPlayerEquipmentInfo(m_currentEquipmentData));
            }
        }
        //------------------------------------------------------------------------------------
    }
}