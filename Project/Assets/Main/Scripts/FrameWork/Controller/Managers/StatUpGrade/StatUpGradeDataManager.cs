using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry.Managers
{
    public class StatUpGradeDataManager : MonoSingleton<StatUpGradeDataManager>
    {
        //------------------------------------------------------------------------------------
        public int GetCurrentUpGradeStatLevel(StatUpGradeType type)
        {
            return StatUpGradeDataContainer.m_upGradeStatLevel[type];
        }
        //------------------------------------------------------------------------------------
        public double GetUpGradeStatPrice(StatUpGradeType type)
        {
            return StatUpGradeDataOperator.GetUpGradeStatPrice(type, GetCurrentUpGradeStatLevel(type));
        }
        //------------------------------------------------------------------------------------
        public double GetCurrentUpGradeStatValue(StatUpGradeType type)
        {
            return StatUpGradeDataOperator.GetCurrentUpGradeStatValue(type, GetCurrentUpGradeStatLevel(type));
        }
        //------------------------------------------------------------------------------------
        public double GetNextUpGradeStatValue(StatUpGradeType type)
        {
            return StatUpGradeDataOperator.GetCurrentUpGradeStatValue(type, GetCurrentUpGradeStatLevel(type) + 1);
        }
        //------------------------------------------------------------------------------------
        public bool IsMaxUpGrade(StatUpGradeType type)
        {
            return StatUpGradeDataOperator.IsMaxStatUpGrade(type, GetCurrentUpGradeStatLevel(type));
        }
        //------------------------------------------------------------------------------------
        public bool InCreaseUpGradeStatLevel(StatUpGradeType type)
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