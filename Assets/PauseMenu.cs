using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseMenu.activeSelf)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }

    public void Pause()
    {
        GameStateManager.Instance.SetState(InputState.Pause);
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Home()
    {
        Time.timeScale = 1f;
        GameStateManager.Instance.SetState(InputState.Menu);
        SceneManager.LoadScene("Main Menu");
    }

    public void Resume()
    {
        GameStateManager.Instance.RestorePreviousState();
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        GameStateManager.Instance.SetState(InputState.Menu);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}