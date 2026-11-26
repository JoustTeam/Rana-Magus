using UnityEngine;
using System;
using System.Collections.Generic;

public static class GridFunctions //stores the grid layout reference for other scripts to call on without a reference
{
    public static GridLayout grid;
    public static Bounds worldBounds;
    public static event Action<Vector3, float, float, Vector3, Vector3> mergeWall;
    public static bool mergingWalls = false;

    static GridFunctions() //finds the grid upon initialisation (assuming there is only 1 grid object)
    {
        grid = GameObject.FindFirstObjectByType<GridLayout>();
        worldBounds = GameObject.Find("CameraBounds").GetComponent<BoxCollider>().bounds;
    }

    public static Vector3 getGridCell(Vector3 pos) //takes a world position and returns the center of the nearest grid cell
    {
        Vector3Int cellPos = grid.WorldToCell(pos);
        return grid.CellToWorld(cellPos);
    }

    public static Vector3 cycleDirection(Vector3 dir)
    {
        if (dir == new Vector3(1,0,0))
        {
            return new Vector3(0,0,1);
        }
        else if (dir == new Vector3(0,0,1))
        {
            return new Vector3(-1,0,0);
        }
        else if (dir == new Vector3(-1,0,0))
        {
            return new Vector3(0,0,-1);
        }
        else
        {
            return dir;
        }
    }

    public static void MergeWalls(Transform wall1, Transform wall2) //merges two walls into one wall
    {
        if (!mergingWalls)
        {
            mergingWalls = true;  
            //getting the scripts of both walls
            Surface sur1 = wall1.gameObject.GetComponent<Surface>();
            Surface sur2 = wall2.gameObject.GetComponent<Surface>();

            //getting their start and end positions
            Vector3 startOfWall1 = sur1.startPos;
            Vector3 endOfWall1 = sur1.endPos;
            Vector3 startOfWall2 = sur2.startPos;
            Vector3 endOfWall2 = sur2.endPos;

            //getting their rotations and scales
            float rotation = wall1.localEulerAngles.y;
            float wall1Scale = wall1.localScale.x - 0.3f;
            float wall2Scale = wall2.localScale.x - 0.3f;
            
            //finding out which point they share/overlap and their midpoint
            List<Vector3> calculatedPos = findMidpoint(startOfWall1, endOfWall1, startOfWall2, endOfWall2, wall1Scale, wall2Scale, rotation);
            Vector3 midPoint = calculatedPos[2];
            wall1Scale = calculatedPos[3].x; //also getting the adjusted scale

            //calculating the new scale
            float newScale = wall1Scale + wall2Scale + 0.3f;

            midPoint.y = wall1.localScale.y / 2; //adjusting for height

            //spawning the new wall
            mergeWall(midPoint, newScale, rotation, calculatedPos[0], calculatedPos[1]);

            mergingWalls = false;
        }
    }

    //finds which point the walls share / overlap
    static List<Vector3> findMidpoint(Vector3 startOfWall1, Vector3 endOfWall1, Vector3 startOfWall2, Vector3 endOfWall2, float wall1Scale, float wall2Scale, float rotation)
    {
        Vector3 midPoint = new Vector3(0,0,0);
        List<Vector3> returnValue = new List<Vector3>();

        //if the walls do share a point
        if (startOfWall1 == startOfWall2)
        {
            //find the mid point and note the start and end of the new wall
            midPoint = (endOfWall1 + endOfWall2) / 2;
            returnValue.Add(endOfWall1);
            returnValue.Add(endOfWall2);
        }
        else if (startOfWall1 == endOfWall2)
        {
            midPoint = (startOfWall2 + endOfWall1) / 2;
            returnValue.Add(startOfWall2);
            returnValue.Add(endOfWall1);
        }
        else if (endOfWall1 == startOfWall2)
        {
            midPoint = (startOfWall1 + endOfWall2) / 2;
            returnValue.Add(startOfWall1);
            returnValue.Add(endOfWall2);
        }
        else if (endOfWall1 == endOfWall2)
        {
            midPoint = (startOfWall1 + startOfWall2) / 2;
            returnValue.Add(startOfWall1);
            returnValue.Add(startOfWall2);
        }
        else //the midpoint is not the same, so we need to find the points in the overlap
        {
            //first make an ordered list of the positions (sorted by x then z), lowest first
            //this is because the biggest and smallest vectors are likely to be the end points:
            //         ----------  wall 1
            // -------------       wall 2
            // largest to smallest
            List<Vector3> orderedPositions = new List<Vector3>();

            orderedPositions.Add(startOfWall1);
            orderedPositions.Add(endOfWall1);
            orderedPositions.Add(startOfWall2);
            orderedPositions.Add(endOfWall2);

            orderedPositions.Sort((a,b) =>
            {
                int result = a.x.CompareTo(b.x);

                if (result == 0)
                    result = a.z.CompareTo(b.z);

                return result;
            });

            //then calculate the overlap based on the rotation
            if (rotation == 0)
            {
                //and then find the new scale
                float diff = Mathf.Abs(orderedPositions[2].x-orderedPositions[1].x);
                wall1Scale -= diff;
            }
            else if (rotation == 90)
            {
                float diff = Mathf.Abs(orderedPositions[2].z-orderedPositions[1].z);
                wall1Scale -= diff;
            }
            else
            {
                float diff = Mathf.Abs(orderedPositions[2].x-orderedPositions[1].x + orderedPositions[2].z-orderedPositions[1].z);
                wall1Scale -= diff;
            }

            //find the midpoint and note the start and end of the new wall
            midPoint = (orderedPositions[0] + orderedPositions[3]) / 2;
            returnValue.Add(orderedPositions[0]);
            returnValue.Add(orderedPositions[3]);
        }

        //add the values to the return list
        returnValue.Add(midPoint);
        returnValue.Add(new Vector3(wall1Scale,0,0)); //using this for ease of storage

        return returnValue;
    }

    public static void DetectRoomFloodFill(Vector3 startPos, Vector3 startDirection) //turn into a floodfill to find all the cells in a room row by row rather than sprial
    {
        Debug.Log("Looking for room");
        Debug.Log(worldBounds.max);
        Debug.Log(worldBounds.min);

        bool isRoom = false;
        bool foundEdge = false;
        bool searchedBothWays = false;
        int checkedDirections = 0;

        Vector3 currentPos = getGridCell(startPos + startDirection);
        Vector3 direction = startDirection;
        Vector3 hitPos = currentPos;

        List<Vector3> searchedCells = new List<Vector3>();
        
        //while(!isRoom || !foundEdge)
        for (int i=0 ; i < 260 ; i++)
        {
            if (currentPos.x > worldBounds.max.x || currentPos.z > worldBounds.max.z || currentPos.x < worldBounds.min.x || currentPos.z < worldBounds.min.z)
            {
                foundEdge = true;
                Debug.Log("Found the edge of the world");
                break;
            }

            if (checkedDirections == 4)
            {
                isRoom = true;
                Debug.Log("Found room");
                break;
            }

            if (searchedCells.Contains(currentPos))
            {
                Debug.Log("Found a searched cell");
                direction *= -1;
                currentPos = hitPos + direction;
                checkedDirections++;
                continue;
            }

            checkedDirections = 0;
            searchedCells.Add(currentPos);

            RaycastHit[] hits = Physics.RaycastAll(currentPos, direction, 1);
            Debug.DrawRay(currentPos, direction, Color.green, 5);

            if (hits.Length == 0)
            {
                currentPos += direction;
                currentPos = getGridCell(currentPos);
            }
            else
            {
                Debug.Log(i.ToString() + ": Occupied cell");
                Surface checkSurface = null;
                foreach (RaycastHit hit in hits)
                {
                    checkSurface = hit.collider.gameObject.GetComponent<Surface>();
                    if (checkSurface != null)
                    {
                        break;
                    }
                }

                if (checkSurface != null)
                {
                    if (checkSurface.type == SurfaceType.Wall)
                    {
                        Debug.Log(i.ToString() + ": Wall in cell");
                        if (searchedBothWays)
                        {
                            hitPos = currentPos;
                            direction = cycleDirection(direction);
                            currentPos += direction;
                        }
                        else
                        {
                            direction *= -1;
                            currentPos += direction;
                            currentPos = getGridCell(currentPos);
                        }
                    }
                }
                else
                {
                    currentPos += direction;
                    currentPos = getGridCell(currentPos);
                }
            }
        }

        Debug.Log(currentPos);
    }

    public static void DetectRoom(Surface startWall) //finds a room based on connecting walls
    {
        Debug.Log("LOOKING FOR ROOM");
        List<Surface> connectingWalls = new List<Surface>();
        List<Surface> uncheckedWalls = new List<Surface>();
        bool foundLoop = false;

        connectingWalls.Add(startWall);
        uncheckedWalls = addNeighbours(uncheckedWalls, startWall, connectingWalls);

        while (uncheckedWalls.Count > 0)
        {
            Debug.Log("ENTERED LOOP");
            if (startWall == null)
            {
                Debug.Log("Start wall has been destroyed");
                return;
            }

            Surface currentWall = uncheckedWalls[0];
            //Debug.Log("Current: " + currentWall.gameObject.transform.position.ToString());

            uncheckedWalls = addNeighbours(uncheckedWalls, currentWall, connectingWalls);
            connectingWalls.Add(currentWall);
            uncheckedWalls.Remove(currentWall);

            if (uncheckedWalls.Contains(startWall) && connectingWalls.Count > 2)
            {
                Debug.Log("Found start");
                foundLoop = true;
                uncheckedWalls.Remove(startWall);
            }
            else if (uncheckedWalls.Contains(startWall) && connectingWalls.Count <= 2)
            {
                uncheckedWalls.Remove(startWall);
            }
        }

        Debug.Log(foundLoop);
        Debug.Log(startWall.gameObject.transform.position);
        string d = "{ ";
        foreach (Surface s in connectingWalls)
        {
            d += s.gameObject.transform.position.ToString() + ", ";
        }
        d += " }";
        Debug.Log(d);
    }

    static List<Surface> addNeighbours(List<Surface> uncheckedWalls, Surface currentWall, List<Surface> connectingWalls)
    {
        List<Surface> newNeighbours = currentWall.getNeighbours();
        Debug.Log(newNeighbours.Count);

        if (newNeighbours.Count > 0)
        {
            Debug.Log("Found neighbours");
            foreach(Surface s in newNeighbours)
            {
                if (s == connectingWalls[0])
                {
                    uncheckedWalls.Add(s);
                }
                else if (!uncheckedWalls.Contains(s) && !connectingWalls.Contains(s))
                {
                    uncheckedWalls.Add(s);
                }
            }
        }
        else
        {
            Debug.Log("Lone wall");
        }

        return uncheckedWalls;
    }
}
