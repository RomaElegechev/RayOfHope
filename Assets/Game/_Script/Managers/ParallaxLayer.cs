using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private float _speedLayer = 0.5f;
    [SerializeField] private float _startPosition = 1080;
    [SerializeField] private float _resetPosition = -540;

    private void Update()
    {
        transform.Translate(Vector2.right * _speedLayer * Time.deltaTime);

        if (transform.localPosition.x > _resetPosition)
        {
            Vector2 newPosition = new Vector2(_startPosition, transform.localPosition.y);
            transform.localPosition = newPosition;
        }
    }
}
