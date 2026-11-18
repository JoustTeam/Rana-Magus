

/*
Holds the context that decides which part of the code is read as valid.
The default in this case would be the camera controls, and everything else 
different usecases of our code such us building ui etc.
*/

public enum InputEventContext
{
    DEFAULT,
    BUILDMODE
}