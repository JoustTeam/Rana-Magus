using UnityEngine;
//Comment to get git to notice the files
[CreateAssetMenu(menuName = "Walls/Wall Style")]
public class WallStyle : ScriptableObject
{
    [Header("Prefabs (must share the same footprint and pivot)")]
    public WallVariantView fullPrefab;
    public WallVariantView cutawayPrefab;

    [Header("Sizing")]
    [Tooltip("Length of each end cap mesh. Caps are never stretched.")]
    public float capLength = 0.1f;

    [Tooltip("Walls snap to this grid so lengths stay predictable.")]
    public float gridSize = 0.5f;

    [Tooltip("Keeps texture density constant however long the wall is.")]
    public float textureTilesPerMeter = 1f;

    public float MinLength => capLength * 2f;

    public float Snap(float value)
    {
        return Mathf.Round(value / gridSize) * gridSize;
    }
}