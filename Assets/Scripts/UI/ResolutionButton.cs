using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ResolutionButton : MonoBehaviour
{
    private Text resolutionText;
    public Text fullscreenText;
    private List<Resolution> resolutions;
    private int currentResolution;
    private bool fullscreen = false;

    void Awake()
    {
        resolutionText = GetComponent<Text>();
    }

    void Start()
    {
        resolutions = GraphicsManager.instance.supportedResolutions;
        currentResolution = resolutions.Count - 1;
        fullscreen = Screen.fullScreen;
        refresh();
    }

    void refresh()
    {
        resolutionText.text = "Resolution " + GraphicsManager.instance.currentResolution.width + "x" + GraphicsManager.instance.currentResolution.height;
        fullscreenText.text = "Full screen : " + (fullscreen ? "On" : "Off");
    }

    public void switchResolution()
    {
        if (Application.isEditor)
            return;

        SoundManager.instance.playSound("ClickOK");
        currentResolution++;
        currentResolution = currentResolution % (resolutions.Count);
        GraphicsManager.instance.setResolution(currentResolution, Screen.fullScreen);
        refresh();
    }

    public void switchFullscreen()
    {
        SoundManager.instance.playSound("ClickOK");
        GraphicsManager.instance.setFullscreen(!Screen.fullScreen);
        fullscreen = !Screen.fullScreen;
        refresh();
    }
}
