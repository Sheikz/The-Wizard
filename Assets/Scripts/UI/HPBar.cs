using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    private Image HPBarImage;
    private GameObject hero;
    private float ratio;

    void Start()
    {
        hero = GameManager.instance.hero.gameObject;
        HPBarImage = GetComponent<Image>();
    }

    void OnGUI()
    {
        HPBarImage.fillAmount = ratio;
    }

    void Update()
    {
        if (hero)
        {
            ratio = hero.GetComponent<Damageable>().getHPRatio();
        }
        else
        {
            ratio = 0;
        }
    }
}
