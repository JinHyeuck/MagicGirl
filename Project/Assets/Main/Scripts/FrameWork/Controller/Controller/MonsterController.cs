using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry
{
    public class MonsterController : MonoBehaviour
    {
        public enum MonsterState : byte
        {
            None = 0,
            Idle,
            Attack,
            Hit,
            Dead,
        }

        [SerializeField]
        private SpriteRenderer m_monsterRenderer;

        [SerializeField]
        private Animator m_monsterAnimator = null;

        private MonsterData m_myMonsterData = null;

        private PlayerController m_playerController = null;

        private string m_spawnID = string.Empty;

        private int m_monsterMaxHp = 0;
        private int m_monsterCurrentHp = 0;
        private int m_monsterDamage = 0;


        private MonsterState m_currentState = MonsterState.None;
        public bool IsDead { get { return m_currentState == MonsterState.Dead; } }

        // Dead
        private float m_releaseWaitTime = 1.0f;
        private float m_monsterDeadTime = 0.0f;

        // Attack
        private bool m_readyPlayerAttack = false;
        private float m_attackRange = 2.0f;

        private float m_attackWaitTime = 0.3f; 
        private float m_attackRecoveryTime = 2.0f;

        // Hit
        private float m_hitRecoveryStartTime = 0.0f;
        private float m_hitRecoveryTime = 0.5f;

        private Action_Attack m_attackScript = new Action_Attack();

        //------------------------------------------------------------------------------------
        public void Init(PlayerController playerController)
        {
            m_playerController = playerController;
            if (m_attackScript == null)
                m_attackScript = new Action_Attack();

            m_attackScript.SetAttackWaitTime(m_attackWaitTime);
            m_attackScript.SetAttackRecovery(m_attackRecoveryTime);
            m_attackScript.ConnectAttackCallBack(OnAttack);
            m_attackScript.ConnectFinishCallBack(OnEndAttack);
        }
        //------------------------------------------------------------------------------------
        public void SetMonster(MonsterData data, string spawnid)
        {
            m_myMonsterData = data;
            m_spawnID = spawnid;
            if (m_monsterRenderer != null)
                m_monsterRenderer.sprite = Managers.MonsterManager.Instance.GetMonsterSprite(data.MonsterImageName);

            m_monsterMaxHp = m_myMonsterData.HP * 2;
            m_monsterCurrentHp = m_monsterMaxHp;
            m_monsterDamage = m_myMonsterData.Damage;
        }
        //------------------------------------------------------------------------------------
        
        public void PlayMonster()
        { // ���Ͱ� �˾Ƽ� �ʱ��� ���·� ������
            Color color = m_monsterRenderer.color;
            color.a = 1.0f;
            m_monsterRenderer.color = color;

            m_monsterRenderer.transform.localEulerAngles = Vector3.zero;

            ReadyPlayerAttack(false);
            ChangeState(MonsterState.Idle);
        }
        //------------------------------------------------------------------------------------
        public void ReadyPlayerAttack(bool ready)
        {
            m_readyPlayerAttack = ready;
        }
        //------------------------------------------------------------------------------------
        private void Update()
        {
            if (m_currentState != MonsterState.Dead)
            {
                if (m_currentState == MonsterState.Idle)
                {
                    if (m_readyPlayerAttack == true && m_currentState != MonsterState.Attack)
                    {
                        if (Vector3.Distance(transform.position, m_playerController.transform.position) < m_attackRange)
                        {
                            ChangeState(MonsterState.Attack);
                        }
                    }
                }
                else if (m_currentState == MonsterState.Attack)
                {
                    if (m_attackScript != null)
                        m_attackScript.Updated();
                }
                else if (m_currentState == MonsterState.Hit)
                {
                    if (Time.time > m_hitRecoveryStartTime + m_hitRecoveryTime)
                    {
                        ChangeState(MonsterState.Idle);
                    }
                }
            }
            else
            {
                if (Time.time > m_monsterDeadTime + m_releaseWaitTime)
                {
                    ReleaseMonster();
                }
                else
                {
                    if (m_releaseWaitTime > 0.0f)
                    {
                        Color color = m_monsterRenderer.color;
                        color.a = (m_releaseWaitTime - (Time.time - m_monsterDeadTime)) / m_releaseWaitTime;
                        m_monsterRenderer.color = color;
                    }
                }
            }
        }
        //------------------------------------------------------------------------------------
        public void OnDamage(int damage)
        {
            m_monsterCurrentHp -= damage;
            Debug.Log(string.Format("���� ���� ������ {0}, CurrentHP {1}, MaxHP {2}", damage, m_monsterCurrentHp, m_monsterMaxHp));
            if (m_monsterCurrentHp <= 0)
            {
                ChangeState(MonsterState.Dead);
            }
            else
            {
                ChangeState(MonsterState.Hit);
            }
        }
        //------------------------------------------------------------------------------------
        private void OnAttack()
        { // AttackScript���� ȣ�����ش�.
            Managers.PlayerManager.Instance.OnDamage(m_monsterDamage);
        }
        //------------------------------------------------------------------------------------
        private void OnEndAttack()
        { // AttackScript���� ȣ�����ش�.
            ChangeState(MonsterState.Idle);
        }
        //------------------------------------------------------------------------------------
        private void ReleaseMonster()
        { // ��¥ ��ü�� ������ �� ȣ��
            Managers.MonsterManager.Instance.ReleaseMonster(this);
        }
        //------------------------------------------------------------------------------------
        public void ForceReleaseMonster()
        { // ������ ������ ������ �׿��� �� ��

            if (m_currentState != MonsterState.Dead)
            { // ���� �� �˾Ƽ� �����Ǵ� ���ΰ� ������ ���µ鸸 ó���Ѵ�.
                if (m_currentState == MonsterState.Attack)
                    m_attackScript.Release();

                ChangeState(MonsterState.None);
                Color color = m_monsterRenderer.color;
                color.a = 0.0f;
                m_monsterRenderer.color = color;

                ReleaseMonster();
            }
        }
        //------------------------------------------------------------------------------------
        private void ChangeState(MonsterState state)
        {
            if (m_currentState == state)
                return;

            m_currentState = state;

            switch (state)
            {
                case MonsterState.Idle:
                    {
                        PlayAnimation(Define.AniTrigger_Idle);
                        break;
                    }
                case MonsterState.Attack:
                    {
                        PlayAnimation(Define.AniTrigger_Attack);
                        if (m_attackScript != null)
                            m_attackScript.PlayAttack();
                        
                        break;
                    }
                case MonsterState.Hit:
                    {
                        PlayAnimation(Define.AniTrigger_Hit);
                        m_hitRecoveryStartTime = Time.time;
                        break;
                    }
                case MonsterState.Dead:
                    {
                        PlayAnimation(Define.AniTrigger_Dead);
                        Managers.MonsterManager.Instance.DeadMonster(m_spawnID);
                        m_monsterDeadTime = Time.time;
                        break;
                    }
                default:
                    {
                        PlayAnimation(Define.AniTrigger_Dead);
                        break;
                    }
            }
        }
        //------------------------------------------------------------------------------------
        private void PlayAnimation(string trigger)
        {
            if (m_monsterAnimator != null)
            {
                m_monsterAnimator.Play(trigger);
            }
        }
        //------------------------------------------------------------------------------------
    }
}