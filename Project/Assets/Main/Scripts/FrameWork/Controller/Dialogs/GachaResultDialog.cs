using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry.UI
{
    public class GachaResultDialog : IDialog
    {
        [SerializeField]
        private Button m_exitBtn;

        [SerializeField]
        private Transform m_elementRoot;

        [SerializeField]
        private UIEquipmentElement m_equipmentElement;

        [SerializeField]
        private UISkillElement m_skillElement;

        private List<UIEquipmentElement> m_equipmentElement_List = new List<UIEquipmentElement>();

        private List<UISkillElement> m_skillElement_List = new List<UISkillElement>();

        //------------------------------------------------------------------------------------
        protected override void OnLoad()
        {
            Message.AddListener<GameBerry.Event.ResultGachaMsg>(ResultGacha);

            if (m_exitBtn != null)
                m_exitBtn.onClick.AddListener(() =>
                {
                    RequestDialogExit<GachaResultDialog>();
                });
        }
        //------------------------------------------------------------------------------------
        protected override void OnUnload()
        {
            Message.RemoveListener<GameBerry.Event.ResultGachaMsg>(ResultGacha);
        }
        //------------------------------------------------------------------------------------
        private void ResultGacha(GameBerry.Event.ResultGachaMsg msg)
        {
            SetEquipmentElements(msg.GachaEquipmentList);
            SetSkillElements(msg.GachaSkillList);
        }
        //------------------------------------------------------------------------------------
        private void SetEquipmentElements(List<EquipmentData> datalist)
        {
            if (datalist == null)
                return;

            int selectindex = -1;

            for (int i = 0; i < datalist.Count; ++i)
            {
                if (m_equipmentElement_List.Count <= i)
                    CreateEquipmentElement();

                SetEquipmentElement(m_equipmentElement_List[i], datalist[i]);
                m_equipmentElement_List[i].SetEquipElement(Managers.PlayerDataManager.Instance.IsEquipElement(datalist[i]));
                m_equipmentElement_List[i].gameObject.SetActive(true);

                selectindex = i;
            }

            for (int i = selectindex + 1; i < m_equipmentElement_List.Count; ++i)
            {
                m_equipmentElement_List[i].gameObject.SetActive(false);
            }
        }
        //------------------------------------------------------------------------------------
        private void SetEquipmentElement(UIEquipmentElement element, EquipmentData data)
        {
            if (element == null || data == null)
                return;

            element.SetEquipmentElement(data, Managers.PlayerDataManager.Instance.GetPlayerEquipmentInfo(data.Type, data.Id));
        }
        //------------------------------------------------------------------------------------
        private void CreateEquipmentElement()
        {
            GameObject clone = Instantiate(m_equipmentElement.gameObject, m_elementRoot.transform);
            UIEquipmentElement element = clone.GetComponent<UIEquipmentElement>();

            m_equipmentElement_List.Add(element);
        }
        //------------------------------------------------------------------------------------
        private void SetSkillElements(List<SkillData> datalist)
        {
            if (datalist == null)
                return;


            int selectindex = -1;

            for (int i = 0; i < datalist.Count; ++i)
            {
                if (m_skillElement_List.Count <= i)
                    CreateSkillElement();

                SetSkillElement(m_skillElement_List[i], datalist[i]);
                m_skillElement_List[i].SetEquipElement(false);
                m_skillElement_List[i].gameObject.SetActive(true);

                selectindex = i;
            }

            for (int i = selectindex + 1; i < m_skillElement_List.Count; ++i)
            {
                m_skillElement_List[i].gameObject.SetActive(false);
            }
        }
        //------------------------------------------------------------------------------------
        private void SetSkillElement(UISkillElement element, SkillData data)
        {
            if (element == null || data == null)
                return;

            element.SetSkillElement(data, Managers.PlayerDataManager.Instance.GetPlayerSkillInfo(data));
        }
        //------------------------------------------------------------------------------------
        private void CreateSkillElement()
        {
            GameObject clone = Instantiate(m_skillElement.gameObject, m_elementRoot.transform);
            UISkillElement element = clone.GetComponent<UISkillElement>();

            m_skillElement_List.Add(element);
        }
        //------------------------------------------------------------------------------------
    }
}