using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphicsWindow : MonoBehaviour 
{
	public void clickOK()
    {
        SoundManager.instance.playSound("ClickOK");
        UIManager.instance.openGraphicsWindow(false);
    }
}
