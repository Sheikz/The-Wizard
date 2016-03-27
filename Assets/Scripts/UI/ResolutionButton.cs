using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ResolutionButton : MonoBehaviour
{
    private Text resolutionText;
    public Text fullscreenText;
    private List<Resolution> resolutions;
    private int currentResolution;
    private bool fullscreen;

    void Awake()
    {
        resolutionText = GetComponent<Text>();
    }

    void Start()
    {
        resolutions = GraphicsManager.instance.supportedResolutions;
        currentResolution = resolutions.Count - 1;
        fullscreen = true;
        refresh();
    }

    void refresh()
    {
        resolutionText.text = "Resolution " + resolutions[currentResolution].width + "x" + resolutions[currentResolution].height;
        fullscreenText.text = "Full screen : " + (fullscreen ? "On" : "Off");
    }

    public void switchResolution()
    {
        if (Application.isEditor)
            return;

        currentResolution++;
        currentResolution = currentResolution % (resolutions.Count);
        GraphicsManager.instance.setResolution(currentResolution, fullscreen);
        refresh();
    }

    public void switchFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        fullscreen = !fullscreen;
        refresh();
    }
}
