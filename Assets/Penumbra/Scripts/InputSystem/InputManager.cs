using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private InputState lastState;

    private void Update()
    {
        if (lastState != GameStateManager.Instance.CurrentState)
        {
            // Estado mudou
            if (GameStateManager.Instance.CurrentState != InputState.Gameplay)
            {
                // N�o � gameplay, para o movimento do player
                if (PlayerController.Instance != null)
                    PlayerController.Instance.Stop();
            }
        }

        lastState = GameStateManager.Instance.CurrentState;

        switch (GameStateManager.Instance.CurrentState)
        {
            case InputState.Gameplay:
                HandleGameplayInput();
                break;

            case InputState.Menu:
                HandleMenuInput();
                break;

        }
    }

    private void HandleGameplayInput()
    {

       PlayerController.Instance.HandleMovement();
       PlayerController.Instance.HandleMouseLook();
        // Abre o menu principal (com abas como invent�rio, mapa etc)
        if (Input.GetKeyDown(KeyCode.I))
        {
           //MenuManager.Instance?.ToggleMainMenu();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayerController.Instance.Interagir();
        }

    }

    private void HandleMenuInput()
    {

        // Navega��o entre itens (dentro do menu atual)
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Navegando para direita");
            MenuManager.Instance?.Navigate(Vector2.right);
        } 
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Navegando para esquerda");
            MenuManager.Instance?.Navigate(Vector2.left);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("Navegando para baixo");
            MenuManager.Instance?.Navigate(Vector2.down);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("Navegando para cima");
            MenuManager.Instance?.Navigate(Vector2.up);

        }
        // Navega��o entre abas (ex: Invent�rio -> Mapa)
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Navengando entre abas para direita");
            MenuManager.Instance?.NavigateTabs(1);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Navegando entre abas para esquerda");
            MenuManager.Instance?.NavigateTabs(-1);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Confirmando a��o");
            MenuManager.Instance?.Confirm();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Fechando o Menu");
            MenuManager.Instance?.Cancel();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Abrindo/Fechando menu");
            MenuManager.Instance?.ToggleMainMenu();
        }

    }


}
