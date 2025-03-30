using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Music Settings")]
    [Tooltip("��������� ��� ������� ������")]
    public AudioClip musicClip;

    [Tooltip("��������� ������")]
    [Range(0f, 1f)]
    public float musicVolume = 1.0f;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ��������� ��� �������� ��������� AudioSource
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            // ����������� AudioSource
            audioSource.clip = musicClip;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.volume = musicVolume;

            // ���������, �������� �� ���������, � ��������� ������
            if (musicClip != null)
            {
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning("MusicManager: ��������� �� ��������.");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// �������� ��� ��������� ������.
    /// </summary>
    public void ToggleMusic()
    {
        if (audioSource != null)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
                Debug.Log("������ ��������������.");
            }
            else
            {
                audioSource.Play();
                Debug.Log("������ ���������������.");
            }
        }
    }

    /// <summary>
    /// ������������� ��������� ������.
    /// </summary>
    /// <param name="volume">�������� ��������� �� 0 �� 1.</param>
    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.Clamp01(volume);
            musicVolume = audioSource.volume;
            Debug.Log($"��������� ������ ����������� �� {musicVolume * 100}%.");
        }
    }

    /// <summary>
    /// ��������� ������.
    /// </summary>
    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            Debug.Log("������ �����������.");
        }
    }

    /// <summary>
    /// �������� ������.
    /// </summary>
    public void PlayMusic()
    {
        if (audioSource != null && musicClip != null)
        {
            audioSource.Play();
            Debug.Log("������ ���������������.");
        }
        else
        {
            Debug.LogWarning("MusicManager: ��������� �� �������� ��� AudioSource �����������.");
        }
    }

    /// <summary>
    /// ������ ��������� �� ����� � ������������� ���.
    /// </summary>
    /// <param name="newClip">����� ��������� ��� ���������������.</param>
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
                Debug.Log("������ �������� � ���������������.");
            }
            else
            {
                Debug.LogWarning("MusicManager: ����� ��������� �� ��������.");
            }
        }
    }
}
