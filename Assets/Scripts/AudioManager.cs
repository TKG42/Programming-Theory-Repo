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
    public AudioClip hissSFX;
    public AudioClip electricSFX;
    public AudioClip zapSFX;
    public AudioClip metalSFX;
    public AudioClip metalSlamSFX;
    public AudioClip clickSFX;
    public AudioClip multiplierSFX;

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
        PlayMusicForScene(scene.name);
        hasPlayedMultiplierThisCycle = false;
    }

    public void PlayMusicForScene(string sceneName)
    {
        AudioClip clip = sceneName switch
        {
            "TitleScreen" => titleTheme,
            "Easy" => easyTheme,
            "Medium" => mediumTheme,
            "Hard" => hardTheme,
            _ => null
        };

        if (clip != null && musicSource.clip != clip)
        {
            musicSource.Stop();
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip);
    }

    public void TryPlayMultiplierSFX()
    {
        if (!hasPlayedMultiplierThisCycle)
        {
            PlaySFX(multiplierSFX);
            hasPlayedMultiplierThisCycle = true;
        }
    }

    public void ResetMultiplierCycle()
    {
        hasPlayedMultiplierThisCycle = false;
    }
}
