using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimeText : MonoBehaviour
{
    private Text text;

    void Awake()
    {
        text = GetComponent<Text>();
    }

    public void setTimeMultiplier(float value)
    {
        Time.timeScale = value;
        text.text = "Time Multiplier : " + value.ToString("0.00");
    }
}
