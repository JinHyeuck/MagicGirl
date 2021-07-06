using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtLab_Depth : MonoBehaviour
{
    public Transform m_target;
    public SpriteRenderer m_renderer;

    public int m_defaultDepth = 0;
    public int m_changeDepth = 10;
    public float changealpha = 127.0f;

    // Update is called once per frame
    void Update()
    {
        if (m_target.transform.position.y > transform.position.y)
        { 
            m_renderer.sortingOrder = m_changeDepth;
            Color color = m_renderer.color;
            color.a = changealpha / 255.0f;
            m_renderer.color = color;
        }
        else
        { 
            m_renderer.sortingOrder = m_defaultDepth;
            Color color = m_renderer.color;
            color.a = 255.0f;
            m_renderer.color = color;
        }
    }
}
