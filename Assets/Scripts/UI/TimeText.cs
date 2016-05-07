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

    public void setDifficultyMultiplier(float value)
    {
        text.text = "Difficulty : " + value.ToString("0.00");
        GameManager.instance.difficulty = value;
    }
}
