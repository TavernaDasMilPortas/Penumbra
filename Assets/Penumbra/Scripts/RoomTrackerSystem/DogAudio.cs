using UnityEngine;

public class DogAudio : MonoBehaviour
{
    [Header("Audio Sources (opcionais)")]
    public AudioSource barkSource;     // latido ao avistar
    public AudioSource growlSource;    // rosnado ao preparar investida
    public AudioSource chargeSource;   // respiração/avanço
    public AudioSource eatingSource;   // som ao comer

    [Header("Clipes (opcionais)")]
    public AudioClip barkClip;
    public AudioClip growlClip;
    public AudioClip chargeClip;
    public AudioClip eatingClip;

    public void PlayBark()
    {
        if (barkSource && barkClip)
            barkSource.PlayOneShot(barkClip);
    }

    public void PlayGrowl()
    {
        if (growlSource && growlClip)
            growlSource.PlayOneShot(growlClip);
    }

    public void PlayCharge()
    {
        if (chargeSource && chargeClip)
            chargeSource.PlayOneShot(chargeClip);
    }

    public void PlayEating()
    {
        if (eatingSource && eatingClip)
            eatingSource.PlayOneShot(eatingClip);
    }
}
