using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    protected UIManager uiManager;
    public virtual void Init(UIManager uiMgr)
    {
        this.uiManager = uiMgr;
    }

    protected abstract UIState GetUIState();
    public void SetActive(UIState state)
    {
        this.gameObject.SetActive(GetUIState() == state);
    }
}
