using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    [SerializeField] private string houseSceneName = "HouseIdle";

    void Start()
    {
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        
        SceneManager.LoadScene(houseSceneName, LoadSceneMode.Additive);
    }

    public void PlayGame()
    {
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        
        SceneManager.LoadSceneAsync("MainGame");
    }
    public void OpenShowcase()
    {
        // Aqui você decide: Se o showcase for interativo (tipo gameplay), trave o cursor.
        // Se for apenas visual (tipo museu com mouse), deixe o cursor livre.

        // Exemplo: Deixando cursor livre para clicar em coisas no showcase
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("Showcase");
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}