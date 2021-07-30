using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GameBerry.UI
{
    public class UIUpGradeElement : MonoBehaviour
    {
        [SerializeField]
        private Image m_upGradeIcon;

        [SerializeField]
        private Text m_upGradeTitle;

        [SerializeField]
        private Text m_upGradeMaxLevel;

        [SerializeField]
        private Text m_upGradeCurrentLevel;

        [SerializeField]
        private Text m_upGradeState;

        [SerializeField]
        private Text m_upGradePrice;

        [SerializeField]
        private EventTrigger m_eventTrigger;
        private EventTrigger.Entry m_pointDownEntry;
        private EventTrigger.Entry m_pointUpEntry;

        private float m_pointDownStartTime = 0.0f;
        private float m_pointDownDelay = 0.5f;
        private float m_pointDownTurm = 0.01f;
        private float m_pointDownTurmTimer = 0.0f;

        private bool m_isPointDown = false;

        private StatType m_isStatUpGradeType = StatType.Max;

        //------------------------------------------------------------------------------------
        public void Init(StatType type, Sprite icon, int maxlevel)
        {
            m_isStatUpGradeType = type;

            if (m_upGradeIcon != null)
                m_upGradeIcon.sprite = icon;

            if (m_upGradeMaxLevel != null)
                m_upGradeMaxLevel.text = maxlevel.ToString();

            if (m_upGradeTitle != null)
                m_upGradeTitle.text = type.ToString();

            SetCurrentLevel();
            SetUpGradeState();
            SetUpGradePrice();

            m_pointDownEntry = new EventTrigger.Entry();
            m_pointDownEntry.eventID = EventTriggerType.PointerDown;
            m_pointDownEntry.callback.AddListener(OnPointDown_UpGrade);

            m_pointUpEntry = new EventTrigger.Entry();
            m_pointUpEntry.eventID = EventTriggerType.PointerUp;
            m_pointUpEntry.callback.AddListener(OnPointUp_UpGrade);

            m_eventTrigger.triggers.Add(m_pointDownEntry);
            m_eventTrigger.triggers.Add(m_pointUpEntry);
        }
        //------------------------------------------------------------------------------------
        private void Update()
        {
            if (m_isPointDown == true)
            {
                if (m_pointDownStartTime + m_pointDownDelay < Time.time)
                {
                    if (m_pointDownTurmTimer > m_pointDownTurm)
                    {
                        UpGrade();
                        m_pointDownTurmTimer = 0.0f;
                    }
                    else
                    {
                        m_pointDownTurmTimer += Time.deltaTime;
                    }
                }
            }
        }
        //------------------------------------------------------------------------------------
        private void OnPointDown_UpGrade(BaseEventData baseEventData)
        {
            m_isPointDown = true;
            m_pointDownStartTime = Time.time;
            m_pointDownTurmTimer = 0.0f;
        }
        //------------------------------------------------------------------------------------
        private void OnPointUp_UpGrade(BaseEventData baseEventData)
        {
            m_isPointDown = false;
            if(m_pointDownStartTime + m_pointDownDelay > Time.time)
                UpGrade();
        }
        //------------------------------------------------------------------------------------
        private void UpGrade()
        {
            Debug.LogWarning("업그레이드ㄱㄱ");

            if (Managers.StatUpGradeDataManager.Instance.IsMaxUpGrade(m_isStatUpGradeType) == true)
            {
                Debug.Log("이미 만랩");
                return;
            }

            bool upgradeSuccess = Managers.StatUpGradeDataManager.Instance.InCreaseUpGradeStatLevel(m_isStatUpGradeType);

            if (upgradeSuccess == true)
            {
                SetCurrentLevel();
                SetUpGradeState();
                SetUpGradePrice();
            }
            else
            {
                Debug.Log("돈없으");
            }
        }
        //------------------------------------------------------------------------------------
        private void SetCurrentLevel()
        {
            if (m_upGradeCurrentLevel != null)
                m_upGradeCurrentLevel.text = Managers.StatUpGradeDataManager.Instance.GetCurrentUpGradeStatLevel(m_isStatUpGradeType).ToString();
        }
        //------------------------------------------------------------------------------------
        private void SetUpGradeState()
        {
            if (m_upGradeState != null)
            {
                string textstr = Managers.StatUpGradeDataManager.Instance.IsMaxUpGrade(m_isStatUpGradeType) == true ? Managers.StatUpGradeDataManager.Instance.GetCurrentUpGradeStatValue(m_isStatUpGradeType).ToString() : string.Format("{0} -> {1}", Managers.StatUpGradeDataManager.Instance.GetCurrentUpGradeStatValue(m_isStatUpGradeType), Managers.StatUpGradeDataManager.Instance.GetNextUpGradeStatValue(m_isStatUpGradeType));
                m_upGradeState.text = textstr;
            }
        }
        //------------------------------------------------------------------------------------
        private void SetUpGradePrice()
        {
            if (m_upGradePrice != null)
            {
                string textstr = Managers.StatUpGradeDataManager.Instance.IsMaxUpGrade(m_isStatUpGradeType) == true ? "MAX" : string.Format("{0:0}", Managers.StatUpGradeDataManager.Instance.GetUpGradeStatPrice(m_isStatUpGradeType));
                m_upGradePrice.text = textstr;
            }
        }
        //------------------------------------------------------------------------------------
    }
}