using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public KeyCode Key;
    public string AniName;
}

public class ArtLab_AnimationPlayer : MonoBehaviour
{
    public Animator Animator;
    public List<PlayerData> AniPlayerList = new List<PlayerData>();

    private void Start()
    {
        Animator = Animator == null ? GetComponent<Animator>() : Animator;
    }

    void Update()
    {
        for (int i = 0; i < AniPlayerList.Count; ++i)
        {
            if (Input.GetKeyUp(AniPlayerList[i].Key))
            {
                Animator.Play(AniPlayerList[i].AniName);
            }
        }
    }
}
