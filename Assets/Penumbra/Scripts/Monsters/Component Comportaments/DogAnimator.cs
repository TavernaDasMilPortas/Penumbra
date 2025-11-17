using UnityEngine;

public class DogAnimator : MonoBehaviour
{
    public Animator animator;

    public void Play(string clipName)
    {
        if (animator != null && !string.IsNullOrEmpty(clipName))
            animator.Play(clipName);
        // Se não tiver animador ou estado → simplesmente ignora
    }
}