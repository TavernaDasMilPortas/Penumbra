using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public InputState CurrentState { get; private set; } = InputState.Gameplay;
    public InputState PreviousState { get; private set; } = InputState.Gameplay;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void SetState(InputState newState)
    {
        if (newState != CurrentState)
        {
            PreviousState = CurrentState;
            CurrentState = newState;
        }
    }

    public void RestorePreviousState()
    {
        InputState temp = CurrentState;
        CurrentState = PreviousState;
        PreviousState = temp;
    }

    public void ResetToGameplay()
    {
        PreviousState = CurrentState;
        CurrentState = InputState.Gameplay;
    }
}
