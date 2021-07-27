using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry
{
    public class PlayerSkillInfo
    {
        public int Id;
        public int Count = 0;
        public int Level = 0;
    }

    public static class SkillDataContainer
    {
        // �÷��̾ ������ ��ų
        public static Dictionary<int, PlayerSkillInfo> m_skillInfo = new Dictionary<int, PlayerSkillInfo>();

        // �÷��̾ ��ų����   <SlotPageId, <SlotID, skillId>> ������ �����ִµ� �����Ѱ� ������ skillId�� -1�̴�.
        public static Dictionary<int, Dictionary<int, int>> m_skillSlotData = new Dictionary<int, Dictionary<int, int>>();

        // �÷��̾�� ����Ǿ��ִ� ������ų��(Passive, Buff)
        public static Dictionary<int, SkillData> m_applyBuffSkillData = new Dictionary<int, SkillData>();

        public static int CurrentOpenSlotCount;
        public static int SkillSlotPage;
    }

    public static class SkillDataOperator
    {
        public static int GetNeedLevelUPSkillSton(SkillData skilldata, PlayerSkillInfo skillinfo)
        {
            int defaultcount = (int)skilldata.SkillGradeType * 5;

            if (skillinfo == null)
                return defaultcount;

            return defaultcount + (defaultcount * (int)((float)(skillinfo.Level) * 0.5f));
        }
        //------------------------------------------------------------------------------------
        public static double GetSkillOptionValue(SkillData skildata, int skilllevel)
        {
            if (skildata == null)
                return 0.0;

            double basevalue = skildata.OptionValue;

            return basevalue + (basevalue * (skilllevel * 0.2));
        }
        //------------------------------------------------------------------------------------
    }
}