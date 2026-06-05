using UnityEngine;

/// Persistent audio hub. Plays one looping music track and layers one-shot SFX.
/// Created in MainMenu (like GameManager) and survives scene loads.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;   // looping background music
    [SerializeField] private AudioSource sfxSource;     // one-shots via PlayOneShot

    [Header("Volumes")]
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 0.6f;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource != null) { musicSource.loop = true; musicSource.volume = musicVolume; }
        if (sfxSource != null)   { sfxSource.loop = false; sfxSource.volume = sfxVolume; }
    }

    /// Play a looping music track. No-op if that exact track is already playing,
    /// so bouncing between scenes that share a track won't restart it.
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null) musicSource.Stop();
    }

    /// Fire a one-shot SFX. Overlaps cleanly (rapid darts, multiple hits).
    public void PlaySfx(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }
}