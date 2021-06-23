using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry.Managers
{
    public class PlayerDataManager : MonoSingleton<PlayerDataManager>
    {
        private float m_playerInfoDataSaveTime = 5.0f;
        private float m_playerInfoDataSaveTimer = 0.0f;

        private Event.RefrashLevelMsg m_refrashLevelMsg = new Event.RefrashLevelMsg();
        private Event.RefrashExpMsg m_refrashExpMsg = new Event.RefrashExpMsg();
        private Event.RefrashGoldMsg m_refrashGoldMsg = new Event.RefrashGoldMsg();
        private Event.RefrashDiaMsg m_refrashDiaMsg = new Event.RefrashDiaMsg();
        private Event.RefrashEquipmentStonMsg m_refrashEquipmentStonMsg = new Event.RefrashEquipmentStonMsg();
        private Event.RefrashSkillStonMsg m_refrashSkillStonMsg = new Event.RefrashSkillStonMsg();

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
                TheBackEnd.TheBackEnd.Instance.PlayerTableUpdate();
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
        public void AddExp(int exp)
        {
            PlayerDataContainer.Exp += exp;

            LevelData data = m_levelLocalChart.GetLevelData(PlayerDataContainer.Level);

            while (PlayerDataContainer.Exp >= data.Exp)
            {
                SetLevelUp();
                PlayerDataContainer.Exp -= data.Exp;
                m_levelLocalChart.GetLevelData(PlayerDataContainer.Level);
            }

            Message.Send(m_refrashLevelMsg);
            Message.Send(m_refrashExpMsg);
        }
        //------------------------------------------------------------------------------------
        public int GetExp()
        {
            return PlayerDataContainer.Exp;
        }
        //------------------------------------------------------------------------------------
        public void AddGold(int gold)
        {
            PlayerDataContainer.Gold += gold;
            Message.Send(m_refrashGoldMsg);
        }
        //------------------------------------------------------------------------------------
        public int GetGold()
        {
            return PlayerDataContainer.Gold;
        }
        //------------------------------------------------------------------------------------
        public void AddDia(int dia)
        {
            PlayerDataContainer.Dia += dia;
            Message.Send(m_refrashDiaMsg);
        }
        //------------------------------------------------------------------------------------
        public int GetDia()
        {
            return PlayerDataContainer.Dia;
        }
        //------------------------------------------------------------------------------------
        public void AddEquipmentSton(int ston)
        {
            PlayerDataContainer.EquipmentSton += ston;
            Message.Send(m_refrashEquipmentStonMsg);
        }
        //------------------------------------------------------------------------------------
        public int GetEquipmentSton()
        {
            return PlayerDataContainer.EquipmentSton;
        }
        //------------------------------------------------------------------------------------
        public void AddSkillSton(int ston)
        {
            PlayerDataContainer.SkillSton += ston;
            Message.Send(m_refrashSkillStonMsg);
        }
        //------------------------------------------------------------------------------------
        public int GetSkillSton()
        {
            return PlayerDataContainer.SkillSton;
        }
        //------------------------------------------------------------------------------------
    }
}