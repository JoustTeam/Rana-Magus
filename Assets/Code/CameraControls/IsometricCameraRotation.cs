using UnityEngine;
using UnityEngine.InputSystem;

public class IsometricCameraRotation : MonoBehaviour
{
    public float rotationSpeed = 50;

    private bool isRotating;
    private float mouseDeltaX;

    void Update()
    {
        // Rotates Camera when right click is being held
        if (isRotating)
        {
            transform.Rotate(Vector3.up, mouseDeltaX * rotationSpeed * Time.deltaTime);
        }
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        isRotating = context.ReadValueAsButton();
    }

    // Detects mouse movements and location
    public void OnMouseMove(InputAction.CallbackContext context)
    {
        mouseDeltaX = context.ReadValue<Vector2>().x;
    }
}
