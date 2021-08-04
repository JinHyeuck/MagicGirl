using System.Collections.Generic;
namespace GameBerry
{
    public enum StatType
    { 
        BaseDamage = 0,
        BaseDamagePer,

        CriticalDamage,
        CriticalPer,

        EndDamagePer,
        SkillDamagePer,

        HPBase,
        HPPer,
        HPRecoveryBase,
        HPRecoveryPer,

        MPBase,
        MPPer,
        MPRecoveryBase,
        MPRecoveryPer,

        MoveSpeed,
        AttackSpeed,

        AddGoldPer,
        AddExpPer,

        CoolTimePer,
        CastingSpeedPer,

        Max,
    }

    public enum HpMpVarianceType
    { 
        None = 0,
        NomalDamage,
        CriticalDamage,
        Miss,
        RecoveryHP,
        RecoveryMP,
    }

    public class StatElementValue
    {
        public double StatValue = 0.0;
    }

    public struct PlayerDamageData
    {
        public double DamageValue;
        public HpMpVarianceType DamageType;
    }

    public static class PlayerDataContainer
    {
        public static int Level;
        public static double Exp;

        public static double Gold;
        public static int Dia;

        public static int EquipmentSton;
        public static int SkillSton;


        public static Dictionary<StatType, List<StatElementValue>> m_addStatValues = new Dictionary<StatType, List<StatElementValue>>();


        // ��꿡 �ʿ��� ����
        // ������ �ƿ�ǲ �� �ʿ��� ģ����
        public static double BaseDamage;
        public static double BaseDamagePer;
        
        public static double CriticalDamage;
        public static double CriticalPer;
        
        public static double EndDamagePer;
        public static double SkillDamagePer;
        // ������ �ƿ�ǲ �� �ʿ��� ģ����

        public static double HPBase;
        public static double HPPer;
        public static double HPRecoveryBase;
        public static double HPRecoveryPer;
        
        public static double MPBase;
        public static double MPPer;
        public static double MPRecoveryBase;
        public static double MPRecoveryPer;
        
        public static double MoveSpeed;
        public static double AttackSpeed;
        
        public static double AddGoldPer;
        public static double AddExpPer;
        
        public static double CoolTimePer;
        public static double CastingSpeedPer;
        // ��꿡 �ʿ��� ����


        // �÷��̾��� ũ��Ƽ���� ������ ������
        public static double OutPutDamage;
        // �÷��̾��� ũ��Ƽ�õ��������� ������ ������
        public static double OutPutCriticalDamage;

        public static double OutPutHP;
        public static double OutPutHPRecovery;

        public static double OutPutMP;
        public static double OutPutMPRecovery;


    }

    public static class PlayerDataOperator
    {
        
        public static void SetOutPutDamage()
        {
            PlayerDataContainer.BaseDamage = GetTotalAddStatValue(StatType.BaseDamage) + Define.PlayerDefaultDamage;
            PlayerDataContainer.BaseDamagePer = GetTotalAddStatValue(StatType.BaseDamagePer) + Define.PlayerDefaultDamagePer;
            PlayerDataContainer.EndDamagePer = GetTotalAddStatValue(StatType.EndDamagePer) + Define.PlayerDefaultEndDamagePer;
            PlayerDataContainer.CriticalDamage = GetTotalAddStatValue(StatType.CriticalDamage) + Define.PlayerDefaultCriticalDamage;
            PlayerDataContainer.SkillDamagePer = GetTotalAddStatValue(StatType.SkillDamagePer) + Define.PlayerDefaultSkillDamagePer;

            PlayerDataContainer.OutPutDamage = PlayerDataContainer.BaseDamage * PlayerDataContainer.BaseDamagePer * PlayerDataContainer.EndDamagePer;
            PlayerDataContainer.OutPutCriticalDamage = PlayerDataContainer.OutPutDamage * PlayerDataContainer.CriticalDamage;
        }
        //------------------------------------------------------------------------------------
        public static void SetOutPutCriticalDamage()
        {
            PlayerDataContainer.CriticalDamage = GetTotalAddStatValue(StatType.CriticalDamage) + Define.PlayerDefaultCriticalDamage;

            PlayerDataContainer.OutPutCriticalDamage = PlayerDataContainer.OutPutDamage * PlayerDataContainer.CriticalDamage;
        }
        //------------------------------------------------------------------------------------
        public static void SetOutPutHP()
        {
            PlayerDataContainer.HPBase = GetTotalAddStatValue(StatType.HPBase) + Define.PlayerDefaultHP;
            PlayerDataContainer.HPPer = GetTotalAddStatValue(StatType.HPPer) + Define.PlayerDefaultHPPer;

            PlayerDataContainer.OutPutHP = PlayerDataContainer.HPBase * PlayerDataContainer.HPPer;
        }
        //------------------------------------------------------------------------------------
        public static void SetOutPutHPRecovery()
        {
            PlayerDataContainer.HPRecoveryBase = GetTotalAddStatValue(StatType.HPRecoveryBase) + Define.PlayerDefaultHPRecovery;
            PlayerDataContainer.HPRecoveryPer = GetTotalAddStatValue(StatType.HPRecoveryPer);

            PlayerDataContainer.OutPutHPRecovery = PlayerDataContainer.HPRecoveryBase * PlayerDataContainer.HPRecoveryPer;
        }
        //------------------------------------------------------------------------------------
        public static void SetOutPutMP()
        {
            PlayerDataContainer.MPBase = GetTotalAddStatValue(StatType.MPBase) + Define.PlayerDefaultMP;
            PlayerDataContainer.MPPer = GetTotalAddStatValue(StatType.MPPer) + Define.PlayerDefaultMPPer;

            PlayerDataContainer.OutPutMP = PlayerDataContainer.MPBase * PlayerDataContainer.MPPer;
        }
        //------------------------------------------------------------------------------------
        public static void SetOutPutMPRecovery()
        {
            PlayerDataContainer.MPRecoveryBase = GetTotalAddStatValue(StatType.MPRecoveryBase) + Define.PlayerDefaultMPRecovery;
            PlayerDataContainer.MPRecoveryPer = GetTotalAddStatValue(StatType.MPRecoveryPer);

            PlayerDataContainer.OutPutMPRecovery = PlayerDataContainer.MPRecoveryBase * PlayerDataContainer.MPRecoveryPer;
        }
        //------------------------------------------------------------------------------------
        private static double GetTotalAddStatValue(StatType type)
        {
            if (PlayerDataContainer.m_addStatValues.ContainsKey(type) == false)
                return 0.0;

            List<StatElementValue> AddStatList = PlayerDataContainer.m_addStatValues[type];
            double AddStat = 0.0;
            for (int i = 0; i < AddStatList.Count; ++i)
            {
                AddStat += AddStatList[i].StatValue;
            }

            return AddStat;
        }
        //------------------------------------------------------------------------------------
        public static PlayerDamageData GetAttackDamage()
        {
            bool ApplyCrtical = UnityEngine.Random.Range(0.01f, 100.0f) <= PlayerDataContainer.CriticalPer;

            PlayerDamageData damageData;
            damageData.DamageType = ApplyCrtical == true ? HpMpVarianceType.CriticalDamage : HpMpVarianceType.NomalDamage;
            damageData.DamageValue = ApplyCrtical == true ? PlayerDataContainer.OutPutCriticalDamage : PlayerDataContainer.OutPutDamage;

            return damageData;
        }
        //------------------------------------------------------------------------------------
    }
}