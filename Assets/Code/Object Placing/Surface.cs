using UnityEngine;

public enum SurfaceType
{
    Floor,
    Wall,
    SelectType

}
public class Surface : MonoBehaviour
{
    public SurfaceType type = SurfaceType.SelectType;
    public float timePlaced;
    public Vector3 startPos;
    public Vector3 endPos;
    bool  shrunk;

    void Start()
    {
        timePlaced = Time.deltaTime;
    }

    void OnCollisionEnter(Collision otherCol)
    {
        if (type == SurfaceType.Wall)
        {
            Surface otherSurface = otherCol.gameObject.GetComponent<Surface>();
            if (otherSurface.type == SurfaceType.Wall)
            {
                Transform otherTrans = otherCol.gameObject.GetComponent<Transform>();

                float myRotation = transform.localEulerAngles.y;
                float otherRotation = otherTrans.localEulerAngles.y;

                if (myRotation > 100)
                {
                    myRotation = 45;
                }
                if (otherRotation > 100)
                {
                    otherRotation = 45;
                }

                if (transform.localRotation == otherTrans.localRotation)
                {
                    Debug.Log("Same Wall");

                    if (timePlaced < otherSurface.timePlaced)
                    {
                        Debug.Log("I'm calling it");
                        GridFunctions.MergeWalls(transform, otherTrans);
                    }
                    else
                    {
                        Debug.Log("I'm stepping back");
                    }
                    
                    Destroy(gameObject);
                }
                else if (myRotation != 45 && otherRotation == 45 || myRotation == 45 && otherRotation != 45)
                {
                    if (startPos == otherSurface.startPos || startPos == otherSurface.endPos || endPos == otherSurface.startPos || endPos == otherSurface.endPos)
                    {
                        if (!shrunk)
                        {
                            transform.localScale = new Vector3(transform.localScale.x - 0.17f, transform.localScale.y, transform.localScale.z);
                            shrunk = true;
                        }
                    }
                }
                else
                {
                    Debug.Log("We're both walls, but not the same wall");
                }
            }
            else
            {
                //Debug.Log("I'm a wall, but they're not");
            }
        }
        else
        {
            //Debug.Log("I'm not a Wall");
        }
    }

    void OnCollisionExit(Collision otherCol)
    {
        Debug.Log("called");
        if (type == SurfaceType.Wall)
        {
            Surface otherSurface = otherCol.gameObject.GetComponent<Surface>();
            if (otherSurface.type == SurfaceType.Wall)
            {
                Transform otherTrans = otherCol.gameObject.GetComponent<Transform>();

                float myRotation = transform.localEulerAngles.y;
                float otherRotation = otherTrans.localEulerAngles.y;

                if (myRotation > 100)
                {
                    myRotation = 45;
                }
                if (otherRotation > 100)
                {
                    otherRotation = 45;
                }

                if (myRotation != 45 && otherRotation == 45 || myRotation == 45 && otherRotation != 45)
                {
                    if (shrunk)
                    {
                        transform.localScale = new Vector3(transform.localScale.x + 0.17f, transform.localScale.y, transform.localScale.z);
                        shrunk = false;
                    }
                }
            }
        }
    }

    public void setPos(Vector3 s, Vector3 e)
    {
        startPos = s;
        endPos = e;
    }
}
