using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry.Contents
{
    public class SkillContent : IContent
    {
        //------------------------------------------------------------------------------------
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.S))
            {
                SkillLocalChart chart = Managers.TableManager.Instance.GetTableClass<SkillLocalChart>();

                List<SkillData> data = new List<SkillData>();
                for (int i = 0; i < 20; ++i)
                {
                    data.Add(chart.m_SkillDatas[Random.Range(0, chart.m_SkillDatas.Count)]);
                }

                Managers.PlayerDataManager.Instance.AddSkillList(data);
            }
        }
        //------------------------------------------------------------------------------------
    }
}