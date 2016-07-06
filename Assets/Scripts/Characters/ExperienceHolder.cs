using UnityEngine;
using System.Collections;

public class ExperienceHolder : MonoBehaviour {

    public int experience;
    public bool shouldGiveExp = true;

    private GameObject floatingText;

    public void Start()
    {
        floatingText = UIManager.instance.floatingText;
    }

    public void die(GameObject killer)
    {
        if (!shouldGiveExp)
            return;

        if (!killer)    // If the killer is dead, no need to give xp
            return;
        ExperienceReceiver xpReceiver = killer.GetComponent<ExperienceReceiver>();
        if (xpReceiver)     // if the killer receives experience, give it
        {
            xpReceiver.addXP(experience);
            FloatingText xpText = (Instantiate(floatingText) as GameObject).GetComponent<FloatingText>();
            xpText.initialize(gameObject, "+" + experience + "xp");
            xpText.setColor(Color.cyan);
        }
    }
}
