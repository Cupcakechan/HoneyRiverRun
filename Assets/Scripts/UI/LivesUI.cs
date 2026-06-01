using System.Collections;
using UnityEngine;

/// Shows the honey-jar lives. Subscribes to GameManager.OnLivesChanged; the jar
/// being lost blinks a few times before disappearing.
public class LivesUI : MonoBehaviour
{
    [Tooltip("Assign left → right. Jars deplete right-to-left (last element hides first).")]
    [SerializeField] private GameObject[] jars;

    [Header("Lost-jar blink")]
    [SerializeField] private int blinkCount = 3;
    [SerializeField] private float blinkInterval = 0.1f;

    private int previousLives;
    private Coroutine[] blinks;

    private void Awake() => blinks = new Coroutine[jars.Length];

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnLivesChanged += Render;
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnLivesChanged -= Render;
    }

    private void Start()
    {
        int lives = GameManager.Instance != null ? GameManager.Instance.Lives : jars.Length;
        SetAllImmediate(lives);     // no blink on the first paint
        previousLives = lives;
    }

    /// Snap every jar to the correct state with no animation.
    private void SetAllImmediate(int lives)
    {
        for (int i = 0; i < jars.Length; i++)
            if (jars[i] != null) jars[i].SetActive(i < lives);
    }

    private void Render(int lives)
    {
        for (int i = 0; i < jars.Length; i++)
        {
            if (jars[i] == null) continue;

            // stop any blink already running on this jar
            if (blinks[i] != null) { StopCoroutine(blinks[i]); blinks[i] = null; }

            bool shouldShow = i < lives;
            bool wasShowing = i < previousLives;

            if (!shouldShow && wasShowing)
                blinks[i] = StartCoroutine(BlinkThenHide(jars[i]));   // this jar just left
            else
                jars[i].SetActive(shouldShow);                        // snap the rest
        }

        previousLives = lives;
    }

    private IEnumerator BlinkThenHide(GameObject jar)
    {
        for (int n = 0; n < blinkCount; n++)
        {
            jar.SetActive(false);
            yield return new WaitForSeconds(blinkInterval);
            jar.SetActive(true);
            yield return new WaitForSeconds(blinkInterval);
        }
        jar.SetActive(false);   // gone for good
    }
}