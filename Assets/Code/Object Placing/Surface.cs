using UnityEngine;
using System.Collections.Generic;

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
    [SerializeField] List<Surface> adjacentWalls;

    void Start()
    {
        timePlaced = Time.deltaTime;
        adjacentWalls = new List<Surface>();
    }

    void OnCollisionEnter(Collision otherCol) //used to aid merging walls and closing gaps
    {
        //first check they're both walls
        if (type == SurfaceType.Wall)
        {
            Surface otherSurface = otherCol.gameObject.GetComponent<Surface>();
            if (otherSurface.type == SurfaceType.Wall)
            {
                adjacentWalls.Add(otherSurface);

                Transform otherTrans = otherCol.gameObject.GetComponent<Transform>();

                //then grab their rotations (rounded to avoid tiny decimals)
                float myRotation = Mathf.Round(transform.localEulerAngles.y);
                float otherRotation = Mathf.Round(otherTrans.localEulerAngles.y);

                //and correct for the -45 appearing as 315
                if (myRotation > 100)
                {
                    myRotation = 45;
                }
                if (otherRotation > 100)
                {
                    otherRotation = 45;
                }

                if (transform.localRotation == otherTrans.localRotation) //if the rotations are the same, they're part of the same wall
                {
                    //Debug.Log("Same Wall");

                    if (timePlaced < otherSurface.timePlaced) //the older wall calls the function, to avoid both of them calling it
                    {
                        //Debug.Log("I'm calling it");
                        GridFunctions.MergeWalls(transform, otherTrans);
                    }
                    else
                    {
                        //Debug.Log("I'm stepping back");
                    }
                    
                    Destroy(gameObject); //destroy the originals
                }
                else if ((myRotation != 45 && otherRotation == 45) || (myRotation == 45 && otherRotation != 45)) //if it is a straight wall meeting a diagonal one
                {
                    //and the walls share an end point (meaning they're connected)
                    if (startPos == otherSurface.startPos || startPos == otherSurface.endPos || endPos == otherSurface.startPos || endPos == otherSurface.endPos)
                    {
                        if (!shrunk) //and the wall isn't already corrected
                        {
                            //reduce the size slightly
                            transform.localScale = new Vector3(transform.localScale.x - 0.17f, transform.localScale.y, transform.localScale.z);
                            shrunk = true;
                        }
                    }
                }
                else
                {
                    //Debug.Log("We're both walls, but not the same wall");
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

    void OnCollisionExit(Collision otherCol)//used to aid closing gaps
    {
        if (type == SurfaceType.Wall)
        {
            Surface otherSurface = otherCol.gameObject.GetComponent<Surface>();
            if (otherSurface.type == SurfaceType.Wall)
            {
                adjacentWalls.Remove(otherSurface);

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

                if (myRotation != 45 && otherRotation == 45 || myRotation == 45 && otherRotation != 45) //same logic as above, just skips checking if they share an end point
                {
                    if (shrunk)
                    {
                        //increase the size slightly (known bug. this will increase the size, even if the wall has two connections that require it to be shrunk. will get it on it maybe later)
                        transform.localScale = new Vector3(transform.localScale.x + 0.17f, transform.localScale.y, transform.localScale.z);
                        shrunk = false;
                    }
                }
            }
        }
    }

    public void setPos(Vector3 s, Vector3 e) //sets it's start and end positions
    {
        startPos = s;
        endPos = e;
    }

    public List<Surface> getNeighbours()
    {
        for (int i=0 ; i<adjacentWalls.Count ; i++)
        {
            if (adjacentWalls[i] == null)
            {
                adjacentWalls.RemoveAt(i);
            }
        }

        return adjacentWalls;
    }
}
