using UnityEngine;

public class GameManager : MonoBehaviour
{

    private void Awake()
    { 

        // Evita destruição ao trocar de cena
        DontDestroyOnLoad(gameObject);
    }
}
