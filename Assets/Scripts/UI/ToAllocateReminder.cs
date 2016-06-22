using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToAllocateReminder : MonoBehaviour
{
    public enum PointsToAllocate { Primary, Secondary, Defensive, Ultimate1, Ultimate2, All };
    public PointsToAllocate pointsToAllocate;
    private Text text;
    private PlayerStats heroStats;

    void Awake()
    {
        text = GetComponentInChildren<Text>();
    }

    public void refresh()
    {
        if (!heroStats)
            heroStats = GameManager.instance.hero.GetComponent<PlayerStats>();

        if (!heroStats)
            return;

        int nbr;
        if (pointsToAllocate == PointsToAllocate.All)
            nbr = heroStats.getTotalToAllocate();
        else
            nbr = heroStats.pointsToAllocate[(int)pointsToAllocate];

        if (nbr == 0)
            gameObject.SetActive(false);
        else
        {
            gameObject.SetActive(true);
            text.text = nbr.ToString();
        }
    }
}
