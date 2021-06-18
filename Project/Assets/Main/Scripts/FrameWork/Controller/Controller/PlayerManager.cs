using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry.Managers
{
    public class PlayerManager : MonoSingleton<PlayerManager>
    {
        private PlayerController m_playerController;
        private Camera m_inGameCamera;

        //------------------------------------------------------------------------------------
        protected override void Init()
        {
            AssetBundleLoader.Instance.Load<GameObject>("ContentResources/PlayerContent", "PlayerController", o =>
            {
                GameObject obj = o as GameObject;

                GameObject clone = Instantiate(obj, transform);

                if (clone != null)
                    m_playerController = clone.GetComponent<PlayerController>();
            });

            AssetBundleLoader.Instance.Load<GameObject>("ContentResources/PlayerContent", "InGameCamera", o =>
            {
                GameObject obj = o as GameObject;

                GameObject clone = Instantiate(obj, transform);

                if (clone != null)
                    m_inGameCamera = clone.GetComponent<Camera>();
            });
        }
        //------------------------------------------------------------------------------------
        public PlayerController GetPlayerController()
        {
            return m_playerController;
        }
        //------------------------------------------------------------------------------------
        public void StartHunting()
        { // �������� ȣ�����ش�. ����� �����ؾ� �� �� ȣ��
            if (m_playerController != null)
                m_playerController.StartHunting();
        }
        //------------------------------------------------------------------------------------
        public void ResetPlayer()
        { // �ַ� �ٸ������� �� �� ���ĳ� ���� ��Ʈ���̸� �� �� �Ѵ�.
            if (m_playerController != null)
                m_playerController.ResetPlayer();
        }
        //------------------------------------------------------------------------------------
        public void StopPlayer()
        { // ĳ���͸� �����. ���� ���µǴ°� �ƴϰ� �׳� ���߱⸸ �Ѵ�.
            if (m_playerController != null)
                m_playerController.StopPlayer();
        }
        //------------------------------------------------------------------------------------
        public void PlayerDead()
        {
            DunjeonManager.Instance.PlayerDead();
        }
        //------------------------------------------------------------------------------------
        public void OnDamage(int damage)
        {
            m_playerController.OnDamage(damage);
        }
        //------------------------------------------------------------------------------------
        public void RecvMonsterKillReward(DunjeonMonsterReward reward)
        { 

        }
        //------------------------------------------------------------------------------------
        public void RecvDunjeonReward(DunjeonMonsterReward reward)
        {

        }
        //------------------------------------------------------------------------------------
    }
}