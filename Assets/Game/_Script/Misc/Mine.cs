using PrimeTween;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MineAction { IronMinigame, IronSend, CoalMinigame, CoalSend, Upgrade}

[System.Serializable]
public class MinePanel
{
    public MineAction action;
    public GameObject panel;
    public Slider slider;
    public TMP_Text sliderText;
    public SpriteRenderer selectButtonSprite;
    public GameObject arrow;
}

public class Mine : MonoBehaviour
{
    public static Mine Instance { get; private set; }

    [SerializeField] private MinePanel[] _activitiesPanels;
    [Header("Animation")]
    [SerializeField] private Color _unactiveColor = new Color(0.541f, 0.541f, 0.541f, 1f);
    [SerializeField] private float _panelDuration;
    [Header("Coal")]
    [SerializeField] private int _coalSend = 4;
    [SerializeField] private int _minCoalMinigame = 0;
    [SerializeField] private int _maxCoalMinigame = 8;
    [Header("Iron")]
    [SerializeField] private int _ironSend = 2;
    [SerializeField] private int _minIronMinigame = 0;
    [SerializeField] private int _maxIronMinigame = 4;
    [Header("Level")]
    [SerializeField] private int _mineLevel = 1;
    [SerializeField] private TMP_Text _mineLevelText;
    [Header("Upgrade")]
    [SerializeField] private TMP_Text _woodCostText;
    [SerializeField] private TMP_Text _coalCostText;
    [SerializeField] private TMP_Text _ironCostText;
    [SerializeField] private int _initialWoodCost = 4;
    [SerializeField] private int _initialCoalCost = 6;
    [SerializeField] private int _initialIronCost = 3;

    public int MineLevel => _mineLevel;

    private int _woodCost;
    private int _coalCost;
    private int _ironCost;

    private MinePanel _currentActivity;

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

    public void SelectAction(MineAction action)
    {
        if (_currentActivity == null)
        {
            _currentActivity = _activitiesPanels.FirstOrDefault(panel => panel.action == action);

            if (_currentActivity.action == MineAction.Upgrade)
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

    public void DisableAction(MineAction action)
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

    public void StartAction(MineAction action)
    {
        switch (action)
        {
            case MineAction.CoalSend: CoalSend(); break;
            case MineAction.CoalMinigame: CoalMinigame(); break;
            case MineAction.IronSend: IronSend(); break;
            case MineAction.IronMinigame: IronMinigame(); break;
            case MineAction.Upgrade: UpgradeCost(); break;
        }
    }

    private void UpdatePanelUI()
    {
        if (_currentActivity != null && _currentActivity.action != MineAction.Upgrade)
        {
            int numberLaborForce = ((int)(GameManager.Instance.LaborForceNumber * _currentActivity.slider.value));

            _currentActivity.selectButtonSprite.color = 
                numberLaborForce > 0 && GameManager.Instance.StepNumber - numberLaborForce >= 0 ? Color.white : _unactiveColor;
            _currentActivity.sliderText.text = numberLaborForce.ToString();
        }
    }

    private void CoalSend()
    {
        int numberLaborForce = ((int)(GameManager.Instance.LaborForceNumber * _currentActivity.slider.value));

        if (numberLaborForce > 0 && GameManager.Instance.StepNumber - numberLaborForce >= 0)
        {
            GameManager.Instance.CoalNumber += _coalSend * numberLaborForce;
            GameManager.Instance.StepNumber -= numberLaborForce;

            GameManager.Instance.EndAction(_coalSend * numberLaborForce);
        }
    }

    private void CoalMinigame()
    {

    }

    private void IronSend()
    {
        int numberLaborForce = ((int)(GameManager.Instance.LaborForceNumber * _currentActivity.slider.value));

        if (numberLaborForce > 0 && GameManager.Instance.StepNumber - numberLaborForce >= 0)
        {
            GameManager.Instance.IronNumber += _ironSend * numberLaborForce;
            GameManager.Instance.StepNumber -= numberLaborForce;

            GameManager.Instance.EndAction(_ironSend * numberLaborForce);
        }
    }

    private void IronMinigame()
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

            _woodCost = (int)(_initialWoodCost * (1f + (_mineLevel / 2f)));
            _coalCost = (int)(_initialCoalCost * (1f + (_mineLevel / 2f)));
            _ironCost = (int)(_initialIronCost * (1f + (_mineLevel / 2f)));
            
            _woodCostText.text = _woodCost.ToString();
            _coalCostText.text = _coalCost.ToString();
            _ironCostText.text = _ironCost.ToString();

            _mineLevel++;
            _mineLevelText.text = _mineLevel.ToString();    
        }
    }
}
