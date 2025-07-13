using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip titleTheme;
    public AudioClip easyTheme;
    public AudioClip mediumTheme;
    public AudioClip hardTheme;

    [Header("SFX")]
    public AudioSource sfxSource;
    public AudioClip eatSFX;
    public AudioClip deathSFX;
    public AudioClip wallBreakSFX;
    public AudioClip wallSplatSFX;
    public AudioClip electricSFX;
    public AudioClip zapSFX;
    public AudioClip metalSFX;
    public AudioClip metalSlamSFX;
    public AudioClip clickSFX;
    public AudioClip multiplierSFX_Level1; // For 2x and 3x
    public AudioClip multiplierSFX_Level2; // For 5x
    public AudioClip recordScratchSFX;


    private bool hasPlayedMultiplierThisCycle = false;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        hasPlayedMultiplierThisCycle = false;
        PlayMusicForScene(scene.name);    
    }

    public void PlayMusicForScene(string sceneName)
    {
        Debug.Log($"[AudioManager] Attempting to play music for: {sceneName}");

        AudioClip clip = sceneName switch
        {
            "TitleScreen" => titleTheme,
            "Easy" => easyTheme,
            "Medium" => mediumTheme,
            "Hard" => hardTheme,
            _ => null
        };

        if (clip == null)
        {
            Debug.LogWarning($"[AudioManager] No music clip found for scene: {sceneName}");
            return;
        }

        if (musicSource.clip != clip)
        {
            Debug.Log($"[AudioManager] Stopping current music and assigning new clip: {clip.name}");
            musicSource.Stop();
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
        else if (!musicSource.isPlaying)
        {
            Debug.Log($"[AudioManager] Clip assigned but not playing — forcing playback: {clip.name}");
            musicSource.Play();
        }
        else
        {
            Debug.Log($"[AudioManager] Clip already playing: {clip.name}");
        }
    }

    public void PlayRecordScratch()
    {
        if (musicSource != null && recordScratchSFX != null)
        {
            musicSource.Stop();

            AudioClip oldClip = musicSource.clip;
            musicSource.clip = null; // Clear clip so PlayMusicForScene will retrigger properly

            musicSource.PlayOneShot(recordScratchSFX);

            // Optional: Log it
            Debug.Log($"[AudioManager] Playing record scratch and clearing music clip (was: {oldClip?.name})");
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip);
    }

    public void TryPlayMultiplierSFX(int level)
    {
        if (hasPlayedMultiplierThisCycle) return;

        switch (level)
        {
            case 5:
                PlaySFX(multiplierSFX_Level2);
                break;
            case 2:
            case 3:
            default:
                PlaySFX(multiplierSFX_Level1);
                break;
        }

        hasPlayedMultiplierThisCycle = true;
    }

    public void ResetMultiplierCycle()
    {
        hasPlayedMultiplierThisCycle = false;
    }

    public bool HasPlayedMultiplierThisCycle() => hasPlayedMultiplierThisCycle;
}
