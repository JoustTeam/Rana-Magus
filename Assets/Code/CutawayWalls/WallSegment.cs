using UnityEngine;

public enum WallMode { WallsUp, Cutaway, WallsDown }
public enum WallState { Full, Cutaway, Hidden }

public class WallSegment : MonoBehaviour
{
    [SerializeField] private WallStyle style;

    private WallVariantView full;
    private WallVariantView cutaway;
    private WallState currentState = WallState.Full;

    public Vector3 Normal => transform.right;

    private void OnEnable()  => WallManager.Register(this);
    private void OnDisable() => WallManager.Unregister(this);

    public void Build(Vector3 start, Vector3 end)
    {
        Vector3 a = SnapToGrid(start);
        Vector3 b = SnapToGrid(end);

        Vector3 direction = b - a;
        direction.y = 0f;
        float length = direction.magnitude;

        if (length < style.MinLength) return;

        transform.SetPositionAndRotation(a, Quaternion.LookRotation(direction.normalized, Vector3.up));

        ClearOldViews();
        full = CreateView(style.fullPrefab, length);
        cutaway = CreateView(style.cutawayPrefab, length);
        WallManager.Refresh(this); 
    }

    public void SetState(WallState state)
    {
        currentState = state;
        if (full != null)    full.gameObject.SetActive(state == WallState.Full);
        if (cutaway != null) cutaway.gameObject.SetActive(state == WallState.Cutaway);
    }

    private WallVariantView CreateView(WallVariantView prefab, float length)
    {
        WallVariantView view = Instantiate(prefab, transform);
        view.transform.localPosition = Vector3.zero;
        view.transform.localRotation = Quaternion.identity;
        view.Resize(length, style.capLength, style.textureTilesPerMeter);
        return view;
    }

    private void ClearOldViews()
    {
        if (full != null) Destroy(full.gameObject);
        if (cutaway != null) Destroy(cutaway.gameObject);
    }

    private Vector3 SnapToGrid(Vector3 p)
    {
        return new Vector3(style.Snap(p.x), p.y, style.Snap(p.z));
    }
}