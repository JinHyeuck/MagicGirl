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
        private Dictionary<int, UIEquipmentElement> m_equipmentElement_Dic = new Dictionary<int, UIEquipmentElement>();

        private EquipmentLocalChart m_equipmentLocalChart = null;
        private EquipmentType m_currentEquipmentType = EquipmentType.Weapon;

        GameBerry.Event.SetEquipmentPopupMsg m_setEquipmentPopupMsg = new GameBerry.Event.SetEquipmentPopupMsg();

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

            Message.AddListener<GameBerry.Event.ChangeEquipElementMsg>(ChangeEquipElement);
            Message.AddListener<GameBerry.Event.RefrashEquipmentInfoListMsg>(RefrashEquipmentInfoList);

            SetElements(m_currentEquipmentType);
        }
        //------------------------------------------------------------------------------------
        protected override void OnUnload()
        {
            Message.RemoveListener<GameBerry.Event.ChangeEquipElementMsg>(ChangeEquipElement);
            Message.RemoveListener<GameBerry.Event.RefrashEquipmentInfoListMsg>(RefrashEquipmentInfoList);
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

            m_equipmentElement_Dic.Clear();

            int selectindex = 0;

            for (int i = 0; i < datalist.Count; ++i)
            {
                if (m_equipmentElement_List.Count <= i)
                    CreateEquipmentElement();

                SetElement(m_equipmentElement_List[i], datalist[i]);
                m_equipmentElement_List[i].SetEquipElement(Managers.PlayerDataManager.Instance.IsEquipElement(datalist[i]));
                m_equipmentElement_List[i].gameObject.SetActive(true);

                m_equipmentElement_Dic.Add(datalist[i].Id, m_equipmentElement_List[i]);

                selectindex = i;
            }

            for (int i = selectindex + 1; i < m_equipmentElement_List.Count; ++i)
            {
                m_equipmentElement_List[i].gameObject.SetActive(false);
            }
        }
        //------------------------------------------------------------------------------------
        private void SetElement(UIEquipmentElement element, EquipmentData data)
        {
            if (element == null || data == null)
                return;

            element.SetEquipmentElement(data, Managers.PlayerDataManager.Instance.GetPlayerEquipmentInfo(data.Type, data.Id));
        }
        //------------------------------------------------------------------------------------
        private void ChangeEquipElement(GameBerry.Event.ChangeEquipElementMsg msg)
        {
            UIEquipmentElement element = null;

            if (m_equipmentElement_Dic.TryGetValue(msg.BeforeEquipmentID, out element) == true)
                element.SetEquipElement(false);

            if (m_equipmentElement_Dic.TryGetValue(msg.AfterEquipmentID, out element) == true)
                element.SetEquipElement(true);
        }
        //------------------------------------------------------------------------------------
        private void RefrashEquipmentInfoList(GameBerry.Event.RefrashEquipmentInfoListMsg msg)
        {
            for (int i = 0; i < msg.datas.Count; ++i)
            {
                if (m_currentEquipmentType == msg.datas[i].Type)
                {
                    SetElement(m_equipmentElement_Dic[msg.datas[i].Id], msg.datas[i]);
                }
            }
        }
        //------------------------------------------------------------------------------------
        private void OnElementCallBack(EquipmentData equipmentData, PlayerEquipmentInfo playerEquipmentInfo)
        {
            m_setEquipmentPopupMsg.equipmentdata = equipmentData;
            m_setEquipmentPopupMsg.equipmentinfo = playerEquipmentInfo;

            Message.Send(m_setEquipmentPopupMsg);

            RequestDialogEnter<EquipmentPopupDialog>();
        }
        //------------------------------------------------------------------------------------
    }
}