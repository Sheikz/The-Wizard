using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InvincibleButton : MonoBehaviour
{
    private Text text;
    private bool invincible = false;

    void Awake()
    {
        text = GetComponentInChildren<Text>();
    }

    public void pushButton()
    {
        if (invincible)
        {
            GameManager.instance.hero.damageable.isInvincible--;
            text.text = "Invincible = Off";
            invincible = false;
        }
        else
        {
            GameManager.instance.hero.damageable.isInvincible++;
            text.text = "Invincible = On";
            invincible = true;
        }
    }
}
