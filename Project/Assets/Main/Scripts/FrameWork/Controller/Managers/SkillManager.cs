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
        private Dictionary<int, int> m_currentSkillSlot = new Dictionary<int, int>();


        // PassiveSkill들은 포함이 안된다.
        private Dictionary<int, SkillCoolTimeData> m_activeSkillList = new Dictionary<int, SkillCoolTimeData>();
        private List<SkillData> m_readySkillData = new List<SkillData>();
        private LinkedList<SkillCoolTimeData> m_usedSkill = new LinkedList<SkillCoolTimeData>();

        private Queue<SkillCoolTimeData> m_skillCoolDataPool = new Queue<SkillCoolTimeData>();
        // PassiveSkill들은 포함이 안된다.

        private SkillData m_defaultSkill = null;

        private SkillLocalChart m_skillLocalChart;

        private Event.SetSlotMsg m_setSlotMsg = new Event.SetSlotMsg();

        //------------------------------------------------------------------------------------
        protected override void Init()
        {
            m_skillLocalChart = TableManager.Instance.GetTableClass<SkillLocalChart>();

            SkillData defa = new SkillData();
            defa.OptionValue = 1.0;
            defa.MaxTarget = 1;
            defa.Range = 1.0f;

            m_defaultSkill = defa;
        }
        //------------------------------------------------------------------------------------
        public void SetSkillSlot()
        {
            foreach (KeyValuePair<int, SkillCoolTimeData> pair in m_activeSkillList)
                m_skillCoolDataPool.Enqueue(pair.Value);

            m_currentSkillSlot = PlayerDataManager.Instance.GetCurrentSkillSlot();
            m_activeSkillList.Clear();
            m_readySkillData.Clear();
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

                        m_readySkillData.Add(data);
                    }
                }
            }

            m_setSlotMsg.SkillSlot = m_currentSkillSlot;

            Message.Send(m_setSlotMsg);
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
                }
                else
                {
                    if (element != null)
                        element.SetCoolTime(currenttime, coolTime);
                }

                node = node.Next;
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
        public void LinkedElement(int skillid, UISkillSlotElement element)
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
            for (int i = 0; i < m_readySkillData.Count; ++i)
            {
                if (m_readySkillData[i].NeedMP <= currUserMp)
                    return m_readySkillData[i];
            }

            return m_defaultSkill;
        }
        //------------------------------------------------------------------------------------
        public void UseSkill(SkillData data)
        {
            if (data == m_defaultSkill)
                return;

            if (m_readySkillData.Contains(data) == true)
                m_readySkillData.Remove(data);

            SkillCoolTimeData cooltime = m_activeSkillList[data.SkillID];
            cooltime.StartTime = Time.time;

            m_usedSkill.AddLast(cooltime);
        }
        //------------------------------------------------------------------------------------
    }
}