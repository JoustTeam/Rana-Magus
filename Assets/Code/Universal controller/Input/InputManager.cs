using UnityEngine;
using UnityEngine.InputSystem;

/*
This script acts as a proxy for the PlayerInput component
such that the input events the game needs to proces will 
be sent through the GameEventManager. This lets any other
script in the project easily subscribe to an input action
without having to deal with the PlayerInput component directly.
*/ 

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    //Examples of how to add methods that can be called on any script with any context.
    //Hook it up in the PlayerInput component [go to events in the player input component,
    //open the gameplay or buildmode and attach this component to the event callback, with the associated function]
    //(Behavior: Invoke Unity Events) and then go the the specific mode (eg. set to Gameplay) and assign them.
    public void MovePressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            EventsManager.Instance.inputEvents.MovePressed(context.ReadValue<Vector2>());
        }
    }

    public void SubmitPressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            EventsManager.Instance.inputEvents.SubmitPressed();
        }
    }

    //mouse event testing
    public void MouseDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            EventsManager.Instance.inputEvents.MouseDown();
        }
        else if (context.canceled)
        {
            EventsManager.Instance.inputEvents.MouseUp();
        }
    }

    public void OnePressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            EventsManager.Instance.inputEvents.OnePressed();
        }
    }

    public void TwoPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            EventsManager.Instance.inputEvents.TwoPressed();
        }
    }

    public void ThreePressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            EventsManager.Instance.inputEvents.ThreePressed();
        }
    }

    public void RotatePressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            EventsManager.Instance.inputEvents.RotatePressed();
        }
    }

    public void SelectObject(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            EventsManager.Instance.inputEvents.SelectObjectPressed();
        }
    }

    public void CancelPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            EventsManager.Instance.inputEvents.CancelPressed();
        }
    }
}