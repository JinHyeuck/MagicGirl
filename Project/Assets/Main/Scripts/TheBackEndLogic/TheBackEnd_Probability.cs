using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

namespace GameBerry.TheBackEnd
{
    public static class TheBackEnd_Probability
    {
        private static Dictionary<string, string> m_probabilityChartFileld = new Dictionary<string, string>();
        //------------------------------------------------------------------------------------
        public static void GetProbabilityCardList()
        {
            SendQueue.Enqueue(Backend.Probability.GetProbabilityCardList, (callback) =>
            {
                Debug.Log("»Ì±â ¸ñ·Ï °¡Á®¿È");
                // ÀÌÈÄ ÀÛ¾÷
                var data = callback.FlattenRows();



                for (int i = 0; i < data.Count; ++i)
                {
                    m_probabilityChartFileld.Add(data[i]["probabilityName"].ToString(), data[i]["selectedProbabilityFileId"].ToString());

                    string logtext = string.Empty;

                    foreach (var key in data[i].Keys)
                    {
                        //if (key.ToString() == "chartName")
                        //{
                        //    TableChartFileld.Add(data[i][key].ToString().ToLower(), data[i]["selectedChartFileId"].ToString());
                        //}

                        logtext += string.Format("{0} : {1} /", key, data[i][key]);

                        var dataa = data[i][key];
                        //foreach (var keya in data[i][key].Keys)
                        //{
                        //    logtext += string.Format("- {0} : {1} -", key, data[i][key][keya]);
                        //}
                    }

                    Debug.Log(logtext);
                }

                foreach (KeyValuePair<string, string> pair in m_probabilityChartFileld)
                    Debug.Log(string.Format("probabilityChartFileld {0} - {1}", pair.Key, pair.Value));

                GetProbabilitys();
            });
        }
        //------------------------------------------------------------------------------------
        public static void GetProbabilitys()
        {
            SendQueue.Enqueue(Backend.Probability.GetProbabilitys, "2331", 10, (callback) =>
            {
                bool issuccess = callback.IsSuccess();
                if (issuccess == false)
                {
                    string adsf = callback.GetErrorCode();
                    string fsda = callback.GetMessage();
                }

                Debug.Log("»Ì¾Æ¿È");
                // ÀÌÈÄ ÀÛ¾÷
                var data = callback.GetFlattenJSON();
                data = data["elements"];

                for (int i = 0; i < data.Count; ++i)
                {
                    string returnValue = string.Empty;

                    foreach (var key in data[i].Keys)
                    {
                        returnValue += string.Format("{0} : {1} / ", key, data[i][key].ToString());
                    }
                    Debug.Log(returnValue);
                }
            });
        }
    }
}