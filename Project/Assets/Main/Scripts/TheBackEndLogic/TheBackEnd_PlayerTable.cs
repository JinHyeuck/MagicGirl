using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

namespace GameBerry.TheBackEnd
{
    public static class TheBackEnd_PlayerTable
    {
        public static string CharInfoInData = string.Empty;
        public static string CharUpGradeStatInData = string.Empty;
        public static string CharEquipmentInfoInData = string.Empty;
        public static string CharSkillInfoInData = string.Empty;

        //------------------------------------------------------------------------------------
        public static void GetTableList()
        {
            SendQueue.Enqueue(Backend.GameData.GetTableList, (callback) =>
            {
                // 이후 처리
                Debug.Log(callback.GetReturnValue());

                if (callback.IsSuccess() == true)
                {
                    var data = callback.GetReturnValuetoJSON();

                    for (int i = 0; i < data["tables"].Count; ++i)
                    {
                        string returnValue = string.Empty;
                        foreach (var key in data["tables"][i].Keys)
                        {
                            returnValue += string.Format("{0} : {1} / ", key, data["tables"][i][key].ToString());
                        }
                        Debug.Log(returnValue);
                    }
                }
            });
        }
        //------------------------------------------------------------------------------------
        #region TheBackEnd_CharacterInfo
        //------------------------------------------------------------------------------------
        public static void GetCharacterInfoTableData()
        {
            SendQueue.Enqueue(Backend.GameData.Get, Define.CharacterInfoTable, new Where(), 10, (bro) =>
            {
                if (bro.IsSuccess() == false)
                {
                    Debug.Log(bro.GetStatusCode());
                    Debug.Log(bro.GetErrorCode());
                    Debug.Log(bro.GetMessage());
                    return;
                }

                var data = bro.FlattenRows();

                Debug.LogError(data.Count == 0 ? "CharacterInfo테이블에 아무것도 없음" : "CharacterInfo테이블에 정보 있음");

                if(data.Count == 0)
                {
                    InsertCharacterInfoTable();
                }
                else
                {
                    for (int i = 0; i < data.Count; ++i)
                    {
                        string returnValue = string.Empty;
                        foreach (var key in data[i].Keys)
                        {
                            if (key == "inDate")
                            {
                                CharInfoInData = data[i][key].ToString();
                            }
                            else if (key == Define.PlayerLevel)
                            {
                                PlayerDataContainer.Level = ((int)data[i][key]);
                            }
                            else if (key == Define.PlayerExp)
                            {
                                PlayerDataContainer.Exp = (System.Convert.ToDouble(data[i][key].ToString()));
                            }
                            else if (key == Define.PlayerGold)
                            {
                                PlayerDataContainer.Gold = (System.Convert.ToDouble(data[i][key].ToString()));
                            }
                            else if (key == Define.PlayerDia)
                            {
                                PlayerDataContainer.Dia = ((int)data[i][key]);
                            }
                            else if (key == Define.PlayerEquipmentSton)
                            {
                                PlayerDataContainer.EquipmentSton = ((int)data[i][key]);
                            }
                            else if (key == Define.PlayerSkillSton)
                            {
                                PlayerDataContainer.SkillSton = ((int)data[i][key]);
                            }
                            else if (key == Define.PlayerEquipID)
                            {
                                string str = data[i][key].ToString();
                                LitJson.JsonData chartJson = LitJson.JsonMapper.ToObject(str);

                                for (int j = 0; j < (int)EquipmentType.Max; ++j)
                                {
                                    EquipmentType type = (EquipmentType)j;
                                    int equipID = (int)chartJson[type.ToString()];
                                    Managers.EquipmentDataManager.Instance.AddEquipID(type, equipID);
                                }
                            }

                            returnValue += string.Format("{0} : {1} / ", key, data[i][key].ToString());
                        }

                        Debug.Log(returnValue);
                    }

                    Message.Send(new Event.CompleteCharacterInfoTableLoadMsg());
                }
            });
        }
        //------------------------------------------------------------------------------------
        public static void GetCharacterInfoTableDataTest(System.Action OnComplete)
        {
            SendQueue.Enqueue(Backend.GameData.Get, Define.CharacterInfoTable, new Where(), 10, (bro) =>
            {
                if (bro.IsSuccess() == false)
                {
                    Debug.Log(bro.GetStatusCode());
                    Debug.Log(bro.GetErrorCode());  

                    Debug.Log(bro.GetMessage());
                    return;
                }

                var data = bro.FlattenRows();

                Debug.LogError(data.Count == 0 ? "CharacterInfo테이블에 아무것도 없음" : "CharacterInfo테이블에 정보 있음");

                if (data.Count == 0)
                {
                    InsertCharacterInfoTable();
                }
                else
                {
                    for (int i = 0; i < data.Count; ++i)
                    {
                        string returnValue = string.Empty;
                        foreach (var key in data[i].Keys)
                        {
                            if (key == Define.PlayerDia)
                            {
                                Managers.PlayerDataManager.Instance.SetDia(((int)data[i][key]));
                            }

                            returnValue += string.Format("{0} : {1} / ", key, data[i][key].ToString());
                        }

                        Debug.Log(returnValue);
                    }

                    if (OnComplete != null)
                        OnComplete();
                }
            });
        }
        //------------------------------------------------------------------------------------
        private static void InsertCharacterInfoTable()
        {
            Dictionary<EquipmentType, int> equipInsertData = new Dictionary<EquipmentType, int>();
            for (int i = 0; i < (int)EquipmentType.Max; ++i)
            {
                equipInsertData.Add((EquipmentType)i, -1);
            }

            string equipstr = LitJson.JsonMapper.ToJson(equipInsertData);

            Param param = new Param();
            param.Add(Define.PlayerLevel, 1);
            param.Add(Define.PlayerExp, 0);
            param.Add(Define.PlayerGold, 0);
            param.Add(Define.PlayerDia, 0);
            param.Add(Define.PlayerEquipmentSton, 0);
            param.Add(Define.PlayerSkillSton, 0);
            param.Add(Define.PlayerEquipID, equipstr);

            Debug.Log("InsertCharacterInfoTable()");

            SendQueue.Enqueue(Backend.GameData.Insert, Define.CharacterInfoTable, param, (callback) =>
            {
                Debug.Log(string.Format("InsertCharacterInfoTable Succcess : {0}, statusCode : {1}", callback.IsSuccess(), callback.GetStatusCode()));

                if (callback.IsSuccess() == true)
                {
                    Debug.Log(callback.GetReturnValue());
                    GetCharacterInfoTableData();
                }
                else
                {
                }
            });
        }
        //------------------------------------------------------------------------------------
        public static void UpdateCharacterInfoTable()
        {
            Param param = new Param();
            param.Add(Define.PlayerLevel, PlayerDataContainer.Level);
            param.Add(Define.PlayerExp, PlayerDataContainer.Exp);
            param.Add(Define.PlayerGold, PlayerDataContainer.Gold);
            param.Add(Define.PlayerDia, PlayerDataContainer.Dia);
            param.Add(Define.PlayerEquipmentSton, PlayerDataContainer.EquipmentSton);
            param.Add(Define.PlayerSkillSton, PlayerDataContainer.SkillSton);
            param.Add(Define.PlayerEquipID, LitJson.JsonMapper.ToJson(EquipmentDataContainer.m_equipId));

            SendQueue.Enqueue(Backend.GameData.Update, Define.CharacterInfoTable, CharInfoInData, param, (callback) =>
            {
                Debug.Log(string.Format("TableUpdate : {0}", callback.IsSuccess()));

                if (callback.IsSuccess() == false)
                {
                    Debug.LogError(string.Format("{0}\n{1}", callback.GetErrorCode(), callback.GetMessage()));
                }
            });
        }
        //------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------
        #region TheBackEnd_CharacterUpGradeStat
        //------------------------------------------------------------------------------------
        public static void GetCharacterUpGradeStatTableData()
        {
            SendQueue.Enqueue(Backend.GameData.Get, Define.CharacterUpGradeStatTable, new Where(), 10, (bro) =>
            {
                if (bro.IsSuccess() == false)
                {
                    Debug.Log(bro.GetStatusCode());
                    Debug.Log(bro.GetErrorCode());
                    Debug.Log(bro.GetMessage());
                    return;
                }

                var data = bro.FlattenRows();

                Debug.LogError(data.Count == 0 ? "CharacterUpGradeStat테이블에 아무것도 없음" : "CharacterUpGradeStat테이블에 정보 있음");

                if (data.Count == 0)
                {
                    InsertCharacterUpGradeStatTable();
                }
                else
                {
                    for (int i = 0; i < data.Count; ++i)
                    {
                        string returnValue = string.Empty;
                        foreach (var key in data[i].Keys)
                        {
                            if (key == "inDate")
                            {
                                CharUpGradeStatInData = data[i][key].ToString();
                            }
                            else if (key == Define.PlayerUpGradeAddDamage)
                            {
                                Managers.StatUpGradeDataManager.Instance.AddUpGradeStatLevel(StatType.BaseDamage, (int)data[i][key]);
                            }
                            else if (key == Define.PlayerUpGradeCriticalDamage)
                            {
                                Managers.StatUpGradeDataManager.Instance.AddUpGradeStatLevel(StatType.CriticalDamage, (int)data[i][key]);
                            }
                            else if (key == Define.PlayerUpGradeCriticalPer)
                            {
                                Managers.StatUpGradeDataManager.Instance.AddUpGradeStatLevel(StatType.CriticalPer, (int)data[i][key]);
                            }
                            else if (key == Define.PlayerUpGradeAddGold)
                            {
                                Managers.StatUpGradeDataManager.Instance.AddUpGradeStatLevel(StatType.AddGoldPer, (int)data[i][key]);
                            }
                            else if (key == Define.PlayerUpGradeMP)
                            {
                                Managers.StatUpGradeDataManager.Instance.AddUpGradeStatLevel(StatType.MPBase, (int)data[i][key]);
                            }
                            else if (key == Define.PlayerUpGradeMPRecovery)
                            {
                                Managers.StatUpGradeDataManager.Instance.AddUpGradeStatLevel(StatType.MPRecoveryBase, (int)data[i][key]);
                            }
                            else if (key == Define.PlayerUpGradeAttackSpeed)
                            {
                                Managers.StatUpGradeDataManager.Instance.AddUpGradeStatLevel(StatType.AttackSpeed, (int)data[i][key]);
                            }
                            else if (key == Define.PlayerUpGradeMoveSpeed)
                            {
                                Managers.StatUpGradeDataManager.Instance.AddUpGradeStatLevel(StatType.MoveSpeed, (int)data[i][key]);
                            }

                            returnValue += string.Format("{0} : {1} / ", key, data[i][key].ToString());
                        }

                        Debug.Log(returnValue);
                    }

                    Message.Send(new Event.CompleteCharacterUpGradeStatTableLoadMsg());
                }
            });
        }
        //------------------------------------------------------------------------------------
        private static void InsertCharacterUpGradeStatTable()
        {
            Param param = new Param();
            param.Add(Define.PlayerUpGradeAddDamage, 0);
            param.Add(Define.PlayerUpGradeCriticalDamage, 0);
            param.Add(Define.PlayerUpGradeCriticalPer, 0);
            param.Add(Define.PlayerUpGradeAddGold, 0);
            param.Add(Define.PlayerUpGradeMP, 0);
            param.Add(Define.PlayerUpGradeMPRecovery, 0);
            param.Add(Define.PlayerUpGradeAttackSpeed, 0);
            param.Add(Define.PlayerUpGradeMoveSpeed, 0);

            Debug.Log("InsertCharacterUpGradeStatTable()");

            SendQueue.Enqueue(Backend.GameData.Insert, Define.CharacterUpGradeStatTable, param, (callback) =>
            {
                Debug.Log(string.Format("InsertCharacterUpGradeStatTable Succcess : {0}, statusCode : {1}", callback.IsSuccess(), callback.GetStatusCode()));

                if (callback.IsSuccess() == true)
                {
                    Debug.Log(callback.GetReturnValue());
                    GetCharacterUpGradeStatTableData();
                }
                else
                {
                }
            });
        }
        //------------------------------------------------------------------------------------
        public static void UpdateCharacterUpGradeStatTable()
        {
            Param param = new Param();
            param.Add(Define.PlayerUpGradeAddDamage, StatUpGradeDataContainer.m_upGradeStatLevel[StatType.BaseDamage]);
            param.Add(Define.PlayerUpGradeCriticalDamage, StatUpGradeDataContainer.m_upGradeStatLevel[StatType.CriticalDamage]);
            param.Add(Define.PlayerUpGradeCriticalPer, StatUpGradeDataContainer.m_upGradeStatLevel[StatType.CriticalPer]);
            param.Add(Define.PlayerUpGradeAddGold, StatUpGradeDataContainer.m_upGradeStatLevel[StatType.AddGoldPer]);
            param.Add(Define.PlayerUpGradeMP, StatUpGradeDataContainer.m_upGradeStatLevel[StatType.MPBase]);
            param.Add(Define.PlayerUpGradeMPRecovery, StatUpGradeDataContainer.m_upGradeStatLevel[StatType.MPRecoveryBase]);
            param.Add(Define.PlayerUpGradeAttackSpeed, StatUpGradeDataContainer.m_upGradeStatLevel[StatType.AttackSpeed]);
            param.Add(Define.PlayerUpGradeMoveSpeed, StatUpGradeDataContainer.m_upGradeStatLevel[StatType.MoveSpeed]);

            SendQueue.Enqueue(Backend.GameData.Update, Define.CharacterUpGradeStatTable, CharUpGradeStatInData, param, (callback) =>
            {
                Debug.Log(string.Format("TableUpdate : {0}", callback.IsSuccess()));

                if (callback.IsSuccess() == false)
                {
                    Debug.LogError(string.Format("{0}\n{1}", callback.GetErrorCode(), callback.GetMessage()));
                }
            });
        }
        //------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------
        #region TheBackEnd_EquipmentInfo
        //------------------------------------------------------------------------------------
        public static void GetCharacterEquipmentInfoTableData()
        {
            SendQueue.Enqueue(Backend.GameData.Get, Define.CharacterEquipmentInfoTable, new Where(), 10, (bro) =>
            {
                if (bro.IsSuccess() == false)
                {
                    Debug.Log(bro.GetStatusCode());
                    Debug.Log(bro.GetErrorCode());
                    Debug.Log(bro.GetMessage());
                    return;
                }

                var data = bro.FlattenRows();

                Debug.LogError(data.Count == 0 ? "CharacterEquipmentInfo테이블에 아무것도 없음" : "CharacterEquipmentInfo테이블에 정보 있음");

                if (data.Count == 0)
                {
                    InsertCharacterEquipmentInfo();
                }
                else
                {
                    for (int i = 0; i < data.Count; ++i)
                    {
                        string returnValue = string.Empty;
                        foreach (var key in data[i].Keys)
                        {
                            if (key == "inDate")
                            {
                                CharEquipmentInfoInData = data[i][key].ToString();
                            }
                            else if (key == Define.CharacterEquipmentInfoTable)
                            {
                                string str = data[i][key].ToString();
                                LitJson.JsonData chartJson = LitJson.JsonMapper.ToObject(str);

                                for (int j = 0; j < (int)EquipmentType.Max; ++j)
                                {
                                    var raw = chartJson[((EquipmentType)j).ToString()];

                                    Dictionary<int, PlayerEquipmentInfo> tempDic = new Dictionary<int, PlayerEquipmentInfo>();

                                    for (int k = 0; k < raw.Count; ++k)
                                    {
                                        PlayerEquipmentInfo equipdata = null;

                                        if (raw[k] != null)
                                        {
                                            var rawelement = raw[k];

                                            equipdata = new PlayerEquipmentInfo
                                            {
                                                Id = rawelement["Id"].ToString().FastStringToInt(),

                                                Count = rawelement["Count"].ToString().FastStringToInt(),

                                                Level = rawelement["Level"].ToString().FastStringToInt()
                                            };
                                        }

                                        tempDic.Add(equipdata.Id, equipdata);
                                    }

                                    Managers.EquipmentDataManager.Instance.AddEquipmentInfo((EquipmentType)j, tempDic);
                                }
                            }

                            returnValue += string.Format("{0} : {1} / ", key, data[i][key].ToString());
                        }

                        Debug.Log(returnValue);
                    }

                    Message.Send(new Event.CompleteCharacterEquipmentInfoLoadMsg());
                }
            });
        }
        //------------------------------------------------------------------------------------
        private static void InsertCharacterEquipmentInfo()
        {
            Dictionary<EquipmentType, Dictionary<int, PlayerEquipmentInfo>> equipmentinfo = new Dictionary<EquipmentType, Dictionary<int, PlayerEquipmentInfo>>();
            for (int i = 0; i < (int)EquipmentType.Max; ++i)
            {
                equipmentinfo.Add((EquipmentType)i, new Dictionary<int, PlayerEquipmentInfo>());
            }

            string str = LitJson.JsonMapper.ToJson(equipmentinfo);

            Param param = new Param();
            param.Add(Define.CharacterEquipmentInfoTable, str);

            Debug.Log("InsertCharacterEquipmentInfo()");

            SendQueue.Enqueue(Backend.GameData.Insert, Define.CharacterEquipmentInfoTable, param, (callback) =>
            {
                Debug.Log(string.Format("InsertCharacterEquipmentInfo Succcess : {0}, statusCode : {1}", callback.IsSuccess(), callback.GetStatusCode()));

                if (callback.IsSuccess() == true)
                {
                    Debug.Log(callback.GetReturnValue());
                    GetCharacterEquipmentInfoTableData();
                }
                else
                {
                }
            });
        }
        //------------------------------------------------------------------------------------
        public static void UpdateCharacterEquipmentInfoTable()
        {
            Param param = new Param();
            param.Add(Define.CharacterEquipmentInfoTable, LitJson.JsonMapper.ToJson(EquipmentDataContainer.m_equipmentInfo));

            SendQueue.Enqueue(Backend.GameData.Update, Define.CharacterEquipmentInfoTable, CharEquipmentInfoInData, param, (callback) =>
            {
                Debug.Log(string.Format("TableUpdate : {0}", callback.IsSuccess()));

                if (callback.IsSuccess() == false)
                {
                    Debug.LogError(string.Format("{0}\n{1}", callback.GetErrorCode(), callback.GetMessage()));
                }
            });
        }
        //------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------
        #region TheBackEnd_CharacterSkillInfo
        //------------------------------------------------------------------------------------
        public static void GetCharacterSkillinfoTableData()
        {
            SendQueue.Enqueue(Backend.GameData.Get, Define.CharacterSkillInfoTable, new Where(), 10, (bro) =>
            {
                if (bro.IsSuccess() == false)
                {
                    Debug.Log(bro.GetStatusCode());
                    Debug.Log(bro.GetErrorCode());
                    Debug.Log(bro.GetMessage());
                    return;
                }

                var data = bro.FlattenRows();

                Debug.LogError(data.Count == 0 ? "CharacterSkillInfo테이블에 아무것도 없음" : "CharacterSkillInfo테이블에 정보 있음");

                if (data.Count == 0)
                {
                    InsertCharacterSkillInfoTable();
                }
                else
                {
                    for (int i = 0; i < data.Count; ++i)
                    {
                        string returnValue = string.Empty;

                        var safd = data[i];

                        foreach (var key in data[i].Keys)
                        {
                            if (key == "inDate")
                            {
                                CharSkillInfoInData = data[i][key].ToString();
                            }
                            else if (key == Define.PlayerSkillInfo)
                            {
                                string str = data[i][key].ToString();
                                LitJson.JsonData chartJson = LitJson.JsonMapper.ToObject(str);
                                Debug.Log(str);

                                for (int j = 0; j < chartJson.Count; ++j)
                                {
                                    PlayerSkillInfo info = new PlayerSkillInfo();

                                    if (chartJson[j] != null)
                                    {
                                        var rawelement = chartJson[j];

                                        info.Id = rawelement["Id"].ToString().FastStringToInt();

                                        info.Count = rawelement["Count"].ToString().FastStringToInt();

                                        info.Level = rawelement["Level"].ToString().FastStringToInt();
                                    }

                                    SkillDataContainer.m_skillInfo.Add(info.Id, info);
                                }
                            }
                            else if (key == Define.PlayerSkillSlotInfo)
                            {
                                string str = data[i][key].ToString();
                                LitJson.JsonData chartJson = LitJson.JsonMapper.ToObject(str);
                                SetSkillSlotData(chartJson);
                            }
                            else if (key == Define.PlayerSkillSlotPage)
                            {
                                SkillDataContainer.SkillSlotPage = data[i][key].ToString().FastStringToInt();
                            }

                            returnValue += string.Format("{0} : {1} / ", key, data[i][key].ToString());
                        }

                        Debug.Log(returnValue);
                    }

                    Message.Send(new Event.CompleteCharacterSkillInfoLoadMsg());
                }
            });
        }
        //------------------------------------------------------------------------------------
        private static void SetSkillSlotData(LitJson.JsonData slotdata)
        {

            foreach (var key in slotdata.Keys)
            {
                Dictionary<int, int> element = new Dictionary<int, int>();
                foreach (var elementkey in slotdata[key].Keys)
                {
                    element.Add(elementkey.ToInt(), slotdata[key][elementkey].ToString().ToInt());
                }

                if (SkillDataContainer.CurrentOpenSlotCount < element.Count)
                    SkillDataContainer.CurrentOpenSlotCount = element.Count;

                SkillDataContainer.m_skillSlotData.Add(key.ToString().ToInt(), element);
            }

            foreach (KeyValuePair<int, Dictionary<int, int>> pair in SkillDataContainer.m_skillSlotData)
            {
                while (pair.Value.Count < SkillDataContainer.CurrentOpenSlotCount)
                {
                    pair.Value.Add(pair.Value.Count, -1);
                }
            }
        }
        //------------------------------------------------------------------------------------
        private static void InsertCharacterSkillInfoTable()
        {
            Dictionary<int, Dictionary<int, int>> skillSlotInsertData = new Dictionary<int, Dictionary<int, int>>();
            Dictionary<int, int> pagevalue = new Dictionary<int, int>();
            pagevalue.Add(0, -1);
            pagevalue.Add(1, -1);

            skillSlotInsertData.Add(0, pagevalue);
            skillSlotInsertData.Add(1, pagevalue);
            skillSlotInsertData.Add(2, pagevalue);


            Param param = new Param();
            param.Add(Define.PlayerSkillInfo, LitJson.JsonMapper.ToJson(SkillDataContainer.m_skillInfo));
            param.Add(Define.PlayerSkillSlotInfo, LitJson.JsonMapper.ToJson(skillSlotInsertData));
            param.Add(Define.PlayerSkillSlotPage, 0);

            Debug.Log("InsertCharacterSkillInfoTable()");

            SendQueue.Enqueue(Backend.GameData.Insert, Define.CharacterSkillInfoTable, param, (callback) =>
            {
                Debug.Log(string.Format("InsertCharacterSkillInfoTable Succcess : {0}, statusCode : {1}", callback.IsSuccess(), callback.GetStatusCode()));

                if (callback.IsSuccess() == true)
                {
                    Debug.Log(callback.GetReturnValue());
                    GetCharacterSkillinfoTableData();
                }
                else
                {
                }
            });
        }
        //------------------------------------------------------------------------------------
        public static void UpdateCharacterSkillInfoTable()
        {
            Param param = new Param();
            param.Add(Define.PlayerSkillInfo, LitJson.JsonMapper.ToJson(SkillDataContainer.m_skillInfo));
            param.Add(Define.PlayerSkillSlotInfo, LitJson.JsonMapper.ToJson(SkillDataContainer.m_skillSlotData));
            param.Add(Define.PlayerSkillSlotPage, SkillDataContainer.SkillSlotPage);

            SendQueue.Enqueue(Backend.GameData.Update, Define.CharacterSkillInfoTable, CharSkillInfoInData, param, (callback) =>
            {
                Debug.Log(string.Format("TableUpdate : {0}", callback.IsSuccess()));

                if (callback.IsSuccess() == false)
                {
                    Debug.LogError(string.Format("{0}\n{1}", callback.GetErrorCode(), callback.GetMessage()));
                }
            });
        }
        //------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------
    }
}