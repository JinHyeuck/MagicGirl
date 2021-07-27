using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry.Managers
{
    public enum DunjeonKinds
    { 
        DefaultDunjeon = 0,
        BossDunjeon,
        StoryDunjeon,
    }

    public class DunjeonManager : MonoSingleton<DunjeonManager>
    {
        DunjeonLocalChart m_dunjeonLocalTable = null;
        private DunjeonData m_currentDunjeonData = null;
        private string m_currentDunjeonKey = string.Empty;

        private Dictionary<DunjeonKinds, DunjeonBase> m_dunJeonLogic_Dic = new Dictionary<DunjeonKinds, DunjeonBase>();
        private DunjeonBase m_currentDunjeonLogic = null;

        private DunjeonMonsterReward m_dunjeonReward = new DunjeonMonsterReward();

        private DunjeonKinds m_currentDunjeonKinds = DunjeonKinds.DefaultDunjeon;

        private InGameBGScaler m_bgRenderer = null;

        private float m_monsterStartPosGab = 3.5f;

        //------------------------------------------------------------------------------------
        protected override void Init()
        {
            AssetBundleLoader.Instance.Load<GameObject>("ContentResources/DunjeonContent", "DunjeonBG", o =>
            {
                GameObject obj = o as GameObject;

                GameObject clone = Instantiate(obj, transform);

                if (clone != null)
                {
                    m_bgRenderer = clone.GetComponent<InGameBGScaler>();
                    m_bgRenderer.SetOperationTarget(Managers.PlayerManager.Instance.GetInGameCameraController().transform);
                }
            });

            m_dunjeonLocalTable = TableManager.Instance.GetTableClass<DunjeonLocalChart>();

            m_currentDunjeonKey = PlayerPrefs.GetString(Define.CurrentDunjeonKey, m_dunjeonLocalTable.m_dunjeonDataDatas[0].DunjeonID);

            m_currentDunjeonKinds = DunjeonKinds.DefaultDunjeon;

            m_dunJeonLogic_Dic.Add(DunjeonKinds.DefaultDunjeon, new Dunjeon_Default());
            m_dunJeonLogic_Dic.Add(DunjeonKinds.BossDunjeon, new Dunjeon_Boss());
            m_dunJeonLogic_Dic.Add(DunjeonKinds.StoryDunjeon, new Dunjeon_Story());

            
        }
        //------------------------------------------------------------------------------------
        private void SetDunjeonLogic(DunjeonKinds dunjeonKinds)
        {
            DunjeonBase dunjeonBase = null;

            if (m_dunJeonLogic_Dic.TryGetValue(dunjeonKinds, out dunjeonBase) == true)
                m_currentDunjeonLogic = dunjeonBase;
        }
        //------------------------------------------------------------------------------------
        private void ChangeDunjeonKinds(DunjeonKinds dunjeonKinds)
        {
            m_currentDunjeonKinds = dunjeonKinds;
        }
        //------------------------------------------------------------------------------------
        public void SetDunjeon()
        { // 던전을 플레이하기 전에 셋팅한다.
            SetDunjeonLogic(m_currentDunjeonKinds);
            m_currentDunjeonLogic.InitDunjeonLogic();

            m_currentDunjeonData = m_dunjeonLocalTable.GetDunjeonData(m_currentDunjeonKey);
            MonsterManager.Instance.SetDunjeonMonsterReward(m_currentDunjeonData.Reward);
            PlayerManager.Instance.ResetPlayer();
        }
        //------------------------------------------------------------------------------------
        public void PlayDunjeon()
        { // 해당 던전이 처음 시작될 때 호출된다.
            CreateMonster();

            StartCoroutine(PlayDunjeonDirection());
        }
        //------------------------------------------------------------------------------------
        private IEnumerator PlayDunjeonDirection()
        { // 시작에 대한 연출이 있다면 여기서 해야한다.
            Contents.GlobalContent.DoDunjeonFade(true);
            yield return new WaitForSeconds(1.5f);

            StartDunjeon();
        }
        //------------------------------------------------------------------------------------
        private void StartDunjeon()
        {
            PlayerManager.Instance.StartHunting();
        }
        //------------------------------------------------------------------------------------
        private void EndDunjeon()
        {
            StartCoroutine(EndDunjeonDirection());
        }
        //------------------------------------------------------------------------------------
        private IEnumerator EndDunjeonDirection()
        { // 끝나는 연출이 있다면 여기서 해야한다.
            Contents.GlobalContent.DoDunjeonFade(false);
            yield return new WaitForSeconds(1.5f);

            SetDunjeon();
            PlayDunjeon();
        }
        //------------------------------------------------------------------------------------
        public void DunjeonClear()
        {
            switch (m_currentDunjeonKinds)
            {
                case DunjeonKinds.DefaultDunjeon:
                    {
                        // 던전에 대한 보상을 주자
                        m_dunjeonReward.Gold = m_currentDunjeonData.Reward.Gold * 10;
                        m_dunjeonReward.Exp = m_currentDunjeonData.Reward.Exp * 10;
                        m_dunjeonReward.EquipmentSton = 0;

                        PlayerManager.Instance.RecvDunjeonReward(m_dunjeonReward);
                        DunjeonRePlay();
                        break;
                    }
                case DunjeonKinds.BossDunjeon:
                    {
                        // 다음레벨의 던전으로 올라가자
                        ChangeDunjeonKinds(DunjeonKinds.DefaultDunjeon);
                        ChangeNextDunjeon();
                        DunjeonRePlay();
                        break;
                    }
                case DunjeonKinds.StoryDunjeon:
                    {
                        ChangeDunjeonKinds(DunjeonKinds.DefaultDunjeon);
                        DunjeonRePlay();
                        break;
                    }
            }
        }
        //------------------------------------------------------------------------------------
        public void ChangeNextDunjeon()
        {
            m_currentDunjeonData = m_dunjeonLocalTable.GetDunjeonData(m_currentDunjeonData.DunjeonIndex + 1);

            PlayerPrefs.SetString(Define.CurrentDunjeonKey, m_currentDunjeonData.DunjeonID);
            PlayerPrefs.Save();
        }
        //------------------------------------------------------------------------------------
        public void DunjeonRePlay()
        {
            PlayerManager.Instance.StopPlayer();
            EndDunjeon();
        }
        //------------------------------------------------------------------------------------
        public void DunjeonFail()
        {
            switch (m_currentDunjeonKinds)
            {
                case DunjeonKinds.DefaultDunjeon:
                    {
                        DunjeonRePlay();
                        break;
                    }
                case DunjeonKinds.BossDunjeon:
                    {
                        // 다음레벨의 던전으로 올라가자
                        ChangeDunjeonKinds(DunjeonKinds.DefaultDunjeon);
                        DunjeonRePlay();
                        break;
                    }
                case DunjeonKinds.StoryDunjeon:
                    {
                        ChangeDunjeonKinds(DunjeonKinds.DefaultDunjeon);
                        DunjeonRePlay();
                        break;
                    }
            }
        }
        //------------------------------------------------------------------------------------
        public void CreateMonster()
        {
            Vector3 monsterstartpos = PlayerManager.Instance.GetPlayerController().transform.position;
            monsterstartpos.x += m_monsterStartPosGab;

            if (m_currentDunjeonKinds == DunjeonKinds.DefaultDunjeon)
            {
                m_currentDunjeonLogic.CreateMonster(m_currentDunjeonData.DunjeonMonsterID[Random.Range(0, m_currentDunjeonData.DunjeonMonsterID.Count)], monsterstartpos);
            }
            else if (m_currentDunjeonKinds == DunjeonKinds.BossDunjeon)
            {
                m_currentDunjeonLogic.CreateMonster(m_currentDunjeonData.DunjeonBossID[Random.Range(0, m_currentDunjeonData.DunjeonBossID.Count)], monsterstartpos);
            }
        }
        //------------------------------------------------------------------------------------
        public void PlayerDead()
        {
            DunjeonFail();
        }
        //------------------------------------------------------------------------------------
        public void AllDeadMonster()
        {
            m_currentDunjeonLogic.CheckClearDunjeon();
        }
        //------------------------------------------------------------------------------------
    }
}