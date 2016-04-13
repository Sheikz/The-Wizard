using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Canvas canvas;
    public GameObject floatingText;
    public GameObject floatingTextHolder;
    public ScreenMaskController screenMask;
    public GameObject centerText;
    public LevelUpManager levelUpManager;
    public GameObject exitMenu;
    public SpellBook spellBook;
    public SpellWindowByType spellWindowByType;
	public CharacterWindow characterWindow;
    public FPSDisplay fpsDisplay;
    public CastingBar castingBar;
    public FloatingHPBar floatingHPBar;
    public ToAllocateReminder toAllocateReminder;
    public GameObject controlsWindow;
    public GameObject graphicsWindow;
    public Tooltip tooltipPrefab;
    public GameObject[] spellIcons;
    public Sprite emptyIcon;
	public InventoryItemIcon inventoryItemIcon;
    public Color[] elementColors;

    [HideInInspector]
    public Image[] coolDownImages;
    private Color originalScreenMaskColor;
    public GameObject deathAnimation;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        name = "UIManager";
    }

    public void setupUI()
    {
        screenMask.gameObject.SetActive(false);
        Color newColor = screenMask.GetComponent<Image>().color;
        newColor.a = (float)120 / 255;
        screenMask.GetComponent<Image>().color = newColor;
        centerText.SetActive(false);
        showMenu(false);
        spellBook.initialize();
        spellBook.close();
        spellWindowByType.initialize();
        spellWindowByType.close();
        linkIcons();
        refreshIconsDescription();
        characterWindow.initialize();
    }

    public void switchMenu()
    {
        SoundManager.instance.playSound("ClickEsc");
        showMenu(!exitMenu.activeSelf);
    }

    public void showMenu(bool value)
    {
        closeWindows();
        if (!value)
            controlsWindow.SetActive(false);

        exitMenu.SetActive(value);
        GameManager.instance.setPause(value);
    }

    public void setScreenColor(Color black)
    {
        Image screenImage = screenMask.GetComponent<Image>();
        originalScreenMaskColor = screenImage.color;
        screenImage.color = Color.black;
    }

    internal void closeWindows()
    {
        spellBook.close();
        spellWindowByType.close();
        controlsWindow.SetActive(false);
        graphicsWindow.SetActive(false);
        exitMenu.SetActive(false);
        characterWindow.gameObject.SetActive(false);
    }

    private void linkIcons()
    {
        coolDownImages = new Image[spellIcons.Length];
        for (int i = 0; i < spellIcons.Length; i++)
        {
            coolDownImages[i] = spellIcons[i].transform.Find("CooldownFill").GetComponent<Image>();
        }
    }

    internal void setIconDescription(InputManager.Command cmd)
    {
        // Only does that for the first 5 commands which are linked to the description of icons
        if ((int)cmd > 4)
            return;

        spellIcons[(int)cmd].transform.Find("Description").GetComponent<Text>().text = InputManager.instance.getIconDescription(cmd);
    }

    public void refreshIconsDescription()
    {
        for (int i = 0; i < spellIcons.Length; i++)
        {
            spellIcons[i].transform.Find("Description").GetComponent<Text>().text = InputManager.instance.getIconDescription((InputManager.Command)i);
        }
    }

    public void resetCooldownImages()
    {
        for (int i = 0; i < coolDownImages.Length; i++)
        {
            coolDownImages[i].fillAmount = 0;
        }
    }
    
    public void setScreenColorOriginal()
    {
        setScreenColor(originalScreenMaskColor);
    }

    public float getFPS()
    {
        return fpsDisplay.getFPS();
    }

    public void openControlsWindow(bool value)
    {
        controlsWindow.SetActive(value);
    }

    public void openGraphicsWindow(bool value)
    {
        graphicsWindow.SetActive(value);
    }

    public void refreshControlWindow()
    {
        controlsWindow.GetComponent<ControlsWindow>().refresh();
    }

    public void refreshUI()
    {
        toAllocateReminder.refresh();
    }
}
