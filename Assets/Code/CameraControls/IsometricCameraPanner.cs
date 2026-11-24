using UnityEngine;
using UnityEngine.InputSystem;

public class IsometricCameraPanner : MonoBehaviour
{
    public float panningSpeed = 20f;

    public BoxCollider cameraBounds;

    private Camera mainCam;
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

        // Restricts camera movement to the box volume
        if (cameraBounds != null)
        {
            Vector3 position = transform.position;
            Bounds bounds = cameraBounds.bounds;

            position.x = Mathf.Clamp(position.x, bounds.min.x, bounds.max.x);
            position.z = Mathf.Clamp(position.z, bounds.min.z, bounds.max.z);

            transform.position = position;
        }
    }

    // Reads Keyboard Inputs
    public void OnPan(InputAction.CallbackContext context)
    {
        positionPan = context.ReadValue<Vector2>();
    }
}

