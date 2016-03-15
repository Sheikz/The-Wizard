using UnityEngine;
using System.Collections;

public class ExperienceReceiver : MonoBehaviour
{
    public float levelMultiplier = 1;
	public int XPForFirstLevel;
    public int additionalXPPerLevel = 50;

	private int level;
	private int currentXP;
	private int totalXP;
	private int XPToNextLevel;
	private LevelUpManager levelUpManager;
    private CharacterStats characterStats;

    void Awake()
    {
        characterStats = GetComponent<CharacterStats>();
    }

	// Use this for initialization
	void Start ()
	{
		level = 1;  // Level start at 1
		totalXP = 0;
		currentXP = 0;
		XPToNextLevel = XPForFirstLevel;
	}

	public void addXP(int xp)
	{
		currentXP += xp;
		totalXP += xp;
		if (currentXP >= XPToNextLevel)
		{
			levelUp();
		}
	}

	public void levelUp()
	{
		level++;
		XPToNextLevel = Mathf.RoundToInt((XPToNextLevel +additionalXPPerLevel) * levelMultiplier);
		currentXP = 0;
        GameManager.instance.levelUp();
		
        if (characterStats)
            characterStats.levelUp();
	}

	void Update()
	{
        if (Input.GetKeyDown(KeyCode.K))
        {
            levelUp();
        }
	}

	public float getXPRatio()
	{
		return (float)currentXP / (float)XPToNextLevel;
	}

	public int getLevel()
	{
		return level;
	}
}
