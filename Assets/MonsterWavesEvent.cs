using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterWavesEvent : RoomEvent {

	public int nbrWaves = 5;
	public int monstersInFirstWave = 3;
	public int monstersNbrIncrementPerWave = 2;
	public int timePerWave = 15;
	public int timeIncrementPerWave = 0;

	private int currentWave = -1;
	private List<NPCController>[] monsterWaves;
    private Text text;

	public override void initialize()
	{
		base.initialize();
		createMonsters();
        text = UIManager.instance.centerText.GetComponent<Text>();
	}

	void createMonsters()
	{
        Debug.Log("creating monsters wave");
        monsterWaves = new List<NPCController>[nbrWaves];
		for (int i=0; i < nbrWaves; i++)
		{
			createWave(out monsterWaves[i], monstersInFirstWave + monstersNbrIncrementPerWave * i);
		}
	}

	void createWave(out List<NPCController> wave, int monsterNumber)
	{
        wave = new List<NPCController>();
		foreach (var monsterPrefab in WorldManager.instance.getMonsters(monsterNumber))
		{
			Tile tileToPutMonster = Utils.pickRandom(roomTiles);
			NPCController newMonster = Instantiate(monsterPrefab, tileToPutMonster.position(), Quaternion.identity) as NPCController;
			newMonster.transform.SetParent(room.map.monsterHolder);
			newMonster.initialize(room);
			newMonster.activate(false);
            wave.Add(newMonster);
		}
	}

    IEnumerator waveTimer(int waveNumber)
    {
        float startTime = Time.time;
        float waveDuration = timePerWave + waveNumber * timeIncrementPerWave;
        while (Time.time - startTime <= waveDuration)
        {
            var timeLeft = waveDuration - (Time.time - startTime);
            var text = "Kill all the monsters!\n";
            text += "Wave " + (waveNumber + 1) + "/" + nbrWaves + "\n";
            text += "Time left: " + timeLeft.ToString("0.00") + "s";
            UIManager.instance.setCenterText(text);
            yield return null;
        }
        failEvent();
    }

	public override void monsterDied(NPCController mc)
	{
        if (eventFinished)
            return;

		monsterWaves[currentWave].Remove(mc);
		if (monsterWaves[currentWave].Count == 0)
			nextWave();
	}

	public override void playerEnteredRoom(PlayerController player)
	{
		startEvent();
		currentWave = -1;
		nextWave();
	}

    void failEvent()
    {
        UIManager.instance.centerText.SetActive(false);
        endEvent(false);
    }

    void successEvent()
    {
        endEvent();
    }

	void nextWave()
	{
        StopAllCoroutines();
        if (currentWave == nbrWaves - 1)
        {
            successEvent();
            return;
        }

        currentWave++;

        StartCoroutine(waveTimer(currentWave));

		foreach(var monster in monsterWaves[currentWave])
		{
			if (!monster)
				continue;

			monster.activate(true);
            monster.setTarget(GameManager.instance.hero.GetComponent<Damageable>());
		}
	}

	public override void playerExitedRoom(PlayerController player)
	{
		room.openEntrance(true);
	}
}
