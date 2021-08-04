using System.Collections.Generic;
using UnityEngine;

namespace GameBerry.Managers
{
    public class PlayerDataManager : MonoSingleton<PlayerDataManager>
    {
        private float m_playerInfoDataSaveTime = 5.0f;
        private float m_playerInfoDataSaveTimer = 0.0f;

        private Event.RefreshLevelMsg m_refreshLevelMsg = new Event.RefreshLevelMsg();
        private Event.RefreshExpMsg m_refreshExpMsg = new Event.RefreshExpMsg();
        private Event.RefreshGoldMsg m_refreshGoldMsg = new Event.RefreshGoldMsg();
        private Event.RefreshDiaMsg m_refreshDiaMsg = new Event.RefreshDiaMsg();
        private Event.RefreshEquipmentStonMsg m_refreshEquipmentStonMsg = new Event.RefreshEquipmentStonMsg();
        private Event.RefreshSkillStonMsg m_refreshSkillStonMsg = new Event.RefreshSkillStonMsg();


        private LevelLocalChart m_levelLocalChart = null;
        //------------------------------------------------------------------------------------
        protected override void Init()
        {
            m_playerInfoDataSaveTimer = Time.time + m_playerInfoDataSaveTime;

            m_levelLocalChart = Managers.TableManager.Instance.GetTableClass<LevelLocalChart>();
        }
        //------------------------------------------------------------------------------------
        protected override void Release()
        {

        }
        //------------------------------------------------------------------------------------
        private void Update()
        {
            if (m_playerInfoDataSaveTimer < Time.time)
            {
                TheBackEnd.TheBackEnd.Instance.UpdateCharacterInfoTable();
                TheBackEnd.TheBackEnd.Instance.UpdateCharacterUpGradeStatTable();
                TheBackEnd.TheBackEnd.Instance.UpdateCharacterEquipmentInfoTable();
                TheBackEnd.TheBackEnd.Instance.UpdateCharacterSkillInfoTable();
                m_playerInfoDataSaveTimer = Time.time + m_playerInfoDataSaveTime;
            }
        }
        //------------------------------------------------------------------------------------
        public int GetLevel()
        {
            return PlayerDataContainer.Level;
        }
        //------------------------------------------------------------------------------------
        public void SetLevelUp()
        {
            PlayerDataContainer.Level += 1;
        }
        //------------------------------------------------------------------------------------
        public void AddExp(double exp)
        {
            PlayerDataContainer.Exp += exp;

            LevelData data = m_levelLocalChart.GetLevelData(PlayerDataContainer.Level);

            while (PlayerDataContainer.Exp >= data.Exp)
            {
                SetLevelUp();
                PlayerDataContainer.Exp -= data.Exp;
                m_levelLocalChart.GetLevelData(PlayerDataContainer.Level);
            }

            Message.Send(m_refreshLevelMsg);
            Message.Send(m_refreshExpMsg);
        }
        //------------------------------------------------------------------------------------
        public double GetExp()
        {
            return PlayerDataContainer.Exp;
        }
        //------------------------------------------------------------------------------------
        #region Goods
        //------------------------------------------------------------------------------------
        public void AddGold(double gold)
        {
            PlayerDataContainer.Gold += gold;
            Message.Send(m_refreshGoldMsg);
        }
        //------------------------------------------------------------------------------------
        public bool UseGold(double gold)
        {
            bool isSuccess = false;

            if (PlayerDataContainer.Gold > gold)
            { 
                PlayerDataContainer.Gold -= gold;
                Message.Send(m_refreshGoldMsg);
                isSuccess = true;
            }

            return isSuccess;
        }
        //------------------------------------------------------------------------------------
        public double GetGold()
        {
            return PlayerDataContainer.Gold;
        }
        //------------------------------------------------------------------------------------
        public void AddDia(int dia)
        {
            PlayerDataContainer.Dia += dia;
            Message.Send(m_refreshDiaMsg);
        }
        //------------------------------------------------------------------------------------
        public void UseDia(int dia)
        {
            PlayerDataContainer.Dia -= dia;
            Message.Send(m_refreshDiaMsg);
        }
        //------------------------------------------------------------------------------------
        public int GetDia()
        {
            return PlayerDataContainer.Dia;
        }
        //------------------------------------------------------------------------------------
        public void SetDia(int dia)
        {
            PlayerDataContainer.Dia = dia;
            Message.Send(m_refreshDiaMsg);
        }
        //------------------------------------------------------------------------------------
        public void AddEquipmentSton(int ston)
        {
            PlayerDataContainer.EquipmentSton += ston;
            Message.Send(m_refreshEquipmentStonMsg);
        }
        //------------------------------------------------------------------------------------
        public int GetEquipmentSton()
        {
            return PlayerDataContainer.EquipmentSton;
        }
        //------------------------------------------------------------------------------------
        public bool UseEquipmentSton(int ston)
        {
            bool isSuccess = false;

            if (PlayerDataContainer.EquipmentSton > ston)
            {
                PlayerDataContainer.EquipmentSton -= ston;
                Message.Send(m_refreshEquipmentStonMsg);
                isSuccess = true;
            }

            return isSuccess;
        }
        //------------------------------------------------------------------------------------
        public void AddSkillSton(int ston)
        {
            PlayerDataContainer.SkillSton += ston;
            Message.Send(m_refreshSkillStonMsg);
        }
        //------------------------------------------------------------------------------------
        public int GetSkillSton()
        {
            return PlayerDataContainer.SkillSton;
        }
        //------------------------------------------------------------------------------------
        public bool UseSkillSton(int ston)
        {
            bool isSuccess = false;

            if (PlayerDataContainer.SkillSton >= ston)
            {
                PlayerDataContainer.SkillSton -= ston;
                Message.Send(m_refreshSkillStonMsg);
                isSuccess = true;
            }

            return isSuccess;
        }
        //------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------
        public void AddStatElementValue(StatType type, StatElementValue elementvalue)
        {
            if (PlayerDataContainer.m_addStatValues.ContainsKey(type) == false)
                PlayerDataContainer.m_addStatValues.Add(type, new List<StatElementValue>());

            PlayerDataContainer.m_addStatValues[type].Add(elementvalue);
        }
        //------------------------------------------------------------------------------------
        public void SetAllOutPutStat()
        {
            SetOutPutDamage();
            SetOutPutCriticalDamage();

            SetOutPutHP();
            SetOutPutHPRecovery();

            SetOutPutMP();
            SetOutPutMPRecovery();
        }
        //------------------------------------------------------------------------------------
        public void SetOutPutDamage()
        {
            PlayerDataOperator.SetOutPutDamage();
        }
        //------------------------------------------------------------------------------------
        public void SetOutPutCriticalDamage()
        {
            PlayerDataOperator.SetOutPutCriticalDamage();
        }
        //------------------------------------------------------------------------------------
        public void SetOutPutHP()
        {
            PlayerDataOperator.SetOutPutHP();
        }
        //------------------------------------------------------------------------------------
        public void SetOutPutHPRecovery()
        {
            PlayerDataOperator.SetOutPutHPRecovery();
        }
        //------------------------------------------------------------------------------------
        public void SetOutPutMP()
        {
            PlayerDataOperator.SetOutPutMP();
        }
        //------------------------------------------------------------------------------------
        public void SetOutPutMPRecovery()
        {
            PlayerDataOperator.SetOutPutMPRecovery();
        }
        //------------------------------------------------------------------------------------
        public PlayerDamageData GetAttackDamage(SkillData attackSkillData)
        {
            PlayerDamageData damageData = PlayerDataOperator.GetAttackDamage();

            double skillratio = SkillDataManager.Instance.GetSkillOptionValue(attackSkillData) * PlayerDataContainer.SkillDamagePer;

            damageData.DamageValue *= skillratio;

            return damageData;
        }
        //------------------------------------------------------------------------------------
        public double GetOutPutHP()
        {
            return PlayerDataContainer.OutPutHP;
        }
        //------------------------------------------------------------------------------------
        public double GetOutPutHPRecovery()
        {
            return PlayerDataContainer.OutPutHPRecovery;
        }
        //------------------------------------------------------------------------------------
        public double GetOutPutMP()
        {
            return PlayerDataContainer.OutPutMP;
        }
        //------------------------------------------------------------------------------------
        public double GetOutPutMPRecovery()
        {
            return PlayerDataContainer.OutPutMPRecovery;
        }
        //------------------------------------------------------------------------------------
    }
}