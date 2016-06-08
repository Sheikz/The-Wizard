using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OptionWindow : MonoBehaviour 
{
    public void clickOK()
    {
        SoundManager.instance.playSound("ClickOK");
        gameObject.SetActive(false);
    }
}
