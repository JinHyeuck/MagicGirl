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

        [SerializeField]
        private UICharacterState m_uiCharacterState;

        [SerializeField]
        private double m_customRecoveryRatio = 0.1;

        [SerializeField]
        private Transform m_varianceTransform;

        private double m_maxHP = 1000.0;
        private double m_currentHP = 0.0;
        private double m_hpRecovery = 0.0;

        private double m_maxMP = 100.0;
        private double m_currentMP = 0.0;
        private double m_mpRecovery = 0.0;
        public double CurrentMP { get { return m_currentMP; } }



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

            if (m_uiCharacterState != null)
                m_uiCharacterState.Init();

            m_attackScript.SetAttackWaitTime(m_attackWaitTime);
            m_attackScript.SetAttackRecovery(m_attackRecoveryTime);
            m_attackScript.ConnectAttackCallBack(OnAttack);
            m_attackScript.ConnectFinishCallBack(OnEndAttack);
            m_currentHP = m_maxHP;
            m_currentMP = m_maxMP;

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

            SetMaxHP(Managers.PlayerDataManager.Instance.GetOutPutHP());
            SetMaxMP(Managers.PlayerDataManager.Instance.GetOutPutMP());

            SetHP(m_maxHP);
            SetMP(m_maxMP);

            SetHPRecovery(Managers.PlayerDataManager.Instance.GetOutPutHPRecovery());
            SetMPRecovery(Managers.PlayerDataManager.Instance.GetOutPutMPRecovery());
        }
        //------------------------------------------------------------------------------------
        public void StopPlayer()
        { // 캐릭터를 멈춘다. 완전 리셋되는건 아니고 그냥 멈추기만 한다.
            ChangeState(PlayerState.Idle);
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

            SetHPMPRecovery();
        }
        //------------------------------------------------------------------------------------
        private void SetHPMPRecovery()
        {
            if (m_characterState == PlayerState.Dead)
                return;

            InCreaseHP(m_hpRecovery * Time.deltaTime);
            InCreaseMP(m_mpRecovery * Time.deltaTime);
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
        public void OnDamage(double damage)
        {
            if (damage <= 0.0)
                Managers.HPMPVarianceManager.Instance.ShowVarianceText(HpMpVarianceType.Miss, damage, m_varianceTransform.position);
            else
                Managers.HPMPVarianceManager.Instance.ShowVarianceText(HpMpVarianceType.NomalDamage, damage, m_varianceTransform.position);

            DeCreaseHP(damage);

            if (m_currentHP <= 0)
            {
                ChangeState(PlayerState.Dead);
            }
            else
                ChangeState(PlayerState.Hit);
        }
        //------------------------------------------------------------------------------------
        private void InCreaseHP(double hp)
        {
            SetHP(m_currentHP + hp);
        }
        //------------------------------------------------------------------------------------
        private void DeCreaseHP(double hp)
        {
            SetHP(m_currentHP - hp);
        }
        //------------------------------------------------------------------------------------
        private void SetHP(double hp)
        {
            m_currentHP = hp;

            if (m_currentHP < 0)
                m_currentHP = 0;

            if (m_currentHP > m_maxHP)
                m_currentHP = m_maxHP;

            double hpratio = m_currentHP / m_maxHP;

            if (m_uiCharacterState != null)
                m_uiCharacterState.SetHPBar(hpratio);
        }
        //------------------------------------------------------------------------------------
        private void SetMaxHP(double maxhp)
        {
            double currratio = m_currentHP / m_maxHP;
            m_currentHP = maxhp * currratio;

            m_maxHP = maxhp;
        }
        //------------------------------------------------------------------------------------
        private void SetHPRecovery(double hprecovery)
        {
            m_hpRecovery = hprecovery * m_customRecoveryRatio;
        }
        //------------------------------------------------------------------------------------
        public void UseMP(double mp)
        {
            // 나중에 엠피 깍아주기

            DeCreaseMP(mp);
        }
        //------------------------------------------------------------------------------------
        private void InCreaseMP(double mp)
        {
            SetMP(m_currentMP + mp);
        }
        //------------------------------------------------------------------------------------
        private void DeCreaseMP(double mp)
        {
            SetMP(m_currentMP - mp);
        }
        //------------------------------------------------------------------------------------
        private void SetMP(double mp)
        {
            m_currentMP = mp;

            if (m_currentMP < 0)
                m_currentMP = 0;

            if (m_currentMP > m_maxMP)
                m_currentMP = m_maxMP;

            double mpratio = m_currentMP / m_maxMP;

            if (m_uiCharacterState != null)
                m_uiCharacterState.SetMPBar(mpratio);
        }
        //------------------------------------------------------------------------------------
        private void SetMaxMP(double maxmp)
        {
            double currratio = m_currentMP / m_maxMP;
            m_currentMP = maxmp * currratio;

            m_maxMP = maxmp;
        }
        //------------------------------------------------------------------------------------
        private void SetMPRecovery(double mprecovery)
        {
            m_mpRecovery = mprecovery * m_customRecoveryRatio;
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