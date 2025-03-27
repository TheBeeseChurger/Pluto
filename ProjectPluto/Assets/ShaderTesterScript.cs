using UnityEngine;

public class ShaderTesterScript : MonoBehaviour
{
    public Material _mat;

    public const float DEFAULT_OFFSET = 0.04f;
    public const float DEFAULT_THRESHOLD = 0.039f;

    public bool OnBool;
    public float MaxOffset;
    public float Threshold;

    void Update()
    {
        if (OnBool && _mat.GetFloat("_chrom_aberr") != 1f)
            _mat.SetFloat("_chrom_aberr", 1f);

        if (!OnBool && _mat.GetFloat("_chrom_aberr") != 0f)
            _mat.SetFloat("_chrom_aberr", 0f);

        if (_mat.GetFloat("_chrom_aberr_offset_max") != MaxOffset)
            _mat.SetFloat("_chrom_aberr_offset_max", MaxOffset);

        if (_mat.GetFloat("_chrom_aberr_threshold") != Threshold)
            _mat.SetFloat("_chrom_aberr_threshold", Threshold);
    }
}
