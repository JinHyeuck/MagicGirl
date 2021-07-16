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
        private Text m_equipmentStonCountText = null;

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

            Message.AddListener<GameBerry.Event.RefreshLevelMsg>(RefreshLevel);
            Message.AddListener<GameBerry.Event.RefreshExpMsg>(RefreshExp);
            Message.AddListener<GameBerry.Event.RefreshGoldMsg>(RefreshGold);
            Message.AddListener<GameBerry.Event.RefreshDiaMsg>(RefreshDia);
            Message.AddListener<GameBerry.Event.RefreshEquipmentStonMsg>(RefreshEquipmentSton);
            Message.AddListener<GameBerry.Event.RefreshSkillStonMsg>(RefreshSkillSton);
        }
        //------------------------------------------------------------------------------------
        protected override void OnUnload()
        {
            Message.RemoveListener<GameBerry.Event.RefreshLevelMsg>(RefreshLevel);
            Message.RemoveListener<GameBerry.Event.RefreshExpMsg>(RefreshExp);
            Message.RemoveListener<GameBerry.Event.RefreshGoldMsg>(RefreshGold);
            Message.RemoveListener<GameBerry.Event.RefreshDiaMsg>(RefreshDia);
            Message.RemoveListener<GameBerry.Event.RefreshEquipmentStonMsg>(RefreshEquipmentSton);
            Message.RemoveListener<GameBerry.Event.RefreshSkillStonMsg>(RefreshSkillSton);
        }
        //------------------------------------------------------------------------------------
        protected override void OnEnter()
        {
            RefreshLevel(null);
            RefreshExp(null);
            RefreshGold(null);
            RefreshDia(null);
            RefreshEquipmentSton(null);
        }
        //------------------------------------------------------------------------------------
        private void RefreshLevel(GameBerry.Event.RefreshLevelMsg msg)
        {
            if (m_levelText != null)
                m_levelText.text = Managers.PlayerDataManager.Instance.GetLevel().ToString();
        }
        //------------------------------------------------------------------------------------
        private void RefreshExp(GameBerry.Event.RefreshExpMsg msg)
        {
            int currentLevel = Managers.PlayerDataManager.Instance.GetLevel();
            double currentExp = Managers.PlayerDataManager.Instance.GetExp();

            LevelData leveldata = m_levelLocalChart.GetLevelData(currentLevel);
            if (leveldata == null)
            {
                if (m_expFilledImage != null)
                    m_expFilledImage.fillAmount = 1.0f;

                if (m_expPercentText != null)
                    m_expPercentText.text = currentExp.ToString();

                return;
            }

            double ratio = currentExp / leveldata.Exp;

            if (m_expFilledImage != null)
                m_expFilledImage.fillAmount = (float)ratio;

            if (m_expPercentText != null)
                m_expPercentText.text = string.Format("{0:0.00}%", ratio * 100.0);
        }
        //------------------------------------------------------------------------------------
        private void RefreshGold(GameBerry.Event.RefreshGoldMsg msg)
        {
            if (m_goldText != null)
                m_goldText.text = string.Format("{0:#,0}", Managers.PlayerDataManager.Instance.GetGold());
        }
        //------------------------------------------------------------------------------------
        private void RefreshDia(GameBerry.Event.RefreshDiaMsg msg)
        {
            if (m_diaCountText != null)
                m_diaCountText.text = string.Format("{0:#,0}", Managers.PlayerDataManager.Instance.GetDia());
        }
        //------------------------------------------------------------------------------------
        private void RefreshEquipmentSton(GameBerry.Event.RefreshEquipmentStonMsg msg)
        {
            if (m_equipmentStonCountText != null)
                m_equipmentStonCountText.text = string.Format("{0:#,0}", Managers.PlayerDataManager.Instance.GetEquipmentSton());
        }
        //------------------------------------------------------------------------------------
        private void RefreshSkillSton(GameBerry.Event.RefreshSkillStonMsg msg)
        {
            
        }
        //------------------------------------------------------------------------------------
    }
}