using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// Press squish for UI buttons: scales down while held, pops back on release.
/// Uses unscaled time so it also animates while the game is paused (timeScale 0).
public class ButtonJuice : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler, ISubmitHandler
{
    [Range(0.5f, 1f)] [SerializeField] private float pressedScale = 0.9f;
    [SerializeField] private float speed = 16f;          // higher = snappier
    [SerializeField] private float submitPulse = 0.1f;   // squish time for keyboard/gamepad submit

    private Vector3 baseScale;
    private float target = 1f;   // scale multiplier on baseScale

    private void Awake() => baseScale = transform.localScale;

    private void OnEnable()
    {
        target = 1f;
        transform.localScale = baseScale;   // reset when a panel re-opens
    }

    private void Update()
    {
        Vector3 want = baseScale * target;
        float t = 1f - Mathf.Exp(-speed * Time.unscaledDeltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, want, t);
    }

    public void OnPointerDown(PointerEventData e) => target = pressedScale;
    public void OnPointerUp(PointerEventData e)   => target = 1f;

    // Keyboard/gamepad submit has no down/up, so give it a quick squish pulse.
    public void OnSubmit(BaseEventData e)
    {
        StopAllCoroutines();
        StartCoroutine(SubmitSquish());
    }

    private IEnumerator SubmitSquish()
    {
        target = pressedScale;
        float t = 0f;
        while (t < submitPulse) { t += Time.unscaledDeltaTime; yield return null; }
        target = 1f;
    }
}