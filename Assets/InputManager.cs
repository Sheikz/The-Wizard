﻿using UnityEngine;
using System.Collections;
using System;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public enum Command { PrimarySpell, SecondarySpell, DefensiveSpell, Ultimate1, Ultimate2,
                            MoveUp, MoveRight, MoveLeft, MoveDown,
                            SpellBook, CharacterWindow};

    private KeyCode[] keys;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        if (keys == null)
            keys = new KeyCode[Enum.GetNames(typeof(Command)).Length];
    }

    void Start()
    {
        setupDefaults();
    }

    /// <summary>
    /// Forbid some key combinations
    /// </summary>
    /// <param name="command"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    internal bool forbiddenCombination(Command command, KeyCode key)
    {
        if (key == KeyCode.Escape)
            return true;
        if ((command == Command.CharacterWindow || command == Command.SpellBook)
            && (key == KeyCode.Mouse0))
            return true;
        return false;
    }

    /// <summary>
    /// Setup default keys
    /// </summary>
    public void setupDefaults()
    {
        keys[(int)Command.PrimarySpell] = KeyCode.Mouse0;
        keys[(int)Command.SecondarySpell] = KeyCode.Mouse1;
        keys[(int)Command.DefensiveSpell] = KeyCode.Space;
        keys[(int)Command.Ultimate1] = KeyCode.A;
        keys[(int)Command.Ultimate2] = KeyCode.E;
        keys[(int)Command.MoveUp] = KeyCode.Z;
        keys[(int)Command.MoveDown] = KeyCode.S;
        keys[(int)Command.MoveLeft] = KeyCode.Q;
        keys[(int)Command.MoveRight] = KeyCode.D;
        keys[(int)Command.SpellBook] = KeyCode.Tab;
        keys[(int)Command.CharacterWindow] = KeyCode.C;
        UIManager.instance.refreshIconsDescription();
        UIManager.instance.refreshControlWindow();
    }

    public float getVerticalInput()
    {
        float result = 0f;
        if (Input.GetKey(getKey(Command.MoveUp)))
            result += 1f;
        if (Input.GetKey(getKey(Command.MoveDown)))
            result -= 1f;

        return result;
    }

    public float getHorizontalInput()
    {
        float result = 0f;
        if (Input.GetKey(getKey(Command.MoveRight)))
            result += 1f;
        if (Input.GetKey(getKey(Command.MoveLeft)))
            result -= 1f;

        return result;
    }

    public KeyCode getKey(Command cmd)
    {
        return keys[(int)cmd];
    }

    /// <summary>
    /// Return true all frames the key is pressed
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    public bool IsKeyPressed(Command cmd)
    {
        return Input.GetKey(keys[(int)cmd]);
    }

    /// <summary>
    /// Return true all frames the key is pressed
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    public bool IsKeyPressed(SpellType type)
    {
        return Input.GetKey(keys[(int)type]);
    }

    /// <summary>
    /// Return true only the frame the key was released
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    internal bool IsKeyUp(Command cmd)
    {
        return Input.GetKeyUp(keys[(int)cmd]);
    }

    /// <summary>
    /// Return the position of the cursor at this moment
    /// </summary>
    /// <returns></returns>
    public Vector3 getCursorPosition()
    {
        Vector3 result;
        result = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        result.z = 0;    // fix because camera see point at z = -5
        return result;
    }

    /// <summary>
    /// Return true only the frame the key was pressed
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    internal bool IsKeyDown(Command cmd)
    {
        return Input.GetKeyDown(keys[(int)cmd]);
    }

    internal void setCommand(Command cmd, KeyCode key)
    {
        for (int i= 0; i < keys.Length; i++)
        {
            if (keys[i] == key)
            {
                keys[i] = keys[(int)cmd];
            }
        }
        keys[(int)cmd] = key;
        UIManager.instance.setIconDescription(cmd);
    }

    internal string getIconDescription(Command cmd)
    {
        KeyCode key = getKey(cmd);
        switch (key)
        {
            case KeyCode.Mouse0: return "M1";
            case KeyCode.Mouse1: return "M2";
            case KeyCode.Mouse2: return "M3";
            case KeyCode.Mouse3: return "M4";
            case KeyCode.Mouse4: return "M5";
            default: return key.ToString();
        }
    }
}
