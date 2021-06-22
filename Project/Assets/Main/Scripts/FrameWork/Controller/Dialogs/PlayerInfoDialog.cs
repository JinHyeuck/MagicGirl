using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry.UI
{
    public class PlayerInfoDialog : IDialog
    {
        [SerializeField]
        private Image m_playerIcon = null;

        [SerializeField]
        private Text m_levelText = null;

        [SerializeField]
        private Image m_jobRankIcon = null;

        [SerializeField]
        private Text m_playerNameText = null;

        [SerializeField]
        private Text m_upGradeStonCountText = null;

        [SerializeField]
        private Text m_diaCountText = null;

        [SerializeField]
        private Button m_rankingBtn = null;

        [SerializeField]
        private Button m_postBtn = null;

        [SerializeField]
        private Button m_settingBtn = null;

        [SerializeField]
        private Image m_expFilledImage = null;

        [SerializeField]
        private Text m_expPercentText = null;

        [SerializeField]
        private Transform m_goldGroup = null;

        [SerializeField]
        private Text m_goldText = null;

        //------------------------------------------------------------------------------------
        protected override void OnLoad()
        {
            if (m_playerNameText != null)
                m_playerNameText.text = TheBackEnd.TheBackEnd.Instance.GetNickPlayerName();
        }
        //------------------------------------------------------------------------------------
        protected override void OnEnter()
        {
            if (m_levelText != null)
                m_levelText.text = Managers.PlayerDataManager.Instance.GetLevel().ToString();

            if (m_goldText != null)
                m_goldText.text = Managers.PlayerDataManager.Instance.GetGold().ToString();

            if (m_diaCountText != null)
                m_diaCountText.text = Managers.PlayerDataManager.Instance.GetDia().ToString();

            if (m_upGradeStonCountText != null)
                m_upGradeStonCountText.text = Managers.PlayerDataManager.Instance.GetEquipmentSton().ToString();
        }
        //------------------------------------------------------------------------------------
    }
}