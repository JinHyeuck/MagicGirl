using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry.UI
{
    public class Popup_OkCancel : MonoBehaviour
    {
        [SerializeField]
        private Transform m_contents;

        [SerializeField]
        private Text m_titleText = null;

        [SerializeField]
        private Button m_okBtn = null;

        [SerializeField]
        private Text m_okText = null;

        [SerializeField]
        private Button m_cancelBtn = null;

        [SerializeField]
        private Text m_cancelText = null;

        private System.Action m_okAction = null;

        private System.Action m_cancelAction = null;

        private System.Action<Popup_OkCancel> m_endHideAction = null;

        private Vector3 originOkBtnPos = Vector3.zero;

        private Coroutine m_directionCoroutine = null;

        private List<Graphic> m_colorDirList = new List<Graphic>();

        private Vector3 m_minSize = 0.4f.ToVector3();

        private Vector3 m_maxSize = 1.0f.ToVector3();

        //------------------------------------------------------------------------------------
        public void Init()
        {
            if (m_okBtn)
            {
                originOkBtnPos = m_okBtn.transform.localPosition;
                m_okBtn.onClick.AddListener(OnClick_OkBtn);
            }

            if (m_cancelBtn)
                m_cancelBtn.onClick.AddListener(OnClick_cancelBtn);

            if (m_contents != null)
                m_colorDirList = m_contents.GetComponentsInAllChildren<Graphic>();
        }
        //------------------------------------------------------------------------------------
        public void Show(string titletext, System.Action okAction, System.Action<Popup_OkCancel> endHideAction)
        {
            if (m_titleText != null)
                m_titleText.text = titletext;

            m_okAction = okAction;
            m_endHideAction = endHideAction;

            if (m_okBtn != null)
            {
                m_okBtn.gameObject.SetActive(true);
                Vector3 pos = m_okBtn.transform.localPosition;
                pos.x = 0.0f;
                m_okBtn.transform.localPosition = pos;
            }

            if (m_cancelBtn != null)
                m_cancelBtn.gameObject.SetActive(false);
            
            if (m_directionCoroutine == null)
                m_directionCoroutine = StartCoroutine(ShowDir());
        }
        //------------------------------------------------------------------------------------
        public void Show(string titletext, System.Action okAction, System.Action cancelAction, System.Action<Popup_OkCancel> endHideAction)
        {
            if (m_titleText != null)
                m_titleText.text = titletext;

            m_okAction = okAction;
            m_cancelAction = cancelAction;
            m_endHideAction = endHideAction;

            if (m_okBtn != null)
            {
                m_okBtn.gameObject.SetActive(true);
                m_okBtn.transform.localPosition = originOkBtnPos;
            }

            if (m_cancelBtn != null)
                m_cancelBtn.gameObject.SetActive(true);

            if (m_directionCoroutine == null)
                m_directionCoroutine = StartCoroutine(ShowDir());
        }
        //------------------------------------------------------------------------------------
        private IEnumerator ShowDir()
        {
            if (m_contents == null)
                yield break;

            float duration = 0.2f;
            float starttime = Time.time;
            float endtime = starttime + duration;

            float ratio = 0.0f;

            m_contents.localScale = m_minSize;

            Vector3 sizeGab = m_maxSize - m_minSize;

            Color color = new Color();

            for (int i = 0; i < m_colorDirList.Count; ++i)
            {
                color = m_colorDirList[i].color;
                color.a = 0.0f;
                m_colorDirList[i].color = color;
            }

            while (Time.time <= endtime)
            {
                ratio = MathDatas.Sin((90.0f * ((Time.time - starttime) / duration)));

                m_contents.localScale = m_minSize + (sizeGab * ratio);

                for (int i = 0; i < m_colorDirList.Count; ++i)
                {
                    color = m_colorDirList[i].color;
                    color.a = ratio;
                    m_colorDirList[i].color = color;
                }

                yield return null;
            }

            m_contents.localScale = m_maxSize;

            for (int i = 0; i < m_colorDirList.Count; ++i)
            {
                color = m_colorDirList[i].color;
                color.a = 1.0f;
                m_colorDirList[i].color = color;
            }

            m_directionCoroutine = null;
        }
        //------------------------------------------------------------------------------------
        private void Hide()
        {
            m_okAction = null;
            m_cancelAction = null;

            if (m_directionCoroutine == null)
                m_directionCoroutine = StartCoroutine(HideDir());
        }
        //------------------------------------------------------------------------------------
        private IEnumerator HideDir()
        {
            if (m_contents == null)
                yield break;

            float duration = 0.1f;
            float starttime = Time.time;
            float endtime = starttime + duration;

            float ratio = 0.0f;

            m_contents.localScale = m_maxSize;

            Vector3 sizeGab = m_maxSize;

            Color color = new Color();

            for (int i = 0; i < m_colorDirList.Count; ++i)
            {
                color = m_colorDirList[i].color;
                color.a = 1.0f;
                m_colorDirList[i].color = color;
            }

            while (Time.time <= endtime)
            {
                ratio = MathDatas.Sin((90.0f * ((Time.time - starttime) / duration)));

                m_contents.localScale = m_maxSize - (sizeGab * ratio);

                for (int i = 0; i < m_colorDirList.Count; ++i)
                {
                    color = m_colorDirList[i].color;
                    color.a = 1.0f - ratio;
                    m_colorDirList[i].color = color;
                }

                yield return null;
            }

            m_contents.localScale = 0.0f.ToVector3();

            for (int i = 0; i < m_colorDirList.Count; ++i)
            {
                color = m_colorDirList[i].color;
                color.a = 0.0f;
                m_colorDirList[i].color = color;
            }

            if (m_endHideAction != null)
                m_endHideAction(this);

            m_endHideAction = null;

            m_directionCoroutine = null;
        }
        //------------------------------------------------------------------------------------
        private void OnClick_OkBtn()
        {
            if (m_okAction != null)
                m_okAction();

            Hide();
        }
        //------------------------------------------------------------------------------------
        private void OnClick_cancelBtn()
        {
            if (m_cancelAction != null)
                m_cancelAction();

            Hide();
        }
        //------------------------------------------------------------------------------------
    }
}