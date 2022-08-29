using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class GameDatabase
{
    public static string saveDirectory = Path.Combine(Application.persistentDataPath, "Saves");
    public static void SaveDataAsync(Game game)
    {
        SaveData saveData = game.gameName switch
        {
            "Minesweeper" => new MinesweeperSaveData(game),
            _ => new SaveData(game),
        };

        var json = JsonUtility.ToJson(saveData);
        Debug.Log(saveDirectory);
        if (!Directory.Exists(saveDirectory)) Directory.CreateDirectory(saveDirectory);
        File.WriteAllTextAsync(Path.Combine(saveDirectory, game.gameName + ".json"), json);
    }

    public static MinesweeperSaveData LoadMinesweeperData(string gameName)
    {
        var json = File.ReadAllText(Path.Combine(saveDirectory, gameName + ".json"));
        var saveData = JsonUtility.FromJson<MinesweeperSaveData>(json);
        Debug.Log(json);
        return saveData;
    }

    public static void RemoveData(string gameName)
    {
        Debug.Log("removing Data");
        File.Delete(Path.Combine(saveDirectory, gameName + ".json"));
    }

    public static bool GameHasSave(string gameName)
    {
        Debug.Log(File.Exists(Path.Combine(saveDirectory, gameName + ".json")));
        return File.Exists(Path.Combine(saveDirectory, gameName + ".json"));
    }
}
