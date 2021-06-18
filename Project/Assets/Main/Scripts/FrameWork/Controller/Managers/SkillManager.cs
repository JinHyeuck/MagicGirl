using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry.Managers
{
    public class SkillCoolTimeData
    {
        public float StartTime = 0.0f;
        public string SkillID = string.Empty;
    }

    public class SkillManager : MonoSingleton<SkillManager>
    {
        private SkillLocalTable m_skillLocalTable = null;

        private List<SkillData> m_equipSkillList = new List<SkillData>(); // 슬롯에 등록한 스킬

        private Dictionary<string, SkillData> m_usableSkillList = new Dictionary<string, SkillData>(); // 열려있는 스킬

        private LinkedList<SkillCoolTimeData> m_coolTimeSkill_Linked = new LinkedList<SkillCoolTimeData>();
        private Dictionary<string, SkillData> m_coolTimeSkill_Dic = new Dictionary<string, SkillData>();

        private SkillData m_defaultSkill = null;

        private string m_openSkillData = string.Empty;
        private string m_equipSkillData = string.Empty;

        private int m_openSkillSlotCount = 0;

        //------------------------------------------------------------------------------------
        protected override void Init()
        {
            m_skillLocalTable = TableManager.Instance.GetTableClass<SkillLocalTable>();

            m_openSkillSlotCount = PlayerPrefs.GetInt(Define.OpenSkillSlotCountKey, 0);

            m_openSkillData = PlayerPrefs.GetString(Define.OpenSkillListKey, string.Empty);
            string[] openskillarr = m_openSkillData.Split(',');

            for (int i = 0; i < openskillarr.Length; ++i)
            {
                m_usableSkillList.Add(openskillarr[i], m_skillLocalTable.GetSkillData(openskillarr[i]));
            }

            m_equipSkillData = PlayerPrefs.GetString(Define.EquipSkillKey, string.Empty);
            string[] equipskillarr = m_equipSkillData.Split(',');

            for (int i = 0; i < equipskillarr.Length; ++i)
            {
                if (m_openSkillSlotCount - 1 >= i)
                {
                    string key = equipskillarr[i];
                    SkillData data = null;
                    if (m_usableSkillList.TryGetValue(key, out data) == true)
                        m_equipSkillList.Add(data);
                    else
                        m_equipSkillList.Add(null);
                }
            }

            m_defaultSkill = m_skillLocalTable.m_SkillDatas[0];
        }
        //------------------------------------------------------------------------------------
        private void Update()
        {
            // 쿨타임 체크
            {
                var node = m_coolTimeSkill_Linked.First;
                while (node != null)
                {
                    SkillData skillData = null;
                    if (m_coolTimeSkill_Dic.TryGetValue(node.Value.SkillID, out skillData) == true)
                    {
                        if (node.Value.StartTime + skillData.CoolTime < Time.time)
                        {
                            m_coolTimeSkill_Dic.Remove(node.Value.SkillID);
                            m_coolTimeSkill_Linked.Remove(node);
                            node = node.Next;
                        }
                    }
                    else
                    {
                        m_coolTimeSkill_Linked.Remove(node);
                        node = node.Next;
                    }
                }
            }
        }
        //------------------------------------------------------------------------------------
        private void AddOpenSkillData(string key)
        {
            // 새로운 스킬이 추가되었을 때는 여기서 한다.
            if (string.IsNullOrEmpty(m_openSkillData) != false)
            {
                m_openSkillData = string.Format("{0},{1}", m_openSkillData, key);
            }
            else
            {
                m_openSkillData = key;
            }

            PlayerPrefs.SetString(Define.OpenSkillListKey, m_openSkillData);

            m_usableSkillList.Add(key, m_skillLocalTable.GetSkillData(key));
        }
        //------------------------------------------------------------------------------------
        private void ChangeEquipSkillData(string key, int index)
        {
            // 슬롯에 장착한 스킬이 달라지면 여기서 한다.
            if (m_equipSkillList.Count <= index)
                return;

            m_equipSkillList[index] = m_skillLocalTable.GetSkillData(key);

            SaveEquipSkillData();
        }
        //------------------------------------------------------------------------------------
        private void RemoveEquipSkillData(string key, int index)
        {
            // 슬롯에 스킬을 빼면 여기로 온다.
            if (m_equipSkillList.Count <= index)
                return;

            m_equipSkillList[index] = null;

            SaveEquipSkillData();
        }
        //------------------------------------------------------------------------------------
        private void SaveEquipSkillData()
        {
            string newdata = string.Empty;

            for (int i = 0; i < m_equipSkillList.Count; ++i)
            {
                string keystr = string.Empty;
                if (m_equipSkillList[i] != null)
                    keystr = m_equipSkillList[i].SkillID;
                else
                    keystr = " ";

                if (i == 0)
                {
                    newdata = string.Format("{0}", keystr);
                }
                else
                {
                    newdata = string.Format("{0},{1}", newdata, keystr);
                }
            }

            m_equipSkillData = newdata;

            PlayerPrefs.SetString(Define.EquipSkillKey, m_equipSkillData);
        }
        //------------------------------------------------------------------------------------
        private void AddSkillSlot()
        {
            m_equipSkillList.Add(null);
            m_openSkillSlotCount = m_equipSkillList.Count;
            PlayerPrefs.SetInt(Define.OpenSkillSlotCountKey, m_openSkillSlotCount);
        }
        //------------------------------------------------------------------------------------
        public SkillData GetNextSkill(int currUserMp)
        {
            for (int i = 0; i < m_equipSkillList.Count; ++i)
            {
                if (m_equipSkillList[i].NeedMP > currUserMp)
                    continue;

                if (m_equipSkillList[i].SkillType != SkillTriggerType.Active)
                    continue;

                if (m_coolTimeSkill_Dic.ContainsKey(m_equipSkillList[i].SkillID) == true)
                    continue;

                return m_equipSkillList[i];
            }

            return m_defaultSkill;
        }
        //------------------------------------------------------------------------------------
        public void UseSkill(SkillData skillData)
        {
            SkillCoolTimeData data = new SkillCoolTimeData();
            data.SkillID = skillData.SkillID;
            data.StartTime = Time.time;

            m_coolTimeSkill_Linked.AddLast(data);
            m_coolTimeSkill_Dic.Add(skillData.SkillID, skillData);
        }
        //------------------------------------------------------------------------------------
    }
}