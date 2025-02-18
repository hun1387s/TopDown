using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeUI : BaseUI
{
    [SerializeField] Button startButton;
    [SerializeField] Button exitButton;

    public override void Init(UIManager uiMgr)
    {
        base.Init(uiMgr);

        startButton.onClick.AddListener(OnClickStartBtn);
        exitButton.onClick.AddListener(OnClickExitBtn);
    }

    public void OnClickStartBtn()
    {
        GameManager.Instance.StartGame();
    }

    public void OnClickExitBtn()
    {
        Application.Quit();
    }
    protected override UIState GetUIState()
    {
        return UIState.Home;
    }
}
