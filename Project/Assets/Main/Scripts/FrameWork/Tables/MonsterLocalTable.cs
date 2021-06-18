using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace GameBerry
{
    [System.Serializable]
    public class MonsterData
    {
        public string MonsterID = string.Empty;
        public string MonsterName = string.Empty;
        public string MonsterImageName = null;

        public int HP = 0;
        public int MP = 0;
        public int Damage = 0;
    }

    public class MonsterLocalTable
    {
        public List<MonsterData> m_monsterDatas = new List<MonsterData>();
        private Dictionary<string, MonsterData> m_monsterDatas_Dic = new Dictionary<string, MonsterData>();

        //------------------------------------------------------------------------------------
        public void InitData()
        {
            JsonData chartJson = JsonMapper.ToObject(TheBackEnd.TheBackEnd.Instance.GetLocalChartData(Define.MonsterTableKey));

            var rows = chartJson["rows"];

            for (int i = 0; i < rows.Count; ++i)
            {
                MonsterData data = new MonsterData
                {
                    MonsterID = rows[i]["itemID"]["S"].ToString(),

                    MonsterName = rows[i]["itemName"]["S"].ToString(),

                    MonsterImageName = string.Format("Monster{0}", i % 4),

                    HP = rows[i]["hpPower"]["S"].ToString().FastStringToInt(),

                    Damage = rows[i]["num"]["S"].ToString().FastStringToInt(),
                };

                m_monsterDatas.Add(data);
                m_monsterDatas_Dic.Add(data.MonsterID, data);
            }
        }
        //------------------------------------------------------------------------------------
        public MonsterData GetMonsterData(string id)
        {
            MonsterData data = null;
            m_monsterDatas_Dic.TryGetValue(id, out data);

            return data;
        }
        //------------------------------------------------------------------------------------
    }
}


