using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TimedEvent
{
    public int triggerSecond;
    public UnityEvent onTrigger;

    public TimedEvent(int second)
    {
        triggerSecond = second;
        onTrigger = new UnityEvent();
    }
}

public class TimerEventScheduler : MonoBehaviour
{
    // Singleton
    public static TimerEventScheduler Instance { get; private set; }

    public TimedEvent[] events;
    private bool[] triggered;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // opcional, se quiser persistir entre cenas
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

    // Adiciona evento em runtime
    public void AddEvent(int triggerSecond, UnityAction action)
    {
        var list = new System.Collections.Generic.List<TimedEvent>(events);
        var newEvent = new TimedEvent(triggerSecond);
        newEvent.onTrigger.AddListener(action);
        list.Add(newEvent);
        events = list.ToArray();

        var triggeredList = new System.Collections.Generic.List<bool>(triggered);
        triggeredList.Add(false);
        triggered = triggeredList.ToArray();
    }
}
