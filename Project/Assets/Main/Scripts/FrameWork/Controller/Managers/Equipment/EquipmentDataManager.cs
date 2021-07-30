using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry.Managers
{
    public class EquipmentDataManager : MonoSingleton<EquipmentDataManager>
    {
        private Dictionary<StatType, StatElementValue> m_addStatValue = new Dictionary<StatType, StatElementValue>();

        private Event.ChangeEquipElementMsg m_changeEquipElementMsg = new Event.ChangeEquipElementMsg();
        private Event.RefreshEquipmentInfoListMsg m_refreshEquipmentInfoListResponseMsg = new Event.RefreshEquipmentInfoListMsg();

        #region Equipment
        //------------------------------------------------------------------------------------
        public void AddEquipmentInfo(EquipmentType type, Dictionary<int, PlayerEquipmentInfo> data)
        {
            EquipmentDataContainer.m_equipmentInfo.Add(type, data);
        }
        //------------------------------------------------------------------------------------
        public void AddEquipID(EquipmentType type, int id)
        {
            EquipmentDataContainer.m_equipId.Add(type, id);
        }
        //------------------------------------------------------------------------------------
        public void SetStatElementValue()
        {
            EquipmentLocalChart equipchart = TableManager.Instance.GetTableClass<EquipmentLocalChart>();

            // 보유시능력 적용
            foreach (KeyValuePair<EquipmentType, Dictionary<int, PlayerEquipmentInfo>> pair in EquipmentDataContainer.m_equipmentInfo)
            {
                foreach (KeyValuePair<int, PlayerEquipmentInfo> valuepair in pair.Value)
                {
                    EquipmentData equipdata = equipchart.GetEquipmentData(valuepair.Key);

                    if (equipdata.ApplyOption.ContainsKey(EquipmentApplyOption.EnableOption) == true)
                    {
                        List<StatType> enableoption = equipdata.ApplyOption[EquipmentApplyOption.EnableOption];

                        for (int i = 0; i < enableoption.Count; ++i)
                        {
                            if (m_addStatValue.ContainsKey(enableoption[i]) == false)
                                m_addStatValue.Add(enableoption[i], new StatElementValue());

                            m_addStatValue[enableoption[i]].StatValue += GetEquipmentOptionValue(equipdata, enableoption[i]);
                        }
                    }
                }
            }

            // 장착시 능력
            foreach (KeyValuePair<EquipmentType, int> pair in EquipmentDataContainer.m_equipId)
            {
                EquipmentData equipdata = equipchart.GetEquipmentData(pair.Value);

                if (equipdata.ApplyOption.ContainsKey(EquipmentApplyOption.EquipmentOption) == true)
                {
                    List<StatType> enableoption = equipdata.ApplyOption[EquipmentApplyOption.EquipmentOption];

                    for (int i = 0; i < enableoption.Count; ++i)
                    {
                        if (m_addStatValue.ContainsKey(enableoption[i]) == false)
                            m_addStatValue.Add(enableoption[i], new StatElementValue());

                        m_addStatValue[enableoption[i]].StatValue += GetEquipmentOptionValue(equipdata, enableoption[i]);
                    }
                }
            }


            foreach (KeyValuePair<StatType, StatElementValue> pair in m_addStatValue)
            {
                PlayerDataManager.Instance.AddStatElementValue(pair.Key, pair.Value);
            }
        }
        //------------------------------------------------------------------------------------
        public PlayerEquipmentInfo GetPlayerEquipmentInfo(EquipmentType type, int id)
        {
            Dictionary<int, PlayerEquipmentInfo> dic = null;

            if (EquipmentDataContainer.m_equipmentInfo.TryGetValue(type, out dic) == true)
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

            return EquipmentDataContainer.m_equipmentInfo[type][id];
        }
        //------------------------------------------------------------------------------------
        public PlayerEquipmentInfo GetPlayerEquipmentInfo(EquipmentData data)
        {
            Dictionary<int, PlayerEquipmentInfo> dic = null;

            if (EquipmentDataContainer.m_equipmentInfo.TryGetValue(data.Type, out dic) == true)
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

            return EquipmentDataContainer.m_equipmentInfo[data.Type][data.Id];
        }
        //------------------------------------------------------------------------------------
        public int GetNeedLevelUPEquipmentSton(EquipmentData equipmentdata, PlayerEquipmentInfo equipmentinfo)
        {
            return EquipmentDataOperator.GetNeedLevelUPEquipmentSton(equipmentdata, equipmentinfo);
        }
        //------------------------------------------------------------------------------------
        public bool IsEquipElement(EquipmentData equipmentdata)
        {
            return GetCurrentEquip(equipmentdata.Type) == equipmentdata.Id;
        }
        //------------------------------------------------------------------------------------
        public int GetCurrentEquip(EquipmentType type)
        {
            if (EquipmentDataContainer.m_equipId.ContainsKey(type) == false)
                return -1;

            return EquipmentDataContainer.m_equipId[type];
        }
        //------------------------------------------------------------------------------------
        public bool SetEquipElement(EquipmentData equipmentdata)
        {
            Dictionary<int, PlayerEquipmentInfo> datadic = null;

            if (EquipmentDataContainer.m_equipmentInfo.TryGetValue(equipmentdata.Type, out datadic) == true)
            {
                if (datadic.ContainsKey(equipmentdata.Id) == true)
                {
                    int beforeid = -1;

                    if (EquipmentDataContainer.m_equipId.ContainsKey(equipmentdata.Type) == false)
                    {
                        EquipmentDataContainer.m_equipId.Add(equipmentdata.Type, equipmentdata.Id);
                    }
                    else
                    {
                        beforeid = EquipmentDataContainer.m_equipId[equipmentdata.Type];
                        EquipmentDataContainer.m_equipId[equipmentdata.Type] = equipmentdata.Id;
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

            if (EquipmentDataContainer.m_equipmentInfo.ContainsKey(equipmentdata.Type) == false)
                EquipmentDataContainer.m_equipmentInfo.Add(equipmentdata.Type, new Dictionary<int, PlayerEquipmentInfo>());

            EquipmentDataContainer.m_equipmentInfo.TryGetValue(equipmentdata.Type, out dic);

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
                if (EquipmentDataContainer.m_equipmentInfo.ContainsKey(equipmentdata[i].Type) == false)
                    EquipmentDataContainer.m_equipmentInfo.Add(equipmentdata[i].Type, new Dictionary<int, PlayerEquipmentInfo>());

                EquipmentDataContainer.m_equipmentInfo.TryGetValue(equipmentdata[i].Type, out dic);

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
        public double GetEquipmentOptionValue(EquipmentData equipmentdata, StatType option)
        {
            PlayerEquipmentInfo info = GetPlayerEquipmentInfo(equipmentdata);

            return EquipmentDataOperator.GetEquipmentOptionValue(equipmentdata, info == null ? 0 : info.Level, option);
        }
        //------------------------------------------------------------------------------------
        public double GetEquipmentNextLevelOptionValue(EquipmentData equipmentdata, StatType option)
        {
            PlayerEquipmentInfo info = GetPlayerEquipmentInfo(equipmentdata);
            int level = info == null ? 0 : info.Level;
            return EquipmentDataOperator.GetEquipmentOptionValue(equipmentdata, level + 1, option);
        }
        //------------------------------------------------------------------------------------
        public bool SetLevelUpEquipment(EquipmentData equipmentdata)
        {
            PlayerEquipmentInfo info = GetPlayerEquipmentInfo(equipmentdata);

            if (info == null)
                return false;

            int needSton = GetNeedLevelUPEquipmentSton(equipmentdata, info);

            if (PlayerDataManager.Instance.UseEquipmentSton(needSton) == false)
                return false;

            info.Level += 1;

            m_refreshEquipmentInfoListResponseMsg.datas.Clear();
            m_refreshEquipmentInfoListResponseMsg.datas.Add(equipmentdata);

            Message.Send(m_refreshEquipmentInfoListResponseMsg);

            return true;
        }
        //------------------------------------------------------------------------------------
        #endregion
    }
}