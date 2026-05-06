using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;

public class LighthouseItem : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private LighthouseAction _action;
    [SerializeField] private ActionType _actionType;
    [Header("Hover Animation")]
    [SerializeField] private float _hoverMultiplier = 1.15f;
    [SerializeField] private float _hoverDuration = 0.2f;

    private Vector2 _initialeScale;

    private void Awake()
    {
        _initialeScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        switch (_actionType)
        {
            case ActionType.Select: Lighthouse.Instance.SelectActivity(_action); break;
            case ActionType.Disable: Lighthouse.Instance.DisableActivity(_action); break;
            case ActionType.Agree: Lighthouse.Instance.StartAction(_action); break;
        } 
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tween.Scale(transform, _initialeScale * _hoverMultiplier, _hoverDuration, Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tween.Scale(transform, _initialeScale, _hoverDuration, Ease.OutBack);
    }
}
