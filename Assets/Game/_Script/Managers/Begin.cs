using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Begin : MonoBehaviour
{
    [SerializeField] private Sprite[] _beginSprites;
    [SerializeField] private SpriteRenderer _beginSprite;
    [SerializeField] private TMP_Text _nameGame;
    [SerializeField] private TMP_Text _beginText;
    [SerializeField] private Button _newGameButton;

    private void Start()
    {
        SoundManager.Instance.PlayMusic("Title");
    }

    public void OnButtonClick()
    {
        StartCoroutine(BeginText());
    }

    private IEnumerator BeginText()
    {
        _nameGame.gameObject.SetActive(false);
        _newGameButton.gameObject.SetActive(false);
        _beginSprite.sprite = _beginSprites[0];
        _beginText.text = "Раньше люди жили в мире и спокойствии, пока не пришёл ОН";
        yield return new WaitForSeconds(5f);
        _beginSprite.sprite = _beginSprites[1];
        _beginText.text = "Оно пришло, но для роста ему нужен был свет";
        yield return new WaitForSeconds(5f);
        _beginSprite.sprite = _beginSprites[2];
        _beginText.text = "Пришла тьма, весь свет пропал, а Оно стало сильнее";
        yield return new WaitForSeconds(5f);
        _beginSprite.sprite = _beginSprites[3];
        _beginText.text = "Оно росло, делая из людей свою армию";
        yield return new WaitForSeconds(5f);
        _beginSprite.sprite = _beginSprites[4];
        _beginText.text = "И лишь в одном месте люди смогли сплотиться в последний раз";
        yield return new WaitForSeconds(5f);
        _beginSprite.sprite = _beginSprites[5];
        _beginText.text = "Сможете ли вы победить или человечество погибнет вместе с вами?";
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("SampleScene");
    }
}
