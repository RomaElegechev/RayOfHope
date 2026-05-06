using System;
using UnityEngine;

public class WinZonaCoal : MonoBehaviour
{
    [SerializeField] private int _type;
    private bool _win;

    private void Start()
    {
        //Main.Instance._minigameEnd += Main__minigameEnd;
    }
    private void Update()
    {

    }

    private void Main__minigameEnd(object sender, EventArgs e)
    {
        //Main.Instance.IsWin(_win);

        if (_win && _type == 2)
        {
            //LeanTween.moveLocalY(gameObject, 12.14f, 0.3f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent(out MinigameArrow arrow))
        {
            _win = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent(out MinigameArrow arrow))
        {
            _win = false;
        }
    }
}
