using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry.UI
{
    [System.Serializable]
    public class SkillPageTabBtn
    {
        public Button ChangeTabBtn;
        public int ButtonID;

        private System.Action<int> CallBack;

        public void SetCallBack(System.Action<int> callback)
        {
            CallBack = callback;

            if (ChangeTabBtn != null)
                ChangeTabBtn.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            if (CallBack != null)
                CallBack(ButtonID);
        }
    }

    public class SkillDialog : IDialog
    {
        [Header("---------SkillContent---------")]
        [SerializeField]
        private RectTransform m_skillRoot;

        [SerializeField]
        private List<SkillPageTabBtn> m_skillPageBtnList = new List<SkillPageTabBtn>();

        [SerializeField]
        private Color m_skillPageApplyColor = Color.white;

        [SerializeField]
        private Color m_skillPageNotApplyColor = Color.white;

        [Header("---------------------------")]
        [SerializeField]
        private UISkillElement m_skillElement;

        private List<UISkillElement> m_skillElement_List = new List<UISkillElement>();
        private Dictionary<int, UISkillElement> m_skillElement_Dic = new Dictionary<int, UISkillElement>();
        private List<UISkillElement> m_equipSkillElement = new List<UISkillElement>();

        private SkillLocalChart m_skillLocalChart = null;

        private GameBerry.Event.SetSkillPopupMsg m_setSkillPopupMsg = new GameBerry.Event.SetSkillPopupMsg();

        //------------------------------------------------------------------------------------
        protected override void OnLoad()
        {
            Message.AddListener<GameBerry.Event.RefreshSkillInfoListMsg>(RefreshSkillInfoList);
            Message.AddListener<GameBerry.Event.ChangeEquipSkillMsg>(ChangeEquipSkill);
            Message.AddListener<GameBerry.Event.ChangeSkillSlotPageMsg>(ChangeSkillSlotPage);

            for (int i = 0; i < m_skillPageBtnList.Count; ++i)
            {
                m_skillPageBtnList[i].SetCallBack(OnClick_SkillSlotPage);
            }

            m_skillLocalChart = Managers.TableManager.Instance.GetTableClass<SkillLocalChart>();

            SetElements();
        }
        //------------------------------------------------------------------------------------
        protected override void OnUnload()
        {
            Message.RemoveListener<GameBerry.Event.RefreshSkillInfoListMsg>(RefreshSkillInfoList);
            Message.RemoveListener<GameBerry.Event.ChangeEquipSkillMsg>(ChangeEquipSkill);
            Message.RemoveListener<GameBerry.Event.ChangeSkillSlotPageMsg>(ChangeSkillSlotPage);
        }
        //------------------------------------------------------------------------------------
        private void SetElements()
        {
            List<SkillData> datalist = m_skillLocalChart.m_SkillDatas;
            if (datalist == null)
                return;

            m_skillElement_Dic.Clear();

            int selectindex = 0;

            for (int i = 0; i < datalist.Count; ++i)
            {
                if (m_skillElement_List.Count <= i)
                    CreateSkillElement();

                SetElement(m_skillElement_List[i], datalist[i]);
                m_skillElement_List[i].SetEquipElement(false);
                m_skillElement_List[i].gameObject.SetActive(true);

                m_skillElement_Dic.Add(datalist[i].SkillID, m_skillElement_List[i]);

                selectindex = i;
            }

            for (int i = selectindex + 1; i < m_skillElement_List.Count; ++i)
            {
                m_skillElement_List[i].gameObject.SetActive(false);
            }
        }
        //------------------------------------------------------------------------------------
        private void CreateSkillElement()
        {
            GameObject clone = Instantiate(m_skillElement.gameObject, m_skillRoot.transform);
            UISkillElement element = clone.GetComponent<UISkillElement>();
            element.Init(OnElementCallBack);

            m_skillElement_List.Add(element);
        }
        //------------------------------------------------------------------------------------
        private void SetElement(UISkillElement element, SkillData data)
        {
            if (element == null || data == null)
                return;

            element.SetSkillElement(data, Managers.SkillDataManager.Instance.GetPlayerSkillInfo(data));
        }
        //------------------------------------------------------------------------------------
        private void RefreshSkillInfoList(GameBerry.Event.RefreshSkillInfoListMsg msg)
        {
            for (int i = 0; i < msg.datas.Count; ++i)
            {
                SetElement(m_skillElement_Dic[msg.datas[i].SkillID], msg.datas[i]);
            }
        }
        //------------------------------------------------------------------------------------
        private void ChangeEquipSkill(GameBerry.Event.ChangeEquipSkillMsg msg)
        {
            for (int i = 0; i < m_equipSkillElement.Count; ++i)
            {
                m_equipSkillElement[i].SetEquipElement(false);
            }

            m_equipSkillElement.Clear();

            for (int i = 0; i < msg.EquipSkillList.Count; ++i)
            {
                UISkillElement element = null;

                if (m_skillElement_Dic.TryGetValue(msg.EquipSkillList[i], out element) == true)
                {
                    element.SetEquipElement(true);
                    m_equipSkillElement.Add(element);
                }
            }
        }
        //------------------------------------------------------------------------------------
        private void OnClick_SkillSlotPage(int pageid)
        {
            Managers.SkillSlotManager.Instance.ChangeSkillSlotPage(pageid);
        }
        //------------------------------------------------------------------------------------
        private void ChangeSkillSlotPage(GameBerry.Event.ChangeSkillSlotPageMsg msg)
        {
            for (int i = 0; i < m_skillPageBtnList.Count; ++i)
            {
                if (m_skillPageBtnList[i].ChangeTabBtn != null)
                {
                    m_skillPageBtnList[i].ChangeTabBtn.image.color = m_skillPageBtnList[i].ButtonID == msg.SkillPageID ? m_skillPageApplyColor : m_skillPageNotApplyColor;
                }
            }
        }
        //------------------------------------------------------------------------------------
        private void OnElementCallBack(SkillData skillData, PlayerSkillInfo playerSkillInfo)
        {
            m_setSkillPopupMsg.skilldata = skillData;
            m_setSkillPopupMsg.skillinfo = playerSkillInfo;

            Message.Send(m_setSkillPopupMsg);

            RequestDialogEnter<SkillPopupDialog>();
        }
        //------------------------------------------------------------------------------------
    }
}