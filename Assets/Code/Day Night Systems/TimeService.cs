using System;
using UnityEngine;

//this whole script is for getting the windows time service
public class TimeService
{
    readonly TimeSettings settings;
    DateTime currentTime;
    readonly TimeSpan sunriseTime;
    readonly TimeSpan sunsetTime;

    readonly Observable<bool> isDayTime;
    readonly Observable<int> currentHour;

    public event Action OnSunrise = delegate { };
    public event Action OnSunset = delegate { };
    public event Action OnHourChange = delegate { };

    //creates the service using the given settings.
    //by default the clock start the systems date at 12pm
    //defaults can be changed depending in the designers
    public TimeService(TimeSettings settings)
    {
        this.settings = settings;
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(settings.startHour);
        sunriseTime = TimeSpan.FromHours(settings.sunriseHour);
        sunsetTime = TimeSpan.FromHours(settings.sunsetHour);

        isDayTime = new Observable<bool>(IsDayTime());
        currentHour = new Observable<int>(currentTime.Hour);

        isDayTime.ValueChanged += day => (day ? OnSunrise : OnSunset)?.Invoke();
        currentHour.ValueChanged += _ => OnHourChange?.Invoke();
    }

    //advances time by the normal clock time in the machine multiplied by TimeMultiplier
    //also checks whether is day or night
    public void UpdateTime(float deltaTime)
    {
        currentTime = currentTime.AddSeconds(deltaTime * settings.TimeMultiplier);
        isDayTime.Value = IsDayTime();
        currentHour.Value = currentTime.Hour;
    }

    //returns the suns rotation angle in degrees. 0-180 for day 180-360 for night
    public float CalculateSunAngle()
    {
        bool isDay = IsDayTime();
        float startDegree = isDay ? 0 : 180;
        TimeSpan start = isDay ? sunriseTime : sunsetTime;
        TimeSpan end = isDay ? sunsetTime : sunriseTime;

        TimeSpan totalTime = CalculateDifference(start, end);
        TimeSpan elapsedTime = CalculateDifference(start, currentTime.TimeOfDay);

        double percentage = elapsedTime.TotalMinutes / totalTime.TotalMinutes;

        return Mathf.Lerp(startDegree, startDegree + 180, (float) percentage);
    }

    //current in game day and time
    public DateTime CurrentTime => currentTime;

    //the actual check for day/night time
    bool IsDayTime() => currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime;

    //formats from military time to digital
    TimeSpan CalculateDifference(TimeSpan from, TimeSpan to)
    {
        TimeSpan difference = to - from;
        return difference.TotalHours < 0 ? difference + TimeSpan.FromHours(24) : difference;
    }
}