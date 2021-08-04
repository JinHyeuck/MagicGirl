using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBerry.Contents;
using GameBerry.UI;

namespace GameBerry.Managers
{
    public class HPMPVarianceManager : MonoSingleton<HPMPVarianceManager>
    {
        private Dictionary<HpMpVarianceType, VarianceColor> m_varianceColor_Dic = new Dictionary<HpMpVarianceType, VarianceColor>();

        private Queue<UIHpMpVarianceText> m_variancePool = new Queue<UIHpMpVarianceText>();

        private UIHpMpVarianceText m_varianceObj;

        [SerializeField]
        private float m_varianceDirectionPos = 30.0f;

        [SerializeField]
        private float m_varianceDirectionDuration = 0.5f;

        [SerializeField]
        private float m_varianceDelayDuration = 0.5f;

        //------------------------------------------------------------------------------------
        public void InitVariance(List<VarianceColor> variancecolor)
        {
            for (int i = 0; i < variancecolor.Count; ++i)
            {
                m_varianceColor_Dic.Add(variancecolor[i].VarianceType, variancecolor[i]);
            }

            AssetBundleLoader.Instance.Load<GameObject>("ContentResources/DunjeonContent", "UIHpMpVarianceText", o =>
            {
                GameObject Obj = o as GameObject;
                if (Obj != null)
                    m_varianceObj = Obj.GetComponent<UIHpMpVarianceText>();
            });

            for (int i = 0; i < 10; ++i)
            {
                CreateVarianceText();
            }
        }
        //------------------------------------------------------------------------------------
        private void CreateVarianceText()
        {
            if (m_varianceObj == null)
                return;

            GameObject clone = Instantiate(m_varianceObj.gameObject, transform);
            if (clone != null)
            { 
                m_variancePool.Enqueue(clone.GetComponent<UIHpMpVarianceText>());
                clone.SetActive(false);
            }
        }
        //------------------------------------------------------------------------------------
        public void ShowVarianceText(HpMpVarianceType type, double variancevalue, Vector3 worldpos)
        {
            if (m_variancePool.Count <= 0)
                CreateVarianceText();

            UIHpMpVarianceText variance = m_variancePool.Dequeue();

            if (variance == null)
                return;

            string text = string.Empty;
            if (type == HpMpVarianceType.Miss)
                text = "Miss";
            else
                text = string.Format("{0 : 0}", variancevalue);

            Color color = Color.white;

            if (m_varianceColor_Dic.ContainsKey(type) == true)
            {
                VarianceColor colordata = null;

                if (m_varianceColor_Dic.TryGetValue(type, out colordata) == true)
                {
                    if (colordata != null)
                        color = colordata.color;
                }
            }

            variance.gameObject.SetActive(true);
            variance.transform.position = worldpos;
            variance.ResetVariance();
            variance.ShowVarianceText(text, color, m_varianceDirectionPos, m_varianceDirectionDuration, m_varianceDelayDuration);
        }
        //------------------------------------------------------------------------------------
        public void PoolVarianceText(UIHpMpVarianceText varianceText)
        {
            if (varianceText == null)
                return;

            varianceText.gameObject.SetActive(false);
            m_variancePool.Enqueue(varianceText);
        }
        //------------------------------------------------------------------------------------
    }
}