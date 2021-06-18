using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry
{
    public class FlowImage : MonoBehaviour
    {
        [SerializeField]
        private Texture MainTexture;
        [SerializeField]
        private Vector2 MainTexture_Tiling = Vector2.one;
        [SerializeField]
        private Vector2 MainTexture_Offset = Vector2.zero;
        [SerializeField]
        private float MoveSpeedX = 0.0f;
        [SerializeField]
        private float MoveSpeedY = 0.0f;

        private Image myImage;

        Material myMaterial = null;

        private void Start()
        {
            ApplyComponent();
        }

        [ContextMenu("ApplyComponent")]
        private void ApplyComponent()
        {
            myImage = GetComponent<Image>();
            if (myImage == null)
                myImage = gameObject.AddComponent<Image>();

            //myImage.sprite = null;

            myMaterial = myImage.material;
            if (myMaterial == null)
            {
                myMaterial = new Material(Shader.Find("39Studio/FlowImage"));
                myImage.material = myMaterial;
            }
            else
            {
                myMaterial = new Material(myMaterial);
                myImage.material = myMaterial;
            }

            myMaterial.SetColor("_Color", myImage.color);

            myMaterial.SetTexture("_MainTex", MainTexture);

            myMaterial.SetTextureScale("_MainTex", MainTexture_Tiling);
            myMaterial.SetTextureOffset("_MainTex", MainTexture_Offset);

            myMaterial.SetFloat("_MainSpeedX", MoveSpeedX);
            myMaterial.SetFloat("_MainSpeedY", MoveSpeedY);
        }
    }
}