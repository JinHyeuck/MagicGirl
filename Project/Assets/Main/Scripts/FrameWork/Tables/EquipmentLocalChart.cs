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
    }

    public class EquipmentData
    {
        public int Index = 0;

        public int Grade = 0;
        public int Quality = 0;
    }

    public class WeaponData : EquipmentData
    {
        public double DamageInt = 0.0;
        public double CriticalDamage = 0.0;
        public double DamagePer = 0.0;
        public double EndDamage = 0.0;
        public double SkillDamage = 0.0;
    }

    public class accessoryData : EquipmentData
    {
        public double Castingspeed = 0.0;
        public double Cooltime = 0.0;
        public double Addexp = 0.0;
    }

    public class NecklaceData : accessoryData
    {
        public double MPInt = 0.0;
        public double MPPer = 0.0;
    }

    public class RinglaceData : accessoryData
    {
        public double MPRecoveryInt = 0.0;
        public double MPRecoveryPer = 0.0;
    }

    public class EquipmentLocalChart : MonoBehaviour
    {
        public List<EquipmentData> m_weaponDatas = new List<EquipmentData>();
        private Dictionary<int, WeaponData> m_weaponDatas_Dic = new Dictionary<int, WeaponData>();

        public List<EquipmentData> m_necklaceDatas = new List<EquipmentData>();
        private Dictionary<int, NecklaceData> m_necklaceDatas_Dic = new Dictionary<int, NecklaceData>();

        public List<EquipmentData> m_ringDatas = new List<EquipmentData>();
        private Dictionary<int, RinglaceData> m_ringDatas_Dic = new Dictionary<int, RinglaceData>();
        //------------------------------------------------------------------------------------
        public void InitData()
        {
            InitWeaponData();
            InitNecklaceData();
            InitRingData();
        }
        //------------------------------------------------------------------------------------
        private void InitWeaponData()
        {
            JsonData chartJson = JsonMapper.ToObject(TheBackEnd.TheBackEnd.Instance.GetLocalChartData(Define.WeaponChartKey));

            var rows = chartJson["rows"];

            for (int i = 0; i < rows.Count; ++i)
            {
                WeaponData data = new WeaponData
                {
                    Index = rows[i]["index"]["S"].ToString().FastStringToInt(),

                    Grade = rows[i]["grade"]["S"].ToString().FastStringToInt(),

                    Quality = rows[i]["quality"]["S"].ToString().FastStringToInt(),

                    DamageInt = rows[i]["damageint"]["S"].ToString().ToDouble(),

                    CriticalDamage = rows[i]["criticaldamage"]["S"].ToString().ToDouble(),

                    DamagePer = rows[i]["damageint"]["S"].ToString().ToDouble(),

                    EndDamage = rows[i]["damageint"]["S"].ToString().ToDouble(),

                    SkillDamage = rows[i]["damageint"]["S"].ToString().ToDouble(),
                };

                m_weaponDatas.Add(data);
                m_weaponDatas_Dic.Add(data.Index, data);
            }
        }
        //------------------------------------------------------------------------------------
        private void InitNecklaceData()
        {
            JsonData chartJson = JsonMapper.ToObject(TheBackEnd.TheBackEnd.Instance.GetLocalChartData(Define.NecklaceChartKey));

            var rows = chartJson["rows"];

            for (int i = 0; i < rows.Count; ++i)
            {
                NecklaceData data = new NecklaceData
                {
                    Index = rows[i]["index"]["S"].ToString().FastStringToInt(),

                    Grade = rows[i]["grade"]["S"].ToString().FastStringToInt(),

                    Quality = rows[i]["quality"]["S"].ToString().FastStringToInt(),

                    Castingspeed = rows[i]["castingspeed"]["S"].ToString().ToDouble(),

                    Cooltime = rows[i]["criticaldamage"]["S"].ToString().ToDouble(),

                    Addexp = rows[i]["cooltime"]["S"].ToString().ToDouble(),

                    MPInt = rows[i]["mp"]["S"].ToString().ToDouble(),

                    MPPer = rows[i]["mpper"]["S"].ToString().ToDouble(),
                };

                m_necklaceDatas.Add(data);
                m_necklaceDatas_Dic.Add(data.Index, data);
            }
        }
        //------------------------------------------------------------------------------------
        private void InitRingData()
        {
            JsonData chartJson = JsonMapper.ToObject(TheBackEnd.TheBackEnd.Instance.GetLocalChartData(Define.RingChartKey));

            var rows = chartJson["rows"];

            for (int i = 0; i < rows.Count; ++i)
            {
                RinglaceData data = new RinglaceData
                {
                    Index = rows[i]["index"]["S"].ToString().FastStringToInt(),

                    Grade = rows[i]["grade"]["S"].ToString().FastStringToInt(),

                    Quality = rows[i]["quality"]["S"].ToString().FastStringToInt(),

                    Castingspeed = rows[i]["castingspeed"]["S"].ToString().ToDouble(),

                    Cooltime = rows[i]["criticaldamage"]["S"].ToString().ToDouble(),

                    Addexp = rows[i]["cooltime"]["S"].ToString().ToDouble(),

                    MPRecoveryInt = rows[i]["mp"]["S"].ToString().ToDouble(),

                    MPRecoveryPer = rows[i]["mpper"]["S"].ToString().ToDouble(),
                };

                m_ringDatas.Add(data);
                m_ringDatas_Dic.Add(data.Index, data);
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
        public NecklaceData GetNecklaceData(int id)
        {
            NecklaceData data = null;
            m_necklaceDatas_Dic.TryGetValue(id, out data);

            return data;
        }
        //------------------------------------------------------------------------------------
        public RinglaceData GetRingData(int id)
        {
            RinglaceData data = null;
            m_ringDatas_Dic.TryGetValue(id, out data);

            return data;
        }
        //------------------------------------------------------------------------------------
        public EquipmentData GetEquipmentData(EquipmentType type, int id)
        {
            if (type == EquipmentType.Weapon)
                return GetWeaponData(id);
            else if (type == EquipmentType.Necklace)
                return GetNecklaceData(id);
            else if (type == EquipmentType.Ring)
                return GetRingData(id);

            return null;
        }
        //------------------------------------------------------------------------------------
        public List<EquipmentData> GetEquipmentList(EquipmentType type)
        {
            if (type == EquipmentType.Weapon)
                return m_weaponDatas;
            else if (type == EquipmentType.Necklace)
                return m_necklaceDatas;
            else if (type == EquipmentType.Ring)
                return m_ringDatas;

            return null;
        }
    }
}