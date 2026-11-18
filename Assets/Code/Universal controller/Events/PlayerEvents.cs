using System;

/*
Events that control the player character. Other systems 
can raise these to lock or unlock player movement,
and the player's movement script subscribes to them to react.
As in InputEvents, each event has a public method that fires it.
This would be where we but the camera and the build mode
*/

public class PlayerEvents
{
    public event Action onDisablePlayerMovement;
    public void DisablePlayerMovement()
    {
        if (onDisablePlayerMovement != null) 
        {
            onDisablePlayerMovement();
        }
    }

    public event Action onEnablePlayerMovement;
    public void EnablePlayerMovement()
    {
        if (onEnablePlayerMovement != null) 
        {
            onEnablePlayerMovement();
        }
    }
}