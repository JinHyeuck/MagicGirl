using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;

namespace GameBerry
{
    public class TheBackendTutorial : MonoBehaviour
    {
        [SerializeField]
        private Text m_logText;

        [SerializeField]
        private RectTransform m_logTransform;

        [SerializeField]
        private ContentSizeFitter m_logTextSizeFitter;

        [SerializeField]
        private RectTransform m_contentTransform;


        private const string m_userId = "testid3";
        private const string m_userPw = "123456";
        private const string m_userNickName = "test3";
        private const string m_tableName = "rankingtable";
        private const string m_rowName = "rankdata";
        private const string m_rankingTitle = "myranking";

        private string m_searchUserNickName = "test1";


        private string m_rowInDate = string.Empty;
        private string m_firstKey = string.Empty;
        private string m_rankingUuid = string.Empty;

        void Awake()
        {
            StartCoroutine(TheBackEndTutorialProcess());
        }

        private IEnumerator TheBackEndTutorialProcess()
        {
            ShowLog("TestProcess Start");


            ShowLog("-------------------------------------");

            yield return new WaitForSeconds(1.0f);

            if (InitTheBackend() == false)
            {
                yield break;
            }

            Debug.Log("GoogleHash : " + Backend.Utils.GetGoogleHash());

            ShowLog("-------------------------------------");

            yield return new WaitForSeconds(1.0f);

            if (Login() == false)
            {
                Login();
            }

            ShowLog("-------------------------------------");

            if (GetUserInfo() == false)
            {
                yield break;
            }

            ShowLog("-------------------------------------");

            GetTableData();

            ShowLog("-------------------------------------");

            UpdateTable();

            ShowLog("-------------------------------------");

            GetSpecialUserTable();

            ShowLog("-------------------------------------");

            GetSpecialRowData();

            ShowLog("-------------------------------------");

            GetRankingTable();

            ShowLog("-------------------------------------");

            UpdateRankingScore();

            ShowLog("-------------------------------------");

            GetRanker();

            ShowLog("-------------------------------------");

            GetMyRanking();

            ShowLog("-------------------------------------");
        }

        private bool InitTheBackend()
        {
            ShowLog("InitTheBackend Start");

            try
            {
                var bro = Backend.Initialize(true);

                if (bro.IsSuccess() == true)
                {
                    ShowLog("InitTheBackend Is Success");
                    return true;
                }
                else
                {
                    ShowLog("InitTheBackend Is Fail");
                }
            }
            catch(System.Exception e)
            {
                ShowLog("InitTheBackend Exception : " + e.ToString());
            }

            return false;
        }

        private bool Login()
        {
            ShowLog("Login Start");

            BackendReturnObject bro = Backend.BMember.CustomLogin(m_userId, m_userPw);

            if (bro.IsSuccess() == true)
            {
                ShowLog("Login Is Success");

                ShowLog(string.Format("UserLocalData UserInData : {0}", Backend.UserInDate));
                ShowLog(string.Format("UserLocalData UserNickName : {0}", Backend.UserNickName));
                //ShowLog(bro.GetReturnValue());


                if (string.IsNullOrEmpty(Backend.UserNickName) == true)
                {
                    //Backend.BMember.CreateNickname(m_userNickName, o =>
                    //{
                    //    if (o.IsSuccess() == true)
                    //    {
                    //        ShowLog("NickNameCreate!!");
                    //    }
                    //});

                    Backend.BMember.CreateNickname(m_userNickName);
                }

                return true;
            }
            else
            {
                ShowLog("Login Is Fail");
                ShowLog(bro.GetStatusCode());
                ShowLog(bro.GetErrorCode());
                ShowLog(bro.GetMessage());

                SignUp();
            }

            return false;
        }

        private bool SignUp()
        {
            ShowLog("SignUp Start");

            BackendReturnObject bro = Backend.BMember.CustomSignUp(m_userId, m_userPw);

            if (bro.IsSuccess() == true)
            {
                ShowLog("SignUp Is Success");
                return true;
            }
            else
            {
                ShowLog("SignUp Is Fail");
            }

            return false;
        }

        private bool GetUserInfo()
        {
            ShowLog("GetUserInfo Start");

            BackendReturnObject bro = Backend.BMember.GetUserInfo();

            if (bro.IsSuccess() == true)
            {
                ShowLog("GetUserInfo Is Success");

                ShowLog(bro.GetReturnValuetoJSON()["row"]["countryCode"] != null ? bro.GetReturnValuetoJSON()["row"]["countryCode"].ToString() : "null");
                ShowLog(bro.GetReturnValuetoJSON()["row"]["nickname"] != null ? bro.GetReturnValuetoJSON()["row"]["nickname"].ToString() : "null");
                ShowLog(bro.GetReturnValuetoJSON()["row"]["inDate"] != null ? bro.GetReturnValuetoJSON()["row"]["inDate"].ToString() : "null");
                ShowLog(bro.GetReturnValuetoJSON()["row"]["emailForFindPassword"] != null ? bro.GetReturnValuetoJSON()["row"]["emailForFindPassword"].ToString() : "null");
                ShowLog(bro.GetReturnValuetoJSON()["row"]["subscriptionType"] != null ? bro.GetReturnValuetoJSON()["row"]["subscriptionType"].ToString() : "null");
                ShowLog(bro.GetReturnValuetoJSON()["row"]["federationId"] != null ? bro.GetReturnValuetoJSON()["row"]["federationId"].ToString() : "null");
                return true;
            }
            else
            {
                ShowLog("GetUserInfo Is Fail");
            }

            return false;
        }

        int GetTableDataCount = 0;
        private void GetTableData()
        {
            ShowLog("GetTableData Start");

            var bro = Backend.GameData.GetMyData(m_tableName, new Where(), 10);
            if (bro.IsSuccess() == false)
            {
                ShowLog(bro.GetStatusCode());
                ShowLog(bro.GetErrorCode());
                ShowLog(bro.GetMessage());
                return;
            }

            var data = bro.FlattenRows();

            for (int i = 0; i < data.Count; ++i)
            {
                string returnValue = string.Empty;
                foreach (var key in data[i].Keys)
                {
                    returnValue += string.Format("{0} : {1} / ", key, data[i][key].ToString());
                }

                ShowLog(returnValue);
            }

            if (bro.HasFirstKey())
            {
                m_firstKey = bro.FirstKeystring();
            }
            else
            {
                m_firstKey = string.Empty;
            }

            if (data.Count > 0)
            {
                m_rowInDate = data[0]["inDate"].ToString();
                ShowLog(string.Format("m_rowInDate : {0}", m_rowInDate));
            }
            else
            {
                if (GetTableDataCount == 0)
                {
                    InsertTableData();
                    GetTableData();
                    GetTableDataCount++;
                }
            }
        }

        private void InsertTableData()
        {
            Backend.GameData.Insert(m_tableName);
        }

        private void UpdateTable()
        {
            System.Random rnd = new System.Random();

            // score 컬럼에 1 ~ 100 사이 랜덤한 숫자를 갱신합니다.
            Param param = new Param();
            param.Add(m_rowName, rnd.Next(1, 100));

            // 게임 정보 업데이트
            // tableName 테이블의 rowIndate row의 score 컬럼을 위 랜덤한 값으로 수정합니다.
            SendQueue.Enqueue(Backend.GameData.Update, m_tableName, m_rowInDate, param, callback =>
            {
                if (callback.IsSuccess() == false)
                {
                    ShowLog("테이블 갱신 실패: " + callback.ToString());
                    return;
                }

                ShowLog("테이블 갱신 성공: " + callback);
            });

            //Backend.GameData.Update(m_tableName, m_rowInDate, param, callback =>
            //{
            //    if (callback.IsSuccess() == false)
            //    {
            //        ShowLog("테이블 갱신 실패: " + callback.ToString());
            //        return;
            //    }

            //    ShowLog("테이블 갱신 성공: " + callback);
            //});
        }

        private void GetSpecialUserTable()
        {
            ShowLog("GetSpecialUserTable Start");


            BackendReturnObject sbro = Backend.Social.GetGamerIndateByNickname(m_searchUserNickName);
            ShowLog("<<<<<<<<<<<<<<<<<<<<<<<<<<");
            ShowLog(sbro.GetReturnValue());
            ShowLog(">>>>>>>>>>>>>>>>>>>>>>>>>>");
            string gamerIndate = sbro.Rows()[0]["inDate"]["S"].ToString();

            Where where = new Where();
            where.Equal("owner_inDate", gamerIndate);


            var bro = Backend.GameData.GetMyData(m_tableName, where, 10);
            if (bro.IsSuccess() == false)
            {
                ShowLog(bro.GetStatusCode());
                ShowLog(bro.GetErrorCode());
                ShowLog(bro.GetMessage());
                return;
            }

            var data = bro.FlattenRows();

            for (int i = 0; i < data.Count; ++i)
            {
                string returnValue = string.Empty;
                foreach (var key in data[i].Keys)
                {
                    returnValue += string.Format("{0} : {1} / ", key, data[i][key].ToString());
                }

                ShowLog(returnValue);
            }
        }

        private void GetSpecialRowData()
        {
            ShowLog("GetSpecialRowData Start");

            // select[]를 이용하여 리턴 시, owner_inDate와 score만 출력되도록 설정
            //string[] select = { m_rowName };

            // 테이블 내 해당 rowIndate를 지닌 row를 조회
            //var bro = Backend.GameData.Get(m_tableName, m_rowInDate);
            //ShowLog(bro);

            //if (bro.IsSuccess() == false)
            //{
            //    ShowLog(bro.GetStatusCode());
            //    ShowLog(bro.GetErrorCode());
            //    ShowLog(bro.GetMessage());
            //    return;
            //}
            //var data = bro.FlattenRows();

            //for (int i = 0; i < data.Count; ++i)
            //{
            //    string returnValue = string.Empty;
            //    foreach (var key in data[i].Keys)
            //    {
            //        returnValue += string.Format("{0} : {1} / ", key, data[i][key].ToString());
            //    }

            //    ShowLog(returnValue);
            //}

            // 테이블 내 해당 rowIndate를 지닌 row를 조회
            // select에 존재하는 컬럼만 리턴
            //Backend.GameData.Get(m_tableName, m_rowInDate, select, (callback) =>
            //{
            //    ShowLog(callback.ToString());
            //});
            //ShowLog(bro);
            //ShowLog("<<<<<<<<<<<<<<<<<<<<<<<<<<");
            //if (bro.IsSuccess() == false)
            //{
            //    ShowLog(bro.GetStatusCode());
            //    ShowLog(bro.GetErrorCode());
            //    ShowLog(bro.GetMessage());
            //    return;
            //}

            //data = bro.FlattenRows();

            //for (int i = 0; i < data.Count; ++i)
            //{
            //    string returnValue = string.Empty;
            //    foreach (var key in data[i].Keys)
            //    {
            //        returnValue += string.Format("{0} : {1} / ", key, data[i][key].ToString());
            //    }

            //    ShowLog(returnValue);
            //}
        }

        private void GetRankingTable()
        {
            ShowLog("GetRankingTable Start");

            var bro = Backend.URank.User.GetRankTableList();

            ShowLog(bro.GetReturnValue());

            var data = bro.FlattenRows();

            ShowLog("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");

            for (int i = 0; i < data.Count; ++i)
            {
                string returnValue = string.Empty;
                foreach (var key in data[i].Keys)
                {
                    returnValue += string.Format("{0} : {1} / ", key, data[i][key].ToString());

                    if (key == "title")
                    {
                        if (data[i][key].ToString().Contains(m_rankingTitle))
                        {
                            m_rankingUuid = data[i]["uuid"].ToString();
                            ShowLog(string.Format("m_rankingUuid : {0}", m_rankingUuid));
                        }
                    }
                }

                ShowLog(returnValue);
            }


        }

        private void UpdateRankingScore()
        {
            ShowLog("UpdateRankingScore Start");

            System.Random rnd = new System.Random();

            Param param = new Param();
            param.Add("rankdata", rnd.Next(1, 100));

            SendQueue.Enqueue(Backend.URank.User.UpdateUserScore, m_rankingUuid, m_tableName, m_rowInDate, param, callback =>
            {
                // 이후 처리
                ShowLog(callback.GetReturnValue().ToString());

            });

            //Backend.URank.User.UpdateUserScore(m_rankingUuid, m_tableName, m_rowInDate, param, callback =>
            //{
            //    // 이후 처리
            //    ShowLog(callback.GetReturnValue().ToString());

            //});
        }

        private void GetRanker()
        {
            ShowLog("GetRanker Start");

            SendQueue.Enqueue(Backend.URank.User.GetRankList, m_rankingUuid, 100, callback => {
                // 이후 처리

                ShowLog(string.Format("GetRanker : {0}", callback.GetReturnValue().ToString()));
            });

            //Backend.URank.User.GetRankList(m_rankingUuid, 100, callback => {
            //    // 이후 처리

            //    ShowLog(string.Format("GetRanker : {0}", callback.GetReturnValue().ToString()));
            //});
        }

        private void GetMyRanking()
        {
            ShowLog("GetMyRanking Start");

            SendQueue.Enqueue(Backend.URank.User.GetMyRank, m_rankingUuid, 5, callback => {
                // 이후 처리

                ShowLog(string.Format("GetMyRanking : {0}", callback.GetReturnValue().ToString()));
            });

            //Backend.URank.User.GetMyRank(m_rankingUuid, 5, callback => {
            //    // 이후 처리

            //    ShowLog(string.Format("GetMyRanking : {0}", callback.GetReturnValue().ToString()));
            //});
        }

        private void ShowLog(string str)
        {
            m_logText.text = string.Format("{0}\n{1}", m_logText.text, str);

            m_logTextSizeFitter.SetLayoutHorizontal();
            m_logTextSizeFitter.SetLayoutVertical();

            m_contentTransform.sizeDelta = m_logTransform.sizeDelta;

            Debug.Log(str);
        }
    }
}