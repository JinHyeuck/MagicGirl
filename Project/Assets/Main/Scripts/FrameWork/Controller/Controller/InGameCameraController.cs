using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry
{
    public class InGameCameraController : MonoBehaviour
    {
        private Transform m_flowTarget;
        private bool m_useSmooth = false;
        private float m_smoothSpeed = 1.0f;

        //------------------------------------------------------------------------------------
        public void SetFlowTarget(Transform flowtarget)
        {
            m_flowTarget = flowtarget;
        }
        //------------------------------------------------------------------------------------
        public void UseSmooth(bool usesmooth, float smoothspeed)
        {
            m_useSmooth = usesmooth;
            m_smoothSpeed = smoothspeed;
        }
        //------------------------------------------------------------------------------------
        private void LateUpdate()
        {
            if (m_flowTarget != null)
            {
                Vector3 pos = transform.position;
                pos.x = m_flowTarget.transform.position.x;
                transform.transform.position = pos;
            }
        }
        //------------------------------------------------------------------------------------
    }
}