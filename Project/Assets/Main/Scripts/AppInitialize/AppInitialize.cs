using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry
{
    public class AppInitialize : MonoBehaviour
    {
        private AppInitializeUI m_appInitUI;

        private System.Action m_loginCompleteCallBack; // 로그인이 끝나면 호출

        //------------------------------------------------------------------------------------
        public void Init()
        {
            ResourceLoader.Instance.Load<GameObject>("AppInitialize/AppInitializeUI", o =>
            {
                GameObject clone = Instantiate(o, UI.UIManager.Instance.screenCanvasContent.transform) as GameObject;
                if (clone != null)
                    m_appInitUI = clone.GetComponent<AppInitializeUI>();

                if (m_appInitUI != null)
                    m_appInitUI.Init();
            });

            Message.AddListener<Event.LoginResultMsg>(LoginResult);
            Message.AddListener<Event.CreateNickNameResultMsg>(CreateNickNameResult);
            
        }
        //------------------------------------------------------------------------------------
        public void Release()
        {
            Message.RemoveListener<Event.LoginResultMsg>(LoginResult);
            Message.RemoveListener<Event.CreateNickNameResultMsg>(CreateNickNameResult);

            Destroy(m_appInitUI.gameObject);
        }
        //------------------------------------------------------------------------------------
        public bool InitBackEnd()
        {
            bool initend = false;

            initend = TheBackEnd.TheBackEnd.Instance.InitTheBackEnd();

            if (initend == false)
            {
                if (m_appInitUI != null)
                {
                    m_appInitUI.VisibleNoticeGroup(true);
                    m_appInitUI.SetNoticeText("서버 초기화 실패\n다시 실행해주세요.");
                }
            }

            return initend;
        }
        //------------------------------------------------------------------------------------
        public void DoLogin(System.Action action)
        {
            m_loginCompleteCallBack = action;

            if (m_appInitUI != null)
                m_appInitUI.VisibleLoginProcess(true);

            if (TheBackEnd.TheBackEnd.Instance.IsNeedSignUp() == true)
            {
                if (m_appInitUI != null)
                {
                    m_appInitUI.SetLoginCallBack(OnSignUpProcess);
                    m_appInitUI.VisibleLoginButtonGroup(true);
                }
            }
            else
            {
                if (TheBackEnd.TheBackEnd.Instance.CheckLoginState() == TheBackEnd.LoginType.Social)
                {
                    TheBackEnd.TheBackEnd.Instance.DoSocialLogin(TheBackEnd.TheBackEnd.Instance.GetFederationType());
                }
                else if (TheBackEnd.TheBackEnd.Instance.CheckLoginState() == TheBackEnd.LoginType.Guest)
                {
                    TheBackEnd.TheBackEnd.Instance.DoGuestLogin();
                }
                else if (TheBackEnd.TheBackEnd.Instance.CheckLoginState() == TheBackEnd.LoginType.CustomLogin)
                {
                    if (m_appInitUI != null)
                    {
                        m_appInitUI.VisibleCustomLogin(true);
                        m_appInitUI.SetCustomLoginCallBack(OnGuestLoginProcess);
                    }
                }
            }

            return;
        }
        //------------------------------------------------------------------------------------
        private void OnSignUpProcess(TheBackEnd.LoginType logintype, BackEnd.FederationType federationtype)
        {
            if (logintype == TheBackEnd.LoginType.Social)
            {
                // 패북, 구글, 애플
                TheBackEnd.TheBackEnd.Instance.DoSocialLogin(federationtype);
            }
            else
            {
                if (logintype == TheBackEnd.LoginType.Guest)
                {
                    TheBackEnd.TheBackEnd.Instance.DoGuestLogin();
                }
                else if(logintype == TheBackEnd.LoginType.CustomLogin)
                {
                    if (m_appInitUI != null)
                        m_appInitUI.SetCustomLoginCallBack(OnGuestLoginProcess);
                }
            }
        }
        //------------------------------------------------------------------------------------
        private void OnGuestLoginProcess(string id, string pw, bool issignup)
        {
            if (issignup == true)
                TheBackEnd.TheBackEnd.Instance.DoCustomSignUp(id, pw);
            else
                TheBackEnd.TheBackEnd.Instance.DoCustomLogin(id, pw);
        }
        //------------------------------------------------------------------------------------
        private void LoginResult(Event.LoginResultMsg msg)
        {
            if (string.IsNullOrEmpty(msg.NickName) == true)
            {
                // 닉네임 설정해야함
                Start_CreateNickName();
            }
            else
            {
                if (m_appInitUI != null)
                    m_appInitUI.VisibleLoginProcess(false);

                if (m_loginCompleteCallBack != null)
                    m_loginCompleteCallBack();
            }
            
        }
        //------------------------------------------------------------------------------------
        private void Start_CreateNickName()
        {
            if (m_appInitUI != null)
            {
                m_appInitUI.VisibleCustomLogin(false);
                m_appInitUI.VisibleCreateNickName(true);
                m_appInitUI.SetNickNameCallBack(OnNickNameProcess);
            }
        }
        //------------------------------------------------------------------------------------
        private void OnNickNameProcess(string nickname)
        {
            TheBackEnd.TheBackEnd.Instance.CreateNickName(nickname);
        }
        //------------------------------------------------------------------------------------
        private void CreateNickNameResult(Event.CreateNickNameResultMsg msg)
        {
            if (msg.IsSuccess == true)
            {
                if (m_appInitUI != null)
                    m_appInitUI.VisibleLoginProcess(false);

                if (m_loginCompleteCallBack != null)
                    m_loginCompleteCallBack();
            }
        }
        //------------------------------------------------------------------------------------
    }
}