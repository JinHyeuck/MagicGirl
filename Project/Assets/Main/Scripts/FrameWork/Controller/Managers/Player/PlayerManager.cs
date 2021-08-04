using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry.Managers
{
    public class PlayerManager : MonoSingleton<PlayerManager>
    {
        private PlayerController m_playerController;
        private InGameCameraController m_inGameCamera;
        private Event.DunjeonPharmingRewardMsg m_dunjeonPharmingRewardMsg = new Event.DunjeonPharmingRewardMsg();


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
        public double GetCurrentPlayerMP()
        {
            return m_playerController.CurrentMP;
        }
        //------------------------------------------------------------------------------------
        public void UseMP(int mp)
        {
            m_playerController.UseMP(mp);
        }
        //------------------------------------------------------------------------------------
        public InGameCameraController GetInGameCameraController()
        {
            return m_inGameCamera;
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
        { // ���͸� �׿��� �� ��°�
            PlayerDataManager.Instance.AddGold(reward.Gold);
            PlayerDataManager.Instance.AddExp(reward.Exp);
            PlayerDataManager.Instance.AddEquipmentSton(reward.EquipmentSton);
            PlayerDataManager.Instance.AddSkillSton(reward.SkillSton);

            if (reward.Gold > 0)
            {
                m_dunjeonPharmingRewardMsg.RewardType = DunjeonRewardType.Gold;
                m_dunjeonPharmingRewardMsg.RewardCount = reward.Gold;
                Message.Send(m_dunjeonPharmingRewardMsg);
            }

            if (reward.Exp > 0)
            {
                m_dunjeonPharmingRewardMsg.RewardType = DunjeonRewardType.Exp;
                m_dunjeonPharmingRewardMsg.RewardCount = reward.Exp;
                Message.Send(m_dunjeonPharmingRewardMsg);
            }

            if (reward.EquipmentSton > 0)
            {
                m_dunjeonPharmingRewardMsg.RewardType = DunjeonRewardType.EquipmentSton;
                m_dunjeonPharmingRewardMsg.RewardCount = reward.EquipmentSton;
                Message.Send(m_dunjeonPharmingRewardMsg);
            }

            if (reward.SkillSton > 0)
            {
                m_dunjeonPharmingRewardMsg.RewardType = DunjeonRewardType.SkillSton;
                m_dunjeonPharmingRewardMsg.RewardCount = reward.SkillSton;
                Message.Send(m_dunjeonPharmingRewardMsg);
            }
        }
        //------------------------------------------------------------------------------------
        public void RecvDunjeonReward(DunjeonMonsterReward reward)
        { // ���� ������ ���� �� ��°�

        }
        //------------------------------------------------------------------------------------
    }
}