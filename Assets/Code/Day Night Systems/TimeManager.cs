using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


//this script controlls the main day/night cycle, advances in game time and updates the ui
public class TimeManager : MonoBehaviour
{
    [Header("Directional Lights")]
    [SerializeField] Light sun;
    [SerializeField] Light moon;

    [Header("Speed Of Light Change Between Lights")]
    [SerializeField] AnimationCurve lightIntensityCurve;

    [Header("Light Settings")]
    [SerializeField] float maxSunIntensity = 1f;
    [SerializeField] float maxMoonIntensity = 0.5f;

    [SerializeField] Color dayAmbientLight; 
    [SerializeField] Color nightAmbientLight;
    [SerializeField] Volume volume;

    [Header("Skybox Blend Material")]
    [SerializeField] Material skyboxMaterial;

    [Header("UI")]
    [SerializeField] RectTransform dial;
    [SerializeField] TextMeshProUGUI timeText;
    float initialDialRotation;

    ColorAdjustments colorAdjustments;

    [Header("Scriptable Object Profile")]
    [SerializeField] TimeSettings timeSettings;

    TimeService service;

    public float DefaultTime = 2000;


    void Start()
    {
        //initializes time service script
        service = new TimeService(timeSettings);

        float defaultSpeed = timeSettings.TimeMultiplier;

        //-90 because for some reason it would offset it byt that much when the game start
        initialDialRotation = dial.rotation.eulerAngles.z - 90f; 
    }

    void Update()
    {
        UpdateTimeOfDay();
        RotateSun();
        UpdateLightSettings();
        UpdateSkyBlend();
    }

    //updates the skybox between day and night using the suns rotation
    void UpdateSkyBlend()
    {
        float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.up);
        float blend = Mathf.Lerp(0, 1, lightIntensityCurve.Evaluate(dotProduct));
        skyboxMaterial.SetFloat("_Blend", blend);
    }

    //fades sun and moon intensity in opposite directions and tint as well as the scene
    void UpdateLightSettings()
    {
        float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.down);
        sun.intensity = Mathf.Lerp(0, maxSunIntensity, lightIntensityCurve.Evaluate(dotProduct));
        moon.intensity = Mathf.Lerp(maxMoonIntensity, 0, lightIntensityCurve.Evaluate(dotProduct));
        if (colorAdjustments == null) return;
        colorAdjustments.colorFilter.value = Color.Lerp(nightAmbientLight, dayAmbientLight, lightIntensityCurve.Evaluate(dotProduct));
    }

    void RotateSun()
    {
        float rotation = service.CalculateSunAngle();
        sun.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.right);

        dial.rotation = Quaternion.Euler(0, 0, rotation + initialDialRotation);
    }

    //updates in game clock using the time service
    void UpdateTimeOfDay()
    {
        service.UpdateTime(Time.deltaTime);
        if (timeText != null)
        {
            timeText.text = service.CurrentTime.ToString("ddd hh:mm tt");
        }
    }

    //time controlls for ui buttons
    public void SpeedUpTime()
    {
        ResetTime();
        timeSettings.TimeMultiplier *= 10;
    }

    public void SlowDownTime()
    {
        ResetTime();
        timeSettings.TimeMultiplier /= 10;
    }

    public void StopTime()
    {
        timeSettings.TimeMultiplier = 0;
    }

    //makes sure that the time is set to the default speed before changing it
    public void ResetTime()
    {
        timeSettings.TimeMultiplier = DefaultTime;
    }
}