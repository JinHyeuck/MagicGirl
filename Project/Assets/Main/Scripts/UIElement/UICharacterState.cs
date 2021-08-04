using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterState : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer m_charHPBer;

    [SerializeField]
    private SpriteRenderer m_charMPBer;

    private float m_hpDefaultWidth;
    private float m_mpDefaultWidth;

    //------------------------------------------------------------------------------------
    public void Init()
    {
        if (m_charHPBer != null)
            m_hpDefaultWidth = m_charHPBer.size.x;

        if (m_charMPBer != null)
            m_mpDefaultWidth = m_charMPBer.size.x;
    }
    //------------------------------------------------------------------------------------
    public void SetHPBar(double ratio)
    {
        if (m_charHPBer == null)
            return;

        Vector2 size = m_charHPBer.size;

        if (ratio <= 0.0f)
        {
            size.x = 0.0f;
            m_charHPBer.size = size;
        }
        else
        {
            size.x = m_hpDefaultWidth * (float)ratio;
            m_charHPBer.size = size;
        }
    }
    //------------------------------------------------------------------------------------
    public void SetMPBar(double ratio)
    {
        if (m_charMPBer == null)
            return;

        Vector2 size = m_charMPBer.size;

        if (ratio <= 0.0f)
        {
            size.x = 0.0f;
            m_charMPBer.size = size;
        }
        else
        {
            size.x = m_mpDefaultWidth * (float)ratio;
            m_charMPBer.size = size;
        }
    }
    //------------------------------------------------------------------------------------
}
