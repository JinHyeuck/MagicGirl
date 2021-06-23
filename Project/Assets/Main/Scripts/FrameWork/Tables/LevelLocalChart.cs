using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace GameBerry
{
    public class LevelData
    {
        public int TargetLevel;
        public int Exp;
        public int RewardDia;
    }

    public class LevelLocalChart
    {
        public List<LevelData> m_expDataDatas = new List<LevelData>();
        private Dictionary<int, LevelData> m_expDataDatas_Dic = new Dictionary<int, LevelData>();
        private int m_maxLevel = 0;

        //------------------------------------------------------------------------------------
        public void InitData()
        {
            JsonData chartJson = JsonMapper.ToObject(TheBackEnd.TheBackEnd.Instance.GetLocalChartData(Define.LevelchartKey));

            var rows = chartJson["rows"];

            for (int i = 0; i < rows.Count; ++i)
            {
                LevelData data = new LevelData
                {
                    TargetLevel = rows[i]["level"]["S"].ToString().FastStringToInt(),

                    Exp = rows[i]["exp"]["S"].ToString().FastStringToInt(),

                    RewardDia = rows[i]["rewarddia"]["S"].ToString().FastStringToInt()
                };

                m_maxLevel = data.TargetLevel > m_maxLevel ? data.TargetLevel : m_maxLevel;

                m_expDataDatas.Add(data);
                m_expDataDatas_Dic.Add(data.TargetLevel, data);
            }
        }
        //------------------------------------------------------------------------------------
        public LevelData GetLevelData(int level)
        {
            LevelData data = null;
            m_expDataDatas_Dic.TryGetValue(level, out data);

            return data;
        }
        //------------------------------------------------------------------------------------
        public LevelData GetNextLevelData(int level)
        {
            if (m_maxLevel > level + 1)
            {
                LevelData data = null;
                m_expDataDatas_Dic.TryGetValue(level + 1, out data);

                return data;
            }
            else
            {
                return null;
            }
        }
        //------------------------------------------------------------------------------------
    }
}