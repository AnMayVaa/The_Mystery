using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Source")]
    public AudioSource bgmSource;

    [Header("Volume")]
    [Range(0f, 1f)]
    public float bgmVolume = 1f;

    [Header("Music Clips")]
    public AudioClip menuMusic;
    public AudioClip streetMusic;
    public AudioClip houseMusic;
    public AudioClip crimeMusic;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        // ปรับเสียงตามค่าที่เซ็ตไว้
        if (bgmSource != null)
            bgmSource.volume = bgmVolume;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Mainmenu_scene":
                PlayMusic(menuMusic);
                break;

            case "Street_scene(final)":
                PlayMusic(streetMusic);
                break;

            case "House_scene(final)":
                PlayMusic(houseMusic);
                break;
            case "Crime_scene(final)":
                PlayMusic(crimeMusic); 
                break;
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (bgmSource == null || clip == null) return;

        if (bgmSource.clip == clip) return; // ถ้าเล่นอยู่แล้วไม่ต้องเปลี่ยน
        bgmSource.clip = clip;
        bgmSource.Play();
    }
}
