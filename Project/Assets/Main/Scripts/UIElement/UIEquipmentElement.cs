using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry.UI
{
    public class UIEquipmentElement : MonoBehaviour
    {
        [SerializeField]
        private Button m_elementbtn;

        [SerializeField]
        private Image m_gradeColorBG;

        [SerializeField]
        private Image m_equipmentIcon;

        [SerializeField]
        private Text m_equipmentLevelText;

        [SerializeField]
        private Transform m_equipMark;

        [SerializeField]
        private Text m_equipmentQuailtyText;

        [SerializeField]
        private Image m_amountCountFilled;

        [SerializeField]
        private Text m_amountCountText;


        [Header("----------Color----------")]
        [SerializeField]
        private Color m_disableColor;

        //------------------------------------------------------------------------------------
        private EquipmentData m_currentEquipmentData = null;
        private PlayerEquipmentInfo m_currentEquipmentInfo = null;

        System.Action<EquipmentData, PlayerEquipmentInfo> m_callBack = null;

        //------------------------------------------------------------------------------------
        public void Init(System.Action<EquipmentData, PlayerEquipmentInfo> callback)
        {
            if (m_elementbtn != null)
                m_elementbtn.onClick.AddListener(OnClick_ElementBtn);

            m_callBack = callback;
        }
        //------------------------------------------------------------------------------------
        public void SetEquipmentElement(EquipmentData equipmentdata, PlayerEquipmentInfo equipmentinfo)
        {
            m_currentEquipmentData = equipmentdata;
            m_currentEquipmentInfo = equipmentinfo;

            if (m_gradeColorBG != null)
                m_gradeColorBG.color = Contents.EquipmentContent.GetGradeColor(m_currentEquipmentData.Grade);

            if (m_equipmentIcon != null)
                m_equipmentIcon.color = equipmentinfo == null ? m_disableColor : Color.white;

            if (m_equipmentIcon != null)
                m_equipmentIcon.sprite = equipmentdata.EquipmentSprite;

            if (m_equipmentLevelText != null)
            {
                if (equipmentinfo == null)
                    m_equipmentLevelText.gameObject.SetActive(false);
                else
                {
                    if (equipmentinfo.Level > 0)
                    {
                        m_equipmentLevelText.gameObject.SetActive(true);
                        m_equipmentLevelText.text = string.Format("+{0}", equipmentinfo.Level);
                    }
                    else
                        m_equipmentLevelText.gameObject.SetActive(false);
                }
            }

            if (m_equipmentQuailtyText != null)
                m_equipmentQuailtyText.text = equipmentdata.Quality.ToString();

            int CurrentAmount = equipmentinfo == null ? 0 : equipmentinfo.Count;

            if (m_amountCountFilled != null)
            {
                float ratio = (float)CurrentAmount / (float)Define.EquipmentComposeAmount;
                m_amountCountFilled.fillAmount = ratio;
            }

            if (m_amountCountText != null)
                m_amountCountText.text = string.Format("{0}/{1}", CurrentAmount, Define.EquipmentComposeAmount);
        }
        //------------------------------------------------------------------------------------
        public void SetEquipElement(bool isequip)
        {
            if (m_equipMark != null)
                m_equipMark.gameObject.SetActive(isequip);
        }
        //------------------------------------------------------------------------------------
        private void OnClick_ElementBtn()
        {
            if (m_callBack != null)
                m_callBack(m_currentEquipmentData, m_currentEquipmentInfo);
        }
        //------------------------------------------------------------------------------------
    }
}