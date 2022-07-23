using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
    public static InputHandler current;

    [SerializeField] float minTimeHold = 0.3f;

    bool takingInput;
    public bool TakingInput
    {
        get { return takingInput; }
        set { takingInput = value; }
    }

    float touchStart = 0f;
    Vector2 touchLastPosition = Vector2.zero;
    InternalTouchPhase touchInternalPhase;

    float multiTouchDist;
    float prevMultiTouchDist;

    bool prevIsPointerOverGameObject;

    enum InternalTouchPhase 
    {
        NEW,
        MOVED,
        HANDLED,
    }


    private void Awake()
    {
        current = this;
    }

    private void Update()
    {
        if (!takingInput) return;
        if (Input.touchCount >= 1) SingleTouchUpdate();
        if (Input.touchCount == 2) MultiTouchUpdate();
    }

    void SingleTouchUpdate()
    {
        var touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                touchInternalPhase = InternalTouchPhase.NEW;
                touchStart = Time.time;
                touchLastPosition = touch.position;
                break;

            case TouchPhase.Stationary:
                if (Time.time - touchStart >= minTimeHold && touchInternalPhase == InternalTouchPhase.NEW && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    InvokeHeld(touch.position);
                    touchInternalPhase = InternalTouchPhase.HANDLED;
                }
                break;

            case TouchPhase.Moved:
                InvokeDragged(Camera.main.ScreenToWorldPoint(touchLastPosition) - Camera.main.ScreenToWorldPoint(touch.position));

                touchLastPosition = touch.position;
                touchInternalPhase = InternalTouchPhase.MOVED;
                break;

            case TouchPhase.Ended:
                if (touchInternalPhase == InternalTouchPhase.NEW && !prevIsPointerOverGameObject) InvokeTapped(touch.position);

                touchInternalPhase = InternalTouchPhase.HANDLED;
                break;
        }
        prevIsPointerOverGameObject = EventSystem.current.IsPointerOverGameObject(touch.fingerId);
    }

    void MultiTouchUpdate()
    {
        var touches = Input.touches;

        foreach (Touch touch in touches)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    multiTouchDist = DistScreenToWorld(touches[0].position, touches[1].position);
                    prevMultiTouchDist = multiTouchDist;
                    break;

                case TouchPhase.Moved:
                    var delta = prevMultiTouchDist - multiTouchDist;
                    if (delta > -5 && delta < 5) InvokePinched(delta);

                    prevMultiTouchDist = multiTouchDist;
                    DistScreenToWorld(touches[0].position, touches[1].position);
                    break;
            }
        }
    }
    float DistScreenToWorld(Vector2 screenPosition1, Vector2 screenPosition2)
    {
        return multiTouchDist = (Camera.main.ScreenToWorldPoint(screenPosition1) - Camera.main.ScreenToWorldPoint(screenPosition2)).magnitude;
    }

    public event Action<Vector3> Tapped;
    public void InvokeTapped(Vector3 position) => Tapped?.Invoke(position);

    public event Action<Vector3> Held;
    public void InvokeHeld(Vector3 position) => Held?.Invoke(position);

    public event Action<Vector2> Dragged;
    public void InvokeDragged(Vector2 positionDelta) => Dragged?.Invoke(positionDelta);

    public event Action<float> Pinched;
    public void InvokePinched(float amount) => Pinched?.Invoke(amount);
}
