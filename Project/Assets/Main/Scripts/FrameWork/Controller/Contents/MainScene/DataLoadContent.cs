using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry.Contents
{
    public class DataLoadContent : IContent
    {
        private int m_reSaveChartRequestCount = 0; // 다시 세이브 보낸 카운트
        private int m_reSaveChartResponseCount = 0; // 다시 세이브 보내서 받은 카운트
        private int m_reSaveChartSuccessCount = 0; // 다시 세이브 보내서 성공한 카운트

        private int m_localChartSaveCount = 0;

        //------------------------------------------------------------------------------------
        protected override void OnLoadStart()
        {
            Message.AddListener<GameBerry.Event.GetAllGameChartResponseMsg>(GetAllGameChartResponse);
            Message.AddListener<GameBerry.Event.GetOneChartAndSaveResponseMsg>(GetOneChartAndSaveResponse);
            Message.AddListener<GameBerry.Event.CompleteCharacterInfoTableLoadMsg>(CompletePlayerTableLoad);
            Message.AddListener<GameBerry.Event.CompleteCharacterUpGradeStatTableLoadMsg>(CompleteCharacterUpGradeStatTableLoad);
            Message.AddListener<GameBerry.Event.CompleteCharacterEquipmentInfoLoadMsg>(CompleteCharacterEquipmentInfoLoad);
            Message.AddListener<GameBerry.Event.CompleteCharacterSkillInfoLoadMsg>(CompleteCharacterSkillInfoLoad);

            StartLoadData();
        }
        //------------------------------------------------------------------------------------
        protected override void OnUnload()
        {
            Message.RemoveListener<GameBerry.Event.GetAllGameChartResponseMsg>(GetAllGameChartResponse);
            Message.RemoveListener<GameBerry.Event.GetOneChartAndSaveResponseMsg>(GetOneChartAndSaveResponse);
            Message.RemoveListener<GameBerry.Event.CompleteCharacterInfoTableLoadMsg>(CompletePlayerTableLoad);
            Message.RemoveListener<GameBerry.Event.CompleteCharacterUpGradeStatTableLoadMsg>(CompleteCharacterUpGradeStatTableLoad);
            Message.RemoveListener<GameBerry.Event.CompleteCharacterEquipmentInfoLoadMsg>(CompleteCharacterEquipmentInfoLoad);
            Message.RemoveListener<GameBerry.Event.CompleteCharacterSkillInfoLoadMsg>(CompleteCharacterSkillInfoLoad);
        }
        //------------------------------------------------------------------------------------
        private void StartLoadData()
        {
            TheBackEnd.TheBackEnd.Instance.GetAllChartList();
            TheBackEnd.TheBackEnd.Instance.GetProbabilityCardList();
            //TheBackEnd.TheBackEnd.Instance.GetProbabilitys();
        }
        //------------------------------------------------------------------------------------
        private void GetAllGameChartResponse(GameBerry.Event.GetAllGameChartResponseMsg msg)
        {
            if (msg.IsSuccess == true)
                CheckSaveLocalChart();
            else
                Debug.Log("차트가져오기 삑사리났다");
        }
        //------------------------------------------------------------------------------------
        private void CheckSaveLocalChart()
        {
            foreach (KeyValuePair<string, string> pair in TheBackEnd.TheBackEnd_GameChart.TableChartFileld)
            {
                if (string.IsNullOrEmpty(pair.Value) == false)
                {
                    // 원래 FileID와 맞지 않다면 최신화
                    string originfileid = PlayerPrefs.GetString(pair.Key, string.Empty);
                    if (originfileid != pair.Value)
                    {
                        TheBackEnd.TheBackEnd.Instance.GetOneChartAndSave(pair.Key);
                        m_reSaveChartRequestCount++;
                    }
                }
            }

            if (m_reSaveChartRequestCount == 0)
            {
                //차트는 끝난거다
                SetLocalClientTable();
            }
        }
        //------------------------------------------------------------------------------------
        private void GetOneChartAndSaveResponse(GameBerry.Event.GetOneChartAndSaveResponseMsg msg)
        {
            if (msg.IsSuccess == true)
            {
                m_reSaveChartSuccessCount++;
            }

            m_reSaveChartResponseCount++;

            if (m_reSaveChartRequestCount == m_reSaveChartResponseCount)
            {
                if (m_reSaveChartRequestCount == m_reSaveChartSuccessCount)
                {
                    // 차트 저장 다 끝남
                    SetLocalClientTable();
                }
                else
                {
                    if (m_localChartSaveCount > 0)
                    {
                        Debug.Log("서버차트 다시 갱신했지만 안됐으니 그냥 이전에 있던 테이블로 사용해라 이전에 있던 차트도 안되면 어쩔수없지뭐");
                        SetLocalClientTable();
                        return;
                    }

                    SaveLocalChartFileId();

                    m_reSaveChartRequestCount = 0;
                    m_reSaveChartResponseCount = 0;
                    m_reSaveChartSuccessCount = 0;

                    CheckSaveLocalChart();
                }
            }
        }
        //------------------------------------------------------------------------------------
        private void SaveLocalChartFileId()
        {
            // 최신FileID로 로컬차트를 가져왔더니 없으면 저장이 안된거다
            foreach (KeyValuePair<string, string> pair in TheBackEnd.TheBackEnd_GameChart.TableChartFileld)
            {
                if (string.IsNullOrEmpty(TheBackEnd.TheBackEnd.Instance.GetLocalChartData(pair.Key)) == true)
                {
                    Debug.Log(string.Format("{0} is SaveFail!", pair.Key));
                }
                else
                {
                    PlayerPrefs.SetString(pair.Key, pair.Value);
                }
            }
        }
        //------------------------------------------------------------------------------------
        private void SetLocalClientTable()
        {
            SaveLocalChartFileId();

            StartCoroutine(LoadLocalClientTable());
        }
        //------------------------------------------------------------------------------------
        private IEnumerator LoadLocalClientTable()
        {
            yield return StartCoroutine(Managers.TableManager.Instance.Load());

            TheBackEnd.TheBackEnd.Instance.GetCharacterInfoTableData();
        }
        //------------------------------------------------------------------------------------
        private void CompletePlayerTableLoad(GameBerry.Event.CompleteCharacterInfoTableLoadMsg msg)
        {
            TheBackEnd.TheBackEnd.Instance.GetCharacterUpGradeStatTableData();
        }
        //------------------------------------------------------------------------------------
        private void CompleteCharacterUpGradeStatTableLoad(GameBerry.Event.CompleteCharacterUpGradeStatTableLoadMsg msg)
        {
            TheBackEnd.TheBackEnd.Instance.GetCharacterEquipmentInfoTableData();
        }
        //------------------------------------------------------------------------------------
        private void CompleteCharacterEquipmentInfoLoad(GameBerry.Event.CompleteCharacterEquipmentInfoLoadMsg msg)
        {
            //SetLoadComplete();
            TheBackEnd.TheBackEnd.Instance.GetCharacterSkillinfoTableData();
        }
        //------------------------------------------------------------------------------------
        private void CompleteCharacterSkillInfoLoad(GameBerry.Event.CompleteCharacterSkillInfoLoadMsg msg)
        {
            SetLoadComplete();
        }
        //------------------------------------------------------------------------------------
        protected override void OnEnter()
        {

        }
        //------------------------------------------------------------------------------------
        protected override void OnExit()
        {

        }
        //------------------------------------------------------------------------------------
    }
}