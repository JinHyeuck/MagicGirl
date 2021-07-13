using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBerry.UI;

namespace GameBerry.Contents
{
    public class EquipmentContent : IContent
    {
        //------------------------------------------------------------------------------------
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.E))
            {
                EquipmentLocalChart chart = Managers.TableManager.Instance.GetTableClass<EquipmentLocalChart>();

                List<EquipmentData> data = new List<EquipmentData>();
                for (int i = 0; i < 20; ++i)
                {
                    List<EquipmentData> listdata = chart.GetEquipmentDataList((EquipmentType)Random.Range(0, 3));
                    data.Add(listdata[Random.Range(0, listdata.Count)]);
                }

                Managers.PlayerDataManager.Instance.AddEquipElementList(data);
            }
        }
        //------------------------------------------------------------------------------------
    }
}