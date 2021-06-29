using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace GameBerry
{
    [System.Serializable]
    public class WeaponData
    {
        public int WeaponIndex = 0;

        public int Grade = 0;
        public int Quality = 0;

        public double DamageInt = 0.0;
        public double CriticalDamage = 0.0;
        public double DamagePer = 0.0;
        public double EndDamage = 0.0;
        public double SkillDamage = 0.0;
    }

    public class WeaponLocalChart
    {
        public List<WeaponData> m_weaponDatas = new List<WeaponData>();
        private Dictionary<int, WeaponData> m_weaponDatas_Dic = new Dictionary<int, WeaponData>();

        //------------------------------------------------------------------------------------
        public void InitData()
        {
            JsonData chartJson = JsonMapper.ToObject(TheBackEnd.TheBackEnd.Instance.GetLocalChartData(Define.MonsterChartKey));

            var rows = chartJson["rows"];

            for (int i = 0; i < rows.Count; ++i)
            {
                WeaponData data = new WeaponData
                {
                    WeaponIndex = rows[i]["index"]["S"].ToString().FastStringToInt(),

                    Grade = rows[i]["grade"]["S"].ToString().FastStringToInt(),

                    Quality = rows[i]["quality"]["S"].ToString().FastStringToInt(),

                    DamageInt = rows[i]["damageint"]["S"].ToString().ToDouble(),

                    CriticalDamage = rows[i]["criticaldamage"]["S"].ToString().ToDouble(),

                    DamagePer = rows[i]["damageint"]["S"].ToString().ToDouble(),

                    EndDamage = rows[i]["damageint"]["S"].ToString().ToDouble(),

                    SkillDamage = rows[i]["damageint"]["S"].ToString().ToDouble(),
                };

                m_weaponDatas.Add(data);
                m_weaponDatas_Dic.Add(data.WeaponIndex, data);
            }
        }
        //------------------------------------------------------------------------------------
        public WeaponData GetWeaponData(int id)
        {
            WeaponData data = null;
            m_weaponDatas_Dic.TryGetValue(id, out data);

            return data;
        }
        //------------------------------------------------------------------------------------
    }
}