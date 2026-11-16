using UnityEngine;
using UnityEngine.InputSystem;

public class IsometricCameraPanner : MonoBehaviour
{
    public float panningSpeed = 20f;

    private Camera mainCam;

    public Vector2 panLimitX;
    public Vector2 panLimitZ;

    private Vector2 positionPan;

    private void Awake()
    {
        mainCam = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        // Move camera relative to its rotation
        transform.position += Quaternion.Euler(0, mainCam.transform.eulerAngles.y, 0)
            * new Vector3(positionPan.x, 0, positionPan.y)
            * (panningSpeed * Time.deltaTime);

        // Panning Limits / Camera Bounds
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, panLimitX.x, panLimitX.y),
            transform.position.y,
            Mathf.Clamp(transform.position.z, panLimitZ.x, panLimitZ.y)
        );
    }

    // Reads Keyboard Inputs
    public void OnPan(InputAction.CallbackContext context)
    {
        positionPan = context.ReadValue<Vector2>();
    }
}
