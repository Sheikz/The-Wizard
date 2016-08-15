using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GraphicsManager : MonoBehaviour
{
    public static GraphicsManager instance;
    public int minWidth;
    public int minHeight;
    public Resolution currentResolution;

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
        setupSupportedResolutions();
        setResolutionFromPlayerPrefs();
    }

    private void setResolutionFromPlayerPrefs()
    {
        bool fullScreen = true;
        Resolution r = supportedResolutions[supportedResolutions.Count - 1];

        if (PlayerPrefs.HasKey("FullScreen"))
            fullScreen = (PlayerPrefs.GetInt("FullScreen") == 1) ? true : false;

        if (PlayerPrefs.HasKey("ResolutionX") && PlayerPrefs.HasKey("ResolutionY") && PlayerPrefs.HasKey("RefreshRate"))
        {
            r.width = PlayerPrefs.GetInt("ResolutionX");
            r.height = PlayerPrefs.GetInt("ResolutionY");
            r.refreshRate = PlayerPrefs.GetInt("RefreshRate");
        }
        setResolution(r, fullScreen);
    }

    private void setupSupportedResolutions()
    {
        supportedResolutions = new List<Resolution>(Screen.resolutions);
        if (Application.isEditor || Application.isWebPlayer)
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
        
    }

    public void setResolution(Resolution res, bool fullscreen)
    {
        currentResolution = res;
        Screen.SetResolution(res.width, res.height, fullscreen, res.refreshRate);
        PlayerPrefs.SetInt("ResolutionX", res.width);
        PlayerPrefs.SetInt("ResolutionY", res.height);
        PlayerPrefs.SetInt("RefreshRate", res.refreshRate);
        PlayerPrefs.SetInt("FullScreen", fullscreen ? 1 : 0);
    }

    public void setResolution(int index, bool fullscreen)
    {
        Resolution r = new Resolution();
        r.width = supportedResolutions[index].width;
        r.height = supportedResolutions[index].height;
        r.refreshRate = supportedResolutions[index].refreshRate;
        setResolution(r, fullscreen);
    }

    public void setFullscreen(bool value)
    {
        Screen.fullScreen = value;
        PlayerPrefs.SetInt("FullScreen", value ? 1 : 0);
    }
}