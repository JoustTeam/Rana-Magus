using UnityEngine;

public class ChangeMode : MonoBehaviour
{
    void OnEnable()
    {
        EventsManager.Instance.inputEvents.on1Pressed += onePressed;
        EventsManager.Instance.inputEvents.on2Pressed += twoPressed;
        EventsManager.Instance.inputEvents.on3Pressed += threePressed;
    }

    void OnDisable()
    {
        EventsManager.Instance.inputEvents.on1Pressed -= onePressed;
        EventsManager.Instance.inputEvents.on2Pressed -= twoPressed;
        EventsManager.Instance.inputEvents.on3Pressed -= threePressed;
    }

    void onePressed(InputEventContext iec)
    {
        Debug.Log("Default Mode");
        EventsManager.Instance.inputEvents.ChangeInputEventContext(InputEventContext.DEFAULT);
    }

    void twoPressed(InputEventContext iec)
    {
        Debug.Log("Wall Build Mode");
        EventsManager.Instance.inputEvents.ChangeInputEventContext(InputEventContext.WALLBUILDMODE);
    }

    void threePressed(InputEventContext iec)
    {
        Debug.Log("Object Build Mode");
        EventsManager.Instance.inputEvents.ChangeInputEventContext(InputEventContext.OBJECTBUILDMODE);
    }
}
