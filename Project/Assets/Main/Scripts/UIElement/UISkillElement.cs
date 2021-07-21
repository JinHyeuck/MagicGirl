using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry.UI
{
    public class UISkillElement : MonoBehaviour
    {
        [SerializeField]
        private Button m_elementbtn;

        [SerializeField]
        private Image m_gradeColorBG;

        [SerializeField]
        private Image m_skillIcon;

        [SerializeField]
        private Text m_skillLevelText;

        [SerializeField]
        private Transform m_equipMark;

        [SerializeField]
        private Image m_amountCountFilled;

        [SerializeField]
        private Text m_amountCountText;


        [Header("----------Color----------")]
        [SerializeField]
        private Color m_disableColor;

        //------------------------------------------------------------------------------------
        private SkillData m_currentSkillData = null;
        private PlayerSkillInfo m_currentSkillInfo = null;

        System.Action<SkillData, PlayerSkillInfo> m_callBack = null;

        //------------------------------------------------------------------------------------
        public void Init(System.Action<SkillData, PlayerSkillInfo> callback)
        {
            if (m_elementbtn != null)
                m_elementbtn.onClick.AddListener(OnClick_ElementBtn);

            m_callBack = callback;
        }
        //------------------------------------------------------------------------------------
        public void SetSkillElement(SkillData SkillData, PlayerSkillInfo equipmentinfo)
        {
            m_currentSkillData = SkillData;
            m_currentSkillInfo = equipmentinfo;

            if (m_gradeColorBG != null)
                m_gradeColorBG.color = Contents.GlobalContent.GetGradeColor(m_currentSkillData.SkillGradeType);

            if (m_skillIcon != null)
                m_skillIcon.color = equipmentinfo == null ? m_disableColor : Color.white;

            if (m_skillIcon != null)
                m_skillIcon.sprite = SkillData.SkillSprite;

            if (m_skillLevelText != null)
            {
                if (equipmentinfo == null)
                    m_skillLevelText.gameObject.SetActive(false);
                else
                {
                    m_skillLevelText.gameObject.SetActive(true);
                    m_skillLevelText.text = string.Format("Lv.{0}", equipmentinfo.Level);
                }
            }

            int CurrentAmount = equipmentinfo == null ? 0 : equipmentinfo.Count;

            int LevelUpAmount = Managers.PlayerDataManager.Instance.GetNeedLevelUPSkillCount(SkillData);

            if (m_amountCountFilled != null)
            {
                float ratio = (float)CurrentAmount / (float)LevelUpAmount;
                m_amountCountFilled.fillAmount = ratio;
            }

            if (m_amountCountText != null)
                m_amountCountText.text = string.Format("{0}/{1}", CurrentAmount, LevelUpAmount);
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
                m_callBack(m_currentSkillData, m_currentSkillInfo);
        }
        //------------------------------------------------------------------------------------
    }
}

