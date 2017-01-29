using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPopup : MonoBehaviour {

    public Image icon;
    public Text text;
    public Button okButton;
    public Text buttonText;

    private static InfoPopup instance;

    void Awake()
    {
        if (instance == null)
            instance = this;

        instance.gameObject.SetActive(false);
    }

    public static void createPopup(Sprite icon, string text)
    {
        instance.icon.sprite = icon;
        instance.text.text = text;
        instance.gameObject.SetActive(true);
    }

}
