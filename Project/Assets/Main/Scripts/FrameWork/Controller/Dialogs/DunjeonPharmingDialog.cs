using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameBerry.Common;

namespace GameBerry.UI
{
    [System.Serializable]
    public class RewardIconData
    {
        public DunjeonRewardType RewardType;
        public Sprite RewardIcon;
    }

    public class PharmingData
    {
        public DunjeonRewardType RewardType;
        public Sprite RewardIcon;
        public int RewardCount;
    }

    public class DunjeonPharmingDialog : IDialog
    {
        [SerializeField]
        private Transform m_pharmingListRoot;

        private UIPharmingElement m_pharmingElement;

        private LinkedList<UIPharmingElement> m_pharmingElementPool = new LinkedList<UIPharmingElement>();

        private Queue<PharmingData> m_pharmingDataPool = new Queue<PharmingData>();
        private Queue<PharmingData> m_waitPharmingData = new Queue<PharmingData>();

        [SerializeField]
        private List<RewardIconData> m_rewardIconData_List = new List<RewardIconData>();
        private Dictionary<DunjeonRewardType, Sprite> m_rewardIcon_Dic = new Dictionary<DunjeonRewardType, Sprite>();

        //------------------------------------------------------------------------------------
        private float m_showTimer = 0.1f;
        private float m_lastShowTime = 0.0f;

        //------------------------------------------------------------------------------------
        protected override void OnLoad()
        {
            AssetBundleLoader.Instance.Load<GameObject>("ContentResources/DunjeonContent", "UIPharmingElement", o =>
            {
                GameObject obj = o as GameObject;
                for (int i = 0; i < 5; ++i)
                {
                    GameObject clone = Instantiate(obj, m_pharmingListRoot);

                    if (clone != null)
                    {
                        UIPharmingElement element = clone.GetComponent<UIPharmingElement>();
                        element.Init(PopParmingData);
                        m_pharmingElementPool.AddLast(element);
                    }

                    clone.SetActive(false);
                }
            });

            if (m_rewardIconData_List != null)
            {
                for (int i = 0; i < m_rewardIconData_List.Count; ++i)
                {
                    if (m_rewardIcon_Dic.ContainsKey(m_rewardIconData_List[i].RewardType) == false)
                        m_rewardIcon_Dic.Add(m_rewardIconData_List[i].RewardType, m_rewardIconData_List[i].RewardIcon);
                }
            }

            Message.AddListener<GameBerry.Event.DunjeonPharmingRewardMsg>(DunjeonPharmingReward);
        }
        //------------------------------------------------------------------------------------
        protected override void OnUnload()
        {
            Message.RemoveListener<GameBerry.Event.DunjeonPharmingRewardMsg>(DunjeonPharmingReward);
        }
        //------------------------------------------------------------------------------------
        private void Update()
        {
            if (m_waitPharmingData.Count > 0)
            {
                if (m_lastShowTime < Time.time)
                { 
                    var node = m_pharmingElementPool.First;
                    UIPharmingElement element = node.Value;
                    element.gameObject.SetActive(true);
                    m_pharmingElementPool.RemoveFirst();

                    element.PlayDirection(m_waitPharmingData.Dequeue());
                    m_pharmingElementPool.AddLast(element);
                    element.transform.SetAsFirstSibling();

                    m_lastShowTime = Time.time + m_showTimer;
                }
            }
        }
        //------------------------------------------------------------------------------------
        private void DunjeonPharmingReward(GameBerry.Event.DunjeonPharmingRewardMsg msg)
        {
            if (m_pharmingDataPool.Count <= 0)
                m_pharmingDataPool.Enqueue(new PharmingData());

            Sprite sprite = null;

            m_rewardIcon_Dic.TryGetValue(msg.RewardType, out sprite);

            PharmingData data = m_pharmingDataPool.Dequeue();
            data.RewardType = msg.RewardType;
            data.RewardIcon = sprite;
            data.RewardCount = msg.RewardCount;

            m_waitPharmingData.Enqueue(data);
        }
        //------------------------------------------------------------------------------------
        private void PopParmingData(PharmingData data)
        {
            m_pharmingDataPool.Enqueue(data);
        }
        //------------------------------------------------------------------------------------
    }
}