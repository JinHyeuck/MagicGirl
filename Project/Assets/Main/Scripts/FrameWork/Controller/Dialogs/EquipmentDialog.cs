using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry.UI
{
    public class EquipmentDialog : IDialog
    {
        [Header("---------ChangeTab---------")]
        [SerializeField]
        private Button m_changeTab_Weapon;

        [SerializeField]
        private Button m_changeTab_Ring;

        [SerializeField]
        private Button m_changeTab_Necklace;

        [Header("---------UpGradeContent---------")]
        [SerializeField]
        private RectTransform m_equipmentRoot;

        [Header("---------------------------")]
        [SerializeField]
        private UIEquipmentElement m_equipmentElement;

        private List<UIEquipmentElement> m_equipmentElement_List = new List<UIEquipmentElement>();

        private EquipmentLocalChart m_equipmentLocalChart = null;
        private EquipmentType m_currentEquipmentType = EquipmentType.Weapon;

        //------------------------------------------------------------------------------------
        protected override void OnLoad()
        {
            m_equipmentLocalChart = Managers.TableManager.Instance.GetTableClass<EquipmentLocalChart>();

            CreateEquipmentElement(m_currentEquipmentType);
        }
        //------------------------------------------------------------------------------------
        private void ChangeEquipmentType(EquipmentType type)
        {
            m_currentEquipmentType = type;
        }
        //------------------------------------------------------------------------------------
        private void CreateEquipmentElement(EquipmentType type)
        {
            List<EquipmentData> datalist = m_equipmentLocalChart.GetEquipmentDataList(m_currentEquipmentType);
            if (datalist == null)
                return;

            for (int i = 0; i < datalist.Count; ++i)
            {
                GameObject clone = Instantiate(m_equipmentElement.gameObject, m_equipmentRoot.transform);
                UIEquipmentElement element = clone.GetComponent<UIEquipmentElement>();
                element.SetEquipmentElement(datalist[i], Managers.PlayerDataManager.Instance.GetPlayerEquipmentInfo(type, datalist[i].Id));

                m_equipmentElement_List.Add(element);
            }
        }
        //------------------------------------------------------------------------------------
        private void SetElementUI()
        { 

        }
        //------------------------------------------------------------------------------------
    }
}