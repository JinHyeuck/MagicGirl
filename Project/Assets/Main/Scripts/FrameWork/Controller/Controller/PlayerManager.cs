using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry.Managers
{
    public class PlayerManager : MonoSingleton<PlayerManager>
    {
        private PlayerController m_playerController;
        private InGameCameraController m_inGameCamera;

        //------------------------------------------------------------------------------------
        protected override void Init()
        {
            AssetBundleLoader.Instance.Load<GameObject>("ContentResources/PlayerContent", "PlayerController", o =>
            {
                GameObject obj = o as GameObject;

                GameObject clone = Instantiate(obj, transform);

                if (clone != null)
                { 
                    m_playerController = clone.GetComponent<PlayerController>();
                    m_playerController.Init();
                }
            });

            AssetBundleLoader.Instance.Load<GameObject>("ContentResources/PlayerContent", "InGameCamera", o =>
            {
                GameObject obj = o as GameObject;

                GameObject clone = Instantiate(obj, transform);

                if (clone != null)
                { 
                    m_inGameCamera = clone.GetComponent<InGameCameraController>();
                    if (m_playerController != null)
                        m_inGameCamera.SetFlowTarget(m_playerController.transform);
                }
            });
        }
        //------------------------------------------------------------------------------------
        public PlayerController GetPlayerController()
        {
            return m_playerController;
        }
        //------------------------------------------------------------------------------------
        public InGameCameraController GetInGameCameraController()
        {
            return m_inGameCamera;
        }
        //------------------------------------------------------------------------------------
        public void StartHunting()
        { // 던전에서 호출해준다. 사냥을 시작해야 할 때 호출
            if (m_playerController != null)
                m_playerController.StartHunting();
        }
        //------------------------------------------------------------------------------------
        public void ResetPlayer()
        { // 주로 다른던전에 들어갈 때 직후나 던전 리트라이를 할 때 한다.
            if (m_playerController != null)
                m_playerController.ResetPlayer();
        }
        //------------------------------------------------------------------------------------
        public void StopPlayer()
        { // 캐릭터를 멈춘다. 완전 리셋되는건 아니고 그냥 멈추기만 한다.
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