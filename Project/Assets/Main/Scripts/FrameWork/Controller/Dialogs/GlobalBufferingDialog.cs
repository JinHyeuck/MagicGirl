using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry.UI
{
    public class GlobalBufferingDialog : IDialog
    {
        [SerializeField]
        private int m_loadingBarAmount = 10;

        [SerializeField]
        private float m_speed = 10.0f;

        [SerializeField]
        private Transform m_loadingCircleGroup = null;

        //------------------------------------------------------------------------------------
        private void Update()
        {
            if(m_loadingCircleGroup != null)
            {
                Vector3 rotate = m_loadingCircleGroup.eulerAngles;
                rotate.z += m_speed * Time.deltaTime;
                m_loadingCircleGroup.eulerAngles = rotate;
            }
        }
        //------------------------------------------------------------------------------------
    }
}