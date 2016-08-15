using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SavingManager
{
    private static string saveGameName = "/savedGame.gd";

    public static void saveGame(SaveGame game)
    {
        BinaryFormatter bf = new BinaryFormatter();
        Debug.Log("Saving game to : " + Application.persistentDataPath + saveGameName);
        FileStream file = File.Create(Application.persistentDataPath + saveGameName);
        bf.Serialize(file, game);
    }

    public static SaveGame loadGame()
    {
        SaveGame game = null;
        if (File.Exists(Application.persistentDataPath + saveGameName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + saveGameName, FileMode.Open);
            game = (SaveGame)bf.Deserialize(file);
            file.Close();
        }
        return game;
    }
}
