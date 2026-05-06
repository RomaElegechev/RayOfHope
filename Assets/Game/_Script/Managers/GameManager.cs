using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using PrimeTween;

[Serializable]
public class Pair
{
    public float min;
    public float max;
}

[Serializable]
public class Minigame
{
    public SpriteRenderer panel;
    public TMP_Text gameText;
    public TMP_Text gameTime;
    public SpriteRenderer arrow;
    public SpriteRenderer[] winZones;
    public float timeGame;
    public Pair timeStart;
    public Pair SpeedArrow;
    public string nameMusic;
}

[Serializable]
public class Upgrade 
{
    public int firCost;
    public TMP_Text firText;
    public int secCost;
    public TMP_Text secText;
    public int stepCost;
    public TMP_Text stepText;
    public int level;
    public TMP_Text levelText;
}

[Serializable]
public class ItemNumber 
{
    public int number;
    public TMP_Text textNumber;
}

[Serializable]
public class Animators 
{
    public SpriteRenderer sprite;
    public Animator animator;
}

[Serializable]
public class EntityStats
{
    public Animator animator;
    public TMP_Text healthText;
    public TMP_Text damageText;
    public TMP_Text armorText;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }

    public int UsualNumber = 0;
    public int SoldierNumber = 0;
    public int WorkerNumber = 0;
    public int CoalNumber = 0;
    public int IronNumber = 0;
    public int FoodNumber = 0;
    public int WoodNumber = 0;

    [HideInInspector] public int StepNumber = 0;
    [HideInInspector] public bool CanEnterBuild = true;
    [HideInInspector] public int PeopleDeathsNumber = 0;
    [HideInInspector] public int NextMoveUsual;
    [HideInInspector] public int NextMoveSoldier;
    [HideInInspector] public int NextMoveWorker;

    public int PeopleNumber => UsualNumber + SoldierNumber + WorkerNumber;
    public int LaborForceNumber => (UsualNumber / 2) + WorkerNumber;
    public int ForceNumber => (UsualNumber / 2) + SoldierNumber;
    public int EarnedResourcesNumber => _earnedResourcesNumber;
    public int RepulsedAttackNumber => _repulsedAttackNumber;
    public float RunTime => Time.time - _startRunTime;


    public event Action OnStartMove;


    private int _earnedResourcesNumber = 0;
    private int _repulsedAttackNumber = 0;


    private float _startRunTime;

    private void Awake()
    {
        Instance = this;
        
        _startRunTime = Time.time;
        StepNumber = LaborForceNumber;

        NextMoveUsual = UsualNumber;
        NextMoveSoldier = SoldierNumber;
        NextMoveWorker = WorkerNumber;
    }

    private void Start()
    {
        CameraManager.Instance.OnBuildExit += OnBuildExit;

        FightManager.Instance.OnGameOver += OnGameOver;
        FightManager.Instance.OnRepulsedAttack += OnRepulsedAttack;
    }

    public void EndAction(int earnedResouces = 0)
    {
        _earnedResourcesNumber += earnedResouces;

        if (StepNumber == 0)
        {
            Interface.Instance.EnemyWarningFadeIn();
        }
    }

    private void OnBuildExit(TypeBuild typeBuild)
    {
        if (StepNumber == 0)
        {
            CanEnterBuild = false;
            FightManager.Instance.StartFight();
        }
        else
        {
            CanEnterBuild = true;
        }
    }

    private void OnRepulsedAttack() => StartMove();

    private void StartMove()
    {
        UsualNumber = NextMoveUsual;
        SoldierNumber = NextMoveSoldier;
        WorkerNumber = NextMoveWorker;

        OnStartMove?.Invoke();
        _repulsedAttackNumber++;

        NextMoveUsual = UsualNumber;
        NextMoveSoldier = SoldierNumber;
        NextMoveWorker = WorkerNumber;

        StepNumber = LaborForceNumber;
        CanEnterBuild = true;
    }

    private void OnGameOver() => print("game over");

    /*
    public static Main Instance { get; private set; }

    public event EventHandler _minigameEnd;
    public event EventHandler OnPlayerSelect;
    private MainInputAction _inputAction;

    [SerializeField] private Minigame[] _panels;
    [SerializeField] private Upgrade[] _upgrades;
    [SerializeField] private Animators[] _animators;
    [SerializeField] private ItemNumber[] _items;

    [SerializeField] private SpriteRenderer _fightPanel;
    [SerializeField] private TMP_Text _winText;

    [SerializeField] private EntityStats[] _entities;

    [SerializeField] private SpriteRenderer _upgradePanel;

    [SerializeField] private int _gameState = 1;

    private int _round = 0;

    private bool _isInMinigame = false;

    private int _minigame = 0;
    private bool _win = false;
    private int _numberWin = 0;

    private float _currentTimeMinigame = 0;

    private bool _enemyTurn = false;
    private bool _isFirstEnd = false;

    private bool _canPress = false;

    private void Awake() 
    {
        Instance = this;
    }

    private void Start() 
    {
        _inputAction = new MainInputAction();
        _inputAction.Enable();

        foreach (var gamePanel in _panels)
        {
            gamePanel.panel.gameObject.SetActive(false);
            gamePanel.gameText.enabled = false;
        }

        EndMinigame(0);
        CheckMusic();

        _inputAction.Game.Select.started += Select_started;
    }

    private void Update() 
    {
        UpdateUI();

        if (Time.time > _currentTimeMinigame && _minigame == 0 && _canPress && _isInMinigame) EndMinigame(0);

        if (Time.time > _currentTimeMinigame && _minigame == 2 && _isInMinigame) EndMinigame(2);
    }


    public void ButtonLootCoal() => MinigameButton(ApperanceMinigame(0), 0);
    public void ButtonLootIron() => MinigameButton(ApperanceMinigame(1), 1);
    public void ButtonLootFood() => MinigameButton(ApperanceMinigame(2), 2);
    public void ButtonLootWood() => MinigameButton(ApperanceMinigame(3), 3);


    private void MinigameButton(IEnumerator game, int minigameId)
    {
        if (!_isInMinigame && _items[5].number > 0 && !_enemyTurn)
        {
            SoundManager.Instance.PlaySFX("Card");
            SoundManager.Instance.PlayMusic(_panels[minigameId].nameMusic, 0.4f);

            _items[5].number--;
            if (minigameId == 0 || minigameId == 2) _currentTimeMinigame = Time.time + _panels[minigameId].timeGame;
            StartCoroutine(game);
        }
    }

    public void ButtonUpgrades()
    {
        SoundManager.Instance.PlaySFX("Card");

        /*
        if (!_isInMinigame && !_upgradePanel.gameObject.activeSelf && !_enemyTurn)
        {
            _upgradePanel.gameObject.SetActive(true);

            //LeanTween.moveY(_upgradePanel.gameObject, 0f, 0.3f)
                     //.setEase(LeanTweenType.easeOutBack);
        }
        else if (_upgradePanel.gameObject.activeSelf && !_enemyTurn) {
            LeanTween.moveY(_upgradePanel.gameObject, 9.9f, 0.3f)
                     .setEase(LeanTweenType.easeInBack)
                     .setOnComplete(() => {
                         _upgradePanel.gameObject.SetActive(false);
                         CheckEnemyTurn();
                     });
        }
    }


    public void ButtonBuildUpgrade() => ButtonUpgrade(0, _items[3].number, _items[4].number);
    public void ButtonFarmUpgrade() => ButtonUpgrade(1, _items[3].number, _items[2].number);
    public void ButtonMineUpgrade() => ButtonUpgrade(2, _items[1].number, _items[2].number);
    public void ButtonShieldUpgrade() => ButtonUpgrade(3, _items[1].number, _items[2].number);


    private void ButtonUpgrade(int upgradeID, int firNumber, int secNumber)
    {
        SoundManager.Instance.PlaySFX("Card");

        if (firNumber >= _upgrades[upgradeID].firCost && secNumber >= _upgrades[upgradeID].secCost
            && _items[5].number >= _upgrades[upgradeID].stepCost)
        {
            firNumber -= _upgrades[upgradeID].firCost;
            secNumber -= _upgrades[upgradeID].secCost;
            _items[5].number -= _upgrades[upgradeID].stepCost;

            _upgrades[upgradeID].firCost = _upgrades[upgradeID].firCost / _upgrades[upgradeID].level
                * (_upgrades[upgradeID].level + 1);
            _upgrades[upgradeID].secCost = _upgrades[upgradeID].secCost / _upgrades[upgradeID].level
                * (_upgrades[upgradeID].level + 1);
            _upgrades[upgradeID].stepCost = _upgrades[upgradeID].stepCost / _upgrades[upgradeID].level
                * (_upgrades[upgradeID].level + 1);

            _upgrades[upgradeID].level++;
        }
    }

    public void IsWin(bool state)
    {
        if (state == true) 
        {
            if (!_win) SoundManager.Instance.PlaySFX("WinGame");

            _numberWin++;
            _win = true;
        }
    }

    private void CheckMusic()
    {
        if (_gameState < 5) SoundManager.Instance.PlayMusic("Phase1");
        else if (_gameState < 10) SoundManager.Instance.PlayMusic("Phase2");
        else SoundManager.Instance.PlayMusic("Phase3");
    }

    private void CheckEnemyTurn() 
    {
        if (_items[5].number <= 0 && !_enemyTurn) StartCoroutine(Fight());
    }

    private IEnumerator Fight()
    {
        _enemyTurn = true;
        yield return new WaitForSeconds(0.05f);

        _entities[0].animator.SetLayerWeight(_gameState / 5, 1);
        _entities[0].animator.gameObject.SetActive(true);
        _winText.gameObject.SetActive(false);
        _fightPanel.gameObject.SetActive(true);

        SoundManager.Instance.PlayMusic("FIght", 0.1f);
        //LeanTween.moveY(_fightPanel.gameObject, 0f, 0.5f).setEaseOutBack();

        int enemyHealth = ((10 + 7 * (_gameState - 1)) * (int)Mathf.Pow(1.5f, (int)(_gameState / 5)));
        int enemyDamage = ((3 + 2 * (_gameState - 1)) * (int)Mathf.Pow(1.5f, (int)(_gameState / 5)));
        float enemyArmor = 0.15f * (int)(_gameState / 5);
        _entities[0].healthText.text = enemyHealth.ToString();
        _entities[0].damageText.text = enemyDamage.ToString();
        _entities[0].armorText.text = enemyArmor.ToString();

        int playerDamage = 3 * _upgrades[3].level;
        int playerHealth = 10 * (_upgrades[0].level + _upgrades[1].level + _upgrades[3].level);
        float playerArmor = Mathf.Min(0.8f, 0.05f * (_upgrades[0].level + _upgrades[2].level + _upgrades[3].level));
        _entities[1].healthText.text = playerHealth.ToString();
        _entities[1].damageText.text = playerDamage.ToString();
        _entities[1].armorText.text = playerArmor.ToString();

        yield return new WaitForSeconds(0.5f);

        while (playerHealth > 0 && enemyHealth > 0) {
            LeanTween.moveLocalY(_entities[0].animator.gameObject, 0.64f, 0.25f)
                     .setEaseInOutBack()
                     .setLoopPingPong(1);

            yield return new WaitForSeconds(0.25f);

            playerHealth -= (int)(enemyDamage * (1 - playerArmor));
            _entities[1].healthText.text = Mathf.Max(0, playerHealth).ToString();

            SoundManager.Instance.PlaySFX("Hit");

            yield return new WaitForSeconds(0.65f);

            LeanTween.moveLocalY(_entities[1].animator.gameObject, 0.5f, 0.25f)
                     .setEaseInOutBack()
                     .setLoopPingPong(1);

            yield return new WaitForSeconds(0.25f);

            enemyHealth -= (int)(playerDamage * (1 - enemyArmor));
            _entities[0].healthText.text = Mathf.Max(0, enemyHealth).ToString();

            SoundManager.Instance.PlaySFX("Damage");

            yield return new WaitForSeconds(0.25f);
        }

        _entities[0].animator.gameObject.SetActive(false);

        if (playerHealth <= 0) {
            SoundManager.Instance.PlaySFX("DefeatEnemy");
            SoundManager.Instance.PlayMusic("Title");

            _winText.text = "You lose!";
            _winText.color = Color.red;
            _winText.gameObject.SetActive(true);

            yield return new WaitForSeconds(5f);

            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            if (_gameState < 15) {
                SoundManager.Instance.PlaySFX("WinEnemy");

                _winText.text = "You win!";
                _winText.color = Color.green;
                _winText.gameObject.SetActive(true);

                yield return new WaitForSeconds(2f);

                LeanTween.moveLocalY(_fightPanel.gameObject, -1000f, 0.5f)
                         .setEaseInOutBack()
                         .setOnComplete(() => _fightPanel.gameObject.SetActive(false));

                _gameState++;
                _enemyTurn = false;

                _animators[_animators.Length - 1].animator.SetLayerWeight(Mathf.Min(2, _gameState / 5), 1);
                CheckMusic();

                _items[0].number += _upgrades[0].level;
                _items[5].number = _items[0].number;
            }
            else {
                SoundManager.Instance.PlaySFX("WinFinalEnemy");

                _winText.text = "You defeat Evil!";
                _winText.color = Color.green;
                _winText.gameObject.SetActive(true);

                SoundManager.Instance.PlayMusic("Title");

                yield return new WaitForSeconds(7.5f);
                SceneManager.LoadScene("SampleScene");
            }
        }
    }

    private IEnumerator ApperanceMinigame(int minigameID)
    {
        if (((minigameID == 1 || minigameID == 3) && _round < _panels[minigameID].timeGame)
            || ((minigameID == 0 || minigameID == 2) && _currentTimeMinigame > Time.time))
        {
            _panels[minigameID].panel.gameObject.SetActive(true);
            LeanTween.moveLocalY(_panels[minigameID].panel.gameObject, 0, 0.3f)
                     .setEase(LeanTweenType.easeOutBack);
            yield return new WaitForSeconds(0.3f);

            _isInMinigame = true;
            _minigame = minigameID;

            _round++;
            if (minigameID == 1 || minigameID == 2)
            { 
                _panels[minigameID].gameTime.text = _round.ToString();

                LeanTween.moveLocalX(_panels[minigameID].arrow.gameObject, -12.12f, 0);

                foreach (var zona in _panels[minigameID].winZones)
                    LeanTween.moveLocalX(zona.gameObject, UnityEngine.Random.Range(-12f, 12f), 0);
            }
            else if (minigameID == 0 || minigameID == 3)
            {
                LeanTween.moveLocalX(_panels[minigameID].arrow.gameObject, -12.12f, 0);
                LeanTween.moveLocalY(_panels[minigameID].arrow.gameObject, 12.95f, 0);

                if (minigameID == 3 || !_isFirstEnd)
                {
                    foreach (var zona in _panels[minigameID].winZones)
                    {
                        LeanTween.moveLocalX(zona.gameObject, UnityEngine.Random.Range(-12f, 12f), 0);
                        LeanTween.moveLocalY(zona.gameObject, -8.67f, 0);
                    }
                }
            }

            LeanTween.moveY(_panels[minigameID].panel.gameObject, 0f, 0.3f)
                     .setEase(LeanTweenType.easeOutBack);

            float timeStartGame =
                UnityEngine.Random.Range(_panels[minigameID].timeStart.min, _panels[minigameID].timeStart.max);

            if (minigameID != 2 || !_isFirstEnd)
            {
                StartCoroutine(TextApperance(minigameID, timeStartGame));
                yield return new WaitForSeconds(timeStartGame);
            }

            _canPress = true;
            if ((minigameID == 1 && UnityEngine.Random.Range(0, 2) == 1) || minigameID == 2)
            {
                LeanTween.moveLocalX(_panels[minigameID].arrow.gameObject, 12.12f,
                  UnityEngine.Random.Range(_panels[minigameID].SpeedArrow.min, _panels[minigameID].SpeedArrow.max))
                  .setEase(LeanTweenType.easeInSine)
                  .setLoopClamp();
            }
            else
            {
                LeanTween.moveLocalX(_panels[minigameID].arrow.gameObject, 12.12f,
                    UnityEngine.Random.Range(_panels[minigameID].SpeedArrow.min, _panels[minigameID].SpeedArrow.max))
                    .setEase(LeanTweenType.easeInOutSine)
                    .setLoopPingPong();
            }
        }
        else EndMinigame(minigameID);
    }

    private IEnumerator TextApperance(int minigameId, float timeToChange) {
        _panels[minigameId].gameText.text = "Ready?";
        _panels[minigameId].gameText.color = Color.yellow;
        _panels[minigameId].gameText.enabled = true;

        yield return new WaitForSeconds(timeToChange);

        _panels[minigameId].gameText.text = "Click!";
        _panels[minigameId].gameText.color = Color.red;
        _canPress = true;

        yield return new WaitForSeconds(0.75f);

        _panels[minigameId].gameText.enabled = false;
    }

    private void EndMinigame(int minigameID) {
        _minigame = -1;
        _isInMinigame = false;
        _currentTimeMinigame = 0;
        _round = 0;
        _numberWin = 0;
        _win = false;
        _canPress = false;
        LeanTween.moveY(_panels[minigameID].panel.gameObject, 9.9f, 0.3f)
                 .setEase(LeanTweenType.easeInBack)
                 .setOnComplete(() => {
                     _panels[minigameID].arrow.gameObject.LeanCancel();
                     _panels[minigameID].panel.gameObject.SetActive(false);
                     _panels[minigameID].gameText.enabled = false;
                     _items[minigameID + 1].number += _numberWin;
                     CheckEnemyTurn();
                     CheckMusic();
                 });
    }

    private void Select_started(InputAction.CallbackContext obj) {
        OnPlayerSelect?.Invoke(this, EventArgs.Empty);

        if (_isInMinigame)
        {
            _panels[_minigame].arrow.gameObject.LeanCancel();

            if (_minigame == 0 && _canPress) StartCoroutine(CheckWin(0, ApperanceMinigame(0)));
            else if (_minigame == 1 && _canPress) StartCoroutine(CheckWin(1, ApperanceMinigame(1)));
            else if (_minigame == 2 && _isInMinigame) {
                _minigameEnd?.Invoke(this, EventArgs.Empty);
                _isFirstEnd = true;
                StartCoroutine(ApperanceMinigame(2));
            }
            else if (_minigame == 3 && _canPress) StartCoroutine(CheckWin(3, ApperanceMinigame(3)));
        }
    }

    private IEnumerator CheckWin(int minigameID, IEnumerator game) {
        _canPress = false;
        _isFirstEnd = true;
        float timeNextGame = 0f;

        if (minigameID == 0 || minigameID == 3) {
            LeanTween.scaleY(_panels[minigameID].arrow.gameObject, 2.61f, 0.5f);
            yield return new WaitForSeconds(0.5f);
        }

        _minigameEnd?.Invoke(this, EventArgs.Empty);
        yield return new WaitForSeconds(0.02f);

        if (_win) {
            _panels[minigameID].gameText.text = "Win";
            _panels[minigameID].gameText.color = Color.green;

            SoundManager.Instance.PlaySFX("WinGame");

            if (minigameID == 0 || minigameID == 3) {
                LeanTween.scaleY(_panels[minigameID].arrow.gameObject, 0.15f, 0.3f)
                         .setOnComplete(() => LeanTween.scaleY(_panels[minigameID].arrow.gameObject, 0.7f, 0.1f));

                timeNextGame += 0.4f;
            }

            if (minigameID == 0 && _numberWin == _panels[minigameID].winZones.Length) _currentTimeMinigame = 0;
        }
        else {
            _panels[minigameID].gameText.text = "Lose";
            _panels[minigameID].gameText.color = Color.red;

            SoundManager.Instance.PlaySFX("DefeatGame");

            if (minigameID == 0 || minigameID == 3) {
                LeanTween.scaleY(_panels[minigameID].arrow.gameObject, 0.7f, 0.3f);

                timeNextGame += 0.3f;
            }
        }

        _panels[minigameID].gameText.enabled = true;
        yield return new WaitForSeconds(timeNextGame + 0.05f);
        _win = false;

        StartCoroutine(game);
    }

    private void UpdateUI() {
        foreach (var item in _items) item.textNumber.text = item.number.ToString();

        for (int i = 0; i < _animators.Length - 1; i++)
            _animators[i].animator.SetLayerWeight(_upgrades[i].level - 1, 1);

        foreach (var cardUpgrade in _upgrades) {
            cardUpgrade.firText.text = cardUpgrade.firCost.ToString();
            cardUpgrade.secText.text = cardUpgrade.secCost.ToString();
            cardUpgrade.stepText.text = cardUpgrade.stepCost.ToString() + " ход";
            cardUpgrade.levelText.text = cardUpgrade.level.ToString();
        }

        _panels[0].gameTime.text = Mathf.Max(0, ((int)(_currentTimeMinigame - Time.time))).ToString();
        _panels[2].gameTime.text = Mathf.Max(0, ((int)(_currentTimeMinigame - Time.time))).ToString();
    }
    */
}

