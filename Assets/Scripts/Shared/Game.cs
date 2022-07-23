using System;
using UnityEngine;

public abstract class Game : MonoBehaviour
{
    [NonSerialized] public Vector2 size;
    [NonSerialized] public Board board;
    protected State state = State.GAMEOVER;

    protected enum State
    {
        START,
        PLAYING,
        GAMEOVER,
        WIN,
    }

    public event Action<bool> EndGame;
    protected virtual void InvokeEndGame(bool won)
    {
        EndGame?.Invoke(won);
    }

    protected abstract void NewGame();

    protected abstract void InputTapped(Vector3 position);

    protected abstract void InputHeld(Vector3 position);

}
