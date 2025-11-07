using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    
    void Update()
    {
        // Verifica se a tecla 'Escape' (Esc) foi pressionada
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
        pauseMenu.SetActive(true);
        
        Time.timeScale = 0f;
    }

    public void Home()
    {
       
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        
        Time.timeScale = 1f;
    }

}