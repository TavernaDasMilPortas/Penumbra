using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton: garante que só exista uma instância
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // remove duplicatas
            return;
        }

        Instance = this;

        // Evita destruição ao trocar de cena
        DontDestroyOnLoad(gameObject);
    }
}
