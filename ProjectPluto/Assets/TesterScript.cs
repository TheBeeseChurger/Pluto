using System;
using UnityEngine;

public class TesterScript : MonoBehaviour
{
    public GameObject tested_prefab;

    private GeneratorScript gen;

    void Start()
    {
        gen = Instantiate(tested_prefab).GetComponent<GeneratorScript>();

        gen.Generate();
    }
}
