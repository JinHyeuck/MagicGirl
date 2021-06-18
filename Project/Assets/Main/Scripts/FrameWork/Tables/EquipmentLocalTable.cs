using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace GameBerry
{
    public enum EquipmentOptionType
    {
        None = 0,
        Damage,
        HP,
        MP,
    }

    [System.Serializable]
    public class EquipmentData
    {
        public string EquipmentID = string.Empty;
        public string EquipmentName = string.Empty;
        public Sprite EquipmentImage = null;
        public EquipmentOptionType OptionType = EquipmentOptionType.None;
        public float OptionValue = 0.0f;

        public int Damage = 0;
        public int CriticalDamage = 0;
    }

    public class EquipmentLocalTable
    {
        public List<EquipmentData> m_equipmentDatas = new List<EquipmentData>();
        private Dictionary<string, EquipmentData> m_equipmentDatas_Dic = new Dictionary<string, EquipmentData>();

        //------------------------------------------------------------------------------------
        public void InitData()
        {
            JsonData chartJson = JsonMapper.ToObject(TheBackEnd.TheBackEnd.Instance.GetLocalChartData(Define.EquipmentTableKey));

            var rows = chartJson["rows"];

            for (int i = 0; i < rows.Count; ++i)
            {
                EquipmentData data = new EquipmentData
                {
                    EquipmentID = rows[i]["itemID"]["S"].ToString(),

                    EquipmentName = rows[i]["itemName"]["S"].ToString(),

                    Damage = rows[i]["hpPower"]["S"].ToString().FastStringToInt(),

                    CriticalDamage = rows[i]["num"]["S"].ToString().FastStringToInt(),
                };

                m_equipmentDatas.Add(data);
                m_equipmentDatas_Dic.Add(data.EquipmentID, data);
            }
        }
        //------------------------------------------------------------------------------------
        public EquipmentData GetEquipmentData(string id)
        {
            EquipmentData data = null;
            m_equipmentDatas_Dic.TryGetValue(id, out data);

            return data;
        }
        //------------------------------------------------------------------------------------
    }
}