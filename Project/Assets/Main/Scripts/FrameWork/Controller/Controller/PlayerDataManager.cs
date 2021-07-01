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
    }
}