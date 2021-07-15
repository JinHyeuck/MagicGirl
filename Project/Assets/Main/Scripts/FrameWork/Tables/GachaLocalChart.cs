using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace GameBerry
{
    public enum GaChaType
    { 
        None = 0,

        WeaponSingle,
        WeaponBundle,

        NecklaceSingle,
        NecklaceBundle,

        RingSingle,
        RingBundle,

        SkillSingle,
        SkillBundle,
    }

    public class GachaData
    {
        public GaChaType Type;
        public int Amount;
        public int Price;

        public string GachaChartName;
    }

    public class GachaLocalChart
    {
        private Dictionary<GaChaType, GachaData> m_gachaDataList_Dic = new Dictionary<GaChaType, GachaData>();

        //------------------------------------------------------------------------------------
        public void InitData()
        {
            JsonData chartJson = JsonMapper.ToObject(TheBackEnd.TheBackEnd.Instance.GetLocalChartData(Define.GachaChartKey));

            var rows = chartJson["rows"];

            for (int i = 0; i < rows.Count; ++i)
            {
                GaChaType eqtype = EnumExtensions.Parse<GaChaType>(rows[i]["gacha_id"]["S"].ToString());

                GachaData data = new GachaData
                {
                    Type = eqtype,

                    Amount = rows[i]["gacha_amount"]["S"].ToString().FastStringToInt(),

                    Price = rows[i]["gacha_price"]["S"].ToString().FastStringToInt(),

                    GachaChartName = rows[i]["gacha_tablename"]["S"].ToString(),
                };

                m_gachaDataList_Dic.Add(data.Type, data);
            }
        }
        //------------------------------------------------------------------------------------
        public GachaData GetGachaData(GaChaType id)
        {
            GachaData data = null;
            m_gachaDataList_Dic.TryGetValue(id, out data);

            return data;
        }
        //------------------------------------------------------------------------------------
    }
}