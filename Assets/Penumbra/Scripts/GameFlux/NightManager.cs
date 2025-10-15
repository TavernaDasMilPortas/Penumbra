using UnityEngine;
using UnityEngine.SceneManagement;

public class NightManager : MonoBehaviour
{
    [Header("Configuração das Noites")]
    public NightData[] nights;
    public int currentNightIndex = 0;

    [Header("Tentativas")]
    public AttemptManager attemptManager;

    private TaskManager taskManager = new TaskManager();

    public static NightManager Instance { get; private set; }

    public NightData CurrentNight => nights[currentNightIndex];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        LoadCurrentNightTasks();
        UpdateUI();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Invoke(nameof(UpdateUI), 0.1f);
    }

    private void LoadCurrentNightTasks()
    {
        taskManager.LoadFromNightData(CurrentNight);
    }

    public void CompleteTask(string taskName)
    {
        if (taskManager.CompleteTask(taskName))
        {
            UpdateUI();

            if (taskManager.AreAllTasksComplete())
                Debug.Log($"🌙 Todas as tasks da {CurrentNight.nightName} concluídas!");
        }
    }

    public void OnPlayerDeath()
    {
        bool completed = taskManager.AreAllTasksComplete();

        if (completed)
        {
            currentNightIndex++;
            if (currentNightIndex >= nights.Length)
            {
                currentNightIndex = nights.Length - 1;
                Debug.Log("⚡ Todas as noites concluídas!");
            }

            // tentativas persistem entre noites
            LoadCurrentNightTasks(); // carrega tasks da nova noite
        }
        else
        {
            attemptManager.AddAttempt();
            if (attemptManager.HasExceededMaxAttempts())
            {
                currentNightIndex = 0;
                LoadCurrentNightTasks();
                attemptManager.ResetAttempts();
                Debug.Log("🔄 Tentativas esgotadas, voltando para a Noite 1!");
            }
        }

        ReloadScene();
    }

    private void ReloadScene()
    {
        ScenePreparator.PrepareNextScene();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void UpdateUI()
    {
        UIManagerNight ui = FindObjectOfType<UIManagerNight>();
        if (ui)
        {
            ui.UpdateNightText(currentNightIndex + 1);
            ui.UpdateAttemptText(attemptManager.CurrentAttempt, attemptManager.MaxAttempts);
            ui.UpdateTasks(taskManager.activeTasks);
        }
    }
}
