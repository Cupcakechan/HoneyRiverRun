using UnityEngine;
using UnityEngine.UI;

/// Drives the honey-fuel bar from HoneyTank events: sets the fill amount and
/// pulses toward Warning Red when fuel is low.
public class FuelBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HoneyTank tank;     // Captain Bumble's tank
    [SerializeField] private Image fillImage;    // Image Type = Filled (Horizontal, Left)

    [Header("Colors")]
    [SerializeField] private Color normalColor  = Color.white;                       // shows the fill art as-is
    [SerializeField] private Color lowFuelColor = new Color(0.910f, 0.294f, 0.235f); // Warning Red #E84B3C

    [Header("Low-Fuel Pulse")]
    [SerializeField] private float pulseSpeed = 6f;

    private bool isLow;

    private void OnEnable()
    {
        if (tank != null) tank.OnFuelChanged += HandleFuelChanged;
    }
    private void OnDisable()
    {
        if (tank != null) tank.OnFuelChanged -= HandleFuelChanged;
    }

    private void Start()
    {
        if (tank != null) HandleFuelChanged(tank.Fraction, tank.IsLow);
    }

    private void HandleFuelChanged(float fraction, bool low)
    {
        if (fillImage != null) fillImage.fillAmount = fraction;
        isLow = low;
        if (!low && fillImage != null) fillImage.color = normalColor;
    }

    private void Update()
    {
        if (!isLow || fillImage == null) return;
        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
        fillImage.color = Color.Lerp(lowFuelColor, normalColor, t);
    }
}