using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	public GameObject heroPrefab;
    
	public LayerManager layerManager;
    //public SpellManager spellManager;
    public RoomBasedMapGenerator mapPrefab;
	
	[HideInInspector]
	public int levelNumber;
	[HideInInspector]
	public PlayerController hero;
	[HideInInspector]
	public bool isShuttingDown = false;
	[HideInInspector]
	public bool isPaused = false;
    [HideInInspector]
    public RoomBasedMapGenerator map;
    private float savedTimeScale = 1.0f;
    public float difficulty = 1.0f;

    void Awake ()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		name = "GameManager";

        createHero();
        map = Instantiate(mapPrefab);
        levelNumber = 1;
	}

    void Start()
    {
        UIManager.instance.setupUI();
        UIManager.instance.spellWindowByType.open();
    }

    public void revealMap()
    {
        map.revealRooms();
    }

    internal void levelUp()
    {
        UIManager.instance.levelUpManager.levelUp();
        map.levelUpMonsters();
        UIManager.instance.refreshUI();
    }

    public void levelUpHero()
    {
        hero.GetComponent<ExperienceReceiver>().levelUp();
    }

    public void createHero()
	{
        hero = Instantiate(heroPrefab).GetComponent<PlayerController>();
		hero.name = "Hero";
	}

    public void endLevel()
	{
		StartCoroutine(delayedEndLevel());
	}

	public IEnumerator delayedEndLevel()
	{
        UIManager.instance.setScreenColor(Color.black);
		UIManager.instance.centerText.SetActive(true);
        UIManager.instance.centerText.GetComponent<Text>().color = Color.white;
        UIManager.instance.centerText.GetComponent<Text>().text = "Loading...";

		levelNumber++;
		hero.gameObject.SetActive(false);
        isShuttingDown = true;
        map.destroy();

		yield return null;      // This is necessary because Unity destroys the objects only at the next Update()

        map = Instantiate(mapPrefab);
        hero.transform.position = new Vector3(0, 5, 0);
        hero.gameObject.SetActive(true);

        UIManager.instance.setScreenColorOriginal();
		hero.resetCooldowns();
		UIManager.instance.resetCooldownImages();
        UIManager.instance.centerText.SetActive(false);
        isShuttingDown = false;
    }

    public void setPause(bool value)
    {
        if (value)
        {
            if (Time.timeScale > 0f)
                savedTimeScale = Time.timeScale;

            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = savedTimeScale;
        }
    }

    public void setDifficulty(float value)
    {
        difficulty = value;
    }

    public void tryAgain()
	{
        UIManager.instance.exitMenu.SetActive(false);
		isShuttingDown = true;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void exitGame()
	{
		Application.Quit();
	}

	void OnApplicationQuit()
	{
		isShuttingDown = true;
	}
}
