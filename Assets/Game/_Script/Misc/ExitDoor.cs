using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExitDoor : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _buildInside;
    [Header("Hover Animation")]
    [SerializeField] private float _hoverScaleMultiplier = 1.15f;
    [SerializeField] private float _hoverDuration = 0.2f;

    private Vector2 _initialScale;

    private void Start()
    {
        _initialScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CameraManager.Instance.ExitBuild();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tween.Scale(transform, _initialScale * _hoverScaleMultiplier, _hoverDuration, Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tween.Scale(transform, _initialScale, _hoverDuration, Ease.OutBack);
    }
}
