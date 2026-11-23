using UnityEngine;

public class GhostWallFunctions : MonoBehaviour
{
    public bool collidingWithWall = false;

    void OnTriggerEnter(Collider other)
    {
        Surface otherScript = other.gameObject.GetComponent<Surface>();

        if (otherScript != null)
        {
            if (otherScript.type == SurfaceType.Wall)
            {
                Transform otherTrans = other.gameObject.GetComponent<Transform>();

                if (transform.localRotation == otherTrans.localRotation)
                {
                    collidingWithWall = true;
                    return;
                }
            }
        }

        collidingWithWall = false;
    }

    void OnTriggerExit(Collider other)
    {
        Surface otherScript = other.gameObject.GetComponent<Surface>();

        if (otherScript != null)
        {
            if (otherScript.type == SurfaceType.Wall)
            {
                Transform otherTrans = other.gameObject.GetComponent<Transform>();

                if (transform.localRotation == otherTrans.localRotation)
                {
                    collidingWithWall = false;
                }
            }
        }
    }

    void OnDisable()
    {
        collidingWithWall = false;
    }

    void OnEnable()
    {
        collidingWithWall = false;
    }
}
