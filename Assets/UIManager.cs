using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject floatingText;
    public GameObject floatingTextHolder;
    public ScreenMaskController screenMask;
    public GameObject centerText;
    public LevelUpManager levelUpManager;
    public GameObject exitMenu;
    public SpellBook spellBook;
    public SpellWindowByType spellWindowByType;
    public FPSDisplay fpsDisplay;

    [HideInInspector]
    public Image[] coolDownImages;
    private Color originalScreenMaskColor;

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
        Awake();
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
    }

    public void switchMenu()
    {
        showMenu(!exitMenu.activeSelf);
    }

    public void showMenu(bool value)
    {
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
    }

    private void linkIcons()
    {
        coolDownImages = new Image[5];
        coolDownImages[2] = GameObject.Find("SpellIconDefensive").transform.Find("CooldownFill").GetComponent<Image>();
        if (!coolDownImages[2])
        {
            Debug.LogError("Defensive Spell cooldown image not found!");
            Debug.Break();
        }
        coolDownImages[0] = GameObject.Find("SpellIconPrimarySpell").transform.Find("CooldownFill").GetComponent<Image>();
        if (!coolDownImages[0])
        {
            Debug.LogError("Primary Spell cooldown image not found!");
            Debug.Break();
        }
        coolDownImages[1] = GameObject.Find("SpellIconSecondarySpell").transform.Find("CooldownFill").GetComponent<Image>();
        if (!coolDownImages[1])
        {
            Debug.LogError("Secondary Spell cooldown image not found!");
            Debug.Break();
        }
        coolDownImages[3] = GameObject.Find("SpellIconUltimate1").transform.Find("CooldownFill").GetComponent<Image>();
        if (!coolDownImages[3])
        {
            Debug.LogError("Secondary Spell cooldown image not found!");
            Debug.Break();
        }
        coolDownImages[4] = GameObject.Find("SpellIconUltimate2").transform.Find("CooldownFill").GetComponent<Image>();
        if (!coolDownImages[4])
        {
            Debug.LogError("Secondary Spell cooldown image not found!");
            Debug.Break();
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
}
