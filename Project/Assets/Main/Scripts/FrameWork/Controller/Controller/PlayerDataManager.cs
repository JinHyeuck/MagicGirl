using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry.Managers
{
    public class PlayerDataManager : MonoSingleton<PlayerDataManager>
    {
        private float m_playerInfoDataSaveTime = 30.0f;
        private float m_playerInfoDataSaveTimer = 0.0f;


        //------------------------------------------------------------------------------------
        protected override void Init()
        {
            m_playerInfoDataSaveTimer = Time.time + m_playerInfoDataSaveTime;
        }
        //------------------------------------------------------------------------------------
        private void Update()
        {
            if (m_playerInfoDataSaveTimer < Time.time)
            {
                TheBackEnd.TheBackEnd.Instance.PlayerTableUpdate();
            }
        }
        //------------------------------------------------------------------------------------
        public int GetLevel()
        {
            return PlayerDataContainer.Level;
        }
        //------------------------------------------------------------------------------------
        public void AddExp(int exp)
        {
            PlayerDataContainer.Exp += exp;
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
        }
        //------------------------------------------------------------------------------------
        public int GetGold()
        {
            return PlayerDataContainer.Gold;
        }
        //------------------------------------------------------------------------------------
        public void AddDia(int gold)
        {
            PlayerDataContainer.Gold += gold;
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
        }
        //------------------------------------------------------------------------------------
        public int GetEquipmentSton()
        {
            return PlayerDataContainer.EquipmentSton;
        }
        //------------------------------------------------------------------------------------
    }
}