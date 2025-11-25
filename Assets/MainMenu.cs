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

    public void QuitGame()
    {
        Application.Quit();
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}