using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    [Header("References")]
    public FlashlightSystem flashlightSystem;
    public Image batteryFill; // assign BatteryBarFill here

    [Header("Colors")]
    public Color fullColor = Color.green;
    public Color midColor = Color.yellow;
    public Color lowColor = Color.red;

    [Header("Thresholds")]
    [Range(0f, 1f)] public float midThreshold = 0.5f; // 50%
    [Range(0f, 1f)] public float lowThreshold = 0.2f; // 20%

    private void Update()
    {
        if (flashlightSystem == null || batteryFill == null) return;

        float percent = flashlightSystem.GetBatteryPercentage();
        batteryFill.fillAmount = percent;

        // Pick color based on thresholds
        if (percent <= lowThreshold)
            batteryFill.color = lowColor;
        else if (percent <= midThreshold)
            batteryFill.color = midColor;
        else
            batteryFill.color = fullColor;
    }
}
