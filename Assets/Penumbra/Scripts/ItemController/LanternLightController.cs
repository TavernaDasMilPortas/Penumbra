using UnityEngine;

public class LanternLightController : MonoBehaviour
{
    [Header("Referências")]
    public Light lanternLight;
    public TimeLoopManager timeLoopManager;

    [Header("Config Luz")]
    public float maxIntensity = 2f;
    public float minIntensity = 0.1f;

    private float startTime;

    void Start()
    {
        if (lanternLight == null)
            lanternLight = GetComponentInChildren<Light>();

        startTime = timeLoopManager.startTimeInSeconds;

        // escutar evento do timer
        TimeLoopManager.OnSecondPassed += UpdateLight;
    }

    private void OnDestroy()
    {
        TimeLoopManager.OnSecondPassed -= UpdateLight;
    }

    void UpdateLight(int currentSecond)
    {
        if (currentSecond <= 0)
        {
            lanternLight.intensity = 0f;
            lanternLight.enabled = false;
            return;
        }

        float t = currentSecond / startTime; // 1 → 0 conforme o tempo acaba
        lanternLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
    }
}

