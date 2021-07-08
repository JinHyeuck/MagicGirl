using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace GameBerry
{
    public enum EquipmentGradeType
    { 
        None = 0,
        Common,
        UnCommon,
        Rare,
        Unique,
        Relic,
        Legendary,
        Eqic,
        Transcendence,
    }

    public enum EquipmentQualityType
    { 
        None = 0,
        Row,
        Middle,
        High,
        Highest,
    }

    public enum EquipmentType
    { 
        Weapon = 0,
        Necklace,
        Ring,

        Max,
    }

    public enum EquipmentOption
    {
        DamageInt = 0,
        CriticalDamage,
        DamagePer,
        EndDamage,
        SkillDamage,

        MPInt,
        MPPer,

        MPRecoveryInt,
        MPRecoveryPer,

        Castingspeed,
        Cooltime,
        Addexp,

        Max,
    }

    public class EquipmentData
    {
        public int Id = 0;

        public string EquipmentName;

        public EquipmentType Type;

        public EquipmentGradeType Grade = 0;
        public EquipmentQualityType Quality = 0;

        public Sprite EquipmentSprite;

        public Dictionary<EquipmentOption, double> Option = new Dictionary<EquipmentOption, double>();

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

        public EquipmentData PrevData = null;
        public EquipmentData NextData = null;
    }

    public class EquipmentLocalChart : MonoBehaviour
    {
        private Dictionary<int, EquipmentData> m_equipmentData_Dic = new Dictionary<int, EquipmentData>();
        private Dictionary<EquipmentType, List<EquipmentData>> m_equipmentDataList_Dic = new Dictionary<EquipmentType, List<EquipmentData>>();

        private string m_resourcePath = "TableResources/Textures/Equipment";

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

                    EquipmentName = rows[i]["equipment_name"]["S"].ToString(),

                    Type = eqtype,

                    Grade = (EquipmentGradeType)rows[i]["grade"]["S"].ToString().FastStringToInt(),

                    Quality = (EquipmentQualityType)rows[i]["quality"]["S"].ToString().FastStringToInt(),

                    EquipmentSprite = Util.GetSpriteOnAssetBundle(string.Format("{0}/{1}", m_resourcePath, eqtype), eqtype.ToString()),

                    //DamageInt = rows[i]["damageint"]["S"].ToString().ToDouble(),

                    //CriticalDamage = rows[i]["criticaldamage"]["S"].ToString().ToDouble(),

                    //DamagePer = rows[i]["damageper"]["S"].ToString().ToDouble(),

                    //EndDamage = rows[i]["enddamage"]["S"].ToString().ToDouble(),

                    //SkillDamage = rows[i]["skilldamage"]["S"].ToString().ToDouble(),

                    //MPInt = rows[i]["mpint"]["S"].ToString().ToDouble(),

                    //MPPer = rows[i]["mpper"]["S"].ToString().ToDouble(),

                    //MPRecoveryInt = rows[i]["mprecovery"]["S"].ToString().ToDouble(),

                    //MPRecoveryPer = rows[i]["mprecoveryper"]["S"].ToString().ToDouble(),

                    //Castingspeed = rows[i]["castingspeed"]["S"].ToString().ToDouble(),

                    //Cooltime = rows[i]["cooltime"]["S"].ToString().ToDouble(),

                    //Addexp = rows[i]["addexp"]["S"].ToString().ToDouble(),
                };

                for (int j = 0; j < (int)EquipmentOption.Max; ++j)
                {
                    EquipmentOption option = (EquipmentOption)j;
                    try
                    {
                        string id = option.ToString().ToLower();
                        SetEquipmentOption(data ,option, rows[i][id]);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError(ex.ToString());
                    }
                }

                m_equipmentData_Dic.Add(data.Id, data);
                if (m_equipmentDataList_Dic.ContainsKey(eqtype) == false)
                {
                    m_equipmentDataList_Dic.Add(eqtype, new List<EquipmentData>());
                }

                m_equipmentDataList_Dic[eqtype].Add(data);
            }

            foreach (KeyValuePair<EquipmentType, List<EquipmentData>> pair in m_equipmentDataList_Dic)
            {
                pair.Value.Sort(SortEquipmentData);
                //pair.Value.Sort(SortEquipmentData);

                //EquipmentData prevdata = null;

                for (int i = 0; i < pair.Value.Count; ++i)
                {
                    if (i == 0)
                        pair.Value[i].PrevData = pair.Value[pair.Value.Count - 1];
                    else
                        pair.Value[i].PrevData = pair.Value[i - 1];

                    if (i >= pair.Value.Count - 1)
                        pair.Value[i].NextData = pair.Value[0];
                    else
                        pair.Value[i].NextData = pair.Value[i + 1];
                }

                if (pair.Value.Count > 1)
                {
                    pair.Value[0].PrevData = pair.Value[pair.Value.Count - 1];
                    pair.Value[pair.Value.Count - 1].NextData = pair.Value[0];
                }
            }
        }
        //------------------------------------------------------------------------------------
        private void SetEquipmentOption(EquipmentData equipdata, EquipmentOption option, JsonData jsondata)
        {
            if (equipdata == null || jsondata == null)
                return;

            double optionvalue = jsondata["S"].ToString().ToDouble();

            if (optionvalue > 0.0)
            {
                equipdata.Option.Add(option, optionvalue);
            }
        }
        //------------------------------------------------------------------------------------
        private int SortEquipmentData(EquipmentData x, EquipmentData y)
        {
            if ((int)x.Grade < (int)y.Grade)
            {
                x.NextData = y;
                y.PrevData = x;

                return -1;
            }
            else if ((int)x.Grade > (int)y.Grade)
            {
                y.NextData = x;
                x.PrevData = y;

                return 1;
            }
            else
            {
                if ((int)x.Quality < (int)y.Quality)
                {
                    x.NextData = y;
                    y.PrevData = x;

                    return -1;
                }
                else if ((int)x.Quality > (int)y.Quality)
                {
                    y.NextData = x;
                    x.PrevData = y;

                    return 1;
                }
            }

            return 0;
        }
        //------------------------------------------------------------------------------------
        public EquipmentData GetEquipmentData(int id)
        {
            EquipmentData data = null;
            m_equipmentData_Dic.TryGetValue(id, out data);

            return data;
        }
        //------------------------------------------------------------------------------------
        public List<EquipmentData> GetEquipmentDataList(EquipmentType type)
        {
            List<EquipmentData> datalist = null;

            m_equipmentDataList_Dic.TryGetValue(type, out datalist);

            return datalist;
        }
        //------------------------------------------------------------------------------------
        public EquipmentData GetNextEquipmentData(int id)
        {
            EquipmentData data = GetEquipmentData(id);

            if (data != null)
            {
                List<EquipmentData> datalist = GetEquipmentDataList(data.Type);

                if (datalist != null && datalist.Count > 1)
                {
                    if (data.NextData != datalist[0])
                        return data.NextData;
                }
            }

            return null;
        }
        //------------------------------------------------------------------------------------
    }
}