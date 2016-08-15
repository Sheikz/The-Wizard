using UnityEngine;
using System.Collections.Generic;
using System;

public class ControlsWindow : MonoBehaviour
{
    private ConfigControlButton[] configControlButtons;
    private bool m_isOpen = false;
    public bool isOpen {
        get
        {
            return m_isOpen;
        }
    }

    void Awake()
    {
        configControlButtons = GetComponentsInChildren<ConfigControlButton>();
    }

    void OnEnable()
    {
        m_isOpen = true;
    }

    void OnDisable()
    {
        m_isOpen = false;
        cancelInput();
    }

    public void cancelInput()
    {
        foreach (ConfigControlButton button in configControlButtons)
        {
            button.cancelInput();
        }
    }

    internal void refresh()
    {
        if (configControlButtons == null)
            return;

        foreach (ConfigControlButton button in configControlButtons)
        {
            button.refresh();
        }
    }

    public void clickOK()
    {
        SoundManager.instance.playSound("ClickOK");
        UIManager.instance.openControlsWindow(false);
    }

    public void clickDefaults()
    {
        SoundManager.instance.playSound("ClickOK");
        InputManager.instance.setupDefaults();
        InputManager.instance.saveDataToPlayerPrefs();
        InputManager.instance.refresh();
    }
}
