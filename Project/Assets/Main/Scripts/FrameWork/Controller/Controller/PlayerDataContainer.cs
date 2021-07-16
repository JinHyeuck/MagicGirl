using System.Collections.Generic;
namespace GameBerry
{
    public class PlayerEquipmentInfo
    {
        public int Id;
        public int Count = 0;
        public int Level = 0;
    }

    public class PlayerSkillInfo
    {
        public int Id;
        public int Count = 0;
        public int Level = 0;
    }

    public static class PlayerDataContainer
    {
        // �÷��̾� ���� ��ȭ ��Ȳ
        public static Dictionary<StatUpGradeType, int> m_upGradeStatLevel = new Dictionary<StatUpGradeType, int>();

        // �÷��̾ �������ִ� ���
        public static Dictionary<EquipmentType, Dictionary<int, PlayerEquipmentInfo>> m_equipmentInfo = new Dictionary<EquipmentType, Dictionary<int, PlayerEquipmentInfo>>();

        // �÷��̾ ������ ���
        public static Dictionary<EquipmentType, int> m_equipId = new Dictionary<EquipmentType, int>();

        // �÷��̾ ������ ��ų
        public static Dictionary<int, PlayerSkillInfo> m_skillInfo = new Dictionary<int, PlayerSkillInfo>();

        // �÷��̾ ��ų����   <SlotPageId, <SlotID, skillId>> ������ �����ִµ� �����Ѱ� ������ skillId�� -1�̴�.
        public static Dictionary<int, Dictionary<int, int>> m_skillSlotData = new Dictionary<int, Dictionary<int, int>>();

        public static int Level;
        public static double Exp;

        public static double Gold;
        public static int Dia;

        public static int EquipmentSton;
        public static int SkillSton;

        public static double Damage;
        public static double CriticalDamage;
        public static double CriticalDamagePer;

        public static double DamagePer;
        public static double EndDamage;
        public static double SkillDamage;

        public static int OpenSlotCount;
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
        public static double GetUpGradeStatPrice(StatUpGradeType type, int upgradestate)
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
        public static int GetOperationQualityValue(QualityType qualityType)
        {
            int orivalue = (int)qualityType;

            if (orivalue > 0)
            {
                return orivalue + ((orivalue - 1) * Define.EquipmentQualityOperationValue);
            }

            return orivalue;
        }
        //------------------------------------------------------------------------------------
        public static int GetNeedLevelUPEquipmentSton(EquipmentData equipmentdata, PlayerEquipmentInfo equipmentinfo)
        {
            int defaultcount = GetOperationQualityValue(equipmentdata.Quality) + ((int)equipmentdata.Grade * 20);

            if (equipmentinfo == null)
                return defaultcount;

            return defaultcount + (defaultcount * (int)((float)equipmentinfo.Level * 0.5f));
        }
        //------------------------------------------------------------------------------------
        public static double GetEquipmentOptionValue(EquipmentData equipmentdata, int equiplevel, EquipmentOption option)
        {
            if (equipmentdata == null)
                return 0.0;

            double basevalue = 0.0;

            equipmentdata.Option.TryGetValue(option, out basevalue);

            return basevalue + (equiplevel * basevalue);
        }
        //------------------------------------------------------------------------------------
        public static int GetNeedLevelUPSkillSton(SkillData skilldata, PlayerSkillInfo skillinfo)
        {
            int defaultcount = (int)skilldata.SkillGradeType * 5;

            if (skillinfo == null)
                return defaultcount;

            return defaultcount + (defaultcount * (int)((float)skillinfo.Level * 0.5f));
        }
        //------------------------------------------------------------------------------------
        public static double GetSkillOptionValue(SkillData skildata, int skilllevel)
        {
            if (skildata == null)
                return 0.0;

            double basevalue = skildata.OptionValue;

            return basevalue + ((skilllevel * basevalue) * 0.2);
        }
        //------------------------------------------------------------------------------------
    }
}