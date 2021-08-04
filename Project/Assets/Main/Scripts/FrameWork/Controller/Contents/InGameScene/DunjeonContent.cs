using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBerry.UI;

namespace GameBerry.Contents
{
    [System.Serializable]
    public class VarianceColor
    {
        public HpMpVarianceType VarianceType = HpMpVarianceType.None;
        public Color color;
    }

    public class DunjeonContent : IContent
    {
        [SerializeField]
        private List<VarianceColor> m_varianceColor_List = new List<VarianceColor>();

        //------------------------------------------------------------------------------------
        protected override void OnLoadComplete()
        {
            Managers.DunjeonManager manager = Managers.DunjeonManager.Instance;
            Managers.HPMPVarianceManager.Instance.InitVariance(m_varianceColor_List);
        }
        //------------------------------------------------------------------------------------
        protected override void OnEnter()
        {
            IDialog.RequestDialogEnter<DunjeonPharmingDialog>();
        }
        //------------------------------------------------------------------------------------
    }
}