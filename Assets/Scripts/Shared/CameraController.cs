using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Game game;
    [SerializeField] float maxCameraSize = 20;
    [SerializeField] float minCameraSize = 5;
    [SerializeField] float reSizeMultiplier = 0.1f;

    [System.NonSerialized] public float startSize;

    void Start()
    {
        InputHandler.current.Dragged += Move;
        InputHandler.current.Pinched += ReSize;
    }

    void Move(Vector2 delta)
    {
        transform.Translate(delta, Space.World);

        if (game == null) return;

        var camera = Camera.main;
        Vector2 boardOrigin = game.board.transform.position;
        var boardOppositeCorner = boardOrigin + game.size;

        if (boardOppositeCorner.x < camera.transform.position.x || 
            boardOrigin.x > camera.transform.position.x) transform.Translate(new Vector2(-delta.x, 0), Space.World);

        if (boardOppositeCorner.y < camera.transform.position.y ||
            boardOrigin.y > camera.transform.position.y) transform.Translate(new Vector2(0, -delta.y), Space.World);
    }

    void ReSize(float amount)
    {
        var size = Camera.main.orthographicSize;
        size += amount * reSizeMultiplier;

        if (size > maxCameraSize) Camera.main.orthographicSize = maxCameraSize;
        else if (size < minCameraSize) Camera.main.orthographicSize = minCameraSize;
        else Camera.main.orthographicSize = size;
    }
}
