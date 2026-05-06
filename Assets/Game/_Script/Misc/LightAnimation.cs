using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class LightAnimator : MonoBehaviour
{
    [SerializeField] private float _minLightStrengh = 2f;
    [SerializeField] private float _lightDuration = 3f;

    private Light2D _light;

    private float _maxLightStrengh;

    private void Awake()
    {
        _light = GetComponent<Light2D>();
        _maxLightStrengh = _light.intensity;

        StartCoroutine(LightAnimation());
    }

    private IEnumerator LightAnimation()
    {
        while (true)
        {
            float time = 0;
            float percent;

            while (time < _lightDuration)
            {
                time += Time.deltaTime;
                percent = time / _lightDuration;

                _light.intensity = Mathf.Lerp(_maxLightStrengh, _minLightStrengh, percent);

                yield return null;
            }

            _light.intensity = _minLightStrengh;

            time = 0;

            while (time < _lightDuration)
            {
                time += Time.deltaTime;
                percent = time / _lightDuration;

                _light.intensity = Mathf.Lerp(_minLightStrengh, _maxLightStrengh, percent);

                yield return null;
            }
        }
    }
}
