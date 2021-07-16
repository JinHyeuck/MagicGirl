using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry.UI
{
    [System.Serializable]
    public class GachaButton
    {
        public GaChaType Type = GaChaType.None;

        public Button GachaBtn;
        public Text GachaAmount;
        public Text GachaPrice;

        private System.Action<GaChaType> m_callback = null;

        //------------------------------------------------------------------------------------
        public void SetCallBack(System.Action<GaChaType> callback)
        {
            m_callback = callback;
        }
        //------------------------------------------------------------------------------------
        public void OnClick_Btn()
        {
            if (m_callback != null)
                m_callback(Type);
        }
        //------------------------------------------------------------------------------------
    }

    public class GachaDialog : IDialog
    {
        [SerializeField]
        private List<GachaButton> m_gachaBtnList = new List<GachaButton>();

        private GameBerry.Event.PlayGachaMsg m_playGachaMsg = new GameBerry.Event.PlayGachaMsg();

        //------------------------------------------------------------------------------------
        protected override void OnLoad()
        {
            GachaLocalChart gachaLocalChart = Managers.TableManager.Instance.GetTableClass<GachaLocalChart>();

            for (int i = 0; i < m_gachaBtnList.Count; ++i)
            {
                GachaButton btn = m_gachaBtnList[i];

                GachaData data = gachaLocalChart.GetGachaData(btn.Type);
                if (data == null)
                    continue;

                if (btn.GachaBtn != null)
                { 
                    btn.GachaBtn.onClick.AddListener(btn.OnClick_Btn);
                    btn.SetCallBack(OnClick_GachaBtn);
                }

                if (btn.GachaAmount != null)
                    btn.GachaAmount.text = string.Format("{0}회", data.Amount);

                if (btn.GachaPrice != null)
                    btn.GachaPrice.text = data.Price.ToString();
            }
        }
        //------------------------------------------------------------------------------------
        private void OnClick_GachaBtn(GaChaType type)
        {
            if (type == GaChaType.None)
                return;

            m_playGachaMsg.Type = type;
            Message.Send(m_playGachaMsg);
        }
        //------------------------------------------------------------------------------------
    }
}