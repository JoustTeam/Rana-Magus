using UnityEngine;

//contains general time data

[CreateAssetMenu(fileName = "TimeSettings", menuName = "TimeSettings")]
public class TimeSettings : ScriptableObject
{
    public float TimeMultiplier = 2000;
    public float startHour = 12;
    public float sunriseHour = 6;
    public float sunsetHour = 18;
}