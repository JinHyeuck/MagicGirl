using UnityEngine;
using GameBerry.UI;

namespace GameBerry.Contents
{
    public class GlobalContent : IContent
    {
        private static GameBerry.Event.DoFadeMsg m_fadeMsg = new GameBerry.Event.DoFadeMsg();

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