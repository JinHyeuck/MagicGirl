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
            Message.AddListener<GameBerry.Event.SetSkillSlotMsg>(SetSlot);

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
            Message.RemoveListener<GameBerry.Event.SetSkillSlotMsg>(SetSlot);
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

            element.SetSkillElement(data, Managers.PlayerDataManager.Instance.GetPlayerSkillInfo(data));
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
        private void SetSlot(GameBerry.Event.SetSkillSlotMsg msg)
        {
            for (int i = 0; i < m_equipSkillElement.Count; ++i)
            {
                m_equipSkillElement[i].SetEquipElement(false);
            }

            IEnumerator enumerator = msg.SkillSlot.Values.GetEnumerator();

            while (enumerator.MoveNext() == true)
            {
                int skillid = (int)enumerator.Current;
                UISkillElement element = null;

                if (m_skillElement_Dic.TryGetValue(skillid, out element) == true)
                {
                    element.SetEquipElement(true);
                    m_equipSkillElement.Add(element);
                }
            }
        }
        //------------------------------------------------------------------------------------
        private void OnClick_SkillSlotPage(int pageid)
        {
            Managers.SkillManager.Instance.ChangeSkillSlotPage(pageid);
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