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
        None = 0,
        Passive,
        Active,
    }

    [System.Serializable]
    public class SkillData
    {
        public string SkillID = string.Empty;
        public string SkillName = string.Empty;
        public SkillTriggerType SkillType = SkillTriggerType.None;
        public SkillOptionType OptionType = SkillOptionType.None;
        public float OptionValue = 0.0f;
        public int MaxTarget;
        public float CoolTime;
        public float Range;
        public int Damage;
        public int NeedMP;

        public Sprite SkillImage = null;
        public Sprite SKillEffect = null;
    }

    public class SkillLocalChart
    {
        [SerializeField]
        public List<SkillData> m_SkillDatas = new List<SkillData>();
        private Dictionary<string, SkillData> m_SkillDatas_Dic = new Dictionary<string, SkillData>();

        //------------------------------------------------------------------------------------
        public void InitData()
        {
            JsonData chartJson = JsonMapper.ToObject(TheBackEnd.TheBackEnd.Instance.GetLocalChartData(Define.SkillItemChartKey));

            var rows = chartJson["rows"];

            for (int i = 0; i < rows.Count; ++i)
            {
                float tempopervalue = (float)rows[i]["num"]["S"].ToString().FastStringToInt();

                float temprange = 1.0f + (tempopervalue * 0.1f);

                SkillData data = new SkillData
                {
                    SkillID = rows[i]["itemID"]["S"].ToString(),

                    SkillName = rows[i]["itemName"]["S"].ToString(),

                    CoolTime = tempopervalue * 0.5f,

                    Range = temprange,

                    Damage = rows[i]["hpPower"]["S"].ToString().FastStringToInt(),

                    NeedMP = i != 0 ? rows[i]["num"]["S"].ToString().FastStringToInt() : 0
                };

                m_SkillDatas.Add(data);
                m_SkillDatas_Dic.Add(data.SkillID, data);
            }
        }
        //------------------------------------------------------------------------------------
        public SkillData GetSkillData(string id)
        {
            SkillData data = null;
            m_SkillDatas_Dic.TryGetValue(id, out data);

            return data;
        }
        //------------------------------------------------------------------------------------
    }
}