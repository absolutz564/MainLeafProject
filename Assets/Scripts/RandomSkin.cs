using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSkin : MonoBehaviour
{

    public SkinnedMeshRenderer Skin;

    public List<Material> SkinMaterias;

    public void Awake()
    {
        Skin.material = SkinMaterias[Random.Range(0, SkinMaterias.Count - 1)];
    }
}
