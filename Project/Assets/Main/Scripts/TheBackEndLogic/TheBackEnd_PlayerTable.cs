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
                // ���� ó��
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

                Debug.LogError(data.Count == 0 ? "CharacterInfo���̺� �ƹ��͵� ����" : "CharacterInfo���̺� ���� ����");

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
                                    PlayerDataContainer.m_equipId.Add(type, equipID);
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
            param.Add(Define.PlayerEquipID, LitJson.JsonMapper.ToJson(PlayerDataContainer.m_equipId));

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

                Debug.LogError(data.Count == 0 ? "CharacterUpGradeStat���̺� �ƹ��͵� ����" : "CharacterUpGradeStat���̺� ���� ����");

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
                                PlayerDataContainer.m_upGradeStatLevel.Add(StatUpGradeType.AddDamage, (int)data[i][key]);
                            }
                            else if (key == Define.PlayerUpGradeCriticalDamage)
                            {
                                PlayerDataContainer.m_upGradeStatLevel.Add(StatUpGradeType.CriticalDamage, (int)data[i][key]);
                            }
                            else if (key == Define.PlayerUpGradeCriticalPer)
                            {
                                PlayerDataContainer.m_upGradeStatLevel.Add(StatUpGradeType.CriticalPercent, (int)data[i][key]);
                            }
                            else if (key == Define.PlayerUpGradeAddGold)
                            {
                                PlayerDataContainer.m_upGradeStatLevel.Add(StatUpGradeType.AddGold, (int)data[i][key]);
                            }
                            else if (key == Define.PlayerUpGradeMP)
                            {
                                PlayerDataContainer.m_upGradeStatLevel.Add(StatUpGradeType.AddMP, (int)data[i][key]);
                            }
                            else if (key == Define.PlayerUpGradeMPRecovery)
                            {
                                PlayerDataContainer.m_upGradeStatLevel.Add(StatUpGradeType.AddMpRecovery, (int)data[i][key]);
                            }
                            else if (key == Define.PlayerUpGradeCastingSpeed)
                            {
                                PlayerDataContainer.m_upGradeStatLevel.Add(StatUpGradeType.CastingSpeed, (int)data[i][key]);
                            }
                            else if (key == Define.PlayerUpGradeMoveSpeed)
                            {
                                PlayerDataContainer.m_upGradeStatLevel.Add(StatUpGradeType.MoveSpeed, (int)data[i][key]);
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
            param.Add(Define.PlayerUpGradeCastingSpeed, 0);
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
            param.Add(Define.PlayerUpGradeAddDamage, PlayerDataContainer.m_upGradeStatLevel[StatUpGradeType.AddDamage]);
            param.Add(Define.PlayerUpGradeCriticalDamage, PlayerDataContainer.m_upGradeStatLevel[StatUpGradeType.CriticalDamage]);
            param.Add(Define.PlayerUpGradeCriticalPer, PlayerDataContainer.m_upGradeStatLevel[StatUpGradeType.CriticalPercent]);
            param.Add(Define.PlayerUpGradeAddGold, PlayerDataContainer.m_upGradeStatLevel[StatUpGradeType.AddGold]);
            param.Add(Define.PlayerUpGradeMP, PlayerDataContainer.m_upGradeStatLevel[StatUpGradeType.AddMP]);
            param.Add(Define.PlayerUpGradeMPRecovery, PlayerDataContainer.m_upGradeStatLevel[StatUpGradeType.AddMpRecovery]);
            param.Add(Define.PlayerUpGradeCastingSpeed, PlayerDataContainer.m_upGradeStatLevel[StatUpGradeType.CastingSpeed]);
            param.Add(Define.PlayerUpGradeMoveSpeed, PlayerDataContainer.m_upGradeStatLevel[StatUpGradeType.MoveSpeed]);

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

                Debug.LogError(data.Count == 0 ? "CharacterEquipmentInfo���̺� �ƹ��͵� ����" : "CharacterEquipmentInfo���̺� ���� ����");

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

                                    PlayerDataContainer.m_equipmentInfo.Add((EquipmentType)j, tempDic);
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
            param.Add(Define.CharacterEquipmentInfoTable, LitJson.JsonMapper.ToJson(PlayerDataContainer.m_equipmentInfo));

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

                Debug.LogError(data.Count == 0 ? "CharacterSkillInfo���̺� �ƹ��͵� ����" : "CharacterSkillInfo���̺� ���� ����");

                if (data.Count == 0)
                {
                    InsertCharacterSkillInfoTable();
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
                                CharSkillInfoInData = data[i][key].ToString();
                            }
                            else if (key == Define.CharacterSkillInfoTable)
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
                                }
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
        private static void InsertCharacterSkillInfoTable()
        {
            Dictionary<string, PlayerSkillInfo> skillInsertData = new Dictionary<string, PlayerSkillInfo>();

            //for (int i = 0; i < 10; ++i)
            //{
            //    skillInsertData.Add(i.ToString(), new PlayerSkillInfo
            //    {
            //        Id = i,
            //        Count = i,
            //        Level = i,
            //    });
            //}

            string equipstr = LitJson.JsonMapper.ToJson(skillInsertData);

            Param param = new Param();
            param.Add(Define.CharacterSkillInfoTable, equipstr);

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
            param.Add(Define.CharacterSkillInfoTable, PlayerDataContainer.m_skillInfo);


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