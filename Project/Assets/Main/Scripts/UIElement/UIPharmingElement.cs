using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry.UI
{
    public class UIPharmingElement : MonoBehaviour
    {
        [SerializeField]
        private Image m_pharmingIcon;

        [SerializeField]
        private Text m_pharmingText;

        private CanvasGroup m_canvasGroup;

        PharmingData m_pharmingData = null;
        System.Action<PharmingData> m_endCallback = null;

        private float m_directionWaitTime = 1.0f;
        private float m_alphaDirectionTime = 1.0f;

        private Coroutine m_directionCoroutine = null;

        //------------------------------------------------------------------------------------
        public void Init(System.Action<PharmingData> callback)
        {
            m_canvasGroup = GetComponent<CanvasGroup>();
            m_endCallback = callback;
        }
        //------------------------------------------------------------------------------------
        public void PlayDirection(PharmingData data)
        {
            if (m_directionCoroutine != null)
            {
                OnDirectionEnd();
                StopCoroutine(m_directionCoroutine);
            }

            if (m_canvasGroup != null)
                m_canvasGroup.alpha = 1.0f;

            m_pharmingData = data;

            if (m_pharmingIcon != null)
                m_pharmingIcon.sprite = m_pharmingData.RewardIcon;

            if (m_pharmingText != null)
                m_pharmingText.text = string.Format("+{0}", m_pharmingData.RewardCount);

            m_directionCoroutine = StartCoroutine(ElementDirection());
        }
        //------------------------------------------------------------------------------------
        private IEnumerator ElementDirection()
        {
            yield return new WaitForSeconds(m_directionWaitTime);

            float starttime = Time.time;

            while (starttime + m_alphaDirectionTime > Time.time)
            {
                if (m_canvasGroup != null && m_alphaDirectionTime > 0.0f)
                {
                    float alpha = m_canvasGroup.alpha;
                    alpha = (m_alphaDirectionTime - (Time.time - starttime)) / m_alphaDirectionTime;
                    m_canvasGroup.alpha = alpha;
                }

                yield return null;
            }

            if (m_canvasGroup != null)
                m_canvasGroup.alpha = 0.0f;

            m_directionCoroutine = null;
            OnDirectionEnd();
        }
        //------------------------------------------------------------------------------------
        private void OnDirectionEnd()
        {
            if (m_endCallback != null)
                m_endCallback(m_pharmingData);
        }
        //------------------------------------------------------------------------------------
    }
}