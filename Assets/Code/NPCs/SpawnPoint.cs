using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Unity.VisualScripting;
using System.Collections.Generic;

//Placeholder, replace with day/night cycle implementation
public enum TimeOfDay
{
    Day,
    Night
}
public class SpawnPoint : MonoBehaviour
{
    [Header("Set in engine for debug purposes")]
    public TimeOfDay timeOfDay;
    public GameObject NPCToSpawn;
    public int NumberToSpawn;
    public int maxNumberOfNPCS;

    [Header("Time in milliseconds")]
    public int minTimeBetweenSpawn;
    public int maxTimeBetweenSpawn;

    private bool _isWaiting = false;

    //private GameObject[] allSpawnedObjects;
    private List<GameObject> _allSpawnedObject = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        switch (timeOfDay)
        {
            case TimeOfDay.Day:
                minTimeBetweenSpawn = 10;
                maxTimeBetweenSpawn = 100;
                break;
            case TimeOfDay.Night:
                minTimeBetweenSpawn = 100;
                maxTimeBetweenSpawn = 300;
                break;
        }

        if (_allSpawnedObject.Count < maxNumberOfNPCS && !_isWaiting)
        {
            PauseSpawn(Random.Range(minTimeBetweenSpawn, maxTimeBetweenSpawn));
        }
        
    }

    public void PauseSpawn(int time)
    {
        _isWaiting = true;
        float totalPauseTime = 0f;
        while (totalPauseTime < time)
        {
            totalPauseTime += Time.deltaTime;
        }
        SpawnNPC();

    }
    //To be added later : Swapping NPC types depending on time of day
    public void SpawnNPC()
    {
        GameObject npc = Instantiate(NPCToSpawn, transform.position, Quaternion.identity);
        AddNPCToList(npc);
        _isWaiting = false;
    }

    public void AddNPCToList(GameObject npc)
    {
        _allSpawnedObject.Add(npc);
    }
}
