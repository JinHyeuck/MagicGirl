using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry.UI
{
    [System.Serializable]
    public class UpGradeSpriteData
    {
        public StatUpGradeType Type;
        public Sprite SpriteData;
    }

    public class PlayerStatDialog : IDialog
    {
        [Header("---------ChangeTab---------")]
        [SerializeField]
        private Button m_changeTab_Upgrade;

        [SerializeField]
        private Button m_changeTab_Promotion;

        [Header("---------PlayerProfile---------")]
        [SerializeField]
        private Image m_playerIcon;

        [SerializeField]
        private Text m_levelText;

        [SerializeField]
        private Image m_expFilledImage;

        [SerializeField]
        private Text m_expText;

        [SerializeField]
        private Text m_expPercentText;

        [Header("---------UpGradeContent---------")]
        [SerializeField]
        private RectTransform m_upGradeContentRoot;

        [SerializeField]
        private RectTransform m_upGradeContentStartPos;

        [SerializeField]
        private float m_upGradeElementPosGab = -170.0f;

        [SerializeField]
        private UIUpGradeElement m_uIUpGradeElement;

        [SerializeField]
        private List<UpGradeSpriteData> m_upGradeSpriteData = new List<UpGradeSpriteData>();



        private LevelLocalChart m_levelLocalChart = null;
        private StatUpGradeLocalChart m_statUpGradeLocalChart = null;

        //------------------------------------------------------------------------------------
        protected override void OnLoad()
        {
            m_levelLocalChart = Managers.TableManager.Instance.GetTableClass<LevelLocalChart>();
            m_statUpGradeLocalChart = Managers.TableManager.Instance.GetTableClass<StatUpGradeLocalChart>();

            Message.AddListener<GameBerry.Event.RefreshLevelMsg>(RefrashLevel);
            Message.AddListener<GameBerry.Event.RefreshExpMsg>(RefrashExp);

            if (m_upGradeContentRoot != null)
            {
                float rootsize = (MathDatas.Abs(m_upGradeContentStartPos.sizeDelta.x) * 2.0f) +
                    (MathDatas.Abs(m_upGradeElementPosGab) * m_upGradeSpriteData.Count);

                Vector2 sizedelta = m_upGradeContentRoot.sizeDelta;
                sizedelta.y = rootsize;
                m_upGradeContentRoot.sizeDelta = sizedelta;
            }

            for (int i = 0; i < m_upGradeSpriteData.Count; ++i)
            {
                GameObject clone = Instantiate(m_uIUpGradeElement.gameObject, m_upGradeContentRoot.transform);
                RectTransform trans = clone.GetComponent<RectTransform>();

                trans.localPosition = m_upGradeContentStartPos.localPosition;

                Vector2 pos = trans.localPosition;
                pos.y += m_upGradeElementPosGab * i;
                trans.localPosition = pos;

                UIUpGradeElement element = clone.GetComponent<UIUpGradeElement>();

                StatUpGradeData data = m_statUpGradeLocalChart.GetStatUpGradeData(m_upGradeSpriteData[i].Type);

                int maxlevel = 0;

                if (data != null)
                    maxlevel = data.MaxLevel;

                element.Init(m_upGradeSpriteData[i].Type, m_upGradeSpriteData[i].SpriteData, maxlevel);
            }
        }
        //------------------------------------------------------------------------------------
        protected override void OnUnload()
        {
            Message.RemoveListener<GameBerry.Event.RefreshLevelMsg>(RefrashLevel);
            Message.RemoveListener<GameBerry.Event.RefreshExpMsg>(RefrashExp);
        }
        //------------------------------------------------------------------------------------
        protected override void OnEnter()
        {
            if (m_levelText != null)
                m_levelText.text = Managers.PlayerDataManager.Instance.GetLevel().ToString();

            RefrashExp(null);
        }
        //------------------------------------------------------------------------------------
        private void RefrashLevel(GameBerry.Event.RefreshLevelMsg msg)
        {
            if (m_levelText != null)
                m_levelText.text = Managers.PlayerDataManager.Instance.GetLevel().ToString();
        }
        //------------------------------------------------------------------------------------
        private void RefrashExp(GameBerry.Event.RefreshExpMsg msg)
        {
            int currentLevel = Managers.PlayerDataManager.Instance.GetLevel();
            double currentExp = Managers.PlayerDataManager.Instance.GetExp();

            LevelData leveldata = m_levelLocalChart.GetLevelData(currentLevel);
            if (leveldata == null)
            {
                if (m_expFilledImage != null)
                    m_expFilledImage.fillAmount = 1.0f;

                if (m_expText != null)
                    m_expText.text = currentExp.ToString();

                if (m_expPercentText != null)
                    m_expPercentText.text = currentExp.ToString();

                return;
            }

            double ratio = currentExp / leveldata.Exp;
            if (ratio > 1.0f)
                ratio = 1.0f;

            if (m_expFilledImage != null)
                m_expFilledImage.fillAmount = (float)ratio;

            if (m_expText != null)
                m_expText.text = string.Format("{0}/{1}", currentExp, leveldata.Exp);

            if (m_expPercentText != null)
                m_expPercentText.text = string.Format("{0:0.00}%", ratio * 100.0f);
        }
        //------------------------------------------------------------------------------------
    }
}