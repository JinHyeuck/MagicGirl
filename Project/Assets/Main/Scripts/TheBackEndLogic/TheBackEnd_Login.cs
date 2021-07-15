using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

namespace GameBerry.TheBackEnd
{
    public enum LoginType
    {
        None = 0,
        Guest,
        Social,
        CustomLogin,  // 테스트용 커스텀로그
    }

    //public enum FederationType
    //{
    //    Google,
    //    Facebook,
    //    Apple
    //}

    public static class TheBackEnd_Login
    {
        public static string UserInDate = string.Empty;
        public static string NickName = string.Empty;

        public static Event.SetNoticeMsg m_setNoticeMsg = new Event.SetNoticeMsg();
        public static Event.CreateNickNameResultMsg m_createNickNameResultMsg = new Event.CreateNickNameResultMsg();

        //------------------------------------------------------------------------------------
        public static LoginType CheckLoginType()
        {
#if CUSTOM_LOGIN
            return LoginType.CustomLogin;
#endif
            return (LoginType)PlayerPrefs.GetInt(Define.LoginTypeKey, 0);
        }
        //------------------------------------------------------------------------------------
        public static FederationType GetSocialLoginType()
        {
            return (FederationType)PlayerPrefs.GetInt(Define.SocialTypeKey, 0);
            //return FederationType.Google;
        }
        //------------------------------------------------------------------------------------
        public static void DoCustomSignUp(string id, string pw)
        {
            SendQueue.Enqueue(Backend.BMember.CustomSignUp, id, pw, callback => {
                if (callback.IsSuccess())
                {
                    Debug.Log("회원가입에 성공했습니다");
                }

                m_setNoticeMsg.NoticeStr = callback.GetMessage();

                Message.Send(m_setNoticeMsg);
            });
        }
        //------------------------------------------------------------------------------------
        public static void DoCustomLogin(string id, string pw)
        {
            Debug.Log("DoCustomLogin");
            SendQueue.Enqueue(Backend.BMember.CustomLogin, id, pw, callback =>
            {
                if (callback.IsSuccess())
                {
                    Debug.Log("로그인에 성공했습니다");

                    CompleteLogin();
                }
                else
                {
                    Debug.Log(string.Format("DoCustomLogin GetErrorCode {0}", callback.GetErrorCode()));
                    Debug.Log(string.Format("DoCustomLogin GetMessage {0}", callback.GetMessage()));
                }

                m_setNoticeMsg.NoticeStr = callback.GetMessage();

                Message.Send(m_setNoticeMsg);
            });
        }
        //------------------------------------------------------------------------------------
        public static void DoGuestLogin()
        {
            SendQueue.Enqueue(Backend.BMember.GuestLogin, "GuestLogin", callback =>
            {
                if (callback.IsSuccess())
                {
                    Debug.Log("로그인에 성공했습니다");

                    CompleteLogin();
                }

                m_setNoticeMsg.NoticeStr = callback.GetMessage();

                Message.Send(m_setNoticeMsg);
            });
        }
        //------------------------------------------------------------------------------------
        public static void DoSocialLogin(string token, FederationType federationtype)
        {
            SendQueue.Enqueue(Backend.BMember.AuthorizeFederation, token, federationtype, string.Format("{0}로 가입함", federationtype), callback =>
            {
                if (callback.IsSuccess())
                {
                    Debug.Log("로그인에 성공했습니다");

                    CompleteLogin();
                }

                m_setNoticeMsg.NoticeStr = callback.GetMessage();

                Message.Send(m_setNoticeMsg);
            });
        }
        //------------------------------------------------------------------------------------
        private static void CompleteLogin()
        {
            UserInDate = Backend.UserInDate;
            NickName = Backend.UserNickName;

            Message.Send(new Event.LoginResultMsg { NickName = NickName, IsSuccess = true});
        }
        //------------------------------------------------------------------------------------
        public static void CreateNickName(string nickname)
        {
            SendQueue.Enqueue(Backend.BMember.CreateNickname, nickname, callback =>
            {
                if (callback.IsSuccess() == true)
                {
                    NickName = Backend.UserNickName;
                    Debug.Log("NickNameCreate!!");
                }

                m_createNickNameResultMsg.IsSuccess = callback.IsSuccess();
                m_createNickNameResultMsg.ErrorMessage = callback.IsSuccess() == true ? string.Empty : callback.GetMessage();

                Message.Send(m_createNickNameResultMsg);
                m_setNoticeMsg.NoticeStr = callback.GetMessage();

                Message.Send(m_setNoticeMsg);
            });

            //Backend.BMember.CreateNickname(nickname, callback =>
            //{
            //    if (callback.IsSuccess() == true)
            //    {
            //        Debug.Log("NickNameCreate!!");
            //    }
            //    else
            //    {

            //    }

            //    m_createNickNameResultMsg.IsSuccess = callback.IsSuccess();
            //    m_createNickNameResultMsg.ErrorMessage = callback.IsSuccess() == true ? string.Empty : callback.GetMessage();

            //    Message.Send(m_createNickNameResultMsg);
            //    m_setNoticeMsg.NoticeStr = callback.GetMessage();

            //    Message.Send(m_setNoticeMsg);
            //});
        }
    }
}