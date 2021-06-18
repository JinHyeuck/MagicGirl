using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GameBerry.UI
{
    public class GlobalFadeDialog : IDialog
    {
        [SerializeField]
        private Image m_fade;

        protected override void OnLoad()
        {
            Message.AddListener<GameBerry.Event.DoFadeMsg>(DoFade);
        }

        protected override void OnUnload()
        {
            Message.RemoveListener<GameBerry.Event.DoFadeMsg>(DoFade);
        }

        private void DoFade(GameBerry.Event.DoFadeMsg msg)
        {
            if (m_fade != null)
            {
                m_fade.gameObject.SetActive(true);

                if (msg.visible == false)
                {
                    Color color = m_fade.color;
                    color.a = 0.0f;
                    m_fade.color = color;

                    m_fade.DOFade(1.0f, msg.duration);
                }
                else if (msg.visible == true)
                {
                    Color color = m_fade.color;
                    color.a = 1.0f;
                    m_fade.color = color;

                    m_fade.DOFade(0.0f, msg.duration).OnComplete(() =>
                    {
                        m_fade.gameObject.SetActive(false);
                    });
                }
            }
        }
    }
}