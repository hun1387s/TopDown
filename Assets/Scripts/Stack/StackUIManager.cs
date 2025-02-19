using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;


public class StackUIManager : MonoBehaviour
{
    static StackUIManager instance;
    public static StackUIManager Instance
    {
        get { return instance; }
    }

    UIState currentState = UIState.Home;
    StackHomeUI homeUI = null;
    StackGameUI gameUI = null;
    StackScoreUI scoreUI = null;

    TheStack theStack = null;

    private void Awake()
    {
        instance = this;
        theStack = FindObjectOfType<TheStack>();

        homeUI = GetComponentInChildren<StackHomeUI>(true);
        homeUI?.Init(this); // homeUI가 null이 아니면 동작해라

        gameUI = GetComponentInChildren<StackGameUI>(true);
        gameUI?.Init(this); // homeUI가 null이 아니면 동작해라

        scoreUI = GetComponentInChildren<StackScoreUI>(true);
        scoreUI?.Init(this); // homeUI가 null이 아니면 동작해라

        ChangeState(UIState.Home);
    }

    public void ChangeState(UIState state)
    {
        currentState = state;
        homeUI?.SetActive(currentState);
        gameUI?.SetActive(currentState);
        scoreUI?.SetActive(currentState);
    }

    public void OnClickStart()
    {
        theStack.Restart();
        ChangeState(UIState.Game);
    }

    public void OnClickExit()
    {
        SceneManager.LoadScene(0);
    }

    public void UpdateScore()
    {
        gameUI.SetUI(theStack.Score, theStack.Combo, theStack.MaxCombo);
    }

    public void SetScoreUI()
    {
        scoreUI.SetUI(theStack.Score, theStack.MaxCombo, theStack.BestScore, theStack.BestCombo);
        ChangeState(UIState.Score);
    }


}
