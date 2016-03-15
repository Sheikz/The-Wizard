using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    public float refreshTime = 0.5f;

    private int frameCount = 0;
    private float timeCounter = 0f;
    private float lastFrameRate = 0f;
    private Text fpsDisplay;

    void Awake()
    {
        fpsDisplay = GetComponent<Text>();
    }

    void Update()
    {
        if (timeCounter < refreshTime)
        {
            timeCounter += Time.unscaledDeltaTime;
            frameCount++;
        }
        else
        {
            lastFrameRate = frameCount / refreshTime;
            frameCount = 0;
            timeCounter = 0;
        }
    }

    void OnGUI()
    {
        fpsDisplay.text = "FPS: " + lastFrameRate;
    }

    public float getFPS()
    {
        return lastFrameRate;
    }
   
}