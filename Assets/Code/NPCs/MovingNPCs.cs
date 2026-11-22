using System.Linq;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MovingNPCs : MonoBehaviour
{

    private NavMeshAgent agent;
    //public MonoBehaviour locationMarker; 
    public GameObject[] allPossibleDestinations;
    public GameObject debugDestination;

    private bool isAtLocation = false;



    //Mostly used for debug to seperate NPCs
    [Header("Leave blank for no names")]
    public string[] NPCNames;

    public TextMeshProUGUI nameTag;

    //Constructor for SpawnPoint to use to create NPCs
    public MovingNPCs(float moveSpeed)
    {
        _moveSpeed = moveSpeed;
    }

    [SerializeField] private float _moveSpeed;
    private Vector3 _targetLocation;


    //Need something to determine time of day

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        agent = GetComponent<NavMeshAgent>();

        //Picks a random name from the list if it is not empty and it can find the text

        if (nameTag != null)
        {
            if (NPCNames.Length == 0)
            {
                nameTag.text = NPCNames[Random.Range(0, NPCNames.Length)];
            }
            else
            {
                nameTag.text = "";
            }
        }
        

        //Temporary for testing
        agent.destination = _targetLocation;

    }

    // Update is called once per frame
    void Update()
    {
        //checkStatus();

        
        if (agent.velocity == Vector3.zero && !isAtLocation)
        {
            Debug.Log("NPC Paused");
            PauseMovement(50);
        }
    }

    public void CheckStatus()
    {
        //This can be used to pause it later

    }

    private void UpdateLocation()
    {
        Debug.Log(name + " moving to " + _targetLocation);
        agent.destination = _targetLocation;
    }

    //Looks for all objects with the tag "NPCDestination" and adds them to a list of possible destinations
    private void CalculateDestination()
    {

        isAtLocation = false;

        allPossibleDestinations = GameObject.FindGameObjectsWithTag("NPCDestination");

        int i = Random.Range(0, allPossibleDestinations.Length);
        _targetLocation = allPossibleDestinations[i].transform.position;
    }


    /// <summary>
    /// Pauses movement for the time input, then continues by calling to update the destination
    /// Time is in milliseconds
    /// </summary>
    /// <param name="time"></param>
    public void PauseMovement(int time)
    {
        isAtLocation = true;
        float totalPauseTime = 0f;
        while (totalPauseTime < time)
        {
            totalPauseTime += Time.deltaTime;
            Debug.Log(totalPauseTime);
        }

        Debug.Log("NPC moving");
        CalculateDestination();
        //moveToMouse();
        UpdateLocation();
    }

    //Debug Only - Do not use
    public void MoveToMouse()
    {
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out hitInfo))
        {
            _targetLocation = hitInfo.point;
        }
    }
}
