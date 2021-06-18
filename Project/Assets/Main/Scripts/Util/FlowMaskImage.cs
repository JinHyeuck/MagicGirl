using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace GameBerry
{
    public class FlowMaskImage : MonoBehaviour
    {
        [SerializeField]
        private Texture MainTexture;
        [SerializeField]
        private Vector2 MainTexture_Tiling = Vector2.one;
        [SerializeField]
        private Vector2 MainTexture_Offset = Vector2.zero;

        [SerializeField]
        private Texture MaskTexture;
        [SerializeField]
        private Vector2 MaskTexture_Tiling = Vector2.one;
        [SerializeField]
        private Vector2 MaskTexture_Offset = Vector2.zero;


        private Image myImage;

        Material myMaterial = null;

        private void Start()
        {
            if (myImage != null)
                ApplyComponent();
        }

        [ContextMenu("ApplyComponent")]
        private void ApplyComponent()
        {
            myImage = GetComponent<Image>();
            if (myImage == null)
                myImage = gameObject.AddComponent<Image>();

            myImage.sprite = null;

            myMaterial = new Material(Shader.Find("39Studio/FlowMaskImage"));
            myImage.material = myMaterial;

            myMaterial.SetColor("_Color", myImage.color);

            myMaterial.SetTexture("_MainTex", MainTexture);
            myMaterial.SetTexture("_Mask", MaskTexture);

            myMaterial.SetTextureScale("_MainTex", MainTexture_Tiling);
            myMaterial.SetTextureOffset("_MainTex", MainTexture_Offset);

            myMaterial.SetTextureScale("_Mask", MaskTexture_Tiling);
            myMaterial.SetTextureOffset("_Mask", MaskTexture_Offset);
        }
    }
}
