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
        gen.PaintCell(gen.GetCell(0, 0), MazeCellScript.WallColor.red, 1.0f);
        gen.PaintCell(gen.GetCell(0, 1), MazeCellScript.WallColor.red, 0.5f);
        gen.PaintCell(gen.GetCell(0, 2), MazeCellScript.WallColor.red, 0.1f);
        gen.PaintCell(gen.GetCell(0, 3), MazeCellScript.WallColor.red, 0.0f);
    }
}
