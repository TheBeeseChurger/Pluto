using UnityEngine;

[ExecuteInEditMode]
public class CamShaderScript : MonoBehaviour
{
    public Material material;

    void Start()
    {
        material = new Material(Shader.Find("Custom/CRT"));
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetTexture("_MainTex", source);
        Graphics.Blit(source, destination, material);
    }

}
