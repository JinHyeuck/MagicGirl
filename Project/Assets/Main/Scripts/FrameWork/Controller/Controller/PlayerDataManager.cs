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
        private Event.ChangeEquipElementMsg m_changeEquipElementMsg = new Event.ChangeEquipElementMsg();
        private Event.RefreshEquipmentInfoListMsg m_refreshEquipmentInfoListResponseMsg = new Event.RefreshEquipmentInfoListMsg();
        private Event.RefreshSkillInfoListMsg m_refreshSkillInfoListMsg = new Event.RefreshSkillInfoListMsg();


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
        #region StatUpGrade
        //------------------------------------------------------------------------------------
        public int GetCurrentUpGradeStatLevel(StatUpGradeType type)
        {
            return PlayerDataContainer.m_upGradeStatLevel[type];
        }
        //------------------------------------------------------------------------------------
        public double GetUpGradeStatPrice(StatUpGradeType type)
        {
            return PlayerDataOperator.GetUpGradeStatPrice(type, GetCurrentUpGradeStatLevel(type));
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
        #endregion
        //------------------------------------------------------------------------------------
        #region Equipment
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
        public PlayerEquipmentInfo GetPlayerEquipmentInfo(EquipmentData data)
        {
            Dictionary<int, PlayerEquipmentInfo> dic = null;

            if (PlayerDataContainer.m_equipmentInfo.TryGetValue(data.Type, out dic) == true)
            {
                if (dic != null)
                {
                    PlayerEquipmentInfo equipmentinfo = null;
                    if (dic.TryGetValue(data.Id, out equipmentinfo) == true)
                        return equipmentinfo;
                    else
                        return null;
                }
                else
                    return null;
            }
            else
                return null;

            return PlayerDataContainer.m_equipmentInfo[data.Type][data.Id];
        }
        //------------------------------------------------------------------------------------
        public int GetNeedLevelUPEquipmentSton(EquipmentData equipmentdata, PlayerEquipmentInfo equipmentinfo)
        {
            return PlayerDataOperator.GetNeedLevelUPEquipmentSton(equipmentdata, equipmentinfo);
        }
        //------------------------------------------------------------------------------------
        public bool IsEquipElement(EquipmentData equipmentdata)
        {
            return GetCurrentEquip(equipmentdata.Type) == equipmentdata.Id;
        }
        //------------------------------------------------------------------------------------
        public int GetCurrentEquip(EquipmentType type)
        {
            if (PlayerDataContainer.m_equipId.ContainsKey(type) == false)
                return -1;

            return PlayerDataContainer.m_equipId[type];
        }
        //------------------------------------------------------------------------------------
        public bool SetEquipElement(EquipmentData equipmentdata)
        {
            Dictionary<int, PlayerEquipmentInfo> datadic = null;

            if (PlayerDataContainer.m_equipmentInfo.TryGetValue(equipmentdata.Type, out datadic) == true)
            {
                if (datadic.ContainsKey(equipmentdata.Id) == true)
                {
                    int beforeid = -1;

                    if (PlayerDataContainer.m_equipId.ContainsKey(equipmentdata.Type) == false)
                    {
                        PlayerDataContainer.m_equipId.Add(equipmentdata.Type, equipmentdata.Id);
                    }
                    else
                    {
                        beforeid = PlayerDataContainer.m_equipId[equipmentdata.Type];
                        PlayerDataContainer.m_equipId[equipmentdata.Type] = equipmentdata.Id;
                    }

                    m_changeEquipElementMsg.EquipementType = equipmentdata.Type;
                    m_changeEquipElementMsg.BeforeEquipmentID = beforeid;
                    m_changeEquipElementMsg.AfterEquipmentID = equipmentdata.Id;

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
        //------------------------------------------------------------------------------------
        public void AddEquipElement(EquipmentData equipmentdata, int amount = 1)
        {
            Dictionary<int, PlayerEquipmentInfo> dic = null;
            PlayerEquipmentInfo info = null;

            m_refreshEquipmentInfoListResponseMsg.datas.Clear();

            if (PlayerDataContainer.m_equipmentInfo.ContainsKey(equipmentdata.Type) == false)
                PlayerDataContainer.m_equipmentInfo.Add(equipmentdata.Type, new Dictionary<int, PlayerEquipmentInfo>());

            PlayerDataContainer.m_equipmentInfo.TryGetValue(equipmentdata.Type, out dic);

            if (dic.TryGetValue(equipmentdata.Id, out info) == true)
                info.Count += amount;
            else
            {
                info = new PlayerEquipmentInfo();
                info.Id = equipmentdata.Id;
                info.Count += amount;
                dic.Add(info.Id, info);
            }

            m_refreshEquipmentInfoListResponseMsg.datas.Add(equipmentdata);

            Message.Send(m_refreshEquipmentInfoListResponseMsg);
        }
        //------------------------------------------------------------------------------------
        public void AddEquipElementList(List<EquipmentData> equipmentdata)
        {
            Dictionary<int, PlayerEquipmentInfo> dic = null;
            PlayerEquipmentInfo info = null;

            m_refreshEquipmentInfoListResponseMsg.datas.Clear();

            for (int i = 0; i < equipmentdata.Count; ++i)
            {
                if (PlayerDataContainer.m_equipmentInfo.ContainsKey(equipmentdata[i].Type) == false)
                    PlayerDataContainer.m_equipmentInfo.Add(equipmentdata[i].Type, new Dictionary<int, PlayerEquipmentInfo>());

                PlayerDataContainer.m_equipmentInfo.TryGetValue(equipmentdata[i].Type, out dic);

                if (dic.TryGetValue(equipmentdata[i].Id, out info) == true)
                    info.Count += 1;
                else
                {
                    info = new PlayerEquipmentInfo();
                    info.Id = equipmentdata[i].Id;
                    info.Count += 1;
                    dic.Add(info.Id, info);
                }

                if (m_refreshEquipmentInfoListResponseMsg.datas.Contains(equipmentdata[i]) == false)
                    m_refreshEquipmentInfoListResponseMsg.datas.Add(equipmentdata[i]);
            }

            Message.Send(m_refreshEquipmentInfoListResponseMsg);
        }
        //------------------------------------------------------------------------------------
        public bool CombineEquipment(EquipmentData from, EquipmentData to, int amount)
        {
            if (from == null || to == null || amount <= 0)
                return false;

            PlayerEquipmentInfo info = GetPlayerEquipmentInfo(from);

            int decreasecount = amount * Define.EquipmentComposeAmount;

            if (decreasecount > info.Count)
                return false;

            AddEquipElement(to, amount);

            m_refreshEquipmentInfoListResponseMsg.datas.Clear();

            info.Count -= decreasecount;

            m_refreshEquipmentInfoListResponseMsg.datas.Add(from);
            Message.Send(m_refreshEquipmentInfoListResponseMsg);

            return true;
        }
        //------------------------------------------------------------------------------------
        public double GetEquipmentOptionValue(EquipmentData equipmentdata, EquipmentOption option)
        {
            PlayerEquipmentInfo info = GetPlayerEquipmentInfo(equipmentdata);

            return PlayerDataOperator.GetEquipmentOptionValue(equipmentdata, info == null ? 0 : info.Level, option);
        }
        //------------------------------------------------------------------------------------
        public double GetEquipmentNextLevelOptionValue(EquipmentData equipmentdata, EquipmentOption option)
        {
            PlayerEquipmentInfo info = GetPlayerEquipmentInfo(equipmentdata);
            int level = info == null ? 0 : info.Level;
            return PlayerDataOperator.GetEquipmentOptionValue(equipmentdata, level + 1, option);
        }
        //------------------------------------------------------------------------------------
        public bool SetLevelUpEquipment(EquipmentData equipmentdata)
        {
            PlayerEquipmentInfo info = GetPlayerEquipmentInfo(equipmentdata);

            if (info == null)
                return false;

            int needSton = GetNeedLevelUPEquipmentSton(equipmentdata, info);

            if (UseEquipmentSton(needSton) == false)
                return false;

            info.Level += 1;

            m_refreshEquipmentInfoListResponseMsg.datas.Clear();
            m_refreshEquipmentInfoListResponseMsg.datas.Add(equipmentdata);

            Message.Send(m_refreshEquipmentInfoListResponseMsg);

            return true;
        }
        //------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------
        #region Skill
        //------------------------------------------------------------------------------------
        public PlayerSkillInfo GetPlayerSkillInfo(SkillData data)
        {
            PlayerSkillInfo skillinfo = null;
            if (PlayerDataContainer.m_skillInfo.TryGetValue(data.SkillID, out skillinfo) == true)
                return skillinfo;
            else
                return null;
        }
        //------------------------------------------------------------------------------------
        public int GetNeedLevelUPSkillCount(SkillData data)
        {
            PlayerSkillInfo info = GetPlayerSkillInfo(data);
            if (info == null)
                return 1;

            return info.Level + 1;
        }
        //------------------------------------------------------------------------------------
        public int GetNeedLevelUPSkillSton(SkillData skilldata, PlayerSkillInfo skillinfo)
        {
            return PlayerDataOperator.GetNeedLevelUPSkillSton(skilldata, skillinfo);
        }
        //------------------------------------------------------------------------------------
        public bool SetLevelUpSkill(SkillData equipmentdata)
        {
            PlayerSkillInfo info = GetPlayerSkillInfo(equipmentdata);

            if (info == null)
                return false;

            int needSkill = GetNeedLevelUPSkillCount(equipmentdata);
            if (needSkill > info.Count)
                return false;


            int needSton = GetNeedLevelUPSkillSton(equipmentdata, info);

            if (UseSkillSton(needSton) == false)
                return false;

            info.Count -= needSkill;
            info.Level += 1;

            m_refreshSkillInfoListMsg.datas.Clear();
            m_refreshSkillInfoListMsg.datas.Add(equipmentdata);

            Message.Send(m_refreshSkillInfoListMsg);

            return true;
        }
        //------------------------------------------------------------------------------------
        public void AddSkillElementList(List<SkillData> skilldata)
        {
            PlayerSkillInfo info = null;

            m_refreshSkillInfoListMsg.datas.Clear();

            for (int i = 0; i < skilldata.Count; ++i)
            {
                if (PlayerDataContainer.m_skillInfo.TryGetValue(skilldata[i].SkillID, out info) == true)
                    info.Count += 1;
                else
                {
                    info = new PlayerSkillInfo();
                    info.Id = skilldata[i].SkillID;
                    info.Count += 1;
                    PlayerDataContainer.m_skillInfo.Add(info.Id, info);
                }

                if (m_refreshSkillInfoListMsg.datas.Contains(skilldata[i]) == false)
                    m_refreshSkillInfoListMsg.datas.Add(skilldata[i]);
            }

            Message.Send(m_refreshSkillInfoListMsg);
        }
        //------------------------------------------------------------------------------------
        public int GetCurrentSkillSlotPage()
        {
            return PlayerDataContainer.SkillSlotPage;
        }
        //------------------------------------------------------------------------------------
        public Dictionary<int, int> GetCurrentSkillSlot()
        {
            Dictionary<int, int> page = null;

            PlayerDataContainer.m_skillSlotData.TryGetValue(GetCurrentSkillSlotPage(), out page);

            return page;
        }
        //------------------------------------------------------------------------------------
        public bool ChangeSkillSlotPage(int pageid)
        {
            if (PlayerDataContainer.m_skillSlotData.ContainsKey(pageid) == true)
            {
                PlayerDataContainer.SkillSlotPage = pageid;
                return true;
            }

            return false;
        }
        //------------------------------------------------------------------------------------
        public void OpenSkillSlot(int slotid)
        {
            foreach (KeyValuePair<int, Dictionary<int, int>> pair in PlayerDataContainer.m_skillSlotData)
            {
                pair.Value.Add(slotid, -1);
            }
        }
        //------------------------------------------------------------------------------------
        public double GetSkillOptionValue(SkillData skilldata)
        {
            PlayerSkillInfo skillinfo = GetPlayerSkillInfo(skilldata);

            int skilllevel = skillinfo == null ? 0 : skillinfo.Level;

            return PlayerDataOperator.GetSkillOptionValue(skilldata, skilllevel);
        }
        //------------------------------------------------------------------------------------
        public void ApplySkillBuff(SkillData skilldata)
        { // 패시브와 버프스킬들이 여기로 들어온다.
            
        }
        //------------------------------------------------------------------------------------
        public void ReleaseSkillBuff(SkillData skilldata)
        { // 패시브와 버프스킬들이 여기로 들어온다. 

        }
        //------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------
    }
}