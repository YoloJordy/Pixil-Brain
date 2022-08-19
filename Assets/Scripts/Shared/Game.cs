using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Game : MonoBehaviour
{
    [NonSerialized] public Vector2 size;
    [NonSerialized] public Board board;
    [NonSerialized] public Dictionary<Vector3Int, Cell> cells;
    public string gameName;
    protected State state = State.GAMEOVER;

    protected enum State
    {
        START,
        PLAYING,
        GAMEOVER,
        WIN,
    }

    public static event Action<bool> EndGame;
    protected static void InvokeEndGame(bool won) => EndGame?.Invoke(won);

    public static event Action BeginGame;
    protected static void InvokeBeginGame() => BeginGame?.Invoke();

    protected abstract void NewGame();

    protected abstract void InputTapped(Vector3 position);

    protected abstract void InputHeld(Vector3 position);

    protected void RemoveData(bool _) => GameDatabase.RemoveData(gameName);

    private void OnApplicationQuit()
    {
        if (state == State.PLAYING) GameDatabase.SaveDataAsync(this);
    }
}
