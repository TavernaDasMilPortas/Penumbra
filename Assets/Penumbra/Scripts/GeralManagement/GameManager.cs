using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton: garante que s� exista uma inst�ncia
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // remove duplicatas
            return;
        }

        Instance = this;

        // Evita destrui��o ao trocar de cena
        DontDestroyOnLoad(gameObject);
    }
}
