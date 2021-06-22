using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

namespace GameBerry.TheBackEnd
{
    public static class TheBackEnd_PlayerTable
    {
        public static string CharInfoInData = string.Empty;

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
        public static void GetTableData()
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

                Debug.LogError(data.Count == 0 ? "기본테이블에 플레이어정보 없음" : "플레이어정보있음");

                if(data.Count == 0)
                {
                    InsertDefaultPlayerInfoTable();
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
                                PlayerDataContainer.Exp = ((int)data[i][key]);
                            }
                            else if (key == Define.PlayerGold)
                            {
                                PlayerDataContainer.Gold = ((int)data[i][key]);
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

                    Message.Send(new Event.CompletePlayerTableLoadMsg());
                }
            });
        }
        //------------------------------------------------------------------------------------
        public static void InsertDefaultPlayerInfoTable()
        {
            Param param = new Param();
            param.Add(Define.PlayerLevel, 1);
            param.Add(Define.PlayerExp, 0);
            param.Add(Define.PlayerGold, 0);
            param.Add(Define.PlayerDia, 0);
            param.Add(Define.PlayerEquipmentSton, 0);
            param.Add(Define.PlayerSkillSton, 0);

            Debug.Log("InsertTable()");

            SendQueue.Enqueue(Backend.GameData.Insert, Define.CharacterInfoTable, param, (callback) =>
            {
                Debug.Log(string.Format("Insert Succcess : {0}, statusCode : {1}", callback.IsSuccess(), callback.GetStatusCode()));

                if (callback.IsSuccess() == true)
                {
                    Debug.Log(callback.GetReturnValue());
                    GetTableData();
                }
                else
                {
                }
            });
        }
        //------------------------------------------------------------------------------------
        public static void PlayerTableUpdate()
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
    }
}