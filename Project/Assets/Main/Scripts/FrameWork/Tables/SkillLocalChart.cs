using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace GameBerry
{
    public enum SkillOptionType
    {
        None = 0,
        Damage,
        RecoveryHP,
        RecoveryMP,
        MoveSpeed,
        AttackSpeed,
    }

    public enum SkillTriggerType
    {
        Passive = 0,
        Active,
        Buff,

        Max,
    }

    [System.Serializable]
    public class SkillData
    {
        public int Index = 0;
        public int SkillID = -1;
        public string SkillName = string.Empty;
        public GradeType SkillGradeType = GradeType.None;
        public SkillTriggerType SkillTriggerType = SkillTriggerType.Max;
        public SkillOptionType OptionType = SkillOptionType.None;
        public double OptionValue = 0.0f;
        public int MaxTarget;
        public double ApplyTime;
        public double CoolTime;
        public double Range;
        public int NeedMP;

        public string SkillSpriteName = null;
        public Sprite SKillEffect = null;
    }

    public class SkillLocalChart
    {
        public List<SkillData> m_SkillDatas = new List<SkillData>();
        private Dictionary<int, SkillData> m_SkillDatas_Dic = new Dictionary<int, SkillData>();

        //------------------------------------------------------------------------------------
        public void InitData()
        {
            JsonData chartJson = JsonMapper.ToObject(TheBackEnd.TheBackEnd.Instance.GetLocalChartData(Define.SkillItemChartKey));

            var rows = chartJson["rows"];

            for (int i = 0; i < rows.Count; ++i)
            {
                SkillData data = new SkillData
                {
                    Index = rows[i]["index"]["S"].ToString().FastStringToInt(),

                    SkillID = rows[i]["skill_id"]["S"].ToString().FastStringToInt(),

                    SkillName = rows[i]["skill_name"]["S"].ToString(),

                    SkillGradeType = (GradeType)rows[i]["skill_grade"]["S"].ToString().FastStringToInt(),

                    SkillTriggerType = EnumExtensions.Parse<SkillTriggerType>(rows[i]["skill_trigger_type"]["S"].ToString()),

                    OptionType = EnumExtensions.Parse<SkillOptionType>(rows[i]["skill_option_type"]["S"].ToString()),

                    OptionValue = rows[i]["skill_value"]["S"].ToString().ToDouble(),

                    MaxTarget = rows[i]["skill_maxtarget"]["S"].ToString().FastStringToInt(),

                    ApplyTime = rows[i]["skill_applytime"]["S"].ToString().ToDouble(),

                    CoolTime = rows[i]["skill_cooltime"]["S"].ToString().ToDouble(),

                    Range = rows[i]["skill_range"]["S"].ToString().ToDouble(),

                    NeedMP = i != 0 ? rows[i]["skill_needmp"]["S"].ToString().FastStringToInt() : 0
                };

                data.SkillSpriteName = string.Format("skill_{0}", data.Index);

                m_SkillDatas.Add(data);
                m_SkillDatas_Dic.Add(data.SkillID, data);
            }

            m_SkillDatas.Sort((x, y) =>
            {
                if (x.Index < y.Index)
                {
                    return -1;
                }
                else if (x.Index > y.Index)
                {
                    return 1;
                }

                return 0;
            });
        }
        //------------------------------------------------------------------------------------
        public SkillData GetSkillData(int id)
        {
            SkillData data = null;
            m_SkillDatas_Dic.TryGetValue(id, out data);

            return data;
        }
        //------------------------------------------------------------------------------------
    }
}