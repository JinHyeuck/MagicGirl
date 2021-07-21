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


        // PassiveSkill���� ������ �ȵȴ�.
        // <Skillid, SkillCoolTimeData> ��Ÿ���� �ִ� ��ų���� ����ȴ�.
        private Dictionary<int, SkillCoolTimeData> m_activeSkillList = new Dictionary<int, SkillCoolTimeData>();
        // ������ ����� �� �ִ� ��ų��
        private List<SkillData> m_readyActiveSkillData = new List<SkillData>();
        private LinkedList<SkillData> m_readyBuffSkillData = new LinkedList<SkillData>();
        // ��ų��� �� �־��ش�. -> ��Ÿ�� üũ�뵵
        private LinkedList<SkillCoolTimeData> m_usedSkill = new LinkedList<SkillCoolTimeData>();
        // ������ ������ Ǯ
        private Queue<SkillCoolTimeData> m_skillCoolDataPool = new Queue<SkillCoolTimeData>();
        // PassiveSkill���� ������ �ȵȴ�.

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
                            // ���� �����ٰ� �������� �˷��ֱ�
                        }
                        else
                        {
                            if (element != null)
                                element.SetBuffApplyTime(currenttime, applytime);
                            // ���� ���� ���ӽð�
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
            // ��ų���� �͵��������� ���� ���ϰ� ���⼭ ������ �־��ֱ�
            // ��ų���� �͵��������� ���� ���ϰ� ���⼭ ������ �־��ֱ�
            // ��ų���� �͵��������� ���� ���ϰ� ���⼭ ������ �־��ֱ�

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