using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry.Managers
{
    public class SkillDataManager : MonoSingleton<SkillDataManager>
    {
        private Event.RefreshSkillInfoListMsg m_refreshSkillInfoListMsg = new Event.RefreshSkillInfoListMsg();

        //------------------------------------------------------------------------------------
        public PlayerSkillInfo GetPlayerSkillInfo(SkillData data)
        {
            PlayerSkillInfo skillinfo = null;
            if (SkillDataContainer.m_skillInfo.TryGetValue(data.SkillID, out skillinfo) == true)
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
            return SkillDataOperator.GetNeedLevelUPSkillSton(skilldata, skillinfo);
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

            if (PlayerDataManager.Instance.UseSkillSton(needSton) == false)
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
                if (SkillDataContainer.m_skillInfo.TryGetValue(skilldata[i].SkillID, out info) == true)
                    info.Count += 1;
                else
                {
                    info = new PlayerSkillInfo();
                    info.Id = skilldata[i].SkillID;
                    info.Count += 1;
                    SkillDataContainer.m_skillInfo.Add(info.Id, info);
                }

                if (m_refreshSkillInfoListMsg.datas.Contains(skilldata[i]) == false)
                    m_refreshSkillInfoListMsg.datas.Add(skilldata[i]);
            }

            Message.Send(m_refreshSkillInfoListMsg);
        }
        //------------------------------------------------------------------------------------
        public int GetCurrentSkillSlotPage()
        {
            return SkillDataContainer.SkillSlotPage;
        }
        //------------------------------------------------------------------------------------
        public Dictionary<int, int> GetCurrentSkillSlot()
        {
            Dictionary<int, int> page = null;

            SkillDataContainer.m_skillSlotData.TryGetValue(GetCurrentSkillSlotPage(), out page);

            return page;
        }
        //------------------------------------------------------------------------------------
        public bool ChangeSkillSlotPage(int pageid)
        {
            if (SkillDataContainer.m_skillSlotData.ContainsKey(pageid) == true)
            {
                SkillDataContainer.SkillSlotPage = pageid;
                return true;
            }

            return false;
        }
        //------------------------------------------------------------------------------------
        public void OpenSkillSlot(int slotid)
        {
            foreach (KeyValuePair<int, Dictionary<int, int>> pair in SkillDataContainer.m_skillSlotData)
            {
                pair.Value.Add(slotid, -1);
            }
        }
        //------------------------------------------------------------------------------------
        public double GetSkillOptionValue(SkillData skilldata)
        {
            PlayerSkillInfo skillinfo = GetPlayerSkillInfo(skilldata);

            int skilllevel = skillinfo == null ? 0 : skillinfo.Level;

            return SkillDataOperator.GetSkillOptionValue(skilldata, skilllevel);
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
    }
}