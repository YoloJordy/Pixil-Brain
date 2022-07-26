using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController current;

    [SerializeField] Game game;
    [SerializeField] float maxCameraSize = 20;
    [SerializeField] float minCameraSize = 5;
    [SerializeField] float borderWidth = 1;
    [SerializeField] float reSizeMultiplier = 0.01f;

    [System.NonSerialized] public float startSize;

    void Start()
    {
        if (current == null) current = this;

        Game.BeginGame += ResetCamera;
        Game.EndGame += ResetCamera;
    }

    public void Move(Vector2 delta)
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

    void ResetCamera()
    {
        if (game == null) return;

        var width = game.size.x * Screen.height / Screen.width / 2;
        var height = game.size.y / 2;
        var newSize = (width > height ? width : height) +1;

        maxCameraSize = newSize;
        Camera.main.orthographicSize = newSize;
        Camera.main.transform.position = new Vector3(game.size.x / 2, game.size.y / 2, Camera.main.transform.position.z);
    }
    void ResetCamera(bool unused) => ResetCamera();

    public void Resize(float amount)
    {
        var size = Camera.main.orthographicSize;
        size += amount * reSizeMultiplier * size;

        if (size > maxCameraSize) Camera.main.orthographicSize = maxCameraSize;
        else if (size < minCameraSize) Camera.main.orthographicSize = minCameraSize;
        else Camera.main.orthographicSize = size;
    }
}
