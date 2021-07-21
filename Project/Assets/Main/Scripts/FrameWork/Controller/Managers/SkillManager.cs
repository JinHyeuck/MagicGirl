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


        //// PassiveSkill들은 포함이 안된다.
        //// <Skillid, SkillCoolTimeData> 쿨타임이 있는 스킬들이 저장된다.
        //private Dictionary<int, SkillCoolTimeData> m_activeSkillList = new Dictionary<int, SkillCoolTimeData>();
        //// 언제든 사용할 수 있는 스킬들

        //// 스킬사용 시 넣어준다. -> 쿨타임 체크용도
        //private LinkedList<SkillCoolTimeData> m_usedSkill = new LinkedList<SkillCoolTimeData>();
        //// 쓰래기 방지용 풀
        //private Queue<SkillCoolTimeData> m_skillCoolDataPool = new Queue<SkillCoolTimeData>();
        //// PassiveSkill들은 포함이 안된다.







        private Dictionary<int, SkillSlotData> m_playerSkillSlotData = new Dictionary<int, SkillSlotData>();
        private Dictionary<SkillData, int> m_skillSlotID = new Dictionary<SkillData, int>();

        // 쿨타임 체크용도
        private LinkedList<SkillSlotData> m_slotCoolTimeData = new LinkedList<SkillSlotData>();

        private LinkedList<SkillData> m_readyActiveSkillData = new LinkedList<SkillData>();
        private LinkedList<SkillData> m_readyBuffSkillData = new LinkedList<SkillData>();









        private SkillData m_defaultSkill = null;

        public SkillData NextActiveSkill = null;

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
        }
        //------------------------------------------------------------------------------------
        public void SetSlotSkill(int slotid, SkillData skilldata)
        {
            if (m_playerSkillSlotData.ContainsKey(slotid) == false)
                return;

            if (m_playerSkillSlotData[slotid].CurrSlotState != SlotState.OpenSlot)
                return;

            if (m_skillSlotID.ContainsKey(skilldata) == true)
            { // 원래 슬롯에 있었다면
                int prevSlot = m_skillSlotID[skilldata];

                if (prevSlot == slotid)
                    return;

                m_skillSlotID.Remove(skilldata);

                SkillSlotData prevSlotData = m_playerSkillSlotData[prevSlot];
                SkillSlotData nextSlotData = m_playerSkillSlotData[slotid];

                if (nextSlotData.SkillData != null)
                {
                    if (nextSlotData.SkillData.SkillTriggerType == SkillTriggerType.Passive
                    || nextSlotData.SkillData.SkillTriggerType == SkillTriggerType.Buff)
                    {
                        // 버프 및 패시브로 사용한 능력 제거
                    }

                    // 이전 슬롯이 사용하려고 했던 스킬이면 제거
                    if (m_readyActiveSkillData.Contains(nextSlotData.SkillData) == true)
                        m_readyActiveSkillData.Remove(nextSlotData.SkillData);

                    if (NextActiveSkill == nextSlotData.SkillData)
                        SetNextActiveSkill();

                    m_skillSlotID.Remove(nextSlotData.SkillData);
                }

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
                //IsReadySkill == false;
                SkillSlotData nextSlotData = m_playerSkillSlotData[slotid];

                if (nextSlotData.SkillData != null)
                {
                    if (nextSlotData.SkillData.SkillTriggerType == SkillTriggerType.Passive
                    || nextSlotData.SkillData.SkillTriggerType == SkillTriggerType.Buff)
                    {
                        // 버프 및 패시브로 사용한 능력 제거
                    }

                    // 이전 슬롯이 사용하려고 했던 스킬이면 제거
                    if (m_readyActiveSkillData.Contains(nextSlotData.SkillData) == true)
                        m_readyActiveSkillData.Remove(nextSlotData.SkillData);

                    if (NextActiveSkill == nextSlotData.SkillData)
                        SetNextActiveSkill();

                    m_skillSlotID.Remove(nextSlotData.SkillData);
                }

                nextSlotData.SkillData = skilldata;
                nextSlotData.IsReadySkill = false;
                nextSlotData.IsUsed = true;
                nextSlotData.StartTime = Time.time;

                nextSlotData.SetElement();
            }

            m_skillSlotID.Add(skilldata, slotid);
        }
        //------------------------------------------------------------------------------------
        //public void SetSkillSlot()
        //{
        //    foreach (KeyValuePair<int, SkillCoolTimeData> pair in m_activeSkillList)
        //        m_skillCoolDataPool.Enqueue(pair.Value);

        //    m_currentSkillSlot = PlayerDataManager.Instance.GetCurrentSkillSlot();
        //    m_activeSkillList.Clear();
        //    m_readyActiveSkillData.Clear();
        //    m_readyBuffSkillData.Clear();
        //    m_usedSkill.Clear();

        //    foreach (KeyValuePair<int, int> pair in m_currentSkillSlot)
        //    {
        //        if (pair.Value != -1)
        //        {
        //            SkillData data = m_skillLocalChart.GetSkillData(pair.Value);

        //            if (data == null)
        //                continue;

        //            if (data.SkillTriggerType == SkillTriggerType.Active || data.SkillTriggerType == SkillTriggerType.Buff)
        //            {
        //                if (m_skillCoolDataPool.Count <= 0)
        //                    m_skillCoolDataPool.Enqueue(new SkillCoolTimeData());

        //                SkillCoolTimeData cooldata = m_skillCoolDataPool.Dequeue();
        //                cooldata.StartTime = 0.0f;
        //                cooldata.SkillData = data;
        //                cooldata.UIElement = null;

        //                m_activeSkillList.Add(data.SkillID, cooldata);

        //                if (data.SkillTriggerType == SkillTriggerType.Active)
        //                {
        //                    m_readyActiveSkillData.Add(data);
        //                }
        //                else if (data.SkillTriggerType == SkillTriggerType.Buff)
        //                {
        //                    m_readyBuffSkillData.AddLast(data);
        //                }
        //            }
        //        }
        //    }

        //    m_setSkillSlotMsg.SkillSlot = m_currentSkillSlot;

        //    Message.Send(m_setSkillSlotMsg);
        //}
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
                            // 버프 끝났다고 누구에게 알려주기
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
            // 스킬슬롯 와따리갔따리 하지 못하게 여기서 스롯쿨 넣어주기
            // 스킬슬롯 와따리갔따리 하지 못하게 여기서 스롯쿨 넣어주기
            // 스킬슬롯 와따리갔따리 하지 못하게 여기서 스롯쿨 넣어주기

            if (PlayerDataManager.Instance.ChangeSkillSlotPage(slotpage) == true)
            {
                m_currentSkillSlot = PlayerDataManager.Instance.GetCurrentSkillSlot();

                foreach (KeyValuePair<int, int> pair in m_currentSkillSlot)
                {
                    SetSlotSkill(pair.Key, m_skillLocalChart.GetSkillData(pair.Value));
                }
            }
        }
        //------------------------------------------------------------------------------------
        public void LinkSlotElement(int skillid, UISkillSlotElement element)
        {
            //SkillCoolTimeData data = null;
            //if (m_activeSkillList.TryGetValue(skillid, out data) == true)
            //{
            //    data.UIElement = element;
            //}
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

            //SkillCoolTimeData cooltime = null;

            //if (m_activeSkillList.TryGetValue(data.SkillID, out cooltime) == false)
            //    return;

            //cooltime.StartTime = Time.time;

            //m_usedSkill.AddLast(cooltime);
        }
        //------------------------------------------------------------------------------------
        //public void ReadyToChangeSkillSlot(SkillData skilldata, PlayerSkillInfo skillinfo)
        //{
        //    if (skillinfo == null)
        //        return;


        //}
        ////------------------------------------------------------------------------------------
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

            //SetSkillSlot();
        }
        //------------------------------------------------------------------------------------
    }
}