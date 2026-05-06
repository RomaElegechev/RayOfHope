using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioLowPassFilter))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Data")]
    [SerializeField] private ObjectPool _poolSFX;
    [SerializeField] private SoundSO _soundLibrary;
    [SerializeField] private SoundSO _musicLibrary;
    [Header("VolumeSettings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float soundVolume = 1f;
    [Header("LowPassFilter")]
    [SerializeField] private float _musicVolumePaused = 0.6f;
    [SerializeField] private float _normalLowPass = 22000f;
    [SerializeField] private float _pausedLowPass = 1000f;
    [SerializeField] private float _durationTime = 0.5f;

    private AudioSource _audioSource;
    private AudioLowPassFilter _lowPassFilter;

    private bool _isPause;
    private float _initiallyVolume;

    private void Awake()
    {
        Instance = this;
        _audioSource = GetComponent<AudioSource>();
        _lowPassFilter = GetComponent<AudioLowPassFilter>();
    }


    public void PlaySFX(string NameClip, float volume = 1f)
    {
        AudioClip clip = _soundLibrary.GetClipByName(NameClip);

        if (clip == null) return;

        GameObject sound = _poolSFX.Get();
        AudioSource audioSource = sound.GetComponent<AudioSource>();
        audioSource.volume = volume * soundVolume * masterVolume;
        audioSource.pitch = UnityEngine.Random.Range(0.85f, 1.15f);
        audioSource.clip = clip;
        audioSource.Play();

        _poolSFX.Return(sound, clip.length + 0.1f);
    }

    public void PlayMusic(string state, float volume = 1f)
    {
        AudioClip clip = _musicLibrary.GetClipByName(state);

        if (clip == null) return;

        _audioSource.clip = clip;
        _initiallyVolume = volume;
        _audioSource.volume = _initiallyVolume * musicVolume * masterVolume;
        _audioSource.Play();
    }


    public void UpdateMusicVolume() => _audioSource.volume = _initiallyVolume * musicVolume * masterVolume;


    public void SetPauseState(bool isPause)
    {
        if (_isPause == isPause) return;

        _isPause = isPause;

        if (_isPause) StartCoroutine(TransitionPausedMusic());
        else StartCoroutine(TransitionDefaultMusic());
    }

    private IEnumerator TransitionPausedMusic()
    {
        float duration = _durationTime;
        float time = 0f;

        float startVolume = _audioSource.volume;
        float startLowPass = _lowPassFilter.cutoffFrequency;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;

            _audioSource.volume = Mathf.Lerp(startVolume, startVolume * _musicVolumePaused, t);
            _lowPassFilter.cutoffFrequency = Mathf.Lerp(startLowPass, _pausedLowPass, t);

            yield return null;
        }

        _audioSource.volume = startVolume * _musicVolumePaused;
        _lowPassFilter.cutoffFrequency = _pausedLowPass;
    }

    private IEnumerator TransitionDefaultMusic()
    {
        float duration = _durationTime;
        float time = 0f;

        float startVolume = _audioSource.volume;
        float startLowPass = _lowPassFilter.cutoffFrequency;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;

            _audioSource.volume = Mathf.Lerp(startVolume, _initiallyVolume, t);
            _lowPassFilter.cutoffFrequency = Mathf.Lerp(startLowPass, _normalLowPass, t);

            yield return null;
        }

        _audioSource.volume = _initiallyVolume;
        _lowPassFilter.cutoffFrequency = _normalLowPass;
    }
}
