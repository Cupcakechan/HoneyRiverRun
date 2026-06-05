using UnityEngine;

/// Persistent audio hub. Plays one looping music track and layers one-shot SFX.
/// Created in MainMenu (like GameManager) and survives scene loads.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Volumes")]
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 0.6f;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;

    [Header("UI SFX")]
    [SerializeField] private AudioClip uiHover;
    [SerializeField] private AudioClip uiClick;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource != null) { musicSource.loop = true; musicSource.volume = musicVolume; }
        if (sfxSource != null)   { sfxSource.loop = false; sfxSource.volume = sfxVolume; }
    }

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

    /// Fire a one-shot SFX. volumeScale (0..1) lets a caller play quieter/louder.
    public void PlaySfx(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, volumeScale);
    }

    // ── UI helpers ──
    public void PlayUIHover() => PlaySfx(uiHover);
    public void PlayUIClick() => PlaySfx(uiClick);
}