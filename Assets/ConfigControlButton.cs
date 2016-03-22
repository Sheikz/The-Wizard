using UnityEngine;
using System;
using UnityEngine.UI;

public class ConfigControlButton : MonoBehaviour
{
    public InputManager.Command command;
    private enum ConfigControlButtonState {Nothing, WaitingForInput };

    private Text text;
    private ConfigControlButtonState state = ConfigControlButtonState.Nothing;

    void Awake()
    {
        text = GetComponent<Text>();
    }

    void Start()
    {
        text.text = command.ToString() + " : " + InputManager.instance.getKey(command).ToString();
    }

    void Update()
    {
        if (state == ConfigControlButtonState.Nothing)
            return;

        if (!Input.anyKey)
            return;

        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(key))
            {
                InputManager.instance.setCommand(command, key);
                state = ConfigControlButtonState.Nothing;
                refresh();
                text.color = Color.white;
                return;
            }
        }
    }

    public void refresh()
    {
        text.text = command.ToString() + " : " + InputManager.instance.getIconDescription(command);
    }

    public void onClick()
    {
        text.color = Color.green;
        state = ConfigControlButtonState.WaitingForInput;
    }
}
