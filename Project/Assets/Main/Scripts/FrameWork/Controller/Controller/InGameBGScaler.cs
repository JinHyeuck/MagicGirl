using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry
{
    public class InGameBGScaler : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer m_spriteRenderer = null;

        [SerializeField]
        private float m_addXSize = 10.0f;

        private Transform m_operatorTarget = null;

        private Vector2 m_originSize = Vector2.one;

        //------------------------------------------------------------------------------------
        private void Awake()
        {
            if (m_spriteRenderer != null)
                m_originSize = m_spriteRenderer.size;
        }
        //------------------------------------------------------------------------------------
        public void SetOperationTarget(Transform target)
        {
            m_operatorTarget = target;
        }
        //------------------------------------------------------------------------------------
        void Update()
        {
            if (m_operatorTarget != null)
            {
                float operx = m_operatorTarget.position.x + m_addXSize;
                if (operx < m_originSize.x)
                { 
                    m_spriteRenderer.size = m_originSize;
                    return;
                }

                if (m_spriteRenderer.size.x < operx)
                {
                    Vector2 newscale = m_spriteRenderer.size;
                    newscale.x = operx;
                    m_spriteRenderer.size = newscale;
                }
            }
        }
        //------------------------------------------------------------------------------------
    }
}