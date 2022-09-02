using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Game : MonoBehaviour
{
    [NonSerialized] public Vector2 size;
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

    protected virtual void Start()
    {
        if (GameDatabase.GameHasSave(gameName))
        {
            LoadGame();
        }

        InputHandler.current.Tapped += InputTapped;
        InputHandler.current.Held += InputHeld;
        EndGame += RemoveData;
    }

    protected abstract void LoadGame();

    protected abstract void NewGame();

    protected abstract void InputTapped(Vector3 position);

    protected abstract void InputHeld(Vector3 position);

    protected void RemoveData(bool _) => GameDatabase.RemoveData(gameName);

    private void OnApplicationFocus(bool focus)
    {
        if (focus) return;

        StopAllCoroutines();
        if (state == State.PLAYING) GameDatabase.SaveDataAsync(this); 
    }
    private void OnDestroy()
    {
        if (state == State.PLAYING) GameDatabase.SaveDataAsync(this);
    }
}
