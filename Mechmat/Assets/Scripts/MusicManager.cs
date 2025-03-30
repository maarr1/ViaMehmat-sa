using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Music Settings")]
    [Tooltip("Аудиоклип для фоновой музыки")]
    public AudioClip musicClip;

    [Tooltip("Громкость музыки")]
    [Range(0f, 1f)]
    public float musicVolume = 1.0f;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Добавляем или получаем компонент AudioSource
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            // Настраиваем AudioSource
            audioSource.clip = musicClip;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.volume = musicVolume;

            // Проверяем, назначен ли аудиоклип, и запускаем музыку
            if (musicClip != null)
            {
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning("MusicManager: Аудиоклип не назначен.");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Включает или выключает музыку.
    /// </summary>
    public void ToggleMusic()
    {
        if (audioSource != null)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
                Debug.Log("Музыка приостановлена.");
            }
            else
            {
                audioSource.Play();
                Debug.Log("Музыка воспроизводится.");
            }
        }
    }

    /// <summary>
    /// Устанавливает громкость музыки.
    /// </summary>
    /// <param name="volume">Значение громкости от 0 до 1.</param>
    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.Clamp01(volume);
            musicVolume = audioSource.volume;
            Debug.Log($"Громкость музыки установлена на {musicVolume * 100}%.");
        }
    }

    /// <summary>
    /// Выключает музыку.
    /// </summary>
    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            Debug.Log("Музыка остановлена.");
        }
    }

    /// <summary>
    /// Включает музыку.
    /// </summary>
    public void PlayMusic()
    {
        if (audioSource != null && musicClip != null)
        {
            audioSource.Play();
            Debug.Log("Музыка воспроизводится.");
        }
        else
        {
            Debug.LogWarning("MusicManager: Аудиоклип не назначен или AudioSource отсутствует.");
        }
    }

    /// <summary>
    /// Меняет аудиоклип на новый и воспроизводит его.
    /// </summary>
    /// <param name="newClip">Новый аудиоклип для воспроизведения.</param>
    public void ChangeMusic(AudioClip newClip)
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            musicClip = newClip;
            audioSource.clip = musicClip;

            if (musicClip != null)
            {
                audioSource.Play();
                Debug.Log("Музыка изменена и воспроизводится.");
            }
            else
            {
                Debug.LogWarning("MusicManager: Новый аудиоклип не назначен.");
            }
        }
    }
}
