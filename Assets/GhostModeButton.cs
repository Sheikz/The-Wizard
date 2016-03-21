using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GhostModeButton : MonoBehaviour
{
    private Text text;
    private bool ghostMode = false;

    void Awake()
    {
        text = GetComponentInChildren<Text>();
    }

    public void pushButton()
    {
        if (ghostMode)
        {
            GameManager.instance.hero.circleCollider.enabled = true;
            text.text = "Ghost Mode = Off";
            ghostMode = false;
        }
        else
        {
            GameManager.instance.hero.circleCollider.enabled = false;
            text.text = "Ghost Mode = On";
            ghostMode = true;
        }
    }
}
