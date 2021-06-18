using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry
{
    public enum PlayerState : byte
    { 
        None = 0,
        Idle,
        Run,
        Attack,
        Hit,
        Dead,
    }

    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private Animator m_playerAnimator = null;

        private SkillData m_nextAttackSkill;

        private float m_attackRange = 2.0f;

        private int m_maxHP = 0;
        private int m_currentHP = 0;

        private int m_maxMP = 0;
        private int m_currentMP = 0;

        private PlayerState m_characterState = PlayerState.None;

        private Action_Attack m_attackScript = new Action_Attack();

        //------------------------------------------------------------------------------------
        public void Init()
        {
            if (m_attackScript == null)
                m_attackScript = new Action_Attack();

            m_attackScript.ConnectAttackCallBack(OnAttack);
            m_attackScript.ConnectFinishCallBack(OnEndAttack);
        }
        //------------------------------------------------------------------------------------
        public void StartHunting()
        {  // 사냥을 시작해야 할 때 호출
            ChangeState(PlayerState.Run);
        }
        //------------------------------------------------------------------------------------
        public void ResetPlayer()
        { // 주로 다른던전에 들어갈 때 직후나 던전 리트라이를 할 때 한다.
            ChangeState(PlayerState.Idle);
        }
        //------------------------------------------------------------------------------------
        public void StopPlayer()
        { // 캐릭터를 멈춘다. 완전 리셋되는건 아니고 그냥 멈추기만 한다.
            ChangeState(PlayerState.Idle);
        }
        //------------------------------------------------------------------------------------
        public void OnDamage(int damage)
        {
            m_currentHP -= damage;
            if (m_currentHP <= 0)
            { 
                m_currentHP = 0;
                ChangeState(PlayerState.Dead);
            }
            else
                ChangeState(PlayerState.Hit);
        }
        //------------------------------------------------------------------------------------
        private void Update()
        {
            SelectState();

            if (m_characterState == PlayerState.Attack)
            {
                m_attackScript.Updated();
            }
        }
        //------------------------------------------------------------------------------------
        private void SelectState()
        {
            if (m_characterState == PlayerState.None)
                return;

            if (Managers.MonsterManager.Instance.GetForeFrontMonster() != null)
            {
                if (m_nextAttackSkill == null)
                    Managers.SkillManager.Instance.GetNextSkill(m_currentMP);

                if (m_nextAttackSkill != null)
                {
                    if (m_characterState != PlayerState.Attack)
                    {
                        if (Vector3.Distance(Managers.MonsterManager.Instance.GetForeFrontMonster().transform.position, transform.position) < m_nextAttackSkill.Range)
                        {
                            ChangeState(PlayerState.Attack);
                            return;
                        }
                        else
                        { 
                            ChangeState(PlayerState.Run);
                            return;
                        }
                    }
                }
            }

            
        }
        //------------------------------------------------------------------------------------
        private void OnAttack()
        {
            UseSkill(m_nextAttackSkill);
        }
        //------------------------------------------------------------------------------------
        private void OnEndAttack()
        {
            ChangeState(PlayerState.Run);
        }
        //------------------------------------------------------------------------------------
        private void UseSkill(SkillData skillData)
        {
            Managers.MonsterManager.Instance.OnDamage(skillData.Range, skillData.Damage, transform.position);
        }
        //------------------------------------------------------------------------------------
        private void ChangeState(PlayerState characterState)
        {
            m_characterState = characterState;

            switch (characterState)
            {
                case PlayerState.Idle:
                    {
                        break;
                    }
                case PlayerState.Run:
                    {
                        break;
                    }
                case PlayerState.Attack:
                    {
                        if (m_attackScript != null)
                            m_attackScript.PlayAttack();
                        break;
                    }
                case PlayerState.Hit:
                    {
                        break;
                    }
                case PlayerState.Dead:
                    {
                        break;
                    }
            }
        }
        //------------------------------------------------------------------------------------
    }
}