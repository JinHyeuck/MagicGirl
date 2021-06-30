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

        private EquipmentLocalChart m_equipmentLocalChart = null;

        private EquipmentType m_currentEquipmentType = EquipmentType.Weapon;

        //------------------------------------------------------------------------------------
        protected override void OnLoad()
        {
            m_equipmentLocalChart = Managers.TableManager.Instance.GetTableClass<EquipmentLocalChart>();


        }
        //------------------------------------------------------------------------------------
        private void ChangeEquipmentType(EquipmentType type)
        {
            m_currentEquipmentType = type;
        }
        //------------------------------------------------------------------------------------
        private void SetElementUI()
        { 

        }
        //------------------------------------------------------------------------------------
    }
}