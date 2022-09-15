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
        get => takingInput; 
        set => takingInput = value;
    }

    float touchStart = 0f;
    InternalTouchPhase touchInternalPhase;
    float currentTouchDelta;

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
        if (Input.touchCount == 1) TouchUpdate();
    }

    void TouchUpdate()
    {
        var touch = Input.GetTouch(0);
        var touchPosition = touch.position;

        switch (touch.phase)
        {
            case TouchPhase.Began:
                touchInternalPhase = InternalTouchPhase.NEW;
                touchStart = Time.time;
                break;

            case TouchPhase.Stationary:
                if (Time.time - touchStart >= minTimeHold && touchInternalPhase == InternalTouchPhase.NEW && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    InvokeHeld(touchPosition);
                    touchInternalPhase = InternalTouchPhase.HANDLED;
                }
                break;

            case TouchPhase.Moved:
                touchInternalPhase = InternalTouchPhase.MOVED;
                break;

            case TouchPhase.Ended:
                if ((touchInternalPhase == InternalTouchPhase.NEW || currentTouchDelta < 0.1f) && !prevIsPointerOverGameObject) InvokeTapped(touchPosition);
                currentTouchDelta = 0;
                touchInternalPhase = InternalTouchPhase.HANDLED;
                break;
        }

        prevIsPointerOverGameObject = EventSystem.current.IsPointerOverGameObject(touch.fingerId);
    }

    private void LateUpdate()
    {
        if (!takingInput) return;
        if (Input.touchCount >= 1) TouchLateUpdate();
    }

    void TouchLateUpdate()
    {
        var touch0 = Input.GetTouch(0);
        var touchPosition = touch0.position;
        var touchLastPosition = touch0.position - touch0.deltaPosition;
        currentTouchDelta += touch0.deltaPosition.magnitude;

        if (Input.touchCount == 2)
        {
            MultiTouchLateUpdate();

            var touch1 = Input.GetTouch(1);
            var touchCenterPosition = (touch0.position + touch1.position) / 2;
            var touchCenterLastPosition = (touch0.position - touch0.deltaPosition + touch1.position - touch1.deltaPosition) / 2;

            InvokeDragged(Camera.main.ScreenToWorldPoint(touchCenterLastPosition) - Camera.main.ScreenToWorldPoint(touchCenterPosition));
        }
        else InvokeDragged(Camera.main.ScreenToWorldPoint(touchLastPosition) - Camera.main.ScreenToWorldPoint(touchPosition));
    }

    void MultiTouchLateUpdate()
    {
        var touches = Input.touches;
        var touch0 = touches[0];
        var touch1 = touches[1];

        var prevPos1 = touch0.position - touch0.deltaPosition;
        var prevPos2 = touch1.position - touch1.deltaPosition;
        var prevDist = (prevPos1 - prevPos2).magnitude;

        var dist = (touch0.position - touch1.position).magnitude;

        var delta = prevDist - dist;
        InvokePinched(delta);

        if (touch1.phase == TouchPhase.Ended || touch0.phase == TouchPhase.Ended)
        {
            FlipTakingInput();
            Invoke(nameof(FlipTakingInput), 0.05f);
        }
    }

    void FlipTakingInput()
    {
        TakingInput = !TakingInput;
        touchStart = Time.time;
    }

    public event Action<Vector3> Tapped;
    public void InvokeTapped(Vector3 position) => Tapped?.Invoke(position);

    public event Action<Vector3> Held;
    public void InvokeHeld(Vector3 position) => Held?.Invoke(position);

    public event Action<Vector2> Dragged;
    public void InvokeDragged(Vector2 positionDelta)
    {
        Dragged?.Invoke(positionDelta);
    }

    public event Action<float> Pinched;
    public void InvokePinched(float amount) 
    {
        Pinched?.Invoke(amount); 
    }
}
