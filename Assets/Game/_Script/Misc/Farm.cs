using PrimeTween;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum FarmAction { FoodSend, FoodMinigame, Upgrade, FoodPutting}

[System.Serializable]
public class FarmPanel
{
    public GameObject panel;
    public FarmAction action;
    public Slider slider;
    public TMP_Text sliderText;
    public SpriteRenderer selectButtonSprite;
    public GameObject arrow;
}

public class Farm : MonoBehaviour
{
    public static Farm Instance { get; private set; }

    [SerializeField] private FarmPanel[] _activitiesPanels;
    [Header("Animation")]
    [SerializeField] private Color _unactiveColor = new Color(0.541f, 0.541f, 0.541f, 1f);
    [SerializeField] private float _panelDuration;
    [Header("Food")]
    [SerializeField] private int _foodSend = 4;
    [SerializeField] private int _minFoodMinigame = 0;
    [SerializeField] private int _maxFoodMinigame = 8;
    [Header("Level")]
    [SerializeField] private int _farmLevel = 1;
    [SerializeField] private TMP_Text _farmLevelText;
    [Header("Upgrade")]
    [SerializeField] private TMP_Text _woodCostText;
    [SerializeField] private TMP_Text _coalCostText;
    [SerializeField] private TMP_Text _foodCostText;
    [SerializeField] private int _initialWoodCost = 2;
    [SerializeField] private int _initialCoalCost = 4;
    [SerializeField] private int _initialFoodCost = 6;

    private int _woodCost;
    private int _coalCost;
    private int _foodCost;

    private FarmPanel _currentActivity;

    private void Awake()
    {
        Instance = this;

        _woodCost = _initialWoodCost;
        _coalCost = _initialCoalCost;
        _foodCost = _initialFoodCost;

        _woodCostText.text = _woodCost.ToString();
        _coalCostText.text = _coalCost.ToString();
        _foodCostText.text = _foodCost.ToString();
    }

    private void Update()
    {
        UpdatePanelUI();
    }

    public void SelectActivity(FarmAction action)
    {
        if (_currentActivity == null)
        {
            _currentActivity = _activitiesPanels.FirstOrDefault(panel => panel.action == action);

            if (_currentActivity.action == FarmAction.Upgrade)
            {
                if (GameManager.Instance.WoodNumber >= _woodCost && GameManager.Instance.CoalNumber >= _coalCost
                    && GameManager.Instance.FoodNumber >= _foodCost)
                {
                    _currentActivity.selectButtonSprite.color = Color.white;
                }
                else _currentActivity.selectButtonSprite.color = _unactiveColor;
            }

            _currentActivity.panel.SetActive(true);
            Tween.LocalPositionY(_currentActivity.panel.transform, 0f, _panelDuration, Ease.OutBack);
        }
    }

    public void DisableActivity(FarmAction action)
    {
        if (_currentActivity.action == action)
        {
            Tween.LocalPositionY(_currentActivity.panel.transform, 1050f, _panelDuration, Ease.InBack)
                .OnComplete(() => {
                    _currentActivity.panel.SetActive(false);
                    _currentActivity = null;
                });
        }
    }

    public void StartAction(FarmAction action)
    {
        switch (action)
        {
            case FarmAction.FoodSend: FoodSend(); break;
            case FarmAction.FoodMinigame: FoodMinigame(); break;
            case FarmAction.Upgrade: UpgradeCost(); break;
        }
    }

    private void UpdatePanelUI()
    {
        if (_currentActivity != null && _currentActivity.action != FarmAction.Upgrade)
        {
            int numberLaborForce = ((int)(GameManager.Instance.LaborForceNumber * _currentActivity.slider.value));

            _currentActivity.selectButtonSprite.color =
                numberLaborForce > 0 && GameManager.Instance.StepNumber - numberLaborForce >= 0 ? Color.white : _unactiveColor;
            _currentActivity.sliderText.text = numberLaborForce.ToString();
        }
    }

    private void FoodSend()
    {
        int numberLaborForce = ((int)(GameManager.Instance.LaborForceNumber * _currentActivity.slider.value));

        if (numberLaborForce > 0 && GameManager.Instance.StepNumber - numberLaborForce >= 0)
        {
            GameManager.Instance.FoodNumber += _foodSend * numberLaborForce;
            GameManager.Instance.StepNumber -= numberLaborForce;

            GameManager.Instance.EndAction(_foodSend * numberLaborForce);
        }
    }

    private void FoodMinigame()
    {

    }

    private void UpgradeCost()
    {
        if (GameManager.Instance.WoodNumber >= _woodCost && GameManager.Instance.CoalNumber >= _coalCost
            && GameManager.Instance.FoodNumber >= _foodCost)
        {
            GameManager.Instance.WoodNumber -= _woodCost;
            GameManager.Instance.CoalNumber -= _coalCost;
            GameManager.Instance.WoodNumber -= _foodCost;

            _woodCost = (int)(_initialWoodCost * (1f + (_farmLevel / 2f)));
            _coalCost = (int)(_initialCoalCost * (1f + (_farmLevel / 2f)));
            _foodCost = (int)(_initialFoodCost * (1f + (_farmLevel / 2f)));

            _woodCostText.text = _woodCost.ToString();
            _coalCostText.text = _coalCost.ToString();
            _foodCostText.text = _foodCost.ToString();

            _farmLevel++;
            _farmLevelText.text = _farmLevel.ToString();
        }
    }
}
