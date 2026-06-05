using UnityEngine;

/// Tells the AudioManager which track to play for the scene it lives in.
public class MusicCue : MonoBehaviour
{
    [SerializeField] private AudioClip track;

    private void Start()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic(track);
    }
}