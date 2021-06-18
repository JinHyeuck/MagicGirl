using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowBones : MonoBehaviour {

    public List<Transform> bones = new List<Transform>();

    [ContextMenu("ShowMyBones")]
    public void ShowMyBones()
    {
        bones.Clear();

        SkinnedMeshRenderer renderer = GetComponent<SkinnedMeshRenderer>();

        if (renderer == null)
            return;

        for (int i = 0; i < renderer.bones.Length; ++i)
        {
            bones.Add(renderer.bones[i]);
        }
    }
}
