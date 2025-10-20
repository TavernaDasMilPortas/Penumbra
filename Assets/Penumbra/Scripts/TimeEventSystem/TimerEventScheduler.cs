using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TimedEvent
{
    public int triggerSecond;
    public string description;
    public UnityEvent onTrigger;

    public TimedEvent(int second, string desc = "")
    {
        triggerSecond = second;
        description = desc;
        onTrigger = new UnityEvent();
    }
}

public class TimerEventScheduler : MonoBehaviour
{
    public static TimerEventScheduler Instance { get; private set; }

    public TimedEvent[] events;
    private bool[] triggered;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        triggered = new bool[events.Length];
    }

    void OnEnable()
    {
        TimeLoopManager.OnSecondPassed += HandleSecondPassed;
    }

    void OnDisable()
    {
        TimeLoopManager.OnSecondPassed -= HandleSecondPassed;
    }

    private void HandleSecondPassed(int second)
    {
        for (int i = 0; i < events.Length; i++)
        {
            if (!triggered[i] && second == events[i].triggerSecond)
            {
                triggered[i] = true;
                events[i].onTrigger.Invoke();
            }
        }
    }

    /// <summary>
    /// Adiciona um evento ao agendador com tempo, ação e descrição opcional.
    /// </summary>
    public void AddEvent(int triggerSecond, UnityAction action, string description = "")
    {
        var list = new System.Collections.Generic.List<TimedEvent>(events);
        var newEvent = new TimedEvent(triggerSecond, description);
        newEvent.onTrigger.AddListener(action);
        list.Add(newEvent);
        events = list.ToArray();

        var triggeredList = new System.Collections.Generic.List<bool>(triggered);
        triggeredList.Add(false);
        triggered = triggeredList.ToArray();

        // 🔍 Log de depuração aprimorado
        string sourceName = action.Target != null ? action.Target.ToString() : "Objeto desconhecido";
        string methodName = action.Method != null ? action.Method.Name : "Método desconhecido";
        string descText = string.IsNullOrEmpty(description) ? "" : $" ({description})";

        Debug.Log($"🕒 Evento registrado por '{sourceName}.{methodName}' para o segundo {triggerSecond}{descText}.");
    }
}
