using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HPText : MonoBehaviour {

	private GameObject hero;
	private Text text;
	// Use this for initialization
	void Start ()
	{
		hero = GameManager.instance.hero.gameObject;
		text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (hero)
			text.text = hero.GetComponent<Damageable>().getHP().ToString();
		else
			text.text = "0";
	}
}
