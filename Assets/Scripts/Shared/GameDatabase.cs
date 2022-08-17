using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class GameDatabase
{
    public static void SaveDataAsync(Game game)
    {
        SaveData saveData = new(game);
        string json = JsonUtility.ToJson(saveData);
        Debug.Log(json);
        File.WriteAllTextAsync(Application.dataPath + "/StreamingAssets/" + game.gameName + ".json", json);
    }

    public static SaveData LoadData(string gameName)
    {
        string json = File.ReadAllText(Application.dataPath + "/StreamingAssets/" + gameName + ".json");
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);

        return saveData;
    }

    public static void RemoveData(string gameName)
    {
        Debug.Log("removing Data");
        File.Delete(Application.dataPath + "/StreamingAssets/" + gameName + ".json");
    }

    public static bool GameHasSave(string gameName)
    {
        return File.Exists(Application.dataPath + "/StreamingAssets/" + gameName + ".json");
    }
}
