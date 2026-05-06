using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MineItem : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private MineAction _action;
    [SerializeField] private ActionType _actionType;
    [Header("Hover Animation")]
    [SerializeField] private float _hoverMultiplier = 1.15f;
    [SerializeField] private float _hoverDuration = 0.2f;

    private Vector2 _initialeScale;

    private void Start()
    { 
        _initialeScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        switch (_actionType)
        {
            case ActionType.Select: Mine.Instance.SelectAction(_action); break;
            case ActionType.Disable: Mine.Instance.DisableAction(_action); break;
            case ActionType.Agree: Mine.Instance.StartAction(_action); break;
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
