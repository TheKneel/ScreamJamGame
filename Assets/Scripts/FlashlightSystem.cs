using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightSystem : MonoBehaviour
{
    [Header("References")]
    public Light flashlight; // assign in inspector (child of playerCamera)

    [Header("Battery Settings")]
    public float maxBattery = 100f;
    public float drainRate = 5f;   // % drained per minute
    public float rechargeRate = 10f; // % recharge per second (optional if you want pickups instead)

    private float currentBattery;
    private bool isOn = false;

    private void Awake()
    {
        currentBattery = maxBattery;
        if (flashlight != null)
            flashlight.enabled = false; // start off
    }

    private void Update()
    {
        if (isOn && currentBattery > 0f)
        {
            currentBattery -= drainRate * Time.deltaTime;
            if (currentBattery <= 0f)
            {
                currentBattery = 0f;
                ToggleFlashlight(false);
            }
        }

        // Dim flashlight intensity based on battery % (even if battery is low but > 0)
        if (flashlight != null)
        {
            float percent = currentBattery / maxBattery;

            // Example: intensity goes from 3 (full) to 0 (dead)
            flashlight.intensity = Mathf.Lerp(0f, 3f, percent);

            // Optional: range goes from 5 (dead) to 20 (full)
            flashlight.range = Mathf.Lerp(5f, 20f, percent);
        }
    }

    public void OnFlashlight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentBattery > 0f)
                ToggleFlashlight(!isOn);
        }
    }

    private void ToggleFlashlight(bool state)
    {
        isOn = state;
        if (flashlight != null)
            flashlight.enabled = isOn;
    }

    // Optional: Call this when picking up batteries
    public void Recharge(float amount)
    {
        currentBattery = Mathf.Clamp(currentBattery + amount, 0f, maxBattery);
    }

    public float GetBatteryPercentage()
    {
        return currentBattery / maxBattery;
    }
}
