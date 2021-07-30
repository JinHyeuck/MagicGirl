using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry
{
    public static class StatUpGradeDataContainer
    {
        // 플레이어 스텟 강화 현황
        public static Dictionary<StatType, int> m_upGradeStatLevel = new Dictionary<StatType, int>();
    }

    public static class StatUpGradeDataOperator
    {
        private static StatUpGradeLocalChart m_statUpGradeLocalChart = null;

        //------------------------------------------------------------------------------------
        public static void Init()
        {
            m_statUpGradeLocalChart = Managers.TableManager.Instance.GetTableClass<StatUpGradeLocalChart>();
        }
        //------------------------------------------------------------------------------------
        public static double GetUpGradeStatPrice(StatType type, int upgradestate)
        {
            if (m_statUpGradeLocalChart == null)
                return double.MaxValue;

            StatUpGradeData data = m_statUpGradeLocalChart.GetStatUpGradeData(type);

            if (data == null)
                return double.MaxValue;

            double price = data.DefaultPrice + (upgradestate * data.DefaultIncreasePrice) + ((upgradestate / data.AddIncreaseLevel) * data.AddIncreaseLevelPrice);

            return price;
        }
        //------------------------------------------------------------------------------------
        public static double GetCurrentUpGradeStatValue(StatType type, int upgradestate)
        {
            if (m_statUpGradeLocalChart == null)
                return 0.0;

            StatUpGradeData data = m_statUpGradeLocalChart.GetStatUpGradeData(type);

            if (data == null)
                return 0.0;

            return data.AddValue * upgradestate;
        }
        //------------------------------------------------------------------------------------
        public static bool IsMaxStatUpGrade(StatType type, int upgradestate)
        {
            if (m_statUpGradeLocalChart == null)
                return true;

            StatUpGradeData data = m_statUpGradeLocalChart.GetStatUpGradeData(type);

            if (data == null)
                return true;

            if (data.MaxLevel <= upgradestate)
                return true;

            return false;
        }
        //------------------------------------------------------------------------------------
    }
}

