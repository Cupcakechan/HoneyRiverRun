using UnityEngine;

/// Shows the honey-jar lives. Subscribes to GameManager.OnLivesChanged and
/// shows one jar per remaining life.
public class LivesUI : MonoBehaviour
{
    [Tooltip("Assign left → right. Jars deplete right-to-left (last element hides first).")]
    [SerializeField] private GameObject[] jars;

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
        if (GameManager.Instance != null)
            Render(GameManager.Instance.Lives);
    }

    private void Render(int lives)
    {
        for (int i = 0; i < jars.Length; i++)
            if (jars[i] != null)
                jars[i].SetActive(i < lives);
    }
}