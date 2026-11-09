using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlaceWalls : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] Vector3 mousePos;
    public InputAction mouseDown;

    [Header("Flags")]
    [SerializeField] bool placingWall = false;
    [SerializeField] bool drawingGhost = false;
    [SerializeField] bool xOrz = false; //determines if the line is along the x or z axis (true for z, false for x)
    [SerializeField] bool diagonal = false; //determines if the line is diagonal

    [Header("Prefabs and Objects")]
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject ghostWall;
    
    [Header("Wall Postion")]
    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 endPos;

    void OnEnable() //setting up the event handlers for input
    {
        mouseDown.Enable();
        mouseDown.started += mouseClick;
        mouseDown.canceled += mouseUp;
        resetGhostWall(); //ensures the ghost wall is properly set up
    }

    void OnDisable() //removing event handlers
    {
        mouseDown.started -= mouseClick;
        mouseDown.canceled -= mouseUp;
        mouseDown.Disable();
    }

    void Update()
    {
        //finding the mouse's position on the floor object
        rayCastMousePos();
        //snapping it to the grid
        mousePos.y = 0;
        Vector3Int cellPos = GridFunctions.grid.WorldToCell(mousePos);
        transform.position = GridFunctions.grid.CellToWorld(cellPos);

        if (Mouse.current.leftButton.isPressed) //checking for input to draw the ghost wall continuously
        {
            endPos = transform.position;
            drawGhost();
        }
    }

    void rayCastMousePos() //fires a ray from the mouse to the world
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out hit)) //if there is as hit, set the mouse position to that location
        {
            mousePos = hit.point;
        }
    }

    void mouseClick(InputAction.CallbackContext ctx) //detects when the left mouse button is pressed
    {
        if (!placingWall) //if there is not a wall currently placing
        {
            placingWall = true;
            startPos = transform.position; //set the start of the wall to the pointer's current position
        }
    }

    void mouseUp(InputAction.CallbackContext ctx) //detects when the left mouse button is unpressed
    {
        endPos = transform.position; //sets the end position to the pointer's current position

        if (startPos == endPos) //if the start and end positions are the same, there is no wall
        {
            Debug.Log("not in line");
            placingWall = false;
            return;
        }
        else
        {
            // figuring out the direction of the wall using the vector direction
            //
            // - horizontal:
            // -----> =x +z
            // <----- =x -z
            //
            // - vertical:
            //  ^    |
            //  |    |
            //  |    |
            //  |    v
            //  +x  -x
            //  =z  =z
            //
            // - diagonal:
            //         |
            //  -x +z  |  +x +z
            //       \ | /
            // -----------------
            //       / | \
            //  -x -z  |  +x -z
            //         |

            //finding the change in x and the change in z
            float xDiff = endPos.x - startPos.x;
            float zDiff = endPos.z - startPos.z;
            //making the differences positive
            if (xDiff < 0)
            {
                xDiff *= -1;
            }
            if (zDiff < 0)
            {
                zDiff *= -1;
            }

            if (diagonal) //if the line is diagonal
            {
                if (xDiff != zDiff) //and the gradient isn't 1 (aka the pointer has drifted off course)
                {
                    Debug.Log("DRIFTED");
                    //use the ghost wall's position instead of the pointer's
                    endPos = 2*ghostWall.transform.position - startPos;
                }
            }
            else if (xOrz && startPos.z != endPos.z) //if the line is along the z axis, but the z coords do not match
            {
                Debug.Log("DRIFTED");
                //use the z of the start position
                endPos = new Vector3(endPos.x, endPos.y, startPos.z);
            }
            else if (!xOrz && startPos.x != endPos.x) //if the line is along the x axis, but the x coords do not match
            {
                Debug.Log("DRIFTED");
                //use the x of the start position
                endPos = new Vector3(startPos.x, endPos.y, endPos.z);
            }

            //call place wall
            placeWall();
        }

        resetGhostWall(); //reset the ghost wall at the end
    }

    void placeWall() //instansiates a wall in the correct location, scale and rotation
    {
        //setting the diagonal angle
        float angleDiagonal = 45;
        if (diagonal) //if the line is diagonal
        {
            //checking if it needs to be rotated down instead of up

            float xDiff = endPos.x - startPos.x;
            float zDiff = endPos.z - startPos.z;

            if (zDiff * xDiff > 0)
            {
                angleDiagonal = -45; //updating the angle for this case
            }

        }

        //finding the midpoint of the line to use as its position
        Vector3 midPoint = (startPos + endPos) / 2;
        midPoint.y = 1.5f;

        //setting the distance var to calculate it's length
        float dist = 0;
        if (diagonal)
        {
            dist = (startPos - endPos).magnitude;
        }
        else if (xOrz)
        {
            dist = endPos.x - startPos.x;
        }
        else
        {
            dist = endPos.z - startPos.z;
        }

        if (dist < 0) //ensuring distance is positive
        {
            dist *= -1;
        }

        midPoint.y = wallPrefab.transform.localScale.y / 2; //setting the y so the walls don't poke through the ground
        Debug.Log("Placed at: " + midPoint.ToString());
        GameObject newWall = Instantiate(wallPrefab, midPoint, Quaternion.identity); //creating the wall
        newWall.transform.localScale = new Vector3(dist,newWall.transform.localScale.y,newWall.transform.localScale.z); //setting its scale to match the distance

        //setting its rotation
        if (diagonal) //diagonal
        {
            newWall.transform.localEulerAngles = new Vector3(0,angleDiagonal,0);
        }
        else if (xOrz) //horizontal
        {
            newWall.transform.localEulerAngles = new Vector3(0,0,0);
        }
        else //vertical
        {
            newWall.transform.localEulerAngles = new Vector3(0,90,0);
        }

        //reset the flags
        placingWall = false;
        diagonal = false;
    }

    void drawGhost() //draws a ghost wall
    {
        if (startPos == endPos) //if the start and end are the same, there is no wall
        {
            if (!drawingGhost) //if there is no current ghost wall, stop trying to draw a new one
            {
                return;
            }
        }
        else
        {
            if (startPos.x == endPos.x) //if the line is along the x axis
            {
                xOrz = false;
                diagonal = false;
            }
            else if (startPos.z == endPos.z) //if the line is along the z axis
            {
                xOrz = true;
                diagonal = false;
            }
            else //if the line is diagonal
            {
                //finding the change in x and the change in z
                float xDiff = endPos.x - startPos.x;
                float zDiff = endPos.z - startPos.z;
                //making the differences positive
                if (xDiff < 0)
                {
                    xDiff *= -1;
                }
                if (zDiff < 0)
                {
                    zDiff *= -1;
                }

                if (xDiff == zDiff) //if the gradiant is 1, it is valid diagonal line
                {
                    diagonal = true;
                }
                else
                {
                    return;
                }
            }
        }

        drawingGhost = true; //start placing the ghost wall
        
        //finding the diagonal angle
        float angleDiagonal = 45;
        if (diagonal)
        {
            //checking if it needs to be rotated down instead of up

            float xDiff = endPos.x - startPos.x;
            float zDiff = endPos.z - startPos.z;

            if (zDiff * xDiff > 0)
            {
                angleDiagonal = -45; //updating the angle for this case
            }

        }

        //finding the midpoint of the line to use as its position
        Vector3 midPoint;
        midPoint = (startPos + endPos) / 2;
        
        //setting the distance var to calculate it's length
        float dist = 0;
        if (diagonal)
        {
            dist = (startPos - endPos).magnitude;
        }
        else if (xOrz)
        {
            dist = endPos.x - startPos.x;
        }
        else
        {
            dist = endPos.z - startPos.z;
        }

        if (dist < 0) //ensuring distance is positive
        {
            dist *= -1;
        }

        ghostWall.SetActive(true); //activating the ghost wall
        midPoint.y = ghostWall.transform.localScale.y / 2; //setting the y so the walls don't poke through the ground
        //setting its position and scale
        ghostWall.transform.position = midPoint;
        ghostWall.transform.localScale = new Vector3(dist,ghostWall.transform.localScale.y,ghostWall.transform.localScale.z);
        
        //setting its rotation
        if (diagonal) //diagonal
        {
            ghostWall.transform.localEulerAngles = new Vector3(0,angleDiagonal,0);
        }
        else if (xOrz) //horizontal
        {
            ghostWall.transform.localEulerAngles = new Vector3(0,0,0);
        }
        else //vertical
        {
            ghostWall.transform.localEulerAngles = new Vector3(0,90,0);
        }
    }

    void resetGhostWall() //reseting the ghost wall object
    {
        drawingGhost = false; //stop placing the ghost wall
        ghostWall.transform.localScale = new Vector3(1,1.5f,0.3f); //reset its scale
        ghostWall.SetActive(false); //deactivate it
    }
}
