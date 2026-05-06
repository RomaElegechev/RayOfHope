using PrimeTween;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

public class Build : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TypeBuild _type;
    [Header("Animation")]
    [SerializeField] private SpriteRenderer _buildSpriteRenderer;
    [Header("CameraAnimation")]
    [SerializeField] private RectTransform _cameraFocusPoint;
    [Header("HoverAnimation")]
    [SerializeField] private float _hoverScaleMultiplier = 1.15f;
    [SerializeField] private float _hoverScaleDuration = 0.2f;
    [SerializeField] private Light2D _hoverLight;

    private Vector3 _initialeHoverScale;

    private void Start()
    { 
        _initialeHoverScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance.CanEnterBuild)
        {
            GameManager.Instance.CanEnterBuild = false;
            CameraManager.Instance.FocusOnBuilding(_cameraFocusPoint, _type);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tween.Scale(transform, _initialeHoverScale * _hoverScaleMultiplier, _hoverScaleDuration, Ease.OutBack);
        _buildSpriteRenderer.sortingOrder++;
        _hoverLight.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tween.Scale(transform, _initialeHoverScale, _hoverScaleDuration, Ease.OutBack);
        _buildSpriteRenderer.sortingOrder--;
        _hoverLight.enabled = false;
    }
}
