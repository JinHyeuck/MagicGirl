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

    public class PlayerDamageData
    {
        public double AttackDamage;
        public bool IsCritical;
    }

    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private Animator m_playerAnimator = null;

        private int m_maxHP = 1000;
        private int m_currentHP = 0;


        private int m_maxMP = 0;
        private int m_currentMP = 100;
        public int CurrentMP { get { return m_currentMP; } }



        private float m_moveSpeed = 2.0f;

        private float m_attackWaitTime = 0.3f;
        private float m_attackRecoveryTime = 0.5f;

        private PlayerState m_characterState = PlayerState.None;

        private Action_Attack m_attackScript = new Action_Attack();

        // Hit
        private float m_hitRecoveryStartTime = 0.0f;
        private float m_hitRecoveryTime = 0.5f;

        private Vector3 m_originPos = Vector3.one;

        public UnityEngine.UI.Text TestText;

        //------------------------------------------------------------------------------------
        public void Init()
        {
            if (m_attackScript == null)
                m_attackScript = new Action_Attack();
                
            m_attackScript.SetAttackWaitTime(m_attackWaitTime);
            m_attackScript.SetAttackRecovery(m_attackRecoveryTime);
            m_attackScript.ConnectAttackCallBack(OnAttack);
            m_attackScript.ConnectFinishCallBack(OnEndAttack);
            m_currentHP = m_maxHP;

            m_originPos = transform.position;
        }
        //------------------------------------------------------------------------------------
        public void StartHunting()
        {  // 사냥을 시작해야 할 때 호출
            ChangeState(PlayerState.Run);
        }
        //------------------------------------------------------------------------------------
        public void ResetPlayer()
        { // 주로 다른던전에 들어갈 때 직후나 던전 리트라이를 할 때 한다.
            transform.position = m_originPos;
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

            switch (m_characterState)
            {
                case PlayerState.Run:
                    {
                        Vector3 pos = transform.position;
                        pos.x += m_moveSpeed * Time.deltaTime;
                        transform.position = pos;
                        break;
                    }
                case PlayerState.Attack:
                    {
                        if (m_attackScript != null)
                            m_attackScript.Updated();
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
                case PlayerState.None:
                    {
                        break;
                    }
            }

        }
        //------------------------------------------------------------------------------------
        private void SelectState()
        {
            if (m_characterState == PlayerState.None || m_characterState == PlayerState.Dead)
                return;

            if (m_characterState == PlayerState.Hit)
            {
                if (Time.time > m_hitRecoveryStartTime + m_hitRecoveryTime)
                    ChangeState(PlayerState.Idle);
                return;
            }

            if (Managers.MonsterManager.Instance.GetForeFrontMonster() != null)
            {
                if (Managers.SkillSlotManager.Instance.NextActiveSkill != null)
                {
                    if (m_characterState != PlayerState.Attack)
                    {
                        //if (Vector3.Distance(Managers.MonsterManager.Instance.GetForeFrontMonster().transform.position, transform.position) < Managers.SkillSlotManager.Instance.NextActiveSkill.Range)
                        if (MathDatas.Abs(Managers.MonsterManager.Instance.GetForeFrontMonster().transform.position.x - transform.position.x) < Managers.SkillSlotManager.Instance.NextActiveSkill.Range)
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
            UseSkill(Managers.SkillSlotManager.Instance.NextActiveSkill);
        }
        //------------------------------------------------------------------------------------
        private void OnEndAttack()
        {
            ChangeState(PlayerState.Run);
        }
        //------------------------------------------------------------------------------------
        private void UseSkill(SkillData skillData)
        {
            //Managers.MonsterManager.Instance.OnDamage(skillData.Range, (int)skillData.OptionValue, transform.position);
            Managers.MonsterManager.Instance.OnDamage(skillData.Range, Managers.PlayerDataManager.Instance.GetAttackDamage(skillData), transform.position);
            Managers.SkillSlotManager.Instance.UseSkill(skillData);
            UseMP(skillData.NeedMP);
        }
        //------------------------------------------------------------------------------------
        public void UseMP(int mp)
        { 
            // 나중에 엠피 깍아주기
        }
        //------------------------------------------------------------------------------------
        private void ChangeState(PlayerState characterState)
        {
            if (m_characterState == characterState)
                return;

            m_characterState = characterState;

            if (TestText != null)
                TestText.text = characterState.ToString();

            switch (m_characterState)
            {
                case PlayerState.Idle:
                    {
                        PlayAnimation(Define.AniTrigger_Idle);
                        break;
                    }
                case PlayerState.Run:
                    {
                        PlayAnimation(Define.AniTrigger_Run);
                        break;
                    }
                case PlayerState.Attack:
                    {
                        PlayAnimation(Define.AniTrigger_Attack);
                        if (m_attackScript != null)
                            m_attackScript.PlayAttack();
                        break;
                    }
                case PlayerState.Hit:
                    {
                        PlayAnimation(Define.AniTrigger_Hit);
                        m_hitRecoveryStartTime = Time.time;
                        if (m_attackScript != null)
                            m_attackScript.Release();
                        break;
                    }
                case PlayerState.Dead:
                    {
                        PlayAnimation(Define.AniTrigger_Dead);
                        Managers.PlayerManager.Instance.PlayerDead();
                        break;
                    }
            }
        }
        //------------------------------------------------------------------------------------
        private void PlayAnimation(string trigger)
        {
            if (m_playerAnimator != null)
            { 
                m_playerAnimator.Play(trigger);
            }
        }
        //------------------------------------------------------------------------------------
    }
}