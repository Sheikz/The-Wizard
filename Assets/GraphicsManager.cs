using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphicsManager : MonoBehaviour
{
    public static GraphicsManager instance;
    public int minWidth;
    public int minHeight;

    public List<Resolution> supportedResolutions;

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
        setupResolutions();
    }
    
    void Start()
    {

    }

    private void setupResolutions()
    {
        supportedResolutions = new List<Resolution>(Screen.resolutions);
        if (Application.isEditor)
            return;

        for (int i = supportedResolutions.Count-1 ; i >= 0; i--)
        {
            if (( supportedResolutions[i].width <= minWidth) || (supportedResolutions[i].height < minHeight))
                supportedResolutions.RemoveAt(i);
        }
        if (supportedResolutions.Count <= 0)
        {
            Debug.LogError("Not a single supported resolution");
            return;
        }
        setResolution(supportedResolutions[supportedResolutions.Count-1], true);
    }

    public void setResolution(Resolution res, bool fullscreen)
    {
        Screen.SetResolution(res.width, res.height, fullscreen, res.refreshRate);
    }

    public void setResolution(int index, bool fullscreen)
    {
        Screen.SetResolution(supportedResolutions[index].width, supportedResolutions[index].height, fullscreen, supportedResolutions[index].refreshRate);
    }

    public void setFullscreen(bool value)
    {
        Screen.fullScreen = value;
    }
}