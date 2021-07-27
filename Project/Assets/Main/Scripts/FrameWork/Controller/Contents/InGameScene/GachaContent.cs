using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace GameBerry.Contents
{
    public class GachaContent : IContent
    {
        private GachaLocalChart m_gachaLocalChart = null;
        private EquipmentLocalChart m_equipmentLocalChart = null;
        private SkillLocalChart m_skillLocalChart = null;

        private GachaData m_currGachaData = null;

        private GameBerry.Event.ResultGachaMsg m_resultGachaMsg = new GameBerry.Event.ResultGachaMsg();

        //------------------------------------------------------------------------------------
        protected override void OnLoadComplete()
        {
            m_gachaLocalChart = Managers.TableManager.Instance.GetTableClass<GachaLocalChart>();
            m_equipmentLocalChart = Managers.TableManager.Instance.GetTableClass<EquipmentLocalChart>();
            m_skillLocalChart = Managers.TableManager.Instance.GetTableClass<SkillLocalChart>();

            Message.AddListener<GameBerry.Event.PlayGachaMsg>(PlayGacha);
        }
        //------------------------------------------------------------------------------------
        protected override void OnUnload()
        {
            Message.RemoveListener<GameBerry.Event.PlayGachaMsg>(PlayGacha);
        }
        //------------------------------------------------------------------------------------
        private void PlayGacha(GameBerry.Event.PlayGachaMsg msg)
        {
            StartCoroutine(TestPlayGacha(msg));

            return;
            if (m_gachaLocalChart == null)
                return;

            GachaData data = m_gachaLocalChart.GetGachaData(msg.Type);
            if (data == null)
                return;

            TheBackEnd.TheBackEnd.Instance.GetProbabilitys(data.GachaChartName, data.Amount, RecvGachaDataResult);
        }
        //------------------------------------------------------------------------------------
        private IEnumerator TestPlayGacha(GameBerry.Event.PlayGachaMsg msg)
        {
            if (m_gachaLocalChart == null)
                yield break;

            GachaData data = m_gachaLocalChart.GetGachaData(msg.Type);
            if (data == null)
                yield break;

            GlobalContent.VisibleBufferingUI(true);

            bool result = false;

            TheBackEnd.TheBackEnd_PlayerTable.GetCharacterInfoTableDataTest(() =>
            {
                result = true;
            });

            while (result == false)
                yield return null;

            if (Managers.PlayerDataManager.Instance.GetDia() < data.Price)
            {
                GlobalContent.VisibleBufferingUI(false);
                Debug.Log("다이아 부족함");
                yield break;
            }

            m_currGachaData = data;

            TheBackEnd.TheBackEnd.Instance.GetProbabilitys(data.GachaChartName, data.Amount, RecvGachaDataResult);
        }
        //------------------------------------------------------------------------------------
        private void RecvGachaDataResult(JsonData data)
        {
            GlobalContent.VisibleBufferingUI(false);

            Debug.Log("------------뽑기결과");

            List<int> gachaid = new List<int>();

            for (int i = 0; i < data.Count; ++i)
            {
                string returnValue = string.Empty;

                foreach (var key in data[i].Keys)
                {
                    if (key == "itemID")
                    {
                        gachaid.Add(data[i][key].ToString().FastStringToInt());
                    }
                    returnValue += string.Format("{0} : {1} / ", key, data[i][key].ToString());
                }
                Debug.Log(returnValue);
            }

            m_resultGachaMsg.GachaEquipmentList.Clear();
            m_resultGachaMsg.GachaSkillList.Clear();

            for (int i = 0; i < gachaid.Count; ++i)
            {
                EquipmentData equipdata = m_equipmentLocalChart.GetEquipmentData(gachaid[i]);
                if (equipdata != null)
                { 
                    m_resultGachaMsg.GachaEquipmentList.Add(equipdata);
                    continue;
                }

                SkillData skilldata = m_skillLocalChart.GetSkillData(gachaid[i]);
                if(skilldata != null)
                    m_resultGachaMsg.GachaSkillList.Add(skilldata);
            }

            UI.IDialog.RequestDialogEnter<UI.GachaResultDialog>();

            Managers.EquipmentDataManager.Instance.AddEquipElementList(m_resultGachaMsg.GachaEquipmentList);
            Managers.SkillDataManager.Instance.AddSkillElementList(m_resultGachaMsg.GachaSkillList);

            Message.Send(m_resultGachaMsg);

            if (m_currGachaData != null)
            {
                Managers.PlayerDataManager.Instance.UseDia(m_currGachaData.Price);
                TheBackEnd.TheBackEnd.Instance.UpdateCharacterInfoTable();
                TheBackEnd.TheBackEnd.Instance.UpdateCharacterEquipmentInfoTable();
                TheBackEnd.TheBackEnd.Instance.UpdateCharacterSkillInfoTable();
                m_currGachaData = null;
            }
        }
        //------------------------------------------------------------------------------------
    }
}