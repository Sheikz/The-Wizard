using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class XPBar : MonoBehaviour
{
    private Image XPBarImage;
    private ExperienceReceiver heroXP;
    private float ratio;

    void Start()
    {
        heroXP = GameManager.instance.hero.GetComponent<ExperienceReceiver>();
        XPBarImage = GetComponent<Image>();
    }

    void OnGUI()
    {
        XPBarImage.fillAmount = ratio;
    }

    void Update()
    {
        if (heroXP)
            ratio = heroXP.getXPRatio();
        else
            ratio = 0;
    }
}

