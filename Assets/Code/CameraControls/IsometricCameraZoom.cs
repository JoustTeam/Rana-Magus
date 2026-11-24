using UnityEngine;
using UnityEngine.InputSystem;

//Comment to get git to notice the files
public class IsometricCameraZoom : MonoBehaviour
{
    public float zoomSpeed = 200;
    public float zoomSmoothness = 5;

    public float minZoom = 20;
    public float maxZoom = 80;

    public float camPitch = 5f;
    public float rotationStartZoom = 45f;
    public float camLowerAmount = 5f;

    private float currentZoom;
    private Camera mainCam;
    private Vector3 originalCameraPosition;

    private Quaternion originalRotation;
    private Quaternion zoomedRotation;

    private void Awake()
    {
        mainCam = GetComponentInChildren<Camera>();
        currentZoom = mainCam.fieldOfView;

        // Stores the original rotation of the camera
        originalRotation = mainCam.transform.localRotation;

        // Keeps the camera's yaw and roll values when zooming
        Vector3 angles = originalRotation.eulerAngles;
        zoomedRotation = Quaternion.Euler(camPitch, angles.y, angles.z);

        originalCameraPosition = mainCam.transform.localPosition;
    }

    void Update()
    {
        // Smoothly zoom
        mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, currentZoom, zoomSmoothness * Time.deltaTime);

        // Calculate how far camera is zoomed in
        float rotationPercentage = Mathf.InverseLerp(rotationStartZoom, minZoom, currentZoom);

        // Gets the current camera angle    
        Vector3 angles = mainCam.transform.localEulerAngles;

        // Transitions the pitch of the camera to be a horizontal view
        float targetPitch = Mathf.LerpAngle(originalRotation.eulerAngles.x, camPitch, rotationPercentage);
        // Preserves the Yaw and Roll Values of the camera
        Quaternion targetRotation = Quaternion.Euler(targetPitch, angles.y, angles.z);

        // Rotates the camera 
        mainCam.transform.localRotation = Quaternion.Slerp(mainCam.transform.localRotation, targetRotation, zoomSmoothness * Time.deltaTime);

        // Lowers the camera when zooms in
        Vector3 targetPosition = originalCameraPosition;
        targetPosition.y = Mathf.Lerp(originalCameraPosition.y, 10f, rotationPercentage);

        mainCam.transform.localPosition = Vector3.Lerp(mainCam.transform.localPosition, targetPosition, zoomSmoothness * Time.deltaTime);
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float scrollInput = context.ReadValue<Vector2>().y;

            // Caps amount of zoom
            currentZoom = Mathf.Clamp(currentZoom - scrollInput * zoomSpeed * Time.deltaTime, minZoom, maxZoom);
        }
    }
}
