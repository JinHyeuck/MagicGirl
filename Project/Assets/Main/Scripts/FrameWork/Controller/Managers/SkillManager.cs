using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBerry.UI;

namespace GameBerry.Managers
{
    public class SkillCoolTimeData
    {
        public float StartTime = 0.0f;
        public SkillData SkillData = null;
        public UISkillSlotElement UIElement = null;
    }

    public class SkillManager : MonoSingleton<SkillManager>
    {
        // <SlotID, Skillid>
        private Dictionary<int, int> m_currentSkillSlot = new Dictionary<int, int>();


        // PassiveSkill들은 포함이 안된다.
        // <Skillid, SkillCoolTimeData> 쿨타임이 있는 스킬들이 저장된다.
        private Dictionary<int, SkillCoolTimeData> m_activeSkillList = new Dictionary<int, SkillCoolTimeData>();
        // 언제든 사용할 수 있는 스킬들
        private List<SkillData> m_readyActiveSkillData = new List<SkillData>();
        private LinkedList<SkillData> m_readyBuffSkillData = new LinkedList<SkillData>();
        // 스킬사용 시 넣어준다. -> 쿨타임 체크용도
        private LinkedList<SkillCoolTimeData> m_usedSkill = new LinkedList<SkillCoolTimeData>();
        // 쓰래기 방지용 풀
        private Queue<SkillCoolTimeData> m_skillCoolDataPool = new Queue<SkillCoolTimeData>();
        // PassiveSkill들은 포함이 안된다.

        private SkillData m_defaultSkill = null;

        private SkillLocalChart m_skillLocalChart;

        private Event.SetSkillSlotMsg m_setSkillSlotMsg = new Event.SetSkillSlotMsg();

        //------------------------------------------------------------------------------------
        protected override void Init()
        {
            m_skillLocalChart = TableManager.Instance.GetTableClass<SkillLocalChart>();

            SkillData data = new SkillData();
            data.OptionValue = 1.0;
            data.MaxTarget = 1;
            data.Range = 1.0f;

            m_defaultSkill = data;
        }
        //------------------------------------------------------------------------------------
        public void SetSkillSlot()
        {
            foreach (KeyValuePair<int, SkillCoolTimeData> pair in m_activeSkillList)
                m_skillCoolDataPool.Enqueue(pair.Value);

            m_currentSkillSlot = PlayerDataManager.Instance.GetCurrentSkillSlot();
            m_activeSkillList.Clear();
            m_readyActiveSkillData.Clear();
            m_readyBuffSkillData.Clear();
            m_usedSkill.Clear();

            foreach (KeyValuePair<int, int> pair in m_currentSkillSlot)
            {
                if (pair.Value != -1)
                {
                    SkillData data = m_skillLocalChart.GetSkillData(pair.Value);

                    if (data == null)
                        continue;

                    if (data.SkillTriggerType == SkillTriggerType.Active || data.SkillTriggerType == SkillTriggerType.Buff)
                    {
                        if (m_skillCoolDataPool.Count <= 0)
                            m_skillCoolDataPool.Enqueue(new SkillCoolTimeData());

                        SkillCoolTimeData cooldata = m_skillCoolDataPool.Dequeue();
                        cooldata.StartTime = 0.0f;
                        cooldata.SkillData = data;
                        cooldata.UIElement = null;

                        m_activeSkillList.Add(data.SkillID, cooldata);

                        if (data.SkillTriggerType == SkillTriggerType.Active)
                        {
                            m_readyActiveSkillData.Add(data);
                        }
                        else if (data.SkillTriggerType == SkillTriggerType.Buff)
                        {
                            m_readyBuffSkillData.AddLast(data);
                        }
                    }
                }
            }

            m_setSkillSlotMsg.SkillSlot = m_currentSkillSlot;

            Message.Send(m_setSkillSlotMsg);
        }
        //------------------------------------------------------------------------------------
        private void Update()
        {
            var node = m_usedSkill.First;
            while (node != null)
            {
                float coolTime = node.Value.SkillData.CoolTime;
                float currenttime = Time.time - node.Value.StartTime;
                UISkillSlotElement element = node.Value.UIElement;

                if (currenttime >= coolTime)
                {
                    if (element != null)
                        element.EndCoolTime();

                    m_usedSkill.Remove(node);

                    if (node.Value.SkillData.SkillTriggerType == SkillTriggerType.Active)
                        m_readyActiveSkillData.Add(node.Value.SkillData);
                    else if (node.Value.SkillData.SkillTriggerType == SkillTriggerType.Buff)
                        m_readyBuffSkillData.AddLast(node.Value.SkillData);
                }
                else
                {
                    if (element != null)
                    { 
                        element.SetCoolTime(currenttime, coolTime);
                    }

                    if (node.Value.SkillData.SkillTriggerType == SkillTriggerType.Buff)
                    {
                        float applytime = node.Value.SkillData.ApplyTime;
                        if (currenttime >= applytime)
                        {
                            if (element != null)
                                element.EndBuffApplyTime();
                            // 버프 끝났다고 누구에게 알려주기
                        }
                        else
                        {
                            if (element != null)
                                element.SetBuffApplyTime(currenttime, applytime);
                            // 버프 아직 지속시간
                        }
                    }
                }

                node = node.Next;
            }


            var buffnode = m_readyBuffSkillData.First;
            while (buffnode != null)
            {
                if (buffnode.Value.NeedMP <= PlayerManager.Instance.GetCurrentPlayerMP())
                {
                    UseSkill(buffnode.Value);
                    m_readyBuffSkillData.Remove(buffnode);
                }

                buffnode = buffnode.Next;
            }

        }
        //------------------------------------------------------------------------------------
        public void ChangeSkillSlotPage(int slotpage)
        {
            // 스킬슬롯 와따리갔따리 하지 못하게 여기서 스롯쿨 넣어주기
            // 스킬슬롯 와따리갔따리 하지 못하게 여기서 스롯쿨 넣어주기
            // 스킬슬롯 와따리갔따리 하지 못하게 여기서 스롯쿨 넣어주기

            if (PlayerDataManager.Instance.ChangeSkillSlotPage(slotpage) == true)
            {
                SetSkillSlot();
            }
        }
        //------------------------------------------------------------------------------------
        public void LinkSlotElement(int skillid, UISkillSlotElement element)
        {
            SkillCoolTimeData data = null;
            if (m_activeSkillList.TryGetValue(skillid, out data) == true)
            {
                data.UIElement = element;
            }
        }
        //------------------------------------------------------------------------------------
        public SkillData GetReadySkill(int currUserMp)
        {
            for (int i = 0; i < m_readyActiveSkillData.Count; ++i)
            {
                if (m_readyActiveSkillData[i].NeedMP <= currUserMp)
                    return m_readyActiveSkillData[i];
            }

            return m_defaultSkill;
        }
        //------------------------------------------------------------------------------------
        public void UseSkill(SkillData data)
        {
            if (data == m_defaultSkill)
                return;


            if (data.SkillTriggerType == SkillTriggerType.Active)
            {
                if (m_readyActiveSkillData.Contains(data) == true)
                    m_readyActiveSkillData.Remove(data);
            }

            SkillCoolTimeData cooltime = null;

            if (m_activeSkillList.TryGetValue(data.SkillID, out cooltime) == false)
                return;
            
            cooltime.StartTime = Time.time;

            m_usedSkill.AddLast(cooltime);
        }
        //------------------------------------------------------------------------------------
        //public void ReadyToChangeSkillSlot(SkillData skilldata, PlayerSkillInfo skillinfo)
        //{
        //    if (skillinfo == null)
        //        return;


        //}
        ////------------------------------------------------------------------------------------
        ///
        public void ChangeSlotSkill(SkillData skilldata, int slotid)
        {
            if (skilldata == null)
                return;

            if (m_currentSkillSlot.ContainsKey(slotid) == false)
                return;

            if (m_currentSkillSlot.ContainsValue(skilldata.SkillID) == true)
            {
                foreach (KeyValuePair<int, int> pair in m_currentSkillSlot)
                {
                    if (pair.Value == skilldata.SkillID)
                    { 
                        m_currentSkillSlot[pair.Key] = -1;
                        break;
                    }
                }
            }

            m_currentSkillSlot[slotid] = skilldata.SkillID;

            SetSkillSlot();
        }
        //------------------------------------------------------------------------------------
    }
}