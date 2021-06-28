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
            Param param = new Param();
            param.Add(Define.PlayerLevel, 1);
            param.Add(Define.PlayerExp, 0);
            param.Add(Define.PlayerGold, 0);
            param.Add(Define.PlayerDia, 0);
            param.Add(Define.PlayerEquipmentSton, 0);
            param.Add(Define.PlayerSkillSton, 0);

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

            SendQueue.Enqueue(Backend.GameData.Update, Define.CharacterInfoTable, CharInfoInData, param, (callback) =>
            {
                Debug.Log(string.Format("TableUpdate : {0}", callback.IsSuccess()));
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
                            //else if (key == Define.PlayerUpGradeDamage)
                            //{
                            //    PlayerDataContainer.UpGradeStat_AddDamage = ((int)data[i][key]);
                            //}
                            //else if (key == Define.PlayerUpGradeCriticalDamage)
                            //{
                            //    PlayerDataContainer.UpGradeStat_CriticalDamage = ((int)data[i][key]);
                            //}
                            //else if (key == Define.PlayerUpGradeCriticalPer)
                            //{
                            //    PlayerDataContainer.UpGradeStat_CriticalPercent = ((int)data[i][key]);
                            //}
                            //else if (key == Define.PlayerUpGradeAddGold)
                            //{
                            //    PlayerDataContainer.UpGradeStat_AddGold = ((int)data[i][key]);
                            //}
                            //else if (key == Define.PlayerUpGradeMP)
                            //{
                            //    PlayerDataContainer.UpGradeStat_MP = ((int)data[i][key]);
                            //}
                            //else if (key == Define.PlayerUpGradeMPRecovery)
                            //{
                            //    PlayerDataContainer.UpGradeStat_MPRecovery = ((int)data[i][key]);
                            //}
                            //else if (key == Define.PlayerUpGradeCastingSpeed)
                            //{
                            //    PlayerDataContainer.UpGradeStat_CastingSpeed = ((int)data[i][key]);
                            //}
                            //else if (key == Define.PlayerUpGradeMoveSpeed)
                            //{
                            //    PlayerDataContainer.UpGradeStat_MoveSpeed = ((int)data[i][key]);
                            //}

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
            });
        }
        //------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------
    }
}