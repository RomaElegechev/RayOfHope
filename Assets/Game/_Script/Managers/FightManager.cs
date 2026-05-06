using UnityEngine;
using System.Collections;
using PrimeTween;
using TMPro;

public enum FightState { Nothing, Preparation, Fight}

[System.Serializable]
public class EntitySprites
{
    public Sprite sprite;
    public int numberValid;
}

public class FightManager : MonoBehaviour
{
    public static FightManager Instance { get; private set; }

    [Header("Fight")]
    [SerializeField] private GameObject _fightPanel;
    [SerializeField] private float _panelDuration = 0.2f;
    [Header("People: Animation")]
    [SerializeField] private EntitySprites[] _peopleSprites;
    [SerializeField] private float _peopleDuration = 0.5f;
    [SerializeField] private float _peopleFightPosY = 10f;
    [SerializeField] private SpriteRenderer _people;
    [Header("People: Stats")]
    [SerializeField] private TMP_Text _peopleHealthText;
    [SerializeField] private TMP_Text _peopleDamageText;
    [SerializeField] private int _initialPeopleHealth = 15;
    [SerializeField] private int _initialePeopleDamage = 3;
    [Header("Enemy: Animation")]
    [SerializeField] private EntitySprites[] _enemySprites;
    [SerializeField] private float _enemyDuration = 0.5f;
    [SerializeField] private float _enemyFightPosY = -10f;
    [SerializeField] private SpriteRenderer _enemy;
    [Header("Enemy: Stats")]
    [SerializeField] private TMP_Text _enemyHealthText;
    [SerializeField] private TMP_Text _enemyDamageText;
    [SerializeField] private int _initialEnemyHealth = 10;
    [SerializeField] private int _initialEnemyDamage = 2;

    public FightState State => _state;

    private FightState _state;

    private float _peopleInitialPosY;
    private int _peopleHealth;
    private int _peopleDamage;
    private int _currentPeopleHealth;

    private float _enemyInitialPosY;
    private int _enemyHealth;
    private int _enemyDamage;
    private int _currentEnemyHealth;

    public event System.Action OnFightStart;
    public event System.Action OnFightEnd;

    public event System.Action OnGameOver;
    public event System.Action OnRepulsedAttack;

    private void Awake()
    {
        Instance = this;
    }

    public void StartFight()
    {
        StartCoroutine(FightPreparation());
    }

    private IEnumerator FightPreparation()
    {
        yield return new WaitForSeconds(1.25f);
        Interface.Instance.InterfaceFadeOut();

        yield return new WaitForSeconds(0.75f);
        OnFightStart?.Invoke();

        SetEntytiesSprite();
        StartCoroutine(UpdateFightUI());

        _peopleInitialPosY = _people.transform.localPosition.y;
        _enemyInitialPosY = _enemy.transform.localPosition.y;

        _peopleHealth = _initialPeopleHealth * Lighthouse.Instance.LighthouseLevel * GameManager.Instance.ForceNumber;
        _peopleDamage = _initialePeopleDamage * Lighthouse.Instance.LighthouseLevel * GameManager.Instance.ForceNumber;
        _currentPeopleHealth = _peopleHealth;

        _enemyHealth = (int)(_initialEnemyHealth * (1f + GameManager.Instance.RepulsedAttackNumber / 2f));
        _enemyDamage = (int)(_initialEnemyDamage * (1f + GameManager.Instance.RepulsedAttackNumber / 2f));
        _currentEnemyHealth = _enemyHealth;

        yield return new WaitForSeconds(Random.Range(2.5f, 4f));

        _fightPanel.SetActive(true);
        Tween.LocalPositionY(_fightPanel.transform, 0f, _panelDuration, Ease.OutBack)
            .OnComplete(() => StartCoroutine(Fight()));
    }

    private IEnumerator Fight()
    {
        while (_currentEnemyHealth > 0 && _currentPeopleHealth > 0)
        {
            yield return Tween.LocalPositionY(_people.transform, _peopleFightPosY, _peopleDuration, Ease.OutBack)
                .ToYieldInstruction();

            _currentEnemyHealth -= _peopleDamage;

            yield return Tween.LocalPositionY(_enemy.transform, _enemy.transform.localPosition.y + 1f, _enemyDuration / 4f, Ease.InOutBack, 2, CycleMode.Yoyo)
                .ToYieldInstruction();

            yield return Tween.LocalPositionY(_people.transform, _peopleInitialPosY, _peopleDuration, Ease.InBack)
                .ToYieldInstruction();

            yield return Tween.LocalPositionY(_enemy.transform, _enemyFightPosY, _enemyDuration, Ease.OutBack)
                .ToYieldInstruction();

            _currentPeopleHealth -= _enemyDamage;

            yield return Tween.LocalPositionY(_people.transform, _people.transform.localPosition.y - 1f, _peopleDuration / 4f, Ease.InOutBack, 2, CycleMode.Yoyo)
                .ToYieldInstruction();

            yield return Tween.LocalPositionY(_enemy.transform, _enemyInitialPosY, _enemyDuration, Ease.InBack)
                .ToYieldInstruction();
        }

        if (_currentEnemyHealth <= 0) yield return StartCoroutine(EnemyFadeOut());
        if (_currentPeopleHealth <= 0) yield return StartCoroutine(PeopleFadeOut());

        StartCoroutine(FightEnd());
    }

    private IEnumerator FightEnd()
    {
        StopCoroutine(UpdateFightUI());

        if (_currentPeopleHealth <= 0)
        {
            OnGameOver?.Invoke();
            yield break;
        }

        int deaths = (int)((_currentPeopleHealth / (float)_peopleHealth) * (GameManager.Instance.UsualNumber + GameManager.Instance.SoldierNumber));
        GameManager.Instance.PeopleDeathsNumber += deaths;

        if (GameManager.Instance.SoldierNumber - (deaths - Mathf.FloorToInt(deaths / 2f)) >= 0 
             && GameManager.Instance.UsualNumber - Mathf.FloorToInt(deaths / 2f) >= 0)
        {
            GameManager.Instance.SoldierNumber -= deaths - Mathf.FloorToInt(deaths / 2f);
            GameManager.Instance.UsualNumber -= Mathf.FloorToInt(deaths / 2f);
        }
        else if (GameManager.Instance.SoldierNumber - (deaths - Mathf.FloorToInt(deaths / 2f)) > 0)
        {
            GameManager.Instance.SoldierNumber = Mathf.Max(0, GameManager.Instance.SoldierNumber - deaths);
        }
        else
        {
            GameManager.Instance.UsualNumber = Mathf.Max(0, GameManager.Instance.UsualNumber - deaths);
        }

        OnRepulsedAttack?.Invoke();

        yield return Tween.LocalPositionY(_fightPanel.transform, -1050f, _panelDuration / 2, Ease.InBack)
           .ToYieldInstruction();

        _enemy.color = Color.white;
        Interface.Instance.InterfaceFadeIn();

        yield return new WaitForSeconds(1);
        OnFightEnd?.Invoke();
    }

    private IEnumerator PeopleFadeOut()
    {
        float time = 0;
        float percent;

        while (time < _peopleDuration * 2)
        {
            time += Time.deltaTime;
            percent = time / _peopleDuration;

            _people.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), percent);

            yield return null;
        }

        _people.color = new Color(1, 1, 1, 0);
    }

    private IEnumerator EnemyFadeOut()
    {
        float time = 0;
        float percent;

        while (time < _enemyDuration * 2)
        {
            time += Time.deltaTime;
            percent = time / _peopleDuration;

            _enemy.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), percent);

            yield return null;
        }

        _enemy.color = new Color(1, 1, 1, 0);
    }

    private IEnumerator UpdateFightUI()
    {
        while (true)
        {
            _peopleHealthText.text = _currentPeopleHealth.ToString();
            _peopleDamageText.text = _peopleDamage.ToString();

            _enemyHealthText.text = _currentEnemyHealth.ToString();
            _enemyDamageText.text = _enemyDamage.ToString();

            yield return null;
        }
    }

    private void SetEntytiesSprite()
    {
        if (GameManager.Instance.ForceNumber > _peopleSprites[_peopleSprites.Length - 1].numberValid)
        {
            _people.sprite = _peopleSprites[_peopleSprites.Length - 1].sprite;
        }
        else
        {
            for (int i = 0; i < _peopleSprites.Length; i++)
            {
                if (GameManager.Instance.ForceNumber < _peopleSprites[i].numberValid)
                {
                    _people.sprite = _peopleSprites[Mathf.Max(0, i - 1)].sprite;
                    break;
                }
            }
        }
        
        if (GameManager.Instance.RepulsedAttackNumber > _enemySprites[_enemySprites.Length - 1].numberValid)
        {
            _enemy.sprite = _enemySprites[_enemySprites.Length - 1].sprite;
        }
        else
        {
            for (int i = 0; i < _enemySprites.Length; i++)
            {
                if (GameManager.Instance.RepulsedAttackNumber < _enemySprites[i].numberValid)
                {
                    _enemy.sprite = _enemySprites[Mathf.Max(0, i - 1)].sprite;
                    break;
                }
            }
        }
    }
}
