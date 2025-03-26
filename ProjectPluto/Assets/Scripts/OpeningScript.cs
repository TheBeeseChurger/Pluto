using System;
using System.Collections.Generic;
using UnityEngine;

public class OpeningScript : MonoBehaviour
{
    [SerializeField] private List<Credit> credits;

    [SerializeField] private float start_up_time;
    [SerializeField] private float time_between_credits;

    public async void RunCredits()
    {
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

        Debug.Log("Finished Credits");
    }

    private async Awaitable FadeIn(Credit credit, float time)
    {
        float curr_time = 0f;
        
        while (curr_time < time)
        {
            float alpha = Mathf.Lerp(credit.cover_reference.color.a, 0f, curr_time / time);

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
            float alpha = Mathf.Lerp(credit.cover_reference.color.a, 1f, curr_time / time);

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
