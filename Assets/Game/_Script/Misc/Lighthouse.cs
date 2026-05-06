using PrimeTween;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum LighthouseAction { Building, Distribution, Information, Upgrade, FoodPuting }

[System.Serializable]
public class LighthousePanel
{
    public GameObject panel;
    public LighthouseAction action;
    public SpriteRenderer selectButtonSprite;
}

public class Lighthouse : MonoBehaviour
{
    public static Lighthouse Instance { get; private set; }

    [SerializeField] private LighthousePanel[] _activities;
    [Header("Animation")]
    [SerializeField] private Color _unactiveColor = new Color(0.541f, 0.541f, 0.541f, 1f);
    [SerializeField] private float _panelDuration;
    [Header("Passive")]
    [SerializeField] private int _passivePeople = 1;
    [SerializeField] private int _passiveFood = 1;
    [SerializeField] private int _passiveWood = 1;
    [Header("Information")]
    [SerializeField] private TMP_Text _peopleNumberText;
    [SerializeField] private TMP_Text _repulsedAttackNumberText;
    [SerializeField] private TMP_Text _timeRunText;
    [SerializeField] private TMP_Text _peopleDeathsNumberText;
    [SerializeField] private TMP_Text _earnedResourcesNumberText;
    [SerializeField] private TMP_Text _currentDistributionText;
    [Header("Distribution")]
    [SerializeField] private TMP_Text _distibutionText;
    [SerializeField] private Slider _usualSlider;
    [SerializeField] private Slider _soldierSlider;
    [SerializeField] private Slider _workerSlider;
    [SerializeField] private TMP_Text _usualNumberText;
    [SerializeField] private TMP_Text _soldierNumberText;
    [SerializeField] private TMP_Text _workerNumberText;
    [Header("FoodPutting")]
    [SerializeField] private Slider _foodPuttingSlider;
    [SerializeField] private TMP_Text _foodPuttingText;
    [SerializeField] private int _foodInLighthouse = 0;
    [SerializeField] private TMP_Text _foodLighthouseNextMoveText;
    [Header("Level")]
    [SerializeField] private int _lighthouseLevel = 1;
    [SerializeField] private TMP_Text _lighthouseLevelText;
    [Header("Upgrade")]
    [SerializeField] private GameObject _ironCostObject;
    [SerializeField] private GameObject _coalCostObject;
    [SerializeField] private TMP_Text _woodCostText;
    [SerializeField] private TMP_Text _foodCostText;
    [SerializeField] private TMP_Text _ironCostText;
    [SerializeField] private TMP_Text _coalCostText;
    [SerializeField] private int _initialWoodCost = 3;
    [SerializeField] private int _initialFoodCost = 5;
    [SerializeField] private int _initialCoalCost = 5;
    [SerializeField] private int _initialIronCost = 2;

    public int LighthouseLevel => _lighthouseLevel;
    public int FoodLighthouse => _foodInLighthouse;

    private Coroutine _currentCoroutine;

    private int _woodCost;
    private int _foodCost;
    private int _ironCost;
    private int _coalCost;

    private int _nextMoveUsual;
    private int _nextMoveSoldier;
    private int _nextMoveWorker;

    private int _currentFoodPutting = 0;

    private LighthousePanel _currentActivity;

    private void Awake()
    {
        Instance = this;

        _woodCost = _initialWoodCost;
        _foodCost = _initialFoodCost;
        _ironCost = 0;
        _coalCost = 0;

        _woodCostText.text = _woodCost.ToString();
        _foodCostText.text = _foodCost.ToString();
        _ironCostText.text = _ironCost.ToString();
        _coalCostText.text = _coalCost.ToString();
    }

    private void Start()
    {
        GameManager.Instance.OnStartMove += OnStartMove;
    }

    private void OnStartMove()
    {
        if (_foodInLighthouse - GameManager.Instance.PeopleNumber > 0)
        {
            _foodInLighthouse -= GameManager.Instance.PeopleNumber;
            GameManager.Instance.UsualNumber += _passivePeople * _lighthouseLevel;
        }
        else
        {
            _foodInLighthouse = 0;
        }

        GameManager.Instance.FoodNumber += _passiveFood;
        GameManager.Instance.WoodNumber += _passiveWood;
    }

    public void SelectActivity(LighthouseAction action)
    {
        if (_currentActivity == null)
        {
            _currentActivity = _activities.FirstOrDefault(panel => panel.action == action);

            if (_currentActivity.action == LighthouseAction.Information)
            {
                _peopleNumberText.text = GameManager.Instance.PeopleNumber.ToString();
                _repulsedAttackNumberText.text = GameManager.Instance.RepulsedAttackNumber.ToString();

                int minutes = Mathf.FloorToInt(GameManager.Instance.RunTime / 60f);
                int seconds = Mathf.FloorToInt(GameManager.Instance.RunTime % 60f);
                _timeRunText.text = $"{minutes:00}:{seconds:00}";

                _peopleDeathsNumberText.text = GameManager.Instance.PeopleDeathsNumber.ToString();
                _earnedResourcesNumberText.text = GameManager.Instance.EarnedResourcesNumber.ToString();
                _currentDistributionText.text =
                    $"{GameManager.Instance.UsualNumber} Обычных, {GameManager.Instance.SoldierNumber} Военных, {GameManager.Instance.WorkerNumber} Работников";
            }
            else if (_currentActivity.action == LighthouseAction.Distribution)
            {
                _nextMoveUsual = GameManager.Instance.UsualNumber;
                _nextMoveSoldier = GameManager.Instance.SoldierNumber;
                _nextMoveWorker = GameManager.Instance.WorkerNumber;

                _distibutionText.text =
                    $"В следующем ходу: {_nextMoveUsual}+{_passivePeople * _lighthouseLevel} Обычных, {_nextMoveSoldier} Военных, {_nextMoveWorker} Работников";
                _currentCoroutine = StartCoroutine(Distribution());
            }
            else if (_currentActivity.action == LighthouseAction.FoodPuting)
            {
                _foodLighthouseNextMoveText.text = $"{_foodInLighthouse}-{GameManager.Instance.PeopleNumber}";

                if (_foodInLighthouse - GameManager.Instance.PeopleNumber >= 0) _foodLighthouseNextMoveText.color = Color.black;
                else _foodLighthouseNextMoveText.color = Color.red;

                _currentCoroutine = StartCoroutine(FoodPuting());
            }
            else if (_currentActivity.action == LighthouseAction.Upgrade)
            {
                if ((GameManager.Instance.FoodNumber > _foodCost && GameManager.Instance.WoodNumber > _woodCost
                    && GameManager.Instance.IronNumber > _ironCost && GameManager.Instance.CoalNumber > _coalCost))
                {
                    _currentActivity.selectButtonSprite.color = Color.white;
                }
                else _currentActivity.selectButtonSprite.color = _unactiveColor;
            }

            _currentActivity.panel.SetActive(true);
            Tween.LocalPositionY(_currentActivity.panel.transform, 0f, _panelDuration, Ease.OutBack);
        }
    }

    public void DisableActivity(LighthouseAction action)
    {
        if (_currentActivity.action == action)
        {
            if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);

            Tween.LocalPositionY(_currentActivity.panel.transform, 1050f, _panelDuration, Ease.InBack)
                .OnComplete(() => {
                    _currentActivity.panel.SetActive(false);
                    _currentActivity = null;
                });
        }
    }

    public void StartAction(LighthouseAction action)
    {
        switch (action)
        {
            case LighthouseAction.Building: Building(); break;
            case LighthouseAction.Distribution: SelectDistribution(); break;
            case LighthouseAction.Information: break;
            case LighthouseAction.FoodPuting: FoodPutingSelect(); break;
            case LighthouseAction.Upgrade: Upgrade(); break;
        }
    }

    private void Building()
    {
        
    }

    private IEnumerator Distribution()
    {
        int total = GameManager.Instance.PeopleNumber;

        while (true)
        {
            if (_nextMoveSoldier != 0 || _nextMoveWorker != 0 || _nextMoveUsual != 0) _currentActivity.selectButtonSprite.color = Color.white;
            else _currentActivity.selectButtonSprite.color = _unactiveColor;

            float[] weights = new float[3] { _usualSlider.value, _soldierSlider.value, _workerSlider.value };

            float[] raw = new float[3];
            float sum = 0f;
            for (int i = 0; i < 3; i++)
            {
                raw[i] = weights[i] * total;
                sum += raw[i];
            }

            int[] final = new int[3];

            if (sum > 0f)
            {
                int assigned = 0;
                float[] remainders = new float[3];
                for (int i = 0; i < 3; i++)
                {
                    float exact = raw[i] * total / sum;
                    final[i] = Mathf.FloorToInt(exact);
                    remainders[i] = exact - final[i];
                    assigned += final[i];
                }

                int left = total - assigned;
                var indices = new int[3] { 0, 1, 2 };
                System.Array.Sort(indices, (a, b) => remainders[b].CompareTo(remainders[a]));
                for (int i = 0; i < left; i++)
                {
                    final[indices[i]]++;
                }
            }

            _nextMoveUsual = final[0];
            _nextMoveSoldier = final[1];
            _nextMoveWorker = final[2];

            _usualNumberText.text = final[0].ToString();
            _soldierNumberText.text = final[1].ToString();
            _workerNumberText.text = final[2].ToString();

            yield return null;
        }
    }

    private void SelectDistribution()
    {
        if (_nextMoveSoldier != 0 || _nextMoveWorker != 0 || _nextMoveUsual != 0)
        {
            _distibutionText.text =
             $"В следующем ходу: {_nextMoveUsual}+{_passivePeople * _lighthouseLevel} Обычных, {_nextMoveSoldier} Военных, {_nextMoveWorker} Работников";

            GameManager.Instance.NextMoveUsual = _nextMoveUsual;
            GameManager.Instance.NextMoveSoldier = _nextMoveSoldier;
            GameManager.Instance.NextMoveWorker = _nextMoveWorker;
        }
    }

    private IEnumerator FoodPuting()
    {
        while (true)
        {
            _currentFoodPutting = (int)(_foodPuttingSlider.value * GameManager.Instance.FoodNumber);

            _currentActivity.selectButtonSprite.color = _currentFoodPutting > 0 ? Color.white : _unactiveColor;

            _foodPuttingText.text = _currentFoodPutting.ToString();

            yield return null;
        }
    }

    private void FoodPutingSelect()
    {
        if (_currentFoodPutting > 0)
        {
            _foodInLighthouse += _currentFoodPutting;
            GameManager.Instance.FoodNumber -= _currentFoodPutting;

            _foodLighthouseNextMoveText.text = $"{_foodInLighthouse}-{GameManager.Instance.PeopleNumber}";

            if (_foodInLighthouse - GameManager.Instance.PeopleNumber >= 0) _foodLighthouseNextMoveText.color = Color.black;
            else _foodLighthouseNextMoveText.color = Color.red;
        }
    }

    private void Upgrade()
    {
        if (GameManager.Instance.FoodNumber > _foodCost && GameManager.Instance.WoodNumber > _woodCost 
             && GameManager.Instance.IronNumber > _ironCost && GameManager.Instance.CoalNumber > _coalCost)
        {
            GameManager.Instance.FoodNumber -= _foodCost;
            GameManager.Instance.WoodNumber -= _woodCost;
            GameManager.Instance.IronNumber -= _ironCost;
            GameManager.Instance.CoalNumber -= _coalCost;

            _foodCost = (int)(_initialFoodCost * (1f + (_lighthouseLevel / 2f)));
            _woodCost = (int)(_initialWoodCost * (1f + (_lighthouseLevel / 2f)));
            _ironCost = (int)(_initialIronCost * (0.5f + (_lighthouseLevel / 2f)));
            _coalCost = (int)(_initialCoalCost * (0.5f + (_lighthouseLevel / 2f)));

            _ironCostObject.SetActive(true);
            _coalCostObject.SetActive(true);

            _foodCostText.text = _foodCost.ToString();
            _woodCostText.text = _woodCost.ToString();
            _ironCostText.text = _ironCost.ToString();
            _coalCostText.text = _coalCost.ToString();

            _lighthouseLevel++;
            _lighthouseLevelText.text = _lighthouseLevel.ToString();
        }
    }
}
