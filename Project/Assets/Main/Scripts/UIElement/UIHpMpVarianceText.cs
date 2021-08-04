using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GameBerry.UI
{
    public class UIHpMpVarianceText : MonoBehaviour
    {
        [SerializeField]
        private Text m_varianceText;

        //------------------------------------------------------------------------------------
        public void ShowVarianceText(string text, Color color, float upposvalue, float duration, float delay)
        {
            if (m_varianceText != null)
            { 
                m_varianceText.text = text;
                m_varianceText.color = color;
                m_varianceText.transform.localPosition = Vector3.zero;
                m_varianceText.transform.DOLocalMoveY(upposvalue, duration).SetDelay(delay);
                m_varianceText.DOFade(0.0f, duration).SetDelay(delay).OnComplete(()=>
                {
                    Managers.HPMPVarianceManager.Instance.PoolVarianceText(this);
                });
            }
        }
        //------------------------------------------------------------------------------------
        public void ResetVariance()
        {
            if (m_varianceText != null)
                m_varianceText.DOKill();
        }
        //------------------------------------------------------------------------------------
    }
}