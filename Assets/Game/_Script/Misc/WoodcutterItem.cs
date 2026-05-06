using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;

public class WoodcutterItem : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private WoodcutterAction _action;
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
            case ActionType.Select: Woodcutter.Instance.SelectActivity(_action); break;
            case ActionType.Disable: Woodcutter.Instance.DisableActivity(_action); break;
            case ActionType.Agree: Woodcutter.Instance.StartAction(_action); break;
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
