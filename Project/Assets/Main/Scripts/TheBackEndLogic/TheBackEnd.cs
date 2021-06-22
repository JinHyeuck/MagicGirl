using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

namespace GameBerry.TheBackEnd
{
    public class TheBackEnd : MonoSingleton<TheBackEnd>
    {
        //------------------------------------------------------------------------------------
        public bool InitTheBackEnd()
        {
            var bro = Backend.Initialize();

            if (bro.IsSuccess() == true)
            {
                Debug.Log("Backend Initialize Success!");
                return true;
            }
            else
            {
                Debug.Log("Backend Initialize Fail!");
            }

            return false;
        }

        #region TheBackEnd_Login
        //------------------------------------------------------------------------------------
        public bool IsNeedSignUp()
        {
            return TheBackEnd_Login.CheckLoginType() == LoginType.None;
        }
        //------------------------------------------------------------------------------------
        public LoginType CheckLoginState()
        {
            return TheBackEnd_Login.CheckLoginType();
        }
        //------------------------------------------------------------------------------------
        public FederationType GetFederationType()
        {
            return TheBackEnd_Login.GetSocialLoginType();
        }
        //------------------------------------------------------------------------------------
        public void DoCustomSignUp(string id, string pw)
        {
            TheBackEnd_Login.DoCustomSignUp(id, pw);
        }
        //------------------------------------------------------------------------------------
        public void DoCustomLogin(string id, string pw)
        {
            TheBackEnd_Login.DoCustomLogin(id, pw);
        }
        //------------------------------------------------------------------------------------
        public void DoGuestLogin()
        {
            TheBackEnd_Login.DoGuestLogin();
        }
        //------------------------------------------------------------------------------------
        public void DoSocialLogin(BackEnd.FederationType federationtype)
        {
            TheBackEnd_Login.DoSocialLogin(GetFederationToken(), federationtype);
        }
        //------------------------------------------------------------------------------------
        private string GetFederationToken()
        {
            return "";
        }
        //------------------------------------------------------------------------------------
        public void CreateNickName(string nickname)
        {
            TheBackEnd_Login.CreateNickName(nickname);
        }
        //------------------------------------------------------------------------------------
        public string GetNickPlayerName()
        {
            return TheBackEnd_Login.NickName;
        }
        //------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------
        #region TheBackEnd_GameChart
        //------------------------------------------------------------------------------------
        public void GetAllChartList()
        {
            TheBackEnd_GameChart.GetAllChartList();
        }
        //------------------------------------------------------------------------------------
        public void GetOneChartAndSave(string fileidkey)
        {
            TheBackEnd_GameChart.GetOneChartAndSave(fileidkey);
        }
        //------------------------------------------------------------------------------------
        public string GetLocalChartData(string fileidkey)
        {
            return TheBackEnd_GameChart.GetLocalChartData(fileidkey);
        }
        //------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------
        #region TheBackEnd_PlayerTable
        //------------------------------------------------------------------------------------
        public void GetTableData()
        {
            TheBackEnd_PlayerTable.GetTableData();
        }
        //------------------------------------------------------------------------------------
        public void PlayerTableUpdate()
        {
            TheBackEnd_PlayerTable.PlayerTableUpdate();
        }
        //------------------------------------------------------------------------------------
        #endregion
    }
}