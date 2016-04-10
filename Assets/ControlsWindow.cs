using UnityEngine;
using System.Collections.Generic;
using System;

public class ControlsWindow : MonoBehaviour
{
    private ConfigControlButton[] configControlButtons;

    void Awake()
    {
        configControlButtons = GetComponentsInChildren<ConfigControlButton>();
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
}
