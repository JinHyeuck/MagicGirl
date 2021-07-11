using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBerry.UI;

namespace GameBerry.Contents
{
    [System.Serializable]
    public class EquipmentGradeColor
    {
        public EquipmentGradeType GradeType;
        public Color GradeColor;
    }

    public class EquipmentContent : IContent
    {
        [SerializeField]
        private List<EquipmentGradeColor> m_gradleColorList = new List<EquipmentGradeColor>();

        private static Dictionary<EquipmentGradeType, Color> m_gradleColor_Dic = new Dictionary<EquipmentGradeType, Color>();

        //------------------------------------------------------------------------------------
        protected override void OnLoadStart()
        {
            for (int i = 0; i < m_gradleColorList.Count; ++i)
            {
                if (m_gradleColor_Dic.ContainsKey(m_gradleColorList[i].GradeType) == false)
                {
                    m_gradleColor_Dic.Add(m_gradleColorList[i].GradeType, m_gradleColorList[i].GradeColor);
                }
            }

            SetLoadComplete();
        }
        //------------------------------------------------------------------------------------
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.D))
            {
                IDialog.RequestDialogEnter<EquipmentDialog>();
            }
            else if (Input.GetKeyUp(KeyCode.F))
            {
                IDialog.RequestDialogExit<EquipmentDialog>();
            }
            else if (Input.GetKeyUp(KeyCode.G))
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
        public static Color GetGradeColor(EquipmentGradeType gradetype)
        {
            Color color = Color.white;
            m_gradleColor_Dic.TryGetValue(gradetype, out color);

            return color;
        }
        //------------------------------------------------------------------------------------
    }
}