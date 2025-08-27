using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TimerEventReference))]
public class SceneReset : MonoBehaviour
{
    private TimerEventReference timeRef;

    void Awake()
    {
        timeRef = GetComponent<TimerEventReference>();
    }

    void Start()
    {
        // Adiciona o evento ao TimerEventScheduler
        if (TimerEventScheduler.Instance != null)
        {
            TimerEventScheduler.Instance.AddEvent(timeRef.triggerSecond, ResetScene);
        }
        else
        {
            Debug.LogError("Não existe TimerEventScheduler na cena!");
        }
    }

    private void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}