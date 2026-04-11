using UnityEngine;

public class AudioManager : MonoBehaviour
{
    static AudioManager _instance;

    [Header("AudioClips")]
    [SerializeField] AudioClip[] _sfxClip;
    [SerializeField] AudioClip[] _musicClip;

    [Header("AudioSources")]
    [SerializeField] AudioSource _sfxAudioSource;
    [SerializeField] AudioSource _musicSource;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("AudioManager was not found");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        PlayMusic(0);
    }

    public void PlaySfxClip(AudioClip _clip)
    {
        if (_clip != null)
        {
            _sfxAudioSource.PlayOneShot(_clip);
        }
    }

    public void PlaySfxClip(int value)
    {
        if (value < 0 || value >= _sfxClip.Length)
        {
            _sfxAudioSource.Pause();
            return;
        }
        _sfxAudioSource.clip = _sfxClip[value];
        _sfxAudioSource.Play();
    }

    public void PlayMusic(int value)
    {
        if (value < 0 || value >= _musicClip.Length)
        {
            _musicSource.Pause();
            return;
        }

        _musicSource.clip = _musicClip[value];
        _musicSource.Play();
    }
}