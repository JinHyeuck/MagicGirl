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
        private float m_pointDownTurm = 0.1f;
        private float m_pointDownTurmTimer = 0.0f;

        private bool m_isPointDown = false;

        //------------------------------------------------------------------------------------
        private void Start()
        {
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
            Debug.LogError(string.Format("pointDown {0}", baseEventData.ToString()));
        }
        //------------------------------------------------------------------------------------
        private void OnPointUp_UpGrade(BaseEventData baseEventData)
        {
            m_isPointDown = false;
            if(m_pointDownStartTime + m_pointDownDelay > Time.time)
                UpGrade();

            Debug.LogError(string.Format("pointUp {0}", baseEventData.ToString()));
        }
        //------------------------------------------------------------------------------------
        private void UpGrade()
        {
            Debug.LogWarning("업그레이드ㄱㄱ");
        }
    }
}