using UnityEngine;
using UnityEngine.InputSystem;
public class IsometricCameraZoom : MonoBehaviour
{
    public float zoomSpeed = 200;
    public float zoomSmoothness = 5;

    public float minZoom = 2;
    public float maxZoom = 40;

    private float currentZoom;
    private Camera mainCam;

    private void Awake()
    {
        mainCam = GetComponentInChildren<Camera>();
        currentZoom = mainCam.orthographicSize;
    }

    void Update()
    {
        // Smoothly zoom
        mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, currentZoom, zoomSmoothness * Time.deltaTime);
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
