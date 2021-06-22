using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameBerry.Common;

namespace GameBerry.UI
{
    public class DunjeonPharmingDialog : IDialog
    {
        [SerializeField]
        private Transform m_pharmingListRoot;

        [SerializeField]
        private float m_scaleGab;

        private UIPharmingElement m_pharmingElement;

        private ObjectPool<UIPharmingElement> m_pharmingElementPool;


    }
}