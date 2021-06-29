using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry.UI
{
    public class UIInGameBottomMenuElement : MonoBehaviour
    {
        [SerializeField]
        private BottomMenuID m_id = BottomMenuID.None;

        [SerializeField]
        private Button m_btn;

        [SerializeField]
        private Color m_bottomBtnColor_Click;

        [SerializeField]
        private Color m_bottomBtnColor_Release;

        private System.Action<BottomMenuID> m_callBack;
        private bool m_isClicked = false;

        //------------------------------------------------------------------------------------   
        public void Init(System.Action<BottomMenuID> callback)
        {
            if (m_btn == null)
                m_btn = GetComponent<Button>();

            if (m_btn != null)
                m_btn.onClick.AddListener(OnClick);
        }
        //------------------------------------------------------------------------------------
        private void OnClick()
        {
            if (m_callBack != null)
                m_callBack(m_id);
        }
        //------------------------------------------------------------------------------------
    }
}