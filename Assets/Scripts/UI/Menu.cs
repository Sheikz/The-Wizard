using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

    // Use this for initialization
    [ExecuteInEditMode]
    void Start ()
    {
        gameObject.SetActive(false);
	}

    public void showMenu()
    {
        gameObject.SetActive(true);
    }

    public void hideMenu()
    {
        gameObject.SetActive(false);
    }
}
