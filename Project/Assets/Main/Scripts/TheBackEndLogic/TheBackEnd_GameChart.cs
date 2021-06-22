using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

namespace GameBerry.TheBackEnd
{
    public static class TheBackEnd_GameChart
    {
        public static Dictionary<string, string> TableChartFileld = new Dictionary<string, string>();

        private static Event.GetOneChartAndSaveResponseMsg m_getOneChartAndSaveResponseMsg = new Event.GetOneChartAndSaveResponseMsg();

        //------------------------------------------------------------------------------------
        public static void GetAllChartList()
        {
            SendQueue.Enqueue(Backend.Chart.GetChartList, (callback) =>
            {
                if (callback.IsSuccess() == true)
                {
                    Debug.Log("차트 목록 가져옴");
                    // 이후 작업
                    var data = callback.FlattenRows();

                    for (int i = 0; i < data.Count; ++i)
                    {
                        foreach (var key in data[i].Keys)
                        {
                            if (key.ToString() == "chartName")
                            {
                                TableChartFileld.Add(data[i][key].ToString().ToLower(), data[i]["selectedChartFileId"].ToString());
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("차트 목록 못가져옴ㅠㅠ");
                }

                Message.Send(new Event.GetAllGameChartResponseMsg { IsSuccess = callback.IsSuccess() });
            });
        }
        //------------------------------------------------------------------------------------
        public static void GetOneChartAndSave(string fileidkey)
        {
            SendQueue.Enqueue(Backend.Chart.GetOneChartAndSave, TableChartFileld[fileidkey.ToLower()], (callback) =>
            {
                Debug.Log("차트 저장했는데 과연?");

                Debug.Log(callback.GetReturnValue());

                if (callback.IsSuccess() == true)
                {
                    var data = callback.FlattenRows();

                    for (int i = 0; i < data.Count; ++i)
                    {
                        string returnValue = string.Empty;
                        foreach (var key in data[i].Keys)
                        {
                            returnValue += string.Format("{0} : {1} / ", key, data[i][key].ToString());
                        }
                        Debug.Log(returnValue);
                    }
                }

                m_getOneChartAndSaveResponseMsg.IsSuccess = callback.IsSuccess();
                Message.Send(m_getOneChartAndSaveResponseMsg);
            });
        }
        //------------------------------------------------------------------------------------
        public static string GetLocalChartData(string fileidkey)
        {
            return Backend.Chart.GetLocalChartData(TableChartFileld[fileidkey]);
        }
        //------------------------------------------------------------------------------------
    }
}