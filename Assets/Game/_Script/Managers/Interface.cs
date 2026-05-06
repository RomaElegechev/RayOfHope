using PrimeTween;
using TMPro;
using UnityEngine;


public class Interface : MonoBehaviour
{
    public static Interface Instance { get; private set; }

    [SerializeField] private TMP_Text _enemyWarning;
    [SerializeField] private GameObject _UIContainer;
    [SerializeField] private TMP_Text _stepNumber;
    [SerializeField] private TMP_Text _coalNumber;
    [SerializeField] private TMP_Text _ironNumber;
    [SerializeField] private TMP_Text _foodNumber;
    [SerializeField] private TMP_Text _woodNumber;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        UpdateUI();
    }

    public void EnemyWarningFadeIn() => Tween.Scale(_enemyWarning.transform, 1f, 0.5f, Ease.OutSine);

    public void InterfaceFadeOut()
    {
        Tween.LocalPositionY(_UIContainer.transform, -150f, 0.2f, Ease.InBack);
        Tween.Scale(_enemyWarning.transform, 0f, 0.25f, Ease.InSine);
    }
    public void InterfaceFadeIn() => Tween.LocalPositionY(_UIContainer.transform, 0f, 0.2f, Ease.OutBack);

    private void UpdateUI()
    {
        _stepNumber.text = GameManager.Instance.StepNumber.ToString();
        _coalNumber.text = GameManager.Instance.CoalNumber.ToString();
        _ironNumber.text = GameManager.Instance.IronNumber.ToString();
        _foodNumber.text = GameManager.Instance.FoodNumber.ToString();
        _woodNumber.text = GameManager.Instance.WoodNumber.ToString();
    }
}
