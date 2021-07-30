using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace GameBerry
{
    public enum GradeType
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

    public enum QualityType
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

    public enum EquipmentApplyOption
    { 
        EquipmentOption = 0,
        EnableOption,
    }

    public class EquipmentData
    {
        public int Id = 0;

        public string EquipmentName;

        public EquipmentType Type;

        public GradeType Grade = 0;
        public QualityType Quality = 0;

        public Sprite EquipmentSprite;

        public Dictionary<EquipmentApplyOption, List<StatType>> ApplyOption = new Dictionary<EquipmentApplyOption, List<StatType>>();
        public Dictionary<StatType, double> Option = new Dictionary<StatType, double>();

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

    public class EquipmentLocalChart
    {
        private Dictionary<int, EquipmentData> m_equipmentData_Dic = new Dictionary<int, EquipmentData>();
        private Dictionary<EquipmentType, List<EquipmentData>> m_equipmentDataList_Dic = new Dictionary<EquipmentType, List<EquipmentData>>();

        private string m_resourcePath = "TableResources/Textures/Equipment";

        //------------------------------------------------------------------------------------
        public void InitData()
        {
            JsonData chartJson = JsonMapper.ToObject(TheBackEnd.TheBackEnd.Instance.GetLocalChartData(Define.EquipmentChartKey));

            UnityEngine.U2D.SpriteAtlas Atlas = null;

            AssetBundleLoader.Instance.Load<UnityEngine.U2D.SpriteAtlas>(m_resourcePath, "EquipmentAtlas", o =>
            {
                Atlas = o as UnityEngine.U2D.SpriteAtlas;
            });

            var rows = chartJson["rows"];

            for (int i = 0; i < rows.Count; ++i)
            {
                EquipmentType eqtype = EnumExtensions.Parse<EquipmentType>(rows[i]["type"]["S"].ToString());
                
                EquipmentData data = new EquipmentData
                {
                    Id = rows[i]["equipment_id"]["S"].ToString().FastStringToInt(),

                    EquipmentName = rows[i]["equipment_name"]["S"].ToString(),

                    Type = eqtype,

                    Grade = (GradeType)rows[i]["grade"]["S"].ToString().FastStringToInt(),

                    Quality = (QualityType)rows[i]["quality"]["S"].ToString().FastStringToInt(),

                    //EquipmentSprite = Util.GetSpriteOnAssetBundle(string.Format("{0}/{1}", m_resourcePath, eqtype), eqtype.ToString()),
                };

                if (Atlas != null)
                    data.EquipmentSprite = Atlas.GetSprite(eqtype.ToString());

                // 이부분 스텟Maxcout로 하지 말고 equipment_option, enable_option에 있는 것들만 적용하게 바꾸기
                //for (int j = 0; j < (int)StatType.Max; ++j)
                //{
                //    StatType option = (StatType)j;
                //    try
                //    {
                //        string id = option.ToString();
                //        SetEquipmentOption(data ,option, rows[i][id]);
                //    }
                //    catch (System.Exception ex)
                //    {
                //        Debug.LogError(ex.ToString());
                //    }
                //}

                string applyoptions = rows[i]["equipment_option"]["S"].ToString();
                string[] applyoption = applyoptions.Split(',');

                List<StatType> optionlist = new List<StatType>();

                for (int j = 0; j < applyoption.Length; ++j)
                {
                    StatType option = EnumExtensions.Parse<StatType>(applyoption[j]);
                    optionlist.Add(option);

                    SetEquipmentOption(data, option, rows[i][option.ToString()]);
                }

                data.ApplyOption.Add(EquipmentApplyOption.EquipmentOption, optionlist);


                applyoptions = rows[i]["enable_option"]["S"].ToString();
                if (string.IsNullOrWhiteSpace(applyoptions) == false)
                {
                    applyoption = applyoptions.Split(',');

                    optionlist = new List<StatType>();

                    for (int j = 0; j < applyoption.Length; ++j)
                    {
                        StatType option = EnumExtensions.Parse<StatType>(applyoption[j]);
                        optionlist.Add(option);

                        SetEquipmentOption(data, option, rows[i][option.ToString()]);
                    }

                    data.ApplyOption.Add(EquipmentApplyOption.EnableOption, optionlist);
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
        private void SetEquipmentOption(EquipmentData equipdata, StatType option, JsonData jsondata)
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
                return -1;
            }
            else if ((int)x.Grade > (int)y.Grade)
            {
                return 1;
            }
            else
            {
                if ((int)x.Quality < (int)y.Quality)
                {
                    return -1;
                }
                else if ((int)x.Quality > (int)y.Quality)
                {
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