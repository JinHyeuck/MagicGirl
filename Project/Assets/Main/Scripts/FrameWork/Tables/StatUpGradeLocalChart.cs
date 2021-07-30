using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;


namespace GameBerry
{
    public class StatUpGradeData
    {
        public StatType StatUpgradeType = StatType.Max;
        public int MaxLevel = 0;
        public double AddValue = 0.0f;
        public int DefaultPrice = 0;
        public double DefaultIncreasePrice = 0;
        public int AddIncreaseLevel = 0;
        public double AddIncreaseLevelPrice = 0;
    }

    public class StatUpGradeLocalChart
    {
        public List<StatUpGradeData> m_statUpGradeDatas = new List<StatUpGradeData>();
        private Dictionary<StatType, StatUpGradeData> m_statUpGradeDatas_Dic = new Dictionary<StatType, StatUpGradeData>();

        //------------------------------------------------------------------------------------
        public void InitData()
        {
            JsonData chartJson = JsonMapper.ToObject(TheBackEnd.TheBackEnd.Instance.GetLocalChartData(Define.StatUpGradeChartKey));

            var rows = chartJson["rows"];

            for (int i = 0; i < rows.Count; ++i)
            {
                StatType stattype = EnumExtensions.Parse<StatType>(rows[i]["stattype"]["S"].ToString());

                StatUpGradeData data = new StatUpGradeData
                {
                    StatUpgradeType = stattype,

                    MaxLevel = rows[i]["maxlevel"]["S"].ToString().FastStringToInt(),

                    AddValue = System.Convert.ToDouble(rows[i]["addvalue"]["S"].ToString()),

                    DefaultPrice = rows[i]["defprice"]["S"].ToString().FastStringToInt(),

                    DefaultIncreasePrice = System.Convert.ToDouble(rows[i]["defincprice"]["S"].ToString()),

                    AddIncreaseLevel = rows[i]["addinclevel"]["S"].ToString().FastStringToInt(),

                    AddIncreaseLevelPrice = System.Convert.ToDouble(rows[i]["addinclevelprice"]["S"].ToString()),
                };

                m_statUpGradeDatas.Add(data);
                m_statUpGradeDatas_Dic.Add(data.StatUpgradeType, data);
            }
        }
        //------------------------------------------------------------------------------------
        public StatUpGradeData GetStatUpGradeData(StatType type)
        {
            StatUpGradeData data = null;
            m_statUpGradeDatas_Dic.TryGetValue(type, out data);

            return data;
        }
        //------------------------------------------------------------------------------------
    }
}