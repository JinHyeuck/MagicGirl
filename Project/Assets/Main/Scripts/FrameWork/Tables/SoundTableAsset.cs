using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry
{
    [System.Serializable]
    public class SoundData
    {
        public string FileName = "";
        public string FilePath = "";
        public float Volum = 1.0f;
        public bool Loop = false;
    }

    [CreateAssetMenu(fileName = "SoundTable", menuName = "Table/SoundTable", order = 1)]
    public class SoundTableAsset : ScriptableObject
    {
        [SerializeField]
        public List<SoundData> m_soundDatas = new List<SoundData>();
        private Dictionary<string, SoundData> m_soundDatas_Dic = new Dictionary<string, SoundData>();

        void OnEnable()
        {
            for (int i = 0; i < m_soundDatas.Count; ++i)
            {
                m_soundDatas_Dic[m_soundDatas[i].FileName] = m_soundDatas[i];
            }
        }

        public SoundData GetSoundData(int idx)
        {
            if (idx < 0 || idx >= m_soundDatas.Count)
            {
                Debug.LogError("SoundGetIndex is ErrorIndex for SoundDataList");
                return null;
            }

            return m_soundDatas[idx];
        }

        public SoundData GetSoundData(string id)
        {

            return m_soundDatas_Dic[id];
        }

        public List<SoundData> GetSoundDataList() { return m_soundDatas; }

        public void InsertIndex(int idx)
        {
            if (idx == -1)
                return;

            m_soundDatas.Insert(idx, new SoundData());
        }

        public void DeleteIndex(int idx)
        {
            if (idx == -1)
                return;

            m_soundDatas.RemoveAt(idx);
        }

        public void SetAllData(List<SoundData> datas)
        {
            m_soundDatas.Clear();
            for (int i = 0; i < datas.Count; ++i)
            {
                m_soundDatas.Add(datas[i]);
            }
        }
    }
}
