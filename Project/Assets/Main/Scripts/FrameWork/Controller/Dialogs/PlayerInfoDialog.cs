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

        private LevelLocalChart m_levelLocalChart = null;


        //------------------------------------------------------------------------------------
        protected override void OnLoad()
        {
            if (m_playerNameText != null)
                m_playerNameText.text = TheBackEnd.TheBackEnd.Instance.GetNickPlayerName();

            m_levelLocalChart = Managers.TableManager.Instance.GetTableClass<LevelLocalChart>();

            Message.AddListener<GameBerry.Event.RefrashLevelMsg>(RefrashLevel);
            Message.AddListener<GameBerry.Event.RefrashExpMsg>(RefrashExp);
            Message.AddListener<GameBerry.Event.RefrashGoldMsg>(RefrashGold);
            Message.AddListener<GameBerry.Event.RefrashDiaMsg>(RefrashDia);
            Message.AddListener<GameBerry.Event.RefrashEquipmentStonMsg>(RefrashEquipmentSton);
            Message.AddListener<GameBerry.Event.RefrashSkillStonMsg>(RefrashSkillSton);
        }
        //------------------------------------------------------------------------------------
        protected override void OnUnload()
        {
            Message.RemoveListener<GameBerry.Event.RefrashLevelMsg>(RefrashLevel);
            Message.RemoveListener<GameBerry.Event.RefrashExpMsg>(RefrashExp);
            Message.RemoveListener<GameBerry.Event.RefrashGoldMsg>(RefrashGold);
            Message.RemoveListener<GameBerry.Event.RefrashDiaMsg>(RefrashDia);
            Message.RemoveListener<GameBerry.Event.RefrashEquipmentStonMsg>(RefrashEquipmentSton);
            Message.RemoveListener<GameBerry.Event.RefrashSkillStonMsg>(RefrashSkillSton);
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

            int currentLevel = Managers.PlayerDataManager.Instance.GetLevel();
            int currentExp = Managers.PlayerDataManager.Instance.GetExp();

            LevelData leveldata = m_levelLocalChart.GetLevelData(currentLevel);
            if (leveldata == null)
            {
                if (m_expFilledImage != null)
                    m_expFilledImage.fillAmount = 1.0f;

                if (m_expPercentText != null)
                    m_expPercentText.text = currentExp.ToString();

                return;
            }

            float ratio = (float)currentExp / (float)leveldata.Exp;
            if (m_expFilledImage != null)
                m_expFilledImage.fillAmount = ratio;

            if (m_expPercentText != null)
                m_expPercentText.text = string.Format("{0}/{1}", currentExp, leveldata.Exp);
        }
        //------------------------------------------------------------------------------------
        private void RefrashLevel(GameBerry.Event.RefrashLevelMsg msg)
        {
            if (m_levelText != null)
                m_levelText.text = Managers.PlayerDataManager.Instance.GetLevel().ToString();
        }
        //------------------------------------------------------------------------------------
        private void RefrashExp(GameBerry.Event.RefrashExpMsg msg)
        {
            int currentLevel = Managers.PlayerDataManager.Instance.GetLevel();
            int currentExp = Managers.PlayerDataManager.Instance.GetExp();

            LevelData leveldata = m_levelLocalChart.GetLevelData(currentLevel);
            if (leveldata == null)
            {
                if (m_expFilledImage != null)
                    m_expFilledImage.fillAmount = 1.0f;

                if (m_expPercentText != null)
                    m_expPercentText.text = currentExp.ToString();

                return;
            }

            float ratio = (float)currentExp / (float)leveldata.Exp;
            if (m_expFilledImage != null)
                m_expFilledImage.fillAmount = ratio;

            if (m_expPercentText != null)
                m_expPercentText.text = string.Format("{0}/{1}", currentExp, leveldata.Exp);
        }
        //------------------------------------------------------------------------------------
        private void RefrashGold(GameBerry.Event.RefrashGoldMsg msg)
        {
            if (m_goldText != null)
                m_goldText.text = Managers.PlayerDataManager.Instance.GetGold().ToString();
        }
        //------------------------------------------------------------------------------------
        private void RefrashDia(GameBerry.Event.RefrashDiaMsg msg)
        {
            if (m_diaCountText != null)
                m_diaCountText.text = Managers.PlayerDataManager.Instance.GetDia().ToString();
        }
        //------------------------------------------------------------------------------------
        private void RefrashEquipmentSton(GameBerry.Event.RefrashEquipmentStonMsg msg)
        {
            if (m_upGradeStonCountText != null)
                m_upGradeStonCountText.text = Managers.PlayerDataManager.Instance.GetEquipmentSton().ToString();
        }
        //------------------------------------------------------------------------------------
        private void RefrashSkillSton(GameBerry.Event.RefrashSkillStonMsg msg)
        {
            
        }
        //------------------------------------------------------------------------------------
    }
}