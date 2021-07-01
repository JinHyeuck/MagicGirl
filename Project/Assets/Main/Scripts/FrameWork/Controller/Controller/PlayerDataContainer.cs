using System.Collections.Generic;
namespace GameBerry
{
    public class PlayerEquipmentInfo
    {
        public int Index;
        public int Count;
        public int Level;
    }

    public static class PlayerDataContainer
    {
        public static Dictionary<StatUpGradeType, int> m_upGradeStatLevel = new Dictionary<StatUpGradeType, int>();

        public static Dictionary<EquipmentType, Dictionary<int, PlayerEquipmentInfo>> m_equipmentInfo = new Dictionary<EquipmentType, Dictionary<int, PlayerEquipmentInfo>>();

        public static int Level;
        public static double Exp;

        public static double Gold;
        public static int Dia;

        public static int EquipmentSton;
        public static int SkillSton;
    }

    public static class PlayerDataOperator
    {
        private static StatUpGradeLocalChart m_statUpGradeLocalChart = null;

        //------------------------------------------------------------------------------------
        public static void Init()
        {
            m_statUpGradeLocalChart = Managers.TableManager.Instance.GetTableClass<StatUpGradeLocalChart>();
        }
        //------------------------------------------------------------------------------------
        public static double GetUpGradePrice(StatUpGradeType type, int upgradestate)
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
        public static double GetCurrentUpGradeStatValue(StatUpGradeType type, int upgradestate)
        {
            if (m_statUpGradeLocalChart == null)
                return 0.0;

            StatUpGradeData data = m_statUpGradeLocalChart.GetStatUpGradeData(type);

            if (data == null)
                return 0.0;

            return data.AddValue * upgradestate;
        }
        //------------------------------------------------------------------------------------
        public static bool IsMaxUpGrade(StatUpGradeType type, int upgradestate)
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