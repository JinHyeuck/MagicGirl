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

    public class SkillSlotManager : MonoSingleton<SkillSlotManager>
    {
        // <SlotID, Skillid>
        private Dictionary<int, int> m_currentSkillSlot = new Dictionary<int, int>();


        // <SlotId, SkillSlotData> 모든 슬롯에 대한 정보가 저장되어있다.
        private Dictionary<int, SkillSlotData> m_playerSkillSlotData = new Dictionary<int, SkillSlotData>();
        // <SkillData, SlotId> 해당 스킬이 어느 슬롯에 있나
        private Dictionary<SkillData, int> m_skillSlotID = new Dictionary<SkillData, int>();

        // 쿨타임 체크용도
        private LinkedList<SkillSlotData> m_slotCoolTimeData = new LinkedList<SkillSlotData>();

        // 사용할 수 있는 액티브스킬
        private LinkedList<SkillData> m_readyActiveSkillData = new LinkedList<SkillData>();
        // 사용할 수 있는 패시브스킬
        private LinkedList<SkillData> m_readyBuffSkillData = new LinkedList<SkillData>();


        // 기본 평타 스킬
        private SkillData m_defaultSkill = null;

        // 사용될 다음 스킬
        public SkillData NextActiveSkill = null;

        private SkillLocalChart m_skillLocalChart;

        private Event.ChangeSkillSlotPageMsg m_changeSkillSlotPageMsg = new Event.ChangeSkillSlotPageMsg();
        private Event.ChangeEquipSkillMsg m_changeEquipSkillMsg = new Event.ChangeEquipSkillMsg();
        private Event.SetAutoSkillModeMsg m_setAutoSkillModeMsg = new Event.SetAutoSkillModeMsg();
        private Event.ChangeSlotStateMsg m_changeSlotStateMsg = new Event.ChangeSlotStateMsg();

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
            m_currentSkillSlot = SkillDataManager.Instance.GetCurrentSkillSlot();

            IEnumerator enumerator = m_currentSkillSlot.Keys.GetEnumerator();

            for (int i = 0; i < Define.PlayerDefaultSlotTotalCount; ++i)
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

            m_changeSkillSlotPageMsg.SkillPageID = SkillDataManager.Instance.GetCurrentSkillSlotPage();
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

            if (skilldata == null)
            {
                SkillSlotData noneslot = m_playerSkillSlotData[slotid];

                noneslot.SkillData = null;
                noneslot.StartTime = 0.0f;
                noneslot.IsReadySkill = false;
                noneslot.IsUsed = false;
                noneslot.SetElement();

                return;
            }


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
                    // 버프 및 패시브로 사용한 능력 제거

                    ReleaseBuff(nextSlotData.SkillData);
                }

                // 이전 슬롯이 사용하려고 했던 스킬이면 제거
                if (m_readyActiveSkillData.Contains(nextSlotData.SkillData) == true)
                    m_readyActiveSkillData.Remove(nextSlotData.SkillData);

                if (NextActiveSkill == nextSlotData.SkillData)
                    SetNextActiveSkill();

                m_skillSlotID.Remove(nextSlotData.SkillData);
            }

            if (prevSlot != -1)
            { // 원래 슬롯에 있었다면
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
                // 원래 슬롯에 없었다면
                nextSlotData.SkillData = skilldata;
                nextSlotData.IsReadySkill = false;
                nextSlotData.IsUsed = true;
                nextSlotData.StartTime = Time.time;

                nextSlotData.SetElement();
            }

            if(skilldata.SkillTriggerType == SkillTriggerType.Passive)
                ApplyBuff(skilldata);

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
                            SetNextAutoSkill();
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
                            // 버프 끝났다고 누구에게 알려주기
                            ReleaseBuff(coolnode.Value.SkillData);
                        }
                        else
                        {
                            for (int i = 0; i < elements.Count; ++i)
                            {
                                if (elements[i] != null)
                                    elements[i].SetBuffApplyTime(currenttime, applytime);
                            }
                            // 버프 아직 지속시간
                        }
                    }
                }

                coolnode = coolnode.Next;
            }

            if (m_isAutoSkillMode == true)
            {
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
        }
        //------------------------------------------------------------------------------------
        public void ChangeSkillSlotPage(int slotpage)
        {
            if (SkillDataManager.Instance.ChangeSkillSlotPage(slotpage) == true)
            {
                // 슬롯이 바뀐 경우에는 모든 스킬들을 릴리즈 시켜준다.

                foreach (KeyValuePair<SkillData, int> pair in m_skillSlotID)
                {
                    ReleaseBuff(pair.Key);
                }

                m_skillSlotID.Clear();
                m_readyActiveSkillData.Clear();
                m_readyBuffSkillData.Clear();

                m_currentSkillSlot = SkillDataManager.Instance.GetCurrentSkillSlot();

                foreach (KeyValuePair<int, int> pair in m_currentSkillSlot)
                {
                    SetSlotSkill(pair.Key, m_skillLocalChart.GetSkillData(pair.Value));
                }

                SendEquipSlotMsg();

                SetNextActiveSkill();

                m_changeSkillSlotPageMsg.SkillPageID = SkillDataManager.Instance.GetCurrentSkillSlotPage();
                Message.Send(m_changeSkillSlotPageMsg);
            }
        }
        //------------------------------------------------------------------------------------
        private void ApplyBuff(SkillData data)
        {
            SkillDataManager.Instance.ApplySkillBuff(data);
        }
        //------------------------------------------------------------------------------------
        private void ReleaseBuff(SkillData data)
        {
            SkillDataManager.Instance.ReleaseSkillBuff(data);
        }
        //------------------------------------------------------------------------------------
        private SkillData GetReadySkill(int currUserMp)
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
        private void SetNextAutoSkill()
        {
            if (m_isAutoSkillMode == true)
            {
                SetNextActiveSkill();
            }
        }
        //------------------------------------------------------------------------------------
        private void SetNextActiveSkill()
        {
            if (m_isAutoSkillMode == true)
            {
                NextActiveSkill = GetReadySkill(PlayerManager.Instance.GetCurrentPlayerMP());
            }
            else
            {
                NextActiveSkill = m_defaultSkill;
            }
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

                ApplyBuff(data);
            }

            int slotid = m_skillSlotID[data];
            SkillSlotData slotdata = m_playerSkillSlotData[slotid];
            slotdata.StartTime = Time.time;
            slotdata.IsUsed = true;
        }
        //------------------------------------------------------------------------------------
        public void OnClick_SkillSlot(int slotid)
        {
            SkillSlotData slotdata = null;

            if (m_playerSkillSlotData.TryGetValue(slotid, out slotdata) == false)
                return;

            if (slotdata == null)
                return;

            if (slotdata.CurrSlotState == SlotState.LockSlot)
                return;

            if (slotdata.CurrSlotState == SlotState.AddSlot)
            {
                OpenAddSlot(slotdata);
            }
            else if (slotdata.CurrSlotState == SlotState.OpenSlot)
            {
                if (m_isAutoSkillMode == true)
                    return;

                if (slotdata.SkillData == null)
                    return;

                if (slotdata.SkillData.SkillTriggerType == SkillTriggerType.Passive)
                    return;

                if (slotdata.IsUsed == true)
                    return;

                if (slotdata.SkillData.NeedMP > PlayerManager.Instance.GetCurrentPlayerMP())
                    return;

                if (slotdata.SkillData.SkillTriggerType == SkillTriggerType.Active)
                {
                    NextActiveSkill = slotdata.SkillData;
                }
                else if (slotdata.SkillData.SkillTriggerType == SkillTriggerType.Buff)
                {
                    UseSkill(slotdata.SkillData);
                }
            }
            
        }
        //------------------------------------------------------------------------------------
        public void OnClick_AutoTrigger()
        {
            m_isAutoSkillMode = !m_isAutoSkillMode;

            SetNextActiveSkill();

            m_setAutoSkillModeMsg.AutoSkillMode = m_isAutoSkillMode;
            Message.Send(m_setAutoSkillModeMsg);
        }
        //------------------------------------------------------------------------------------
        private void OpenAddSlot(SkillSlotData slot)
        {
            if (slot == null || slot.CurrSlotState != SlotState.AddSlot)
                return;

            if (m_changeSlotStateMsg.SkillSlotData == null)
                m_changeSlotStateMsg.SkillSlotData = new List<SkillSlotData>();

            m_changeSlotStateMsg.SkillSlotData.Clear();

            slot.CurrSlotState = SlotState.OpenSlot;

            m_changeSlotStateMsg.SkillSlotData.Add(slot);

            SkillDataManager.Instance.OpenSkillSlot(slot.SlotID);

            int nextslotid = slot.SlotID + 1;

            if (m_playerSkillSlotData.ContainsKey(nextslotid) == true)
            {
                SkillSlotData nextslotdata = null;

                if (m_playerSkillSlotData.TryGetValue(nextslotid, out nextslotdata) == false)
                    return;

                if (nextslotdata != null)
                {
                    nextslotdata.CurrSlotState = SlotState.AddSlot;
                    m_changeSlotStateMsg.SkillSlotData.Add(nextslotdata);
                }
            }
            else
                nextslotid = -1;

            Message.Send(m_changeSlotStateMsg);
        }
        //------------------------------------------------------------------------------------
    }
}