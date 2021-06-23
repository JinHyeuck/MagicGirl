using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtLab_TestCharMove : MonoBehaviour
{
    public Camera camera;

    public Transform Character;

    public float Speed = 1.0f;

    private void Start()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        Vector3 worldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = Character.position.z;

        Vector3 dir = worldPosition - Character.position;

        Character.position += dir * Speed * Time.deltaTime;

        Vector3 rota = Character.eulerAngles;

        if (worldPosition.x - Character.position.x < 0)
            rota.y = 180.0f;
        else
            rota.y = 0.0f;

        Character.eulerAngles = rota;
    }
}
