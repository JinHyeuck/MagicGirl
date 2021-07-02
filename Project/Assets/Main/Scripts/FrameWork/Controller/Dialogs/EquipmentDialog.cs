using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry.UI
{
    [System.Serializable]
    public class EquipmentChangeTabBtn
    {
        public Button ChangeTabBtn;
        public EquipmentType ButtonType;

        private System.Action<EquipmentType> CallBack;

        public void SetCallBack(System.Action<EquipmentType> callback)
        {
            CallBack = callback;
        }
        public void OnClick()
        {
            if (CallBack != null)
                CallBack(ButtonType);
        }
    }

    public class EquipmentDialog : IDialog
    {
        [Header("---------ChangeTab---------")]
        [SerializeField]
        private Button m_changeTab_Weapon;

        [SerializeField]
        private Button m_changeTab_Ring;

        [SerializeField]
        private Button m_changeTab_Necklace;

        [SerializeField]
        private List<EquipmentChangeTabBtn> m_changeBtnList = new List<EquipmentChangeTabBtn>();

        [Header("---------UpGradeContent---------")]
        [SerializeField]
        private RectTransform m_equipmentRoot;

        [Header("---------------------------")]
        [SerializeField]
        private UIEquipmentElement m_equipmentElement;

        private List<UIEquipmentElement> m_equipmentElement_List = new List<UIEquipmentElement>();

        [SerializeField]
        private UIEquipmentPopup m_uIEquipmentPopup;

        private UIEquipmentPopup m_uIEquipmentPopupInstance;

        private EquipmentLocalChart m_equipmentLocalChart = null;
        private EquipmentType m_currentEquipmentType = EquipmentType.Weapon;

        //------------------------------------------------------------------------------------
        protected override void OnLoad()
        {
            m_equipmentLocalChart = Managers.TableManager.Instance.GetTableClass<EquipmentLocalChart>();

            for (int i = 0; i < m_changeBtnList.Count; ++i)
            {
                if (m_changeBtnList[i].ChangeTabBtn != null)
                {
                    m_changeBtnList[i].SetCallBack(OnClick_ChangeBtn);
                    m_changeBtnList[i].ChangeTabBtn.onClick.AddListener(m_changeBtnList[i].OnClick);
                }
            }

            SetElements(m_currentEquipmentType);
        }
        //------------------------------------------------------------------------------------
        private void OnClick_ChangeBtn(EquipmentType type)
        {
            if (m_currentEquipmentType == type)
                return;

            ChangeEquipmentType(type);
        }
        //------------------------------------------------------------------------------------
        private void ChangeEquipmentType(EquipmentType type)
        {
            m_currentEquipmentType = type;

            SetElements(m_currentEquipmentType);
        }
        //------------------------------------------------------------------------------------
        private void CreateEquipmentElement()
        {
            GameObject clone = Instantiate(m_equipmentElement.gameObject, m_equipmentRoot.transform);
            UIEquipmentElement element = clone.GetComponent<UIEquipmentElement>();
            element.Init(OnElementCallBack);

            m_equipmentElement_List.Add(element);
        }
        //------------------------------------------------------------------------------------
        private void SetElements(EquipmentType type)
        {
            List<EquipmentData> datalist = m_equipmentLocalChart.GetEquipmentDataList(type);
            if (datalist == null)
                return;

            int selectindex = 0;

            for (int i = 0; i < datalist.Count; ++i)
            {
                if (m_equipmentElement_List.Count <= i)
                    CreateEquipmentElement();

                m_equipmentElement_List[i].SetEquipmentElement(datalist[i], Managers.PlayerDataManager.Instance.GetPlayerEquipmentInfo(type, datalist[i].Id));
                m_equipmentElement_List[i].gameObject.SetActive(true);

                selectindex = i;
            }

            for (int i = selectindex + 1; i < m_equipmentElement_List.Count; ++i)
            {
                m_equipmentElement_List[i].gameObject.SetActive(false);
            }
        }
        //------------------------------------------------------------------------------------
        private void OnElementCallBack(EquipmentData equipmentData, PlayerEquipmentInfo playerEquipmentInfo)
        { 

        }
        //------------------------------------------------------------------------------------
    }
}