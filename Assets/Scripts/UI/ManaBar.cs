using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    private Image img;
    private Text txt;
    private SpellCaster hero;

    void Awake()
    {
        img = GetComponent<Image>();
        txt = gameObject.GetComponentInChildren<Text>();
    }

	void Start ()
    {
        hero = GameManager.instance.hero.GetComponent<SpellCaster>();
	}
	
	// Update is called once per frame
	void OnGUI ()
    {
        txt.text = Mathf.FloorToInt(hero.getMana()).ToString();
        img.fillAmount = hero.getManaRatio();
	}
}
