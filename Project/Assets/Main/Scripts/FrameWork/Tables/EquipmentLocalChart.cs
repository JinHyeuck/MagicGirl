using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace GameBerry
{
    public enum EquipmentType
    { 
        Weapon = 0,
        Necklace,
        Ring,
        Max,
    }

    public class EquipmentData
    {
        public int Id = 0;

        public EquipmentType Type;

        public int Grade = 0;
        public int Quality = 0;

        public double DamageInt = 0.0;
        public double CriticalDamage = 0.0;
        public double DamagePer = 0.0;
        public double EndDamage = 0.0;
        public double SkillDamage = 0.0;

        public double MPInt = 0.0;
        public double MPPer = 0.0;

        public double MPRecoveryInt = 0.0;
        public double MPRecoveryPer = 0.0;

        public double Castingspeed = 0.0;
        public double Cooltime = 0.0;
        public double Addexp = 0.0;
    }

    public class EquipmentLocalChart : MonoBehaviour
    {
        private Dictionary<int, EquipmentData> m_equipmentData_Dic = new Dictionary<int, EquipmentData>();
        private Dictionary<EquipmentType, List<EquipmentData>> m_equipmentDataList_Dic = new Dictionary<EquipmentType, List<EquipmentData>>();

        //------------------------------------------------------------------------------------
        public void InitData()
        {
            JsonData chartJson = JsonMapper.ToObject(TheBackEnd.TheBackEnd.Instance.GetLocalChartData(Define.EquipmentChartKey));

            var rows = chartJson["rows"];

            for (int i = 0; i < rows.Count; ++i)
            {
                EquipmentType eqtype = EnumExtensions.Parse<EquipmentType>(rows[i]["type"]["S"].ToString());

                EquipmentData data = new EquipmentData
                {
                    Id = rows[i]["equipment_id"]["S"].ToString().FastStringToInt(),

                    Type = eqtype,

                    Grade = rows[i]["grade"]["S"].ToString().FastStringToInt(),

                    Quality = rows[i]["quality"]["S"].ToString().FastStringToInt(),

                    DamageInt = rows[i]["damageint"]["S"].ToString().ToDouble(),

                    CriticalDamage = rows[i]["criticaldamage"]["S"].ToString().ToDouble(),

                    DamagePer = rows[i]["damageint"]["S"].ToString().ToDouble(),

                    EndDamage = rows[i]["damageint"]["S"].ToString().ToDouble(),

                    SkillDamage = rows[i]["damageint"]["S"].ToString().ToDouble(),

                    MPInt = rows[i]["mp"]["S"].ToString().ToDouble(),

                    MPPer = rows[i]["mpper"]["S"].ToString().ToDouble(),

                    MPRecoveryInt = rows[i]["mprecovery"]["S"].ToString().ToDouble(),

                    MPRecoveryPer = rows[i]["mprecoveryper"]["S"].ToString().ToDouble(),

                    Castingspeed = rows[i]["castingspeed"]["S"].ToString().ToDouble(),

                    Cooltime = rows[i]["cooltime"]["S"].ToString().ToDouble(),

                    Addexp = rows[i]["addexp"]["S"].ToString().ToDouble(),
                };

                m_equipmentData_Dic.Add(data.Id, data);
                if (m_equipmentDataList_Dic.ContainsKey(eqtype) == false)
                {
                    m_equipmentDataList_Dic.Add(eqtype, new List<EquipmentData>());
                }

                m_equipmentDataList_Dic[eqtype].Add(data);
            }
        }
        //------------------------------------------------------------------------------------
        public EquipmentData GetWeaponData(int id)
        {
            EquipmentData data = null;
            m_equipmentData_Dic.TryGetValue(id, out data);

            return data;
        }
        //------------------------------------------------------------------------------------
        public List<EquipmentData> GetEquipmentDataList(EquipmentType type)
        {
            return m_equipmentDataList_Dic[type];
        }
        //------------------------------------------------------------------------------------
    }
}