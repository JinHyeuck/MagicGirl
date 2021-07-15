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
            Debug.Log("InitTheBackEnd");

            var bro = Backend.Initialize(true);

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
        //------------------------------------------------------------------------------------
        public void GetGoogleHash()
        {
            string googlehash = Backend.Utils.GetGoogleHash();

            Debug.Log("구글 해시 키 : " + googlehash);
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
        public void GetCharacterInfoTableData()
        {
            TheBackEnd_PlayerTable.GetCharacterInfoTableData();
        }
        //------------------------------------------------------------------------------------
        public void UpdateCharacterInfoTable()
        {
            TheBackEnd_PlayerTable.UpdateCharacterInfoTable();
        }
        //------------------------------------------------------------------------------------
        public void GetCharacterUpGradeStatTableData()
        {
            TheBackEnd_PlayerTable.GetCharacterUpGradeStatTableData();
        }
        //------------------------------------------------------------------------------------
        public void UpdateCharacterUpGradeStatTable()
        {
            TheBackEnd_PlayerTable.UpdateCharacterUpGradeStatTable();
        }
        //------------------------------------------------------------------------------------
        public void GetCharacterEquipmentInfoTableData()
        {
            TheBackEnd_PlayerTable.GetCharacterEquipmentInfoTableData();
        }
        //------------------------------------------------------------------------------------
        public void UpdateCharacterEquipmentInfoTable()
        {
            TheBackEnd_PlayerTable.UpdateCharacterEquipmentInfoTable();
        }
        //------------------------------------------------------------------------------------
        public void GetCharacterSkillinfoTableData()
        {
            TheBackEnd_PlayerTable.GetCharacterSkillinfoTableData();
        }
        //------------------------------------------------------------------------------------
        public void UpdateCharacterSkillInfoTable()
        {
            TheBackEnd_PlayerTable.UpdateCharacterSkillInfoTable();
        }
        //------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------
        #region TheBackEnd_Probability
        //------------------------------------------------------------------------------------
        public void GetProbabilityCardList()
        {
            TheBackEnd_Probability.GetProbabilityCardList();
        }
        //------------------------------------------------------------------------------------
        public void GetProbabilitys(string chartid, int count, System.Action<LitJson.JsonData> onComplete)
        {
            TheBackEnd_Probability.GetProbabilitys(chartid, count, onComplete);
        }
        //------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------
    }
}