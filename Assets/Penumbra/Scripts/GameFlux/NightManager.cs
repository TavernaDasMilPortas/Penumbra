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
        // 🔹 Agora a primeira noite também prepara a cena e seus spawns
        //ScenePreparator.PrepareScene();

        LoadCurrentNightTasks();
        UpdateUI();
    }
    private void Update()
    {
        if (taskManager != null)
            taskManager.UpdateTimedTasks(Time.deltaTime);
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 🔹 Garante que ao mudar de cena, os spawns da noite atual sejam criados
        ScenePreparator.PrepareScene();
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

            LoadCurrentNightTasks();
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
        ScenePreparator.PrepareScene();
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
