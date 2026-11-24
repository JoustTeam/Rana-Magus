using UnityEngine;
using UnityEngine.InputSystem;
//Comment to get git to notice the files
public class WallTestBuilder : MonoBehaviour
{
    [SerializeField] private WallSegment wallPrefab; 
    [SerializeField] private WallManager manager;
    [SerializeField] private float roomSize = 4f;

    private void Start()
    {
        Vector3 a = new Vector3(0, 0, 0);
        Vector3 b = new Vector3(roomSize, 0, 0);
        Vector3 c = new Vector3(roomSize, 0, roomSize);
        Vector3 d = new Vector3(0, 0, roomSize);

        BuildWall(a, b);
        BuildWall(b, c);
        BuildWall(c, d);
        BuildWall(d, a);
    }

    private void BuildWall(Vector3 start, Vector3 end)
    {
        WallSegment wall = Instantiate(wallPrefab, transform);
        wall.Build(start, end);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            manager.CycleMode();
        }
    }
}