using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OptionsMenu : MonoBehaviour 
{
    public void clickTryAgain()
    {
        SoundManager.instance.playSound("ClickOK");
        GameManager.instance.tryAgain();
    }

    public void clickControls()
    {
        SoundManager.instance.playSound("ClickOK");
        UIManager.instance.openControlsWindow(true);
    }

    public void clickGraphics()
    {
        SoundManager.instance.playSound("ClickOK");
        UIManager.instance.openGraphicsWindow(true);
    }

    public void clickSounds()
    {
        SoundManager.instance.playSound("ClickOK");
        UIManager.instance.openSoundsWindow(true);
    }

    public void clickExit()
    {
        SoundManager.instance.playSound("ClickOK");
        GameManager.instance.exitGame();
    }
}
