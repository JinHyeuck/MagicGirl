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

    public class SkillDialog : IDialog
    {
        [Header("---------SkillContent---------")]
        [SerializeField]
        private RectTransform m_skillRoot;

        [Header("---------------------------")]
        [SerializeField]
        private UISkillElement m_skillElement;

        private List<UISkillElement> m_skillElement_List = new List<UISkillElement>();
        private Dictionary<int, UISkillElement> m_skillElement_Dic = new Dictionary<int, UISkillElement>();

        private SkillLocalChart m_skillLocalChart = null;

        //------------------------------------------------------------------------------------
        protected override void OnLoad()
        {
            Message.AddListener<GameBerry.Event.RefrashSkillInfoListMsg>(RefrashSkillInfoList);

            m_skillLocalChart = Managers.TableManager.Instance.GetTableClass<SkillLocalChart>();

            SetElements();
        }
        //------------------------------------------------------------------------------------
        protected override void OnUnload()
        {
            Message.RemoveListener<GameBerry.Event.RefrashSkillInfoListMsg>(RefrashSkillInfoList);
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
        private void RefrashSkillInfoList(GameBerry.Event.RefrashSkillInfoListMsg msg)
        {
            for (int i = 0; i < msg.datas.Count; ++i)
            {
                SetElement(m_skillElement_Dic[msg.datas[i].SkillID], msg.datas[i]);
            }
        }
        //------------------------------------------------------------------------------------
        private void OnElementCallBack(SkillData skillData, PlayerSkillInfo playerSkillInfo)
        {

        }
        //------------------------------------------------------------------------------------
    }
}