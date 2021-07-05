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

    public class UIEquipmentPopup : MonoBehaviour
    {
        [SerializeField]
        private Text m_equipmentStonCountText;

        [SerializeField]
        private List<EquipmentPopupChangeTab> m_changeTabBtnList;

        [SerializeField]
        private Button m_beforeEquipment;

        [SerializeField]
        private Button m_afterEquipment;

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
        private Text m_combine_NextEquipmentName;

        [SerializeField]
        UIEquipmentElement m_combine_NextEquipmentElement;

        [SerializeField]
        private Text m_combine_CurrentEquipmentIncreaseCount;

        [SerializeField]
        private Button m_decreaseCombineCount;

        [SerializeField]
        private Button m_increaseCombineCount;

        [SerializeField]
        private Text m_combineCount;


        //------------------------------------------------------------------------------------
        private EquipmentPopupPageType m_equipmentPopupPageType = EquipmentPopupPageType.LevelUP;

        private EquipmentData m_currentEquipmentData = null;
        private PlayerEquipmentInfo m_currentEquipmentInfo = null;

        //------------------------------------------------------------------------------------
        public void Init()
        {
            Message.AddListener<GameBerry.Event.RefrashEquipmentStonMsg>(RefrashEquipmentSton);

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

            if (m_levelUPBtn != null)
                m_levelUPBtn.onClick.AddListener(OnClick_LevelUPBtn);

            if (m_equipBtn != null)
                m_equipBtn.onClick.AddListener(OnClick_equipBtn);
        }
        //------------------------------------------------------------------------------------
        private void OnDestroy()
        {
            Message.RemoveListener<GameBerry.Event.RefrashEquipmentStonMsg>(RefrashEquipmentSton);
        }
        //------------------------------------------------------------------------------------
        private void RefrashEquipmentSton(GameBerry.Event.RefrashEquipmentStonMsg msg)
        {
            if (m_equipmentStonCountText != null)
                m_equipmentStonCountText.text = string.Format("{0:#,0}", Managers.PlayerDataManager.Instance.GetEquipmentSton());
        }
        //------------------------------------------------------------------------------------
        public void SetEquipment(EquipmentData equipmentdata, PlayerEquipmentInfo equipmentinfo)
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

            if (m_levelUPBtn != null)
                m_levelUPBtn.gameObject.SetActive(equipmentinfo != null);

            if (m_levelUPBtnText != null)
                m_levelUPBtnText.gameObject.SetActive(equipmentinfo != null);

            if (m_equipBtn != null)
                m_equipBtn.gameObject.SetActive(equipmentinfo != null);

            if (m_equipBtnText != null)
                m_equipBtnText.gameObject.SetActive(equipmentinfo != null);

            if (equipmentinfo != null)
            {
                if (m_levelUPBtnText != null)
                    m_levelUPBtnText.text = Managers.PlayerDataManager.Instance.GetNeedLevelUPEquipmentSton(equipmentdata, equipmentinfo).ToString();

                bool equipstate = Managers.PlayerDataManager.Instance.IsEquipElement(equipmentdata);

                if (m_equipBtn != null)
                {
                    m_equipBtn.image.color = equipstate == true ? Color.gray : Color.white;
                    m_equipBtn.interactable = equipstate == false;
                }

                if (m_equipBtnText != null)
                    m_equipBtnText.text = equipstate == true ? "¿Â¬¯¡ﬂ" : "¿Â¬¯";
            }
        }
        //------------------------------------------------------------------------------------
        private void SetCombineUI(EquipmentData equipmentdata, PlayerEquipmentInfo equipmentinfo)
        {

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

        }
        //------------------------------------------------------------------------------------
        private void OnClick_AfterEquipment()
        {

        }
        //------------------------------------------------------------------------------------
        private void OnClick_LevelUPBtn()
        {

        }
        //------------------------------------------------------------------------------------
        private void OnClick_equipBtn()
        {

        }
        //------------------------------------------------------------------------------------
    }
}