using UnityEngine;

public class GlitchShaderScript : MonoBehaviour
{
    [HideInInspector] public bool is_glitching;
    [HideInInspector] public float offset_glitch = 0.0166f;

    [SerializeField] private Material glitch;
    private void Start()
    {
        glitch.SetFloat("_chrom_aberr_offset_max", offset_glitch);
    }

    void Update()
    {
        float new_f = is_glitching ? 1.0f : 0.0f;

        if (glitch.GetFloat("_chrom_aberr") != new_f)  glitch.SetFloat("_chrom_aberr", new_f);
        if (glitch.GetFloat("_chrom_aberr_offset_max") != offset_glitch) glitch.SetFloat("_chrom_aberr_offset_max", offset_glitch);
    }
}
