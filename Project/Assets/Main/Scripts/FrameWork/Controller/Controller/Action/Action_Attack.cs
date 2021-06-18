using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry
{
    public enum AttackState : byte
    { 
        None = 0,
        Wait,
        Recovery,
        Attack
    }

    public class Action_Attack
    {
        private bool m_readyPlayerAttack = false;
        private float m_attackRange = 1.0f;

        private float m_attackWaitStartTime = 0.0f;
        private float m_attackWaitTime = 0.5f;

        private float m_attackRecoveryStartTime = 0.0f;
        private float m_attackRecoveryTime = 2.0f;

        private AttackState m_currentAttackState = AttackState.None;

        private System.Action m_onAttackCallBack = null;
        private System.Action m_onFinishCallBack = null;

        //------------------------------------------------------------------------------------
        public void ConnectAttackCallBack(System.Action callback)
        {
            m_onAttackCallBack = callback;
        }
        //------------------------------------------------------------------------------------
        public void ConnectFinishCallBack(System.Action callback)
        {
            m_onFinishCallBack = callback;
        }
        //------------------------------------------------------------------------------------
        public void PlayAttack()
        {
            ChangeAttackState(AttackState.Wait);
        }
        //------------------------------------------------------------------------------------
        public void Release()
        {
            ChangeAttackState(AttackState.None);
        }
        //------------------------------------------------------------------------------------
        public void Updated()
        { // �̰� ����Ϸ��� ��Ʈ�ѷ����� �˾Ƽ� ȣ���������
            if (m_currentAttackState == AttackState.Wait)
            {
                if (Time.time > m_attackWaitStartTime + m_attackWaitTime)
                {
                    ChangeAttackState(AttackState.Attack);
                }
            }
            else if (m_currentAttackState == AttackState.Attack)
            {
                ChangeAttackState(AttackState.Recovery);
            }
            else if (m_currentAttackState == AttackState.Recovery)
            {
                if (Time.time > m_attackRecoveryStartTime + m_attackRecoveryTime)
                {
                    ChangeAttackState(AttackState.None);
                    OnFinishAttackState();
                }
            }
        }
        //------------------------------------------------------------------------------------
        private void ChangeAttackState(AttackState attackState)
        {
            m_currentAttackState = attackState;

            switch (attackState)
            {
                case AttackState.Wait:
                    {
                        m_attackWaitStartTime = Time.time;
                        break;
                    }
                case AttackState.Recovery:
                    {
                        m_attackRecoveryStartTime = Time.time;
                        break;
                    }
                case AttackState.Attack:
                    {
                        OnAttack();

                        break;
                    }
                default:
                    {
                        OnFinishAttackState();
                        break;
                    }
            }
        }
        //------------------------------------------------------------------------------------
        private void OnAttack()
        {
            if (m_onAttackCallBack != null)
                m_onAttackCallBack();
        }
        //------------------------------------------------------------------------------------
        private void OnFinishAttackState()
        {
            if (m_onFinishCallBack != null)
                m_onFinishCallBack();
        }
        //------------------------------------------------------------------------------------
    }
}