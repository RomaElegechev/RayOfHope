using PrimeTween;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum WoodcutterAction { WoodSend, WoodMinigame, Upgrade}

[System.Serializable]
public class WoodcutterPanel
{
    public GameObject panel;
    public WoodcutterAction action;
    public Slider slider;
    public TMP_Text sliderText;
    public SpriteRenderer selectButtonSprite;
    public GameObject arrow;
}

public class Woodcutter : MonoBehaviour
{
    public static Woodcutter Instance { get; private set; }

    [SerializeField] private WoodcutterPanel[] _activitiesPanels;
    [Header("Animation")]
    [SerializeField] private Color _unactiveColor = new Color(0.541f, 0.541f, 0.541f, 1f);
    [SerializeField] private float _panelDuration;
    [Header("Wood")]
    [SerializeField] private int _woodSend = 3;
    [SerializeField] private int _minWoodMinigame = 0;
    [SerializeField] private int _maxWoodMinigame = 6;
    [Header("Level")]
    [SerializeField] private int _woodcutterLevel = 1;
    [SerializeField] private TMP_Text _woodcutterLevelText;
    [Header("Upgrade")]
    [SerializeField] private TMP_Text _woodCostText;
    [SerializeField] private TMP_Text _coalCostText;
    [SerializeField] private TMP_Text _ironCostText;
    [SerializeField] private int _initialWoodCost = 4;
    [SerializeField] private int _initialCoalCost = 5;
    [SerializeField] private int _initialIronCost = 2;

    private int _woodCost;
    private int _coalCost;
    private int _ironCost;

    private WoodcutterPanel _currentActivity;

    private void Awake()
    {
        Instance = this;

        _woodCost = _initialWoodCost;
        _coalCost = _initialCoalCost;
        _ironCost = _initialIronCost;

        _woodCostText.text = _woodCost.ToString();
        _coalCostText.text = _coalCost.ToString();
        _ironCostText.text = _ironCost.ToString();
    }

    private void Update()
    {
        UpdatePanelUI();
    }

    public void SelectActivity(WoodcutterAction action)
    {
        if (_currentActivity == null)
        {
            _currentActivity = _activitiesPanels.FirstOrDefault(panel => panel.action == action);

            if (_currentActivity.action == WoodcutterAction.Upgrade)
            {
                if (GameManager.Instance.WoodNumber >= _woodCost && GameManager.Instance.CoalNumber >= _coalCost
                    && GameManager.Instance.IronNumber >= _ironCost)
                {
                    _currentActivity.selectButtonSprite.color = Color.white;
                }
                else _currentActivity.selectButtonSprite.color = _unactiveColor;
            }

            _currentActivity.panel.SetActive(true);
            Tween.LocalPositionY(_currentActivity.panel.transform, 0f, _panelDuration, Ease.OutBack);
        }
    }

    public void DisableActivity(WoodcutterAction action)
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

    public void StartAction(WoodcutterAction action)
    {
        switch (action)
        {
            case WoodcutterAction.WoodSend: WoodSend(); break;
            case WoodcutterAction.WoodMinigame: WoodMinigame(); break;
            case WoodcutterAction.Upgrade: UpgradeCost(); break;
        }
    }

    private void UpdatePanelUI()
    {
        if (_currentActivity != null && _currentActivity.action != WoodcutterAction.Upgrade)
        {
            int numberLaborForce = ((int)(GameManager.Instance.LaborForceNumber * _currentActivity.slider.value));

            _currentActivity.selectButtonSprite.color =
                numberLaborForce > 0 && GameManager.Instance.StepNumber - numberLaborForce >= 0 ? Color.white : _unactiveColor;
            _currentActivity.sliderText.text = numberLaborForce.ToString();
        }
    }

    private void WoodSend()
    {
        int numberLaborForce = ((int)(GameManager.Instance.LaborForceNumber * _currentActivity.slider.value));

        if (numberLaborForce > 0 && GameManager.Instance.StepNumber - numberLaborForce >= 0)
        {
            GameManager.Instance.WoodNumber += _woodSend * numberLaborForce;
            GameManager.Instance.StepNumber -= numberLaborForce;

            GameManager.Instance.EndAction(_woodSend * numberLaborForce);
        }
    }

    private void WoodMinigame()
    {

    }

    private void UpgradeCost()
    {
        if (GameManager.Instance.WoodNumber >= _woodCost && GameManager.Instance.CoalNumber >= _coalCost
            && GameManager.Instance.IronNumber >= _ironCost)
        {
            GameManager.Instance.WoodNumber -= _woodCost;
            GameManager.Instance.CoalNumber -= _coalCost;
            GameManager.Instance.IronNumber -= _ironCost;

            _woodCost = (int)(_initialWoodCost * (1f + (_woodcutterLevel / 2f)));
            _coalCost = (int)(_initialCoalCost * (1f + (_woodcutterLevel / 2f)));
            _ironCost = (int)(_initialIronCost * (1f + (_woodcutterLevel / 2f)));

            _woodCostText.text = _woodCost.ToString();
            _coalCostText.text = _coalCost.ToString();
            _ironCostText.text = _ironCost.ToString();

            _woodcutterLevel++;
            _woodcutterLevelText.text = _woodcutterLevel.ToString();
        }
    }
}
