using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry.Managers
{
    public class MonsterManager : MonoSingleton<MonsterManager>
    {
        private int m_defaultCreateCount = 10;
        private int m_runTimeCreateCount = 3;

        private Queue<MonsterController> m_monsterPool = new Queue<MonsterController>();
        private Dictionary<string, MonsterController> m_spawnedMonster_Dic = new Dictionary<string, MonsterController>();
        private LinkedList<MonsterController> m_spawnedMonster_Linked = new LinkedList<MonsterController>();

        private Dictionary<string, Sprite> m_catchingMonsterSprite = new Dictionary<string, Sprite>();

        private MonsterLocalChart m_monsterLocalTable = null;

        private DunjeonMonsterReward m_rewardData = null;
        private DunjeonMonsterReward m_monsterKillRewardData = new DunjeonMonsterReward();

        private MonsterController m_monsterBase;

        private MonsterController m_foreFrontMonster = null;

        //------------------------------------------------------------------------------------
        protected override void Init()
        {
            m_monsterLocalTable = TableManager.Instance.GetTableClass<MonsterLocalChart>();

            AssetBundleLoader.Instance.Load<GameObject>("ContentResources/MonsterContent", "Monster", o =>
            {
                GameObject obj = o as GameObject;
                if (obj != null)
                {
                    m_monsterBase = obj.GetComponent<MonsterController>();
                }
            });

            CreateMonster(m_defaultCreateCount);
        }
        //------------------------------------------------------------------------------------
        public void SpawnMonster(string monsterid, Vector3 pos)
        {  // 던전 매니저에서 포지션이랑 데이터를 넣어줘서 솬한다.
            if (m_monsterPool.Count <= 0)
            {
                CreateMonster(m_runTimeCreateCount);
            }

            MonsterController monster = m_monsterPool.Dequeue();
            monster.gameObject.SetActive(true);

            string spawnid = string.Format("{0}_{1}", Time.time, m_spawnedMonster_Dic.Count);

            MonsterData monsterData = m_monsterLocalTable.GetMonsterData(monsterid);

            monster.transform.position = pos;
            monster.SetMonster(monsterData, spawnid);
            monster.PlayMonster();
            if (m_spawnedMonster_Linked.Count == 0)
            {
                monster.ReadyPlayerAttack(true);
                m_foreFrontMonster = monster;
            }

            m_spawnedMonster_Linked.AddLast(monster);
            m_spawnedMonster_Dic.Add(spawnid, monster);

        }
        //------------------------------------------------------------------------------------
        public Sprite GetMonsterSprite(string spritename)
        {
            Sprite sprite = null;

            if (m_catchingMonsterSprite.TryGetValue(spritename, out sprite) == false)
            {
                AssetBundleLoader.Instance.Load<UnityEngine.U2D.SpriteAtlas>("ContentResources/MonsterContent", "MonsterAtlas", o =>
                {
                    UnityEngine.U2D.SpriteAtlas spriteAtlas = o as UnityEngine.U2D.SpriteAtlas;

                    sprite = spriteAtlas.GetSprite(spritename);

                });
            }

            return sprite;
        }
        //------------------------------------------------------------------------------------
        public void SetDunjeonMonsterReward(DunjeonMonsterReward reward)
        {
            m_rewardData = reward;
        }
        //------------------------------------------------------------------------------------
        public MonsterController GetForeFrontMonster()
        {
            return m_foreFrontMonster;
        }
        //------------------------------------------------------------------------------------
        public void CreateMonster(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                GameObject clone = Instantiate(m_monsterBase.gameObject, transform);
                clone.SetActive(false);

                MonsterController monsterController = clone.GetComponent<MonsterController>();
                monsterController.Init(PlayerManager.Instance.GetPlayerController());

                m_monsterPool.Enqueue(monsterController);
            }
        }
        //------------------------------------------------------------------------------------
        public void OnDamage(float range, int damage, Vector3 playerpos)
        {
            var node = m_spawnedMonster_Linked.First;
            while (node != null)
            {
                if (Vector3.Distance(playerpos, node.Value.transform.position) < range)
                { 
                    node.Value.OnDamage(damage);
                    node = node.Next;
                }
                else
                    break;
            }
        }
        //------------------------------------------------------------------------------------
        public void DeadMonster(string spawnid)
        { // 몬스터가 죽을 때 호출해준다.
            SendMonsterKillReward();

            MonsterController monster = null;
            if (m_spawnedMonster_Dic.TryGetValue(spawnid, out monster) == true)
            {
                m_spawnedMonster_Dic.Remove(spawnid);
                //monster.gameObject.SetActive(false);

                if (m_spawnedMonster_Linked.First.Value == monster)
                {
                    m_spawnedMonster_Linked.RemoveFirst();
                }
                else
                {
                    m_spawnedMonster_Linked.Remove(monster);
                }

                if (m_spawnedMonster_Linked.First != null)
                {
                    m_spawnedMonster_Linked.First.Value.ReadyPlayerAttack(true);
                    m_foreFrontMonster = m_spawnedMonster_Linked.First.Value;
                }
                else
                    m_foreFrontMonster = null;
            }

            if (m_spawnedMonster_Dic.Count <= 0)
            {
                AllReleaseMonster();
            }
        }
        //------------------------------------------------------------------------------------
        private void SendMonsterKillReward()
        {
            m_monsterKillRewardData.Gold = (int)((float)m_rewardData.Gold * Random.Range(0.9f, 1.0f));
            m_monsterKillRewardData.Exp = m_rewardData.Exp;
            m_monsterKillRewardData.EquipmentSton = (int)((float)m_rewardData.EquipmentSton * Random.Range(0.9f, 1.0f));

            PlayerManager.Instance.RecvMonsterKillReward(m_monsterKillRewardData);
        }
        //------------------------------------------------------------------------------------
        private void AllReleaseMonster()
        {
            var node = m_spawnedMonster_Linked.First;
            while (node != null)
            {
                node.Value.ForceReleaseMonster();
                node = node.Next;
            }

            DunjeonManager.Instance.AllDeadMonster();
        }
        //------------------------------------------------------------------------------------
        public void ReleaseMonster(MonsterController monster)
        {
            monster.gameObject.SetActive(false);
            m_monsterPool.Enqueue(monster);
        }
        //------------------------------------------------------------------------------------
    }
}