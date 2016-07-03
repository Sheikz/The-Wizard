using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConfigControlButton : MonoBehaviour
{
    public InputManager.Command command;
    public InputManager.KeyType keyType;
    private enum ConfigControlButtonState {Nothing, WaitingForInput };

    private Text text;

    private ConfigControlButtonState state = ConfigControlButtonState.Nothing;
    private ControlsWindow controlWindow;

    void Awake()
    {
        text = GetComponent<Text>();

        controlWindow = GetComponentInParent<ControlsWindow>();
    }

    void Start()
    {
        text.text = InputManager.instance.getKey(command, keyType).ToString();
        refresh();
    }

    void Update()
    {
        if (state == ConfigControlButtonState.Nothing)
            return;

        if (!Input.anyKey)
            return;

        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                if (key == KeyCode.Mouse0 && EventSystem.current.currentSelectedGameObject != gameObject)
                    continue;
                if (InputManager.instance.forbiddenCombination(command, key))
                    continue;
                InputManager.instance.setCommand(command, key, keyType);
                state = ConfigControlButtonState.Nothing;
                controlWindow.refresh();
                text.color = Color.white;
                return;
            }
        }
    }

    public void refresh()
    {
        if (text)
            text.text = InputManager.instance.getIconDescription(command, keyType);
    }

    public void onClick()
    {
        SoundManager.instance.playSound("ClickOK");
        controlWindow.cancelInput();
        text.color = Color.green;
        state = ConfigControlButtonState.WaitingForInput;
    }

    public void cancelInput()
    {
        SoundManager.instance.playSound("ClickEsc");
        text.color = Color.white;
        state = ConfigControlButtonState.Nothing;
    }
}
