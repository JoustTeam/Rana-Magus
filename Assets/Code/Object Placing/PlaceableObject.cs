using System;
using System.ComponentModel.Design.Serialization;
using TMPro;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI.Table;

//Modes that can be used to determine whether the object being placed is being placed on a wall or the floor
public enum PlaceMode
{
    Wall,
    Floor
}
public class PlaceableObject : MonoBehaviour
{
    [Header("Public")]
    //public variables
    public PlaceMode placeMode = PlaceMode.Floor;

    public Vector3 mousePos;
    //Set in Engine, once we know what the height is off the floor
    public Vector3 floorHeight;
    //Allowance for difference between floor height and the object being placed
    public float heightAllowance;

    public bool isMoving = false;
    public bool isClickedOn = false;

    //private variables
    [Header("Private")]
    [SerializeField] private GameObject ObjectBeingMoved;
    //private Transform MovingTransform;

    //Input Actions
    //Placeholder: Left Mouse Button
    private InputAction _mouseAction;
    //Placeholder: Q
    private InputAction _startMovingAction;
    //Placeholder: Right Mouse Button
    private InputAction _rotateAction;
    //Debug Only; Placeholder: Space
    private InputAction _printInfoAction;
    private InputAction _deleteAction;
    private InputAction _duplicateAction;
    private InputAction _cancelAction;

    [Header("DEBUG ONLY - Remove Later")]
    [SerializeField] private TextMeshProUGUI EditableText;

    void OnEnable()
    {
        EventsManager.Instance.inputEvents.onMouseDown += callCheckMousePosition;
        EventsManager.Instance.inputEvents.onRotatePressed += callRotateObject;
        EventsManager.Instance.inputEvents.onSelectObjectPressed += StartMoving;
        //EventsManager.Instance.inputEvents.onMouseDown += callCancelAction;
    }

    void OnDisable()
    {
        EventsManager.Instance.inputEvents.onMouseDown -= callCheckMousePosition;
        EventsManager.Instance.inputEvents.onRotatePressed -= callRotateObject;
        EventsManager.Instance.inputEvents.onSelectObjectPressed -= StartMoving;
        //EventsManager.Instance.inputEvents.onMouseDown -= callCancelAction;
    }

    void Start()
    {
        //Check if the mouse cursor is valid

        //_mouseAction = InputSystem.actions.FindAction("Select"); //mouse button -- go straight to CheckMousePosition()
        //_startMovingAction = InputSystem.actions.FindAction("StartMove"); //?? looks like a debug
        //_rotateAction = InputSystem.actions.FindAction("Rotate"); //right click (might change to R) -- one check then goes to RotateObject()
        //_printInfoAction = InputSystem.actions.FindAction("PrintInfo"); //?? never called
        //_deleteAction = InputSystem.actions.FindAction("Delete"); //?? never called
        //_duplicateAction = InputSystem.actions.FindAction("Duplicate"); //?? never called
        //_cancelAction = InputSystem.actions.FindAction("Cancel"); //?? - one check then goes to CancelObjectMovement()

        Debug.Log("Current layer: " + gameObject.layer);

        //EditableText.text = "Currently Selected : NONE";

    }

    // Update is called once per frame
    void Update()
    {
        if (isClickedOn)
        {
            updateMousePosition();
            moveObject();
        }
    }

    //Input Functions
    void callCheckMousePosition(InputEventContext iec)
    {
        if (iec == InputEventContext.OBJECTBUILDMODE)
        {
            
            if (isClickedOn)
            {
                CancelObjectMovement();
            }
            else if (isMoving)
            {
                CheckMousePosition();
            }
        }
        
    }

    void callRotateObject(InputEventContext iec)
    {
        if (iec == InputEventContext.OBJECTBUILDMODE)
        {
            if (isClickedOn)
            {
                RotateObject();
            }
        }
    }

    void StartMoving(InputEventContext iec)
    {
        if (iec == InputEventContext.OBJECTBUILDMODE)
        {
            isMoving = !isMoving;
            if (isMoving)
            {
                Debug.Log("Start moving objects");
            }
            else
            {
                Debug.Log("Stopped moving objects");
                
            }
        }
        
    }


    //Public Functions

    public void updateMousePosition()
    {
        //Should be updated as long as "Clicked On" is true
        //Get the currently moving Objects bounds from the collider
        Collider collider = GetComponent<Collider>();
        Vector3 extents = collider.bounds.extents;

        //NOTE : extents are the distance from the centre of the object to the end

        Vector3 allowance = new Vector3(0, heightAllowance, 0);
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out hitInfo))
        {
            
            string hitSurface = hitInfo.collider.tag.ToString();

            //Surface hitSurface = hitInfo.collider.GetComponent<Surface>();

            if (hitSurface == "Floor")
            {
                mousePos = hitInfo.point;
                mousePos = new Vector3(mousePos.x,mousePos.y + extents.y, mousePos.z);
            }

            else if (hitSurface == "Wall")
            {
                mousePos = hitInfo.point;
                //Gets the position of the wall to know which direction it is
                Vector3 wallDirection = hitInfo.collider.transform.position - transform.position;


                //Determining which extent should be added
                //If Z is positive
                if (wallDirection.z > 0)
                {
                    mousePos = new Vector3(mousePos.x, mousePos.y, mousePos.z - extents.z);
                }
                else if (wallDirection.z < 0)
                {
                    mousePos = new Vector3(mousePos.x, mousePos.y, mousePos.z + extents.z);
                }
                else if (wallDirection.x > 0)
                {
                    mousePos = new Vector3(mousePos.x - extents.x, mousePos.y, mousePos.z);
                }

                else if (wallDirection.x < 0)
                {
                    mousePos = new Vector3(mousePos.x + extents.x, mousePos.y, mousePos.z);
               
                }

                mousePos = new Vector3(mousePos.x + extents.x, mousePos.y, mousePos.z + extents.z);
            }

            //It breaks because it hits this break point over and over
            else
            {
                mousePos = hitInfo.point;
                mousePos = new Vector3(mousePos.x, mousePos.y + extents.y, mousePos.z);
            }

            //
            Vector3Int gridMousePos = GridFunctions.grid.WorldToCell(mousePos);
            mousePos = gridMousePos + new Vector3(0f, extents.y, 0f);

            Vector3 floorHeightUp = hitInfo.point + allowance;
           
        }
        Debug.Log(mousePos);
    }

    public void CheckMousePosition()
    {

        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out hitInfo))
        {
            if (hitInfo.collider == GetComponent<Collider>())
            {
                isClickedOn = !isClickedOn;
                if (isClickedOn)
                {

                    gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                    Debug.Log("Current layer: " + gameObject.layer);
                    //EditableText.text = "Currently Selected : " + name;
                }
                else
                {
                    gameObject.layer = LayerMask.NameToLayer("Default");
                    Debug.Log("Current layer: " + gameObject.layer);

                }
            }
        }
    }


    /// <summary>
    /// A function to Adjust the position passed in by the extents of the object
    /// </summary>
    /// <param name="beforePos"></param>
    /// <param name="extents"></param>
    /// <returns>Adjusted Position</returns>
    public Vector3 PositionAdjustedForBounds(Vector3 beforePos, Vector3 extents)
    {

        //Temporary - needs to be adjusted to work with bounds depending on the direction of the object to the wall
        //Should work for floors already
        Vector3 adjusted = Vector3.zero;
        switch (placeMode)
        {
            case PlaceMode.Floor:
                adjusted = beforePos + new Vector3(0, extents.y, 0);
                break;
            case PlaceMode.Wall:
                adjusted = beforePos + extents;
                break;
        }
        return adjusted;
    }

    public void moveObject()
    {
        transform.position = mousePos;
    }
    //Private Functions

    //Needs to be added later
    private void CheckCollision()
    {
        //if the hit object is of the same type, trigger no placement highlight
        //if it is a wall, change mode

    }

    //Rotates Object
    private void RotateObject()
    {
        var rotation = transform.localRotation.eulerAngles;
        if (placeMode == PlaceMode.Floor)
        {
            rotation.Set(rotation.x, rotation.y + 45f, rotation.z);
        }
        else
        {
            rotation.Set(0f,0f,0f);
        }
        transform.localRotation = Quaternion.Euler(rotation);
    }

    //Cancels object movement on right click
    private void CancelObjectMovement()
    {
        isClickedOn = false;

        gameObject.layer = LayerMask.NameToLayer("Default");
        Debug.Log("Current layer: " + gameObject.layer);
        //EditableText.text = "Currently Selected : NONE";

    }

    [Tooltip("Success of the most recent move")]
    private bool MoveSuccessful()
    {
        //Can't currently succeed
        return false;
    }

}
