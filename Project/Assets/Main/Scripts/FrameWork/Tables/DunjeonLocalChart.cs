using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry
{
    public enum DunjeonRewardType : byte
    {
        None = 0,
        Gold,
        Exp,
        EquipmentSton,
        SkillSton,
    }

    public class DunjeonMonsterReward
    {
        public double Gold;
        public double Exp;
        public int EquipmentSton;
        public int SkillSton;
    }

    public class DunjeonData
    {
        public int DunjeonIndex = 0;
        public string DunjeonID = string.Empty;
        public string DunjeonName = string.Empty;

        public List<string> DunjeonMonsterID = new List<string>();
        public List<string> DunjeonBossID = new List<string>();

        public float HPRatio;
        public float DamageRatio;

        public DunjeonMonsterReward Reward = new DunjeonMonsterReward();
    }

    public class DunjeonLocalChart
    {
        public List<DunjeonData> m_dunjeonDataDatas = new List<DunjeonData>();
        private Dictionary<string, DunjeonData> m_dunjeonDataDatas_Dic = new Dictionary<string, DunjeonData>();

        //------------------------------------------------------------------------------------
        public void InitData()
        {
            MonsterLocalChart monstertable = Managers.TableManager.Instance.GetTableClass<MonsterLocalChart>();

            for (int i = 0; i < 50; ++i)
            {
                DunjeonData data = new DunjeonData();
                data.DunjeonIndex = i;

                data.DunjeonID = string.Format("dunjeon{0}", i.ToString());
                data.DunjeonName = string.Format("dunjeon{0}", i.ToString());

                if (monstertable.m_monsterDatas.Count > i)
                {
                    if (i > 0)
                    {
                        data.DunjeonMonsterID.Add(monstertable.m_monsterDatas[i].MonsterID);
                        data.DunjeonMonsterID.Add(monstertable.m_monsterDatas[i - 1].MonsterID);

                        data.DunjeonBossID.Add(monstertable.m_monsterDatas[i].MonsterID);
                    }
                    else if(i == 0)
                    {
                        data.DunjeonMonsterID.Add(monstertable.m_monsterDatas[i].MonsterID);
                        data.DunjeonBossID.Add(monstertable.m_monsterDatas[i].MonsterID);
                    }
                }
                else
                {
                    data.DunjeonMonsterID.Add(monstertable.m_monsterDatas[monstertable.m_monsterDatas.Count - 1].MonsterID);
                }

                data.HPRatio = 1.0f + (0.01f * i);
                data.DamageRatio = 1.0f + (0.01f * i);

                data.Reward.Gold = (i + 1) * 5;
                data.Reward.Exp = (i + 1) * 3;
                data.Reward.EquipmentSton = (i + 1) * 4;
                data.Reward.SkillSton = (i + 1) * 2;

                m_dunjeonDataDatas.Add(data);
                m_dunjeonDataDatas_Dic.Add(data.DunjeonID, data);
            }
        }
        //------------------------------------------------------------------------------------
        public DunjeonData GetDunjeonData(string id)
        {
            DunjeonData data = null;
            m_dunjeonDataDatas_Dic.TryGetValue(id, out data);

            return data;
        }
        //------------------------------------------------------------------------------------
        public DunjeonData GetDunjeonData(int index)
        {
            if (index < 0 || index >= m_dunjeonDataDatas.Count)
            {
                if (m_dunjeonDataDatas.Count > 0)
                    return m_dunjeonDataDatas[m_dunjeonDataDatas.Count - 1];
                else
                    return null;
            }

            return m_dunjeonDataDatas[index];
        }
        //------------------------------------------------------------------------------------
    }
}