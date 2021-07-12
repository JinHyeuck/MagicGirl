using System.Collections.Generic;
using UnityEngine;
using GameBerry.UI;

namespace GameBerry.Contents
{
    [System.Serializable]
    public class GradeColorData
    {
        public GradeType GradeType;
        public Color GradeColor;
    }

    public class GlobalContent : IContent
    {
        [SerializeField]
        private List<GradeColorData> m_gradleColorList = new List<GradeColorData>();

        private static Dictionary<GradeType, Color> m_gradleColor_Dic = new Dictionary<GradeType, Color>();


        private static GameBerry.Event.DoFadeMsg m_fadeMsg = new GameBerry.Event.DoFadeMsg();

        //------------------------------------------------------------------------------------
        protected override void OnLoadStart()
        {
            for (int i = 0; i < m_gradleColorList.Count; ++i)
            {
                if (m_gradleColor_Dic.ContainsKey(m_gradleColorList[i].GradeType) == false)
                {
                    m_gradleColor_Dic.Add(m_gradleColorList[i].GradeType, m_gradleColorList[i].GradeColor);
                }
            }

            SetLoadComplete();
        }
        //------------------------------------------------------------------------------------
        protected override void OnEnter()
        {
            IDialog.RequestDialogEnter<GlobalPopupDialog>();
            IDialog.RequestDialogEnter<GlobalFadeDialog>();
        }
        //------------------------------------------------------------------------------------
        protected override void OnExit()
        {
            IDialog.RequestDialogExit<GlobalPopupDialog>();
            IDialog.RequestDialogExit<GlobalBufferingDialog>();
            IDialog.RequestDialogExit<GlobalButtonLockDialog>();
            IDialog.RequestDialogExit<GlobalFadeDialog>();
        }
        //------------------------------------------------------------------------------------
        public static Color GetGradeColor(GradeType gradetype)
        {
            Color color = Color.white;
            m_gradleColor_Dic.TryGetValue(gradetype, out color);

            return color;
        }
        //------------------------------------------------------------------------------------
        public static void DoFade(bool visible, float duration = 1.0f)
        {
            // visible : false 투명 -> 검은색으로

            m_fadeMsg.duration = duration;
            m_fadeMsg.visible = visible;

            Message.Send(m_fadeMsg);
        }
        //------------------------------------------------------------------------------------
        public static void VisibleBufferingUI(bool visible)
        {
            if (visible == true)
                IDialog.RequestDialogEnter<GlobalBufferingDialog>();
            else
                IDialog.RequestDialogExit<GlobalBufferingDialog>();
        }
        //------------------------------------------------------------------------------------
        public static void SetButtonLock(bool btnlock)
        {
            if (btnlock == true)
                IDialog.RequestDialogEnter<GlobalButtonLockDialog>();
            else
                IDialog.RequestDialogExit<GlobalButtonLockDialog>();
        }
        //------------------------------------------------------------------------------------
        public static void ShowPopup_Ok(string titletext, System.Action okAction = null)
        {
            Message.Send(new GameBerry.Event.ShowPopup_OkMsg(titletext, okAction));
        }
        //------------------------------------------------------------------------------------
        public static void ShowPopup_OkCancel(string titletext, System.Action okAction = null, System.Action cancelAction = null)
        {
            Message.Send(new GameBerry.Event.ShowPopup_OkCalcelMsg(titletext, okAction, cancelAction));
        }
        //------------------------------------------------------------------------------------
        public static void ShowPopup_Input(string titletext, string defaultstr, string placeholder, System.Action<string> okAction, System.Action cancelAction = null)
        {
            Message.Send(new GameBerry.Event.ShowPopup_InputMsg(titletext, defaultstr, placeholder, okAction, cancelAction));
        }
        //------------------------------------------------------------------------------------
    }
}