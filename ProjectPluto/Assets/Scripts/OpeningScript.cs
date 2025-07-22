using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OpeningScript : MonoBehaviour
{
    [Header("Glitch")]
    [SerializeField] private Material glitch_mat;

    [Header("Credits")]
    [SerializeField] private List<Credit> credits;

    [SerializeField] private float start_up_time;
    [SerializeField] private float time_between_credits;

    private static GameObject audio_head;
    private AudioSource ambience;
    private AudioSource song;

    [Header("Audio")]
    [SerializeField] AudioResource pre_start;

    [Header("Test Resources")]
    [SerializeField] GameObject test_head;

    [ContextMenu("Test Credits")]
    public async void TestCredits()
    {
        var test_audio = Instantiate(test_head);
        await RunCredits(test_audio);
        DestroyImmediate(test_audio);
    }

    public async Awaitable RunCredits(GameObject new_audio)
    {
        audio_head = new_audio;
        ambience = audio_head.transform.GetChild(0).GetComponent<AudioSource>();
        song = audio_head.transform.GetChild(1).GetComponent<AudioSource>();

        ambience.resource = pre_start;
        ambience.volume = 0.5f;
        ambience.Play();

        song.volume = 0.15f;


        await Awaitable.WaitForSecondsAsync(start_up_time);

        glitch_mat.SetFloat("_chrom_aberr", 1f);
        glitch_mat.SetFloat("_chrom_aberr_offset_max", 0f);
        Invoke(nameof(GlitchEffect), 2f);

        foreach (var credit in credits)
        {
            credit.head.SetActive(true);

            if (credit.has_custom_audio)
            {
                song.resource = credit.custom_audio;
                song.Play();
            }
            
            await FadeIn(credit, credit.fade_in_time);
            await Awaitable.WaitForSecondsAsync(credit.credit_time);
            await FadeOut(credit, credit.fade_out_time);

            credit.head.SetActive(false);

            await Awaitable.WaitForSecondsAsync(time_between_credits);
        }
        glitch_mat.SetFloat("_chrom_aberr_offset_max", 0f);
        glitch_mat.SetFloat("_chrom_aberr", 0f);
    }

    private async void GlitchEffect()
    {
        if (this == null) return;

        glitch_mat.SetFloat("_chrom_aberr_offset_max", 0.0166f);
        await Awaitable.WaitForSecondsAsync(UnityEngine.Random.Range(0.2f, 1.0f));
        glitch_mat.SetFloat("_chrom_aberr_offset_max", 0f);

        if (this != null) Invoke(nameof(GlitchEffect), UnityEngine.Random.Range(0.5f, 1.5f));
    }

    private async Awaitable FadeIn(Credit credit, float time)
    {
        float curr_time = 0f;
        
        while (curr_time < time)
        {
            float alpha = Mathf.Lerp(1f, 0f, curr_time / time);

            credit.cover_reference.color = new Color(credit.cover_reference.color.r, credit.cover_reference.color.g, credit.cover_reference.color.b, alpha);

            await Awaitable.NextFrameAsync();
            curr_time += Time.deltaTime;
        }

        credit.cover_reference.color = new Color(credit.cover_reference.color.r, credit.cover_reference.color.g, credit.cover_reference.color.b, 0f);
    }

    private async Awaitable FadeOut(Credit credit, float time)
    {
        float curr_time = 0f;

        while (curr_time < time)
        {
            float alpha = Mathf.Lerp(0f, 1f, curr_time / time);

            credit.cover_reference.color = new Color(credit.cover_reference.color.r, credit.cover_reference.color.g, credit.cover_reference.color.b, alpha);

            await Awaitable.NextFrameAsync();
            curr_time += Time.deltaTime;
        }

        credit.cover_reference.color = new Color(credit.cover_reference.color.r, credit.cover_reference.color.g, credit.cover_reference.color.b, 1f);
    }
}

[Serializable]
public class Credit
{
    public GameObject head;
    public SpriteRenderer cover_reference;

    public bool has_custom_audio;

    [Header("Credit Audio")]
    public AudioResource custom_audio;

    [Header("Credit Timedata")]
    public float credit_time;
    public float fade_in_time;
    public float fade_out_time;
}
