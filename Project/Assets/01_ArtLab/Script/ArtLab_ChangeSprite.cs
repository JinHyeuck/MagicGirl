using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

public class ArtLab_ChangeSprite : MonoBehaviour
{
    public SpriteResolver resolver;

    public string Category;
    public string Label;

    public string OriginCategory;
    public string OriginLabel;

    private void Start()
    {
        if (resolver == null)
            resolver = transform.GetComponent<SpriteResolver>();

        OriginCategory = resolver.GetCategory();
        OriginLabel = resolver.GetLabel();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            resolver.SetCategoryAndLabel(Category, Label);
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            resolver.SetCategoryAndLabel(OriginCategory, OriginLabel);
        }
    }
}
