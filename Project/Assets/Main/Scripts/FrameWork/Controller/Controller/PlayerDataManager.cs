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
        private Event.ChangeEquipElementMsg m_changeEquipElementMsg = new Event.ChangeEquipElementMsg();

        private LevelLocalChart m_levelLocalChart = null;
        //------------------------------------------------------------------------------------
        protected override void Init()
        {
            m_playerInfoDataSaveTimer = Time.time + m_playerInfoDataSaveTime;

            PlayerDataOperator.Init();
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

            Message.Send(m_refrashLevelMsg);
            Message.Send(m_refrashExpMsg);
        }
        //------------------------------------------------------------------------------------
        public double GetExp()
        {
            return PlayerDataContainer.Exp;
        }
        //------------------------------------------------------------------------------------
        public void AddGold(double gold)
        {
            PlayerDataContainer.Gold += gold;
            Message.Send(m_refrashGoldMsg);
        }
        //------------------------------------------------------------------------------------
        public bool UseGold(double gold)
        {
            bool isSuccess = false;

            if (PlayerDataContainer.Gold > gold)
            { 
                PlayerDataContainer.Gold -= gold;
                Message.Send(m_refrashGoldMsg);
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
        public int GetCurrentUpGradeStatLevel(StatUpGradeType type)
        {
            return PlayerDataContainer.m_upGradeStatLevel[type];
        }
        //------------------------------------------------------------------------------------
        public double GetUpGradeStatPrice(StatUpGradeType type)
        {
            return PlayerDataOperator.GetUpGradePrice(type, GetCurrentUpGradeStatLevel(type));
        }
        //------------------------------------------------------------------------------------
        public double GetCurrentUpGradeStatValue(StatUpGradeType type)
        {
            return PlayerDataOperator.GetCurrentUpGradeStatValue(type, GetCurrentUpGradeStatLevel(type));
        }
        //------------------------------------------------------------------------------------
        public double GetNextUpGradeStatValue(StatUpGradeType type)
        {
            return PlayerDataOperator.GetCurrentUpGradeStatValue(type, GetCurrentUpGradeStatLevel(type) + 1);
        }
        //------------------------------------------------------------------------------------
        public bool IsMaxUpGrade(StatUpGradeType type)
        {
            return PlayerDataOperator.IsMaxUpGrade(type, GetCurrentUpGradeStatLevel(type));
        }
        //------------------------------------------------------------------------------------
        public bool InCreaseUpGradeStatLevel(StatUpGradeType type)
        {
            if (UseGold(GetUpGradeStatPrice(type)) == true)
            {
                PlayerDataContainer.m_upGradeStatLevel[type]++;
                return true;
            }
            else
            {
                return false;
            }

            return false;
        }
        //------------------------------------------------------------------------------------
        public PlayerEquipmentInfo GetPlayerEquipmentInfo(EquipmentType type, int id)
        {
            Dictionary<int, PlayerEquipmentInfo> dic = null;

            if (PlayerDataContainer.m_equipmentInfo.TryGetValue(type, out dic) == true)
            {
                if (dic != null)
                {
                    PlayerEquipmentInfo equipmentinfo = null;
                    if (dic.TryGetValue(id, out equipmentinfo) == true)
                        return equipmentinfo;
                    else
                        return null;
                }
                else
                    return null;
            }
            else
                return null;

            return PlayerDataContainer.m_equipmentInfo[type][id];
        }
        //------------------------------------------------------------------------------------
        public int GetNeedLevelUPEquipmentSton(EquipmentData equipmentdata, PlayerEquipmentInfo equipmentinfo)
        {
            return PlayerDataOperator.GetNeedLevelUPEquipmentSton(equipmentdata, equipmentinfo);
        }
        //------------------------------------------------------------------------------------
        public bool IsEquipElement(EquipmentData equipmentdata)
        {
            if (equipmentdata.Type == EquipmentType.Weapon)
            {
                return PlayerDataContainer.WeaponEquipID == equipmentdata.Id;
            }
            else if (equipmentdata.Type == EquipmentType.Necklace)
            {
                return PlayerDataContainer.NecklaceEquipID == equipmentdata.Id;
            }
            else if (equipmentdata.Type == EquipmentType.Ring)
            {
                return PlayerDataContainer.RingEquipID == equipmentdata.Id;
            }

            return false;
        }
        //------------------------------------------------------------------------------------
        public bool SetEquipElement(EquipmentData equipmentdata)
        {
            Dictionary<int, PlayerEquipmentInfo> datadic = null;

            if (PlayerDataContainer.m_equipmentInfo.TryGetValue(equipmentdata.Type, out datadic) == true)
            {
                PlayerEquipmentInfo info = null;

                if (datadic.TryGetValue(equipmentdata.Id, out info) == true)
                {
                    m_changeEquipElementMsg.EquipementType = equipmentdata.Type;
                    m_changeEquipElementMsg.EquipmentID = equipmentdata.Id;

                    Message.Send(m_changeEquipElementMsg);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;

            return false;
        }
    }
}