using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry.UI
{
    public class Popup_Input : MonoBehaviour
    {
        [SerializeField]
        private Transform m_contents;

        [SerializeField]
        private Text m_titleText = null;

        [SerializeField]
        private InputField m_inputField = null;

        [SerializeField]
        private Text m_inputplaceholder = null;

        [SerializeField]
        private Text m_inputText = null;

        [SerializeField]
        private Button m_okBtn = null;

        [SerializeField]
        private Text m_okText = null;

        [SerializeField]
        private Button m_cancelBtn = null;

        [SerializeField]
        private Text m_cancelText = null;

        private string m_inputstr = string.Empty;

        private System.Action<string> m_okAction = null;

        private System.Action m_cancelAction = null;

        private System.Action<Popup_Input> m_endHideAction = null;

        private Coroutine m_directionCoroutine = null;

        private List<Graphic> m_colorDirList = new List<Graphic>();

        private Vector3 m_minSize = 0.4f.ToVector3();

        private Vector3 m_maxSize = 1.0f.ToVector3();

        //------------------------------------------------------------------------------------
        public void Init()
        {
            if (m_inputField)
            {
                m_inputField.onValueChanged.AddListener(OnChange_InputField);
            }

            if (m_okBtn)
                m_okBtn.onClick.AddListener(OnClick_OkBtn);

            if (m_cancelBtn)
                m_cancelBtn.onClick.AddListener(OnClick_cancelBtn);

            if(m_contents != null)
                m_colorDirList = m_contents.GetComponentsInAllChildren<Graphic>();
        }
        //------------------------------------------------------------------------------------
        private void OnChange_InputField(string text)
        {
            if (m_inputText != null)
                m_inputText.text = text;

            m_inputstr = text;
        }
        //------------------------------------------------------------------------------------
        public void Show(string titletext, string defaultstr, string placeholder, System.Action<string> okAction, System.Action cancelAction, System.Action<Popup_Input> endHideAction)
        {
            if (m_titleText != null)
                m_titleText.text = titletext;

            if (m_inputplaceholder != null)
                m_inputplaceholder.text = placeholder;

            if (string.IsNullOrEmpty(defaultstr) == false)
            {
                if (m_inputField != null)
                    m_inputField.text = defaultstr;

                if (m_inputText != null)
                    m_inputText.text = defaultstr;

                m_inputstr = defaultstr;
            }
            else
            {
                if (m_inputField != null)
                    m_inputField.text = string.Empty;

                if (m_inputText != null)
                    m_inputText.text = string.Empty;

                m_inputstr = string.Empty;
            }

            m_okAction = okAction;
            m_cancelAction = cancelAction;
            m_endHideAction = endHideAction;

            if (m_okBtn != null)
                m_okBtn.gameObject.SetActive(true);

            if (m_cancelBtn != null)
                m_cancelBtn.gameObject.SetActive(true);

            if(m_directionCoroutine == null)
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

            if(m_directionCoroutine == null)
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
                m_okAction(m_inputstr);

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