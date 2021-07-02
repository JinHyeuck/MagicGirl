using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry.UI
{
    public class UIEquipmentPopup : MonoBehaviour
    {
        [SerializeField]
        private Button m_changeTab_LevelUpBtn;

        [SerializeField]
        private Button m_changeTabComposeBtn;

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
        private Button m_mountBtn;

        [SerializeField]
        private Text m_mountBtnText;



        //------------------------------------------------------------------------------------
        public void Init()
        {

        }
        //------------------------------------------------------------------------------------
        public void SetEquipment(EquipmentData equipmentdata, PlayerEquipmentInfo equipmentinfo)
        { 

        }
        //------------------------------------------------------------------------------------
    }
}