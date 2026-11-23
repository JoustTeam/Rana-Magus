using UnityEngine;
using System;
using System.Collections.Generic;

public static class GridFunctions //stores the grid layout reference for other scripts to call on without a reference
{
    public static GridLayout grid;
    public static event Action<Vector3, float, float, Vector3, Vector3> mergeWall;
    public static bool mergingWalls = false;

    static GridFunctions() //finds the grid upon initialisation (assuming there is only 1 grid object)
    {
        grid = GameObject.FindFirstObjectByType<GridLayout>();
    }

    public static void MergeWalls(Transform wall1, Transform wall2)
    {
        if (!mergingWalls)
        {
            mergingWalls = true;  
            Surface sur1 = wall1.gameObject.GetComponent<Surface>();
            Surface sur2 = wall2.gameObject.GetComponent<Surface>();

            Vector3 startOfWall1 = sur1.startPos;
            Vector3 endOfWall1 = sur1.endPos;
            Vector3 startOfWall2 = sur2.startPos;
            Vector3 endOfWall2 = sur2.endPos;

            Debug.Log(startOfWall1);
            Debug.Log(endOfWall1);
            Debug.Log(startOfWall2);
            Debug.Log(endOfWall2);

            float rotation = wall1.localEulerAngles.y;
            float wall1Scale = 0;
            float wall2Scale = 0;

            if (rotation == 45 || rotation == -45)
            {
                wall1Scale = wall1.localScale.x;
                wall2Scale = wall2.localScale.x;
            }
            else
            {
                wall1Scale = wall1.localScale.x - 0.3f;
                wall2Scale = wall2.localScale.x - 0.3f;
            }
            

            List<Vector3> calculatedPos = findMidpoint(startOfWall1, endOfWall1, startOfWall2, endOfWall2, wall1Scale, wall2Scale, rotation);
            Vector3 midPoint = calculatedPos[2];
            wall1Scale = calculatedPos[3].x;

            float newScale = wall1Scale + wall2Scale + 0.3f;

            midPoint.y = wall1.localScale.y / 2;

            mergeWall(midPoint, newScale, rotation, calculatedPos[0], calculatedPos[1]);

            mergingWalls = false;
        }
    }

    static List<Vector3> findMidpoint(Vector3 startOfWall1, Vector3 endOfWall1, Vector3 startOfWall2, Vector3 endOfWall2, float wall1Scale, float wall2Scale, float rotation)
    {
        Vector3 midPoint = new Vector3(0,0,0);
        List<Vector3> returnValue = new List<Vector3>();

        if (startOfWall1 == startOfWall2)
        {
            Debug.Log(startOfWall1);
            midPoint = (endOfWall1 + endOfWall2) / 2;
            returnValue.Add(endOfWall1);
            returnValue.Add(endOfWall2);
        }
        else if (startOfWall1 == endOfWall2)
        {
            Debug.Log(startOfWall1);
            midPoint = (startOfWall2 + endOfWall1) / 2;
            returnValue.Add(startOfWall2);
            returnValue.Add(endOfWall1);
        }
        else if (endOfWall1 == startOfWall2)
        {
            Debug.Log(endOfWall1);
            midPoint = (startOfWall1 + endOfWall2) / 2;
            returnValue.Add(startOfWall1);
            returnValue.Add(endOfWall2);
        }
        else if (endOfWall1 == endOfWall2)
        {
            Debug.Log(endOfWall1);
            midPoint = (startOfWall1 + startOfWall2) / 2;
            returnValue.Add(startOfWall1);
            returnValue.Add(startOfWall2);
        }
        else
        {
            if (rotation == 0)
            {
                float diff1 = Mathf.Abs(endOfWall2.x-startOfWall1.x);
                float diff2 = Mathf.Abs(endOfWall1.x-startOfWall2.x);

                if (diff1 < diff2)
                {
                    Debug.Log(diff1);
                    wall1Scale -= diff1;
                    midPoint = (startOfWall2 + endOfWall1) / 2;
                    returnValue.Add(startOfWall2);
                    returnValue.Add(endOfWall1);
                }
                else
                {
                    Debug.Log(diff2);
                    wall1Scale -= diff2;
                    midPoint = (startOfWall1 + endOfWall2) / 2;
                    returnValue.Add(startOfWall1);
                    returnValue.Add(endOfWall2);
                }
            }
            else if (rotation == 90)
            {
                float diff1 = Mathf.Abs(endOfWall2.z-startOfWall1.z);
                float diff2 = Mathf.Abs(endOfWall1.z-startOfWall2.z);

                if (diff1 < diff2)
                {
                    Debug.Log(diff1);
                    wall1Scale -= diff1;
                    midPoint = (startOfWall2 + endOfWall1) / 2;
                    returnValue.Add(startOfWall2);
                    returnValue.Add(endOfWall1);
                }
                else
                {
                    Debug.Log(diff2);
                    wall1Scale -= diff2;
                    midPoint = (startOfWall1 + endOfWall2) / 2;
                    returnValue.Add(startOfWall1);
                    returnValue.Add(endOfWall2);
                }
            }
            else
            {
                float diff1 = Mathf.Abs(endOfWall2.x-startOfWall1.x + endOfWall2.z-startOfWall1.z);
                float diff2 = Mathf.Abs(endOfWall1.x-startOfWall2.x + endOfWall1.z-startOfWall2.z);

                if (diff1 < diff2)
                {
                    Debug.Log(diff1);
                    wall1Scale -= diff1;
                    midPoint = (startOfWall2 + endOfWall1) / 2;
                    returnValue.Add(startOfWall2);
                    returnValue.Add(endOfWall1);
                }
                else
                {
                    Debug.Log(diff2);
                    wall1Scale -= diff2;
                    midPoint = (startOfWall1 + endOfWall2) / 2;
                    returnValue.Add(startOfWall1);
                    returnValue.Add(endOfWall2);
                }
            }
        }

        returnValue.Add(midPoint);
        returnValue.Add(new Vector3(wall1Scale,0,0));

        return returnValue;
    }
}
