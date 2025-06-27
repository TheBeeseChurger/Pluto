using UnityEngine;

public class ShaderTesterScript : MonoBehaviour
{
    public Material _mat;

    public const float DEFAULT_OFFSET = 0f;

    public float MaxOffset;

    void Update()
    {
        if (_mat.GetFloat("_chrom_aberr_offset_max") != MaxOffset)
            _mat.SetFloat("_chrom_aberr_offset_max", MaxOffset);
    }
}
