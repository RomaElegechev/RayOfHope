using PrimeTween;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class ContainerUI
{
    public RectTransform container;
    public Vector3 initialPosition;
    public Vector3 initialScale;
}

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [Header("Dark Effects Camera")]
    [SerializeField] private Volume _cameraVolume;
    [SerializeField] private float _minVignette;
    [SerializeField] private float _minAdjustmentsPost;
    [SerializeField] private float _minAdjustmentsContrast;
    [SerializeField] private float _minAdjustmentsHueShift;
    [SerializeField] private float _minAdjustmentsSaturation;
    [SerializeField] private float _minFilmGrain;
    [SerializeField] private float _effectDuration;
    [Header("Light Effects Camera")]
    [SerializeField] private Color _lightColorVignette;
    [SerializeField] private float _lightVignette;
    [SerializeField] private float _lightAdjustmentsPost;
    [SerializeField] private float _lightAdjustmentsContrast;
    [SerializeField] private float _lightAdjustmentsHueShift;
    [SerializeField] private float _lightAdjustmentsSaturation;
    [SerializeField] private float _lightFilmGrain;
    [Header("Attack Effects Camera")]
    [SerializeField] private float _battleVignette;
    [SerializeField] private Color _battleColorFilter;
    [SerializeField] private float _battleEffectsDuration = 1.5f;
    [Header("Zoom Animation")]
    [SerializeField] private ContainerUI[] _UIcontainers;
    [SerializeField] private float _zoomFactor = 3.4f;
    [SerializeField] private float _focusDuration = 0.6f;
    [SerializeField] private Ease _focusEase = Ease.OutCubic;
    [Header("Darknening")]
    [SerializeField] private ParticleSystem _darknessParticles;

    public Action<TypeBuild> OnBuildEnter;
    public Action<TypeBuild> OnBuildExit;

    private Vignette _vignette;
    private ColorAdjustments _colorAdjustments;
    private FilmGrain _filmGrain;

    private float _darkVignette;
    private float _darkAdjustmentsPost;
    private float _darkAdjustmentsContrast;
    private float _darkAdjustmentsHueShift;
    private float _darkAdjustmentsSaturation;
    private float _darkFilmGrain;

    private TypeBuild _currentBuild;

    private bool _isFocused;

    private void Awake()
    {
        Instance = this;

        _cameraVolume.profile.TryGet(out _vignette);
        _cameraVolume.profile.TryGet(out _colorAdjustments);
        _cameraVolume.profile.TryGet(out _filmGrain);

        _darkVignette = _vignette.intensity.value;

        _darkAdjustmentsPost = _colorAdjustments.postExposure.value;
        _darkAdjustmentsContrast = _colorAdjustments.contrast.value;
        _darkAdjustmentsHueShift = _colorAdjustments.hueShift.value;
        _darkAdjustmentsSaturation = _colorAdjustments.saturation.value;

        _darkFilmGrain = _filmGrain.intensity.value;
    }

    private void Start()
    {
        if (FightManager.Instance != null)
        {
            FightManager.Instance.OnFightStart += OnFightStart;
            FightManager.Instance.OnFightEnd += OnFightEnd;
        }
    }

    private void OnFightStart() => StartCoroutine(FightEffectsFadeIn());
    private void OnFightEnd() => StartCoroutine(FightEffectsFadeOut());

    private IEnumerator FightEffectsFadeIn()
    {
        float time = 0;
        float percent;

        while (time < _battleEffectsDuration)
        {
            time += Time.deltaTime;
            percent = (float)time / _battleEffectsDuration;

            _colorAdjustments.colorFilter.value = Color.Lerp(Color.white, _battleColorFilter, percent);
            _vignette.intensity.value = Mathf.Lerp(_darkVignette, _battleVignette, percent);

            yield return null;
        }

        _colorAdjustments.colorFilter.value = _battleColorFilter;
        _vignette.intensity.value = _battleVignette;
    }

    private IEnumerator FightEffectsFadeOut()
    {
        float time = 0;
        float percent;

        while (time < _battleEffectsDuration)
        {
            time += Time.deltaTime;
            percent = (float)time / _battleEffectsDuration;

            _colorAdjustments.colorFilter.value = Color.Lerp(_battleColorFilter, Color.white, percent);
            _vignette.intensity.value = Mathf.Lerp(_battleVignette, _darkVignette, percent);

            yield return null;
        }

        _colorAdjustments.colorFilter.value = Color.white;
        _vignette.intensity.value = _darkVignette;
    }

    public void FocusOnBuilding(RectTransform focusPoint, TypeBuild type)
    {
        if (_isFocused) return; 
        _isFocused = true;

        _currentBuild = type;

        StartCoroutine(FadeOutOutsideEffects());

        foreach (var container in _UIcontainers)
        {
            container.initialPosition = container.container.transform.localPosition;
            container.initialScale = container.container.transform.localScale;

            Vector3 targetLocalPosition = container.container.InverseTransformPoint(focusPoint.position);
            Vector3 targetOffset = -targetLocalPosition * _zoomFactor;

            Tween.Scale(container.container, container.initialScale * _zoomFactor, _focusDuration, _focusEase);
            Tween.LocalPosition(container.container, targetOffset, _focusDuration, _focusEase);
        }

        StartCoroutine(EnterDarkening());
    }

    public void ExitBuild() => StartCoroutine(ExitDarkening());

    private IEnumerator ExitDarkening()
    {
        _darknessParticles.Play();

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(FadeInOutsideEffects());
    }

    private IEnumerator EnterDarkening()
    {
        yield return new WaitForSeconds(_focusDuration);

        _darknessParticles.Play();
        yield return new WaitForSeconds(_darknessParticles.main.duration / _darknessParticles.main.simulationSpeed * 1.25f);

        StartCoroutine(FadeInInsideEffects());
    }

    private IEnumerator FadeOutOutsideEffects()
    {
        float time = 0;

        while (time < _effectDuration)
        {
            time += Time.deltaTime;
            float percent = time / _effectDuration;

            _vignette.intensity.value = Mathf.Lerp(_darkVignette, _minVignette, percent);

            _colorAdjustments.postExposure.value = Mathf.Lerp(_darkAdjustmentsPost, _minAdjustmentsPost, percent);
            _colorAdjustments.contrast.value = Mathf.Lerp(_darkAdjustmentsContrast, _minAdjustmentsContrast, percent);
            _colorAdjustments.hueShift.value = Mathf.Lerp(_darkAdjustmentsHueShift, _minAdjustmentsHueShift, percent);
            _colorAdjustments.saturation.value = Mathf.Lerp(_darkAdjustmentsSaturation, _minAdjustmentsSaturation, percent);

            _filmGrain.intensity.value = Mathf.Lerp(_darkFilmGrain, _minFilmGrain, percent);

            yield return null;
        }

        _isFocused = false;
    }

    private IEnumerator FadeInOutsideEffects()
    {
        float time = 0;

        OnBuildExit?.Invoke(_currentBuild);

        while (time < _effectDuration)
        {
            time += Time.deltaTime;
            float percent = time / _effectDuration;

            _vignette.intensity.value = Mathf.Lerp(_minVignette, _darkVignette, percent);
            _vignette.color.value = Color.Lerp(_lightColorVignette, Color.black, percent);

            _colorAdjustments.postExposure.value = Mathf.Lerp(_minAdjustmentsPost, _darkAdjustmentsPost, percent);
            _colorAdjustments.contrast.value = Mathf.Lerp(_minAdjustmentsContrast, _darkAdjustmentsContrast, percent);
            _colorAdjustments.hueShift.value = Mathf.Lerp(_minAdjustmentsHueShift, _darkAdjustmentsHueShift, percent);
            _colorAdjustments.saturation.value = Mathf.Lerp(_minAdjustmentsSaturation, _darkAdjustmentsSaturation, percent);

            _filmGrain.intensity.value = Mathf.Lerp(_minFilmGrain, _darkFilmGrain, percent);

            yield return null;
        }

        _isFocused = false;
    }

    private IEnumerator FadeInInsideEffects()
    {
        float time = 0;

        OnBuildEnter?.Invoke(_currentBuild);

        while (time < _effectDuration)
        {
            time += Time.deltaTime;
            float percent = time / _effectDuration;

            _vignette.intensity.value = Mathf.Lerp(_minVignette, _lightVignette, percent);
            _vignette.color.value = Color.Lerp(Color.black, _lightColorVignette, percent);

            _colorAdjustments.postExposure.value = Mathf.Lerp(_minAdjustmentsPost, _lightAdjustmentsPost, percent);
            _colorAdjustments.contrast.value = Mathf.Lerp(_minAdjustmentsContrast, _lightAdjustmentsContrast, percent);
            _colorAdjustments.hueShift.value = Mathf.Lerp(_minAdjustmentsHueShift, _lightAdjustmentsHueShift, percent);
            _colorAdjustments.saturation.value = Mathf.Lerp(_minAdjustmentsSaturation, _lightAdjustmentsSaturation, percent);

            _filmGrain.intensity.value = Mathf.Lerp(_minFilmGrain, _lightFilmGrain, percent);

            yield return null;
        }

        foreach (var container in _UIcontainers)
        {
            container.container.localScale = container.initialScale;
            container.container.localPosition = container.initialPosition;
        }

        _isFocused = false;
    }
}
