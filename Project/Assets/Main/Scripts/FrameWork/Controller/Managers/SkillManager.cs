using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBerry.UI;

namespace GameBerry.Managers
{
    public enum SlotState
    {
        None = 0,
        OpenSlot,
        AddSlot,
        LockSlot,
    }

    public class SkillSlotData
    {
        public int SlotID = -1;
        public SlotState CurrSlotState = SlotState.LockSlot;
        public SkillData SkillData = null;
        //public SkillCoolTimeData SlotCoolTimeData = null;
        public float StartTime = 0.0f;
        public List<UISkillSlotElement> SlotElements = new List<UISkillSlotElement>();

        public bool IsReadySkill = false;
        public bool IsUsed = false;

        public void CapyData(SkillSlotData data)
        {
            this.StartTime = data.StartTime;
            this.IsReadySkill = data.IsReadySkill;
            this.IsUsed = data.IsUsed;
        }

        public void SetElement()
        {
            for (int i = 0; i < SlotElements.Count; ++i)
            {
                SlotElements[i].SetSkill(SkillData);

                if (SkillData != null)
                {
                    float coolTime = SkillData.CoolTime;
                    float currenttime = Time.time - StartTime;

                    if (currenttime < coolTime)
                    {
                        if (SlotElements[i] != null)
                            SlotElements[i].SetCoolTime(currenttime, coolTime);

                        if (IsReadySkill == true && SkillData.SkillTriggerType == SkillTriggerType.Buff)
                        {
                            float applytime = SkillData.ApplyTime;
                            if (currenttime < applytime)
                            {
                                if (SlotElements[i] != null)
                                    SlotElements[i].SetBuffApplyTime(currenttime, applytime);
                            }
                        }
                    }
                }
            }
        }
    }

    public class SkillManager : MonoSingleton<SkillManager>
    {
        // <SlotID, Skillid>
        private Dictionary<int, int> m_currentSkillSlot = new Dictionary<int, int>();


        // <SlotId, SkillSlotData> ��� ���Կ� ���� ������ ����Ǿ��ִ�.
        private Dictionary<int, SkillSlotData> m_playerSkillSlotData = new Dictionary<int, SkillSlotData>();
        // <SkillData, SlotId> �ش� ��ų�� ��� ���Կ� �ֳ�
        private Dictionary<SkillData, int> m_skillSlotID = new Dictionary<SkillData, int>();

        // ��Ÿ�� üũ�뵵
        private LinkedList<SkillSlotData> m_slotCoolTimeData = new LinkedList<SkillSlotData>();

        // ����� �� �ִ� ��Ƽ�꽺ų
        private LinkedList<SkillData> m_readyActiveSkillData = new LinkedList<SkillData>();
        // ����� �� �ִ� �нú꽺ų
        private LinkedList<SkillData> m_readyBuffSkillData = new LinkedList<SkillData>();


        // �⺻ ��Ÿ ��ų
        private SkillData m_defaultSkill = null;

        // ���� ���� ��ų
        public SkillData NextActiveSkill = null;

        private SkillLocalChart m_skillLocalChart;

        private Event.ChangeSkillSlotPageMsg m_changeSkillSlotPageMsg = new Event.ChangeSkillSlotPageMsg();
        private Event.ChangeEquipSkillMsg m_changeEquipSkillMsg = new Event.ChangeEquipSkillMsg();
        private Event.SetAutoSkillModeMsg m_setAutoSkillModeMsg = new Event.SetAutoSkillModeMsg();

        //------------------------------------------------------------------------------------
        private bool m_isAutoSkillMode = false;
        public bool IsAutoSkillMode { get { return m_isAutoSkillMode; } }

        //------------------------------------------------------------------------------------
        protected override void Init()
        {
            m_skillLocalChart = TableManager.Instance.GetTableClass<SkillLocalChart>();

            SkillData data = new SkillData();
            data.OptionValue = 1.0;
            data.MaxTarget = 1;
            data.Range = 1.0f;

            m_defaultSkill = data;

            NextActiveSkill = m_defaultSkill;
        }
        //------------------------------------------------------------------------------------
        public void InitializeSkillSlot()
        {
            m_currentSkillSlot = PlayerDataManager.Instance.GetCurrentSkillSlot();

            IEnumerator enumerator = m_currentSkillSlot.Keys.GetEnumerator();

            for (int i = 0; i < Define.CharacterDefaultSlotTotalCount; ++i)
            {
                SkillSlotData slotdata = new SkillSlotData();

                if (enumerator.MoveNext() == true)
                {
                    slotdata.SlotID = (int)enumerator.Current;
                    slotdata.CurrSlotState = SlotState.OpenSlot;
                }
                else
                {
                    slotdata.SlotID = i;

                    if (m_currentSkillSlot.Count == i)
                        slotdata.CurrSlotState = SlotState.AddSlot;
                }

                m_playerSkillSlotData.Add(slotdata.SlotID, slotdata);
                m_slotCoolTimeData.AddLast(slotdata);
            }

            Message.Send(new Event.InitializeSkillSlotMsg { SkillSlot = m_playerSkillSlotData });

            foreach (KeyValuePair<int, int> pair in m_currentSkillSlot)
            {
                SetSlotSkill(pair.Key, m_skillLocalChart.GetSkillData(pair.Value));
            }

            SendEquipSlotMsg();

            m_changeSkillSlotPageMsg.SkillPageID = PlayerDataManager.Instance.GetCurrentSkillSlotPage();
            Message.Send(m_changeSkillSlotPageMsg);
        }
        //------------------------------------------------------------------------------------
        public void ChangeSlotSkill(int slotid, SkillData skilldata)
        {
            SetSlotSkill(slotid, skilldata);

            foreach (KeyValuePair<int, SkillSlotData> pair in m_playerSkillSlotData)
            {
                if (m_currentSkillSlot.ContainsKey(pair.Key) == true)
                {
                    m_currentSkillSlot[pair.Key] = pair.Value.SkillData != null ? pair.Value.SkillData.SkillID : -1;
                }
            }

            SendEquipSlotMsg();
        }
        //------------------------------------------------------------------------------------
        private void SetSlotSkill(int slotid, SkillData skilldata)
        {
            if (m_playerSkillSlotData.ContainsKey(slotid) == false)
                return;

            if (m_playerSkillSlotData[slotid].CurrSlotState != SlotState.OpenSlot)
                return;

            int prevSlot = -1;

            if (m_skillSlotID.ContainsKey(skilldata) == true)
            {
                prevSlot = m_skillSlotID[skilldata];

                if (prevSlot == slotid)
                    return;
            }

            SkillSlotData nextSlotData = m_playerSkillSlotData[slotid];
            if (nextSlotData.SkillData != null)
            {
                if (nextSlotData.SkillData.SkillTriggerType == SkillTriggerType.Passive
                || nextSlotData.SkillData.SkillTriggerType == SkillTriggerType.Buff)
                {
                    // ���� �� �нú�� ����� �ɷ� ����
                }

                // ���� ������ ����Ϸ��� �ߴ� ��ų�̸� ����
                if (m_readyActiveSkillData.Contains(nextSlotData.SkillData) == true)
                    m_readyActiveSkillData.Remove(nextSlotData.SkillData);

                if (NextActiveSkill == nextSlotData.SkillData)
                    SetNextActiveSkill();

                m_skillSlotID.Remove(nextSlotData.SkillData);
            }

            if (prevSlot != -1)
            { // ���� ���Կ� �־��ٸ�
                m_skillSlotID.Remove(skilldata);

                SkillSlotData prevSlotData = m_playerSkillSlotData[prevSlot];

                nextSlotData.CapyData(prevSlotData);
                nextSlotData.SkillData = skilldata;
                nextSlotData.SetElement();

                prevSlotData.SkillData = null;
                prevSlotData.StartTime = 0.0f;
                prevSlotData.IsReadySkill = false;
                prevSlotData.IsUsed = false;
                prevSlotData.SetElement();
            }
            else
            {
                // ���� ���Կ� �����ٸ�
                nextSlotData.SkillData = skilldata;
                nextSlotData.IsReadySkill = false;
                nextSlotData.IsUsed = true;
                nextSlotData.StartTime = Time.time;

                nextSlotData.SetElement();
            }

            m_skillSlotID.Add(skilldata, slotid);
        }
        //------------------------------------------------------------------------------------
        private void SendEquipSlotMsg()
        {
            m_changeEquipSkillMsg.EquipSkillList.Clear();

            foreach (KeyValuePair<int, int> pair in m_currentSkillSlot)
            {
                m_changeEquipSkillMsg.EquipSkillList.Add(pair.Value);
            }

            Message.Send(m_changeEquipSkillMsg);
        }
        //------------------------------------------------------------------------------------
        private void Update()
        {
            var coolnode = m_slotCoolTimeData.First;
            while (coolnode != null)
            {
                if (coolnode.Value.SkillData == null)
                {
                    coolnode = coolnode.Next;
                    continue;
                }

                if (coolnode.Value.SkillData.SkillTriggerType == SkillTriggerType.Passive)
                { 
                    coolnode = coolnode.Next;
                    continue;
                }

                if (coolnode.Value.IsUsed == false)
                {
                    coolnode = coolnode.Next;
                    continue;
                }

                float coolTime = coolnode.Value.SkillData.CoolTime;
                float currenttime = Time.time - coolnode.Value.StartTime;
                List<UISkillSlotElement> elements = coolnode.Value.SlotElements;

                if (currenttime >= coolTime)
                {
                    for (int i = 0; i < elements.Count; ++i)
                    {
                        if (elements[i] != null)
                            elements[i].EndCoolTime();
                    }


                    if (coolnode.Value.SkillData.SkillTriggerType == SkillTriggerType.Active)
                    {
                        m_readyActiveSkillData.AddLast(coolnode.Value.SkillData);
                        if (NextActiveSkill == m_defaultSkill)
                            SetNextActiveSkill();
                    }
                    else if (coolnode.Value.SkillData.SkillTriggerType == SkillTriggerType.Buff)
                        m_readyBuffSkillData.AddLast(coolnode.Value.SkillData);

                    coolnode.Value.IsUsed = false;
                    coolnode.Value.IsReadySkill = true;
                }
                else
                {
                    for (int i = 0; i < elements.Count; ++i)
                    {
                        if (elements[i] != null)
                            elements[i].SetCoolTime(currenttime, coolTime);
                    }

                    if (coolnode.Value.IsReadySkill == true
                        && coolnode.Value.SkillData.SkillTriggerType == SkillTriggerType.Buff)
                    {
                        float applytime = coolnode.Value.SkillData.ApplyTime;
                        if (currenttime >= applytime)
                        {
                            for (int i = 0; i < elements.Count; ++i)
                            {
                                if (elements[i] != null)
                                    elements[i].EndBuffApplyTime();
                            }
                            // ���� �����ٰ� �������� �˷��ֱ�
                        }
                        else
                        {
                            for (int i = 0; i < elements.Count; ++i)
                            {
                                if (elements[i] != null)
                                    elements[i].SetBuffApplyTime(currenttime, applytime);
                            }
                            // ���� ���� ���ӽð�
                        }
                    }
                }

                coolnode = coolnode.Next;
            }

            var buffnode = m_readyBuffSkillData.First;
            while (buffnode != null)
            {
                if (buffnode.Value.NeedMP <= PlayerManager.Instance.GetCurrentPlayerMP())
                {
                    var nextnode = buffnode.Next;
                    UseSkill(buffnode.Value);

                    buffnode = nextnode;
                    continue;
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
                m_currentSkillSlot = PlayerDataManager.Instance.GetCurrentSkillSlot();

                foreach (KeyValuePair<int, int> pair in m_currentSkillSlot)
                {
                    SetSlotSkill(pair.Key, m_skillLocalChart.GetSkillData(pair.Value));
                }

                SendEquipSlotMsg();

                m_changeSkillSlotPageMsg.SkillPageID = PlayerDataManager.Instance.GetCurrentSkillSlotPage();
                Message.Send(m_changeSkillSlotPageMsg);
            }
        }
        //------------------------------------------------------------------------------------
        public SkillData GetReadySkill(int currUserMp)
        {
            var node = m_readyActiveSkillData.First;

            while (node != null)
            {
                if (node.Value.NeedMP <= currUserMp)
                    return node.Value;
                else
                    node = node.Next;
            }    

            return m_defaultSkill;
        }
        //------------------------------------------------------------------------------------
        private void SetNextActiveSkill()
        {
            NextActiveSkill = GetReadySkill(PlayerManager.Instance.GetCurrentPlayerMP());
        }
        //------------------------------------------------------------------------------------
        public void UseSkill(SkillData data)
        {
            if (data == m_defaultSkill)
            {
                SetNextActiveSkill();
                return;
            }

            if (data.SkillTriggerType == SkillTriggerType.Active)
            {
                if (m_readyActiveSkillData.Contains(data) == true)
                    m_readyActiveSkillData.Remove(data);

                SetNextActiveSkill();
            }
            else if (data.SkillTriggerType == SkillTriggerType.Buff)
            {
                if (m_readyBuffSkillData.Contains(data) == true)
                    m_readyBuffSkillData.Remove(data);
            }

            int slotid = m_skillSlotID[data];
            SkillSlotData slotdata = m_playerSkillSlotData[slotid];
            slotdata.StartTime = Time.time;
            slotdata.IsUsed = true;
        }
        //------------------------------------------------------------------------------------
        public void OnClick_SkillSlot(int slotid)
        { 

        }
        //------------------------------------------------------------------------------------
        public void OnClick_AutoTrigger()
        {
            m_isAutoSkillMode = !m_isAutoSkillMode;

            m_setAutoSkillModeMsg.AutoSkillMode = m_isAutoSkillMode;
            Message.Send(m_setAutoSkillModeMsg);
        }
        //------------------------------------------------------------------------------------
    }
}