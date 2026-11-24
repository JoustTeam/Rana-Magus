using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    private static readonly List<WallSegment> walls = new List<WallSegment>();
    private static WallManager instance;

    private const float FacingThreshold = -0.1f;

    private const float YawChangeThreshold = 1f;

    [SerializeField] private WallMode currentMode = WallMode.Cutaway;

    private Transform cam;
    private float lastYaw;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        walls.Clear();
        instance = null;
    }

    public static void Register(WallSegment wall) => walls.Add(wall);
    public static void Unregister(WallSegment wall) => walls.Remove(wall);

    public static void Refresh(WallSegment wall)
    {
        if (instance != null) instance.Apply(wall);
    }

    private void Awake()
    {
        instance = this;
        cam = Camera.main.transform;
    }

    private void Start()
    {
        lastYaw = cam.eulerAngles.y;
        RefreshAll();
    }

    private void Update()
    {
        float yaw = cam.eulerAngles.y;
        if (Mathf.Abs(Mathf.DeltaAngle(yaw, lastYaw)) > YawChangeThreshold)
        {
            lastYaw = yaw;
            RefreshAll();
        }
    }

    public void SetMode(WallMode mode)
    {
        currentMode = mode;
        RefreshAll();
    }

    public void CycleMode()
    {
        SetMode((WallMode)(((int)currentMode + 1) % 3));
    }

    private void RefreshAll()
    {
        foreach (WallSegment wall in walls) Apply(wall);
    }

    private void Apply(WallSegment wall)
    {
        wall.SetState(StateFor(wall));
    }

    private WallState StateFor(WallSegment wall)
    {
        switch (currentMode)
        {
            case WallMode.WallsUp:   return WallState.Full;
            case WallMode.WallsDown: return WallState.Cutaway;

            case WallMode.Cutaway:
                Vector3 camForward = cam.forward;
                camForward.y = 0f;
                camForward.Normalize();

                bool blocksView = Vector3.Dot(wall.Normal, camForward) < FacingThreshold;
                return blocksView ? WallState.Cutaway : WallState.Full;

            default:
                return WallState.Full;
        }
    }
}