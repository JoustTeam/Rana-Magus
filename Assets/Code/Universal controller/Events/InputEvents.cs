using UnityEngine;
using System;
using UnityEngine.InputSystem;

/* 
Events related to player input. InputManager raises them when the player
presses a button, and any other script can subscribe to react.
Each event is paired with a public method that fires it.
*/


public class InputEvents
{
    //Sets current context. Every script can read it
    public InputEventContext inputEventContext { get; private set; } = InputEventContext.DEFAULT;

    //Must go through this function to change it (eg. DEFAULT -> BUILDMODE),
    //so listeners can handle the same button differently depending on the mode.
    public void ChangeInputEventContext(InputEventContext newContext) 
    {
        this.inputEventContext = newContext;
    }


    //Examples of how to set up input recognition from the input actions
    public event Action<Vector2> onMovePressed;
    public void MovePressed(Vector2 moveDir) 
    {
        if (onMovePressed != null) 
        {
            onMovePressed(moveDir);
        }
    }

    public event Action<InputEventContext> onSubmitPressed;
    public void SubmitPressed()
    {
        if (onSubmitPressed != null) 
        {
            onSubmitPressed(this.inputEventContext);
        }
    }

    public event Action<InputEventContext> onMouseDown;
    public void MouseDown()
    {
        if (onMouseDown != null)
        {
            onMouseDown(this.inputEventContext);
        }
    }

    public event Action<InputEventContext> onMouseUp;
    public void MouseUp()
    {
        if (onMouseUp != null)
        {
            onMouseUp(this.inputEventContext);
        }
    }

    public bool IsMouseDown()
    {
        return Mouse.current.leftButton.isPressed;
    }

    public Vector3 GetMousePos()
    {
        return Mouse.current.position.ReadValue();
    }

    public event Action<InputEventContext> on1Pressed;
    public void OnePressed()
    {
        if (on1Pressed != null)
        {
            on1Pressed(this.inputEventContext);
        }
    }

    public event Action<InputEventContext> on2Pressed;
    public void TwoPressed()
    {
        if (on2Pressed != null)
        {
            on2Pressed(this.inputEventContext);
        }
    }

    public event Action<InputEventContext> on3Pressed;
    public void ThreePressed()
    {
        if (on3Pressed != null)
        {
            on3Pressed(this.inputEventContext);
        }
    }

    public event Action<InputEventContext> onRotatePressed;
    public void RotatePressed()
    {
        if (onRotatePressed != null)
        {
            onRotatePressed(this.inputEventContext);
        }
    }

    public event Action<InputEventContext> onSelectObjectPressed;
    public void SelectObjectPressed()
    {
        if (onSelectObjectPressed != null)
        {
            onSelectObjectPressed(this.inputEventContext);
        }
    }

    //current not in use
    public event Action<InputEventContext> onCancelPressed;
    public void CancelPressed()
    {
        if (onCancelPressed != null)
        {
            onCancelPressed(this.inputEventContext);
        }
    }

}