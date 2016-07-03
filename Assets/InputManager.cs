using UnityEngine;
using System.Collections;
using System;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public enum KeyType { Primary, Alternate };
    public enum Command { PrimarySpell, SecondarySpell, DefensiveSpell, Ultimate1, Ultimate2,
                            MoveUp, MoveRight, MoveLeft, MoveDown,
                            SpellBook, CharacterWindow};

    public bool isMouseHoveringItem = false;
    public Item hoveringOverItem = null;

    private KeyCode[,] keys;

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
            keys = new KeyCode[Enum.GetNames(typeof(Command)).Length, Enum.GetNames(typeof(KeyType)).Length];
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
        for (int i=0; i < keys.GetLength(0); i++)   // Setting everything to none first
        {
            for (int j = 0; j < keys.GetLength(1); j++)
                keys[i, j] = KeyCode.None;
        }

        keys[(int)Command.PrimarySpell, (int)KeyType.Primary] = KeyCode.Mouse0;

        keys[(int)Command.SecondarySpell, (int)KeyType.Primary] = KeyCode.Mouse1;

        keys[(int)Command.DefensiveSpell, (int)KeyType.Primary] = KeyCode.Space;
        keys[(int)Command.DefensiveSpell, (int)KeyType.Alternate] = KeyCode.RightControl;

        keys[(int)Command.Ultimate1, (int)KeyType.Primary] = KeyCode.A;
        keys[(int)Command.Ultimate1, (int)KeyType.Alternate] = KeyCode.RightShift;

        keys[(int)Command.Ultimate2, (int)KeyType.Primary] = KeyCode.E;
        keys[(int)Command.Ultimate2, (int)KeyType.Alternate] = KeyCode.Keypad0;

        keys[(int)Command.MoveUp, (int)KeyType.Primary] = KeyCode.Z;
        keys[(int)Command.MoveUp, (int)KeyType.Alternate] = KeyCode.UpArrow;

        keys[(int)Command.MoveDown, (int)KeyType.Primary] = KeyCode.S;
        keys[(int)Command.MoveDown, (int)KeyType.Alternate] = KeyCode.DownArrow;

        keys[(int)Command.MoveLeft, (int)KeyType.Primary] = KeyCode.Q;
        keys[(int)Command.MoveLeft, (int)KeyType.Alternate] = KeyCode.LeftArrow;

        keys[(int)Command.MoveRight, (int)KeyType.Primary] = KeyCode.D;
        keys[(int)Command.MoveRight, (int)KeyType.Alternate] = KeyCode.RightArrow;

        keys[(int)Command.SpellBook, (int)KeyType.Primary] = KeyCode.Tab;
        keys[(int)Command.SpellBook, (int)KeyType.Alternate] = KeyCode.Keypad1;

        keys[(int)Command.CharacterWindow, (int)KeyType.Primary] = KeyCode.C;
        keys[(int)Command.CharacterWindow, (int)KeyType.Alternate] = KeyCode.Keypad2;

        UIManager.instance.refreshIconsDescription();
        UIManager.instance.refreshControlWindow();
    }

    public float getVerticalInput()
    {
        float result = 0f;
        if (IsCommandPressed(Command.MoveUp))
            result += 1f;
        if (IsCommandPressed(Command.MoveDown))
            result -= 1f;

        return result;
    }

    public float getHorizontalInput()
    {
        float result = 0f;
        if (IsCommandPressed(Command.MoveRight))
            result += 1f;
        if (IsCommandPressed(Command.MoveLeft))
            result -= 1f;

        return result;
    }

    public KeyCode getKey(Command cmd, KeyType keyType)
    {
        return keys[(int)cmd, (int)keyType];
    }

    /// <summary>
    /// Return true all frames the key is pressed
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    public bool IsCommandPressed(Command cmd)
    {
        return Input.GetKey(keys[(int)cmd, (int)KeyType.Primary]) || Input.GetKey(keys[(int)cmd, (int)KeyType.Alternate]);
    }

    /// <summary>
    /// Return true all frames the key is pressed
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    public bool IsSpellPressed(SpellType type)
    {
        return Input.GetKey(keys[(int)type, (int)KeyType.Primary]) || Input.GetKey(keys[(int)type, (int)KeyType.Alternate]);
    }

    /// <summary>
    /// Return true only the frame the key was released
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    internal bool IsCommandUp(Command cmd)
    {
        return Input.GetKeyUp(keys[(int)cmd, (int)KeyType.Primary]) || Input.GetKeyUp(keys[(int)cmd, (int)KeyType.Alternate]);
    }

    /// <summary>
    /// Return true only the frame the key was pressed
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    internal bool IsCommandDown(Command cmd)
    {
        return Input.GetKeyDown(keys[(int)cmd, (int)KeyType.Primary]) || Input.GetKeyDown(keys[(int)cmd, (int)KeyType.Alternate]);
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

    internal void setCommand(Command cmd, KeyCode key, KeyType keyType)
    {
        for (int i= 0; i < keys.GetLength(0); i++)
        {
            for (int j = 0; j < keys.GetLength(1); j++)
            {
                if (keys[i, j] == key)
                {
                    keys[i, j] = keys[(int)cmd, (int)keyType];
                }
            }
        }
        keys[(int)cmd, (int)keyType] = key;
        UIManager.instance.setIconDescription(cmd);
    }

    internal string getIconDescription(Command cmd, KeyType keyType = KeyType.Primary)
    {
        KeyCode key = getKey(cmd, keyType);
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
