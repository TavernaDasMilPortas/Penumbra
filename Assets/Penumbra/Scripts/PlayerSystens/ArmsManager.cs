using UnityEngine;

public class ArmsManager : MonoBehaviour
{
    public static ArmsManager Instance;

    public RightArmController rightArm;
    public LeftArmController leftArm;

    private bool armsEnabled = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ToggleLamp(true);
        RefreshVisibility();
    }

    public void EquipItem(Item newItem)
    {
        if (!armsEnabled) return;

        leftArm?.SetEquippedItem(newItem);
        RefreshVisibility();
    }

    public void ToggleLamp(bool state)
    {
        rightArm?.SetLampVisible(state);
        RefreshVisibility();
    }

    public void SetArmsEnabled(bool enabled)
    {
        armsEnabled = enabled;

        if (rightArm != null) rightArm.gameObject.SetActive(enabled);
        if (leftArm != null) leftArm.gameObject.SetActive(enabled);

        RefreshVisibility();
    }

    private void Update()
    {
        // reforço visual
        RefreshVisibility();
    }

    private void RefreshVisibility()
    {
        if (!armsEnabled) return;

        ForceEnable(leftArm?.transform);
        ForceEnable(rightArm?.transform);
    }

    private void ForceEnable(Transform root)
    {
        if (root == null) return;

        foreach (var r in root.GetComponentsInChildren<Renderer>(true))
            r.enabled = true;
    }
}
