using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ButtonType
{
    Left,
    Right,
    Jump,
    Attack,
    Home
}

public class UiButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public ButtonType ButtonType;
    private bool isPressed;
    
    private void FixedUpdate()
    {
        GameController.Instance.OnButtonPressed(ButtonType, isPressed);
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }
}
