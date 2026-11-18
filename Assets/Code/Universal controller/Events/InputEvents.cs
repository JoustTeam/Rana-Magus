using UnityEngine;
using System;

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

}