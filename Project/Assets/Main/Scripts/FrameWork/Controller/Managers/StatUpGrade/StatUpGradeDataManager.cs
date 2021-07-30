using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry.Managers
{
    public class StatUpGradeDataManager : MonoSingleton<StatUpGradeDataManager>
    {
        private Dictionary<StatType, StatElementValue> m_addStatValue = new Dictionary<StatType, StatElementValue>();

        //------------------------------------------------------------------------------------
        protected override void Init()
        {
            StatUpGradeDataOperator.Init();
        }
        //------------------------------------------------------------------------------------
        public void AddUpGradeStatLevel(StatType type, int level)
        { // 처음 TheBackEnd_PlayerTable에서만 사용된다.
            StatUpGradeDataContainer.m_upGradeStatLevel.Add(type, level);
        }
        //------------------------------------------------------------------------------------
        public void SetStatElementValue()
        {
            foreach (KeyValuePair<StatType, int> pair in StatUpGradeDataContainer.m_upGradeStatLevel)
            {
                if (m_addStatValue.ContainsKey(pair.Key) == false)
                    m_addStatValue.Add(pair.Key, new StatElementValue());

                m_addStatValue[pair.Key].StatValue += GetCurrentUpGradeStatValue(pair.Key);

                PlayerDataManager.Instance.AddStatElementValue(pair.Key, m_addStatValue[pair.Key]);
            }
        }
        //------------------------------------------------------------------------------------
        public int GetCurrentUpGradeStatLevel(StatType type)
        {
            return StatUpGradeDataContainer.m_upGradeStatLevel[type];
        }
        //------------------------------------------------------------------------------------
        public double GetUpGradeStatPrice(StatType type)
        {
            return StatUpGradeDataOperator.GetUpGradeStatPrice(type, GetCurrentUpGradeStatLevel(type));
        }
        //------------------------------------------------------------------------------------
        public double GetCurrentUpGradeStatValue(StatType type)
        {
            return StatUpGradeDataOperator.GetCurrentUpGradeStatValue(type, GetCurrentUpGradeStatLevel(type));
        }
        //------------------------------------------------------------------------------------
        public double GetNextUpGradeStatValue(StatType type)
        {
            return StatUpGradeDataOperator.GetCurrentUpGradeStatValue(type, GetCurrentUpGradeStatLevel(type) + 1);
        }
        //------------------------------------------------------------------------------------
        public bool IsMaxUpGrade(StatType type)
        {
            return StatUpGradeDataOperator.IsMaxStatUpGrade(type, GetCurrentUpGradeStatLevel(type));
        }
        //------------------------------------------------------------------------------------
        public bool InCreaseUpGradeStatLevel(StatType type)
        {
            if (PlayerDataManager.Instance.UseGold(GetUpGradeStatPrice(type)) == true)
            {
                StatUpGradeDataContainer.m_upGradeStatLevel[type]++;
                return true;
            }
            else
            {
                return false;
            }

            return false;
        }
        //------------------------------------------------------------------------------------
    }
}