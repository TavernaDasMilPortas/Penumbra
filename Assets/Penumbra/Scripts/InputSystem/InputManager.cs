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
                // Para o movimento do player
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

            case InputState.Document:
                HandleDocumentInput();
                break;
        }
    }


    private void HandleGameplayInput()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerController.Instance.HandleMovement();
        PlayerController.Instance.HandleMouseLook();
        InteractionHandler.Instance.FindInteractable();
        //FacingSystem.Instance.UpdateVisibleTargets();
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

    private void HandleDocumentInput()
    {
        if (DocumentViewer.Instance == null || DocumentViewer.Instance.CurrentDocument == null) return;
        var doc = DocumentViewer.Instance.CurrentDocument;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // Avan�ar / virar p�gina
        if (Input.GetKeyDown(KeyCode.E))
        {
            doc.ContinueReading();
        }

        // Voltar p�gina / voltar lado
        if (Input.GetKeyDown(KeyCode.Q))
        {
            doc.PreviousReading();
        }

        // Fechar documento diretamente
        if (Input.GetKeyDown(KeyCode.W))
        {
            doc.CloseReading();
        }
    }

}
