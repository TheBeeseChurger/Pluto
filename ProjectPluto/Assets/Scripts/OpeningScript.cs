using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OpeningScript : MonoBehaviour
{
    [SerializeField] private List<Credit> credits;

    [SerializeField] private float start_up_time;
    [SerializeField] private float time_between_credits;

    private static GameObject audio_head;
    private AudioSource ambience;

    [Header("Audio")]
    [SerializeField] AudioResource pre_start;

    [ContextMenu("Test Credits")]
    public async Awaitable RunCredits(GameObject new_audio)
    {
        audio_head = new_audio;
        ambience = audio_head.transform.GetChild(0).GetComponent<AudioSource>();

        ambience.resource = pre_start;
        ambience.volume = 0.5f;
        ambience.Play();

        await Awaitable.WaitForSecondsAsync(start_up_time);

        foreach (var credit in credits)
        {
            credit.head.SetActive(true);

            await FadeIn(credit, credit.fade_in_time);
            await Awaitable.WaitForSecondsAsync(credit.credit_time);
            await FadeOut(credit, credit.fade_out_time);

            credit.head.SetActive(false);

            await Awaitable.WaitForSecondsAsync(time_between_credits);
        }
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
    public float credit_time;
    public float fade_in_time;
    public float fade_out_time;
}
