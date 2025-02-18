using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : BaseUI
{
    [SerializeField] Button restartButton;
    [SerializeField] Button exitButton;

    public override void Init(UIManager uiMgr)
    {
        base.Init(uiMgr);

        restartButton.onClick.AddListener(OnClickRestartBtn);
        exitButton.onClick.AddListener(OnClickExitBtn);
    }

    public void OnClickRestartBtn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnClickExitBtn()
    {
        Application.Quit();
    }
    protected override UIState GetUIState()
    {
        return UIState.GameOver;
    }
}
