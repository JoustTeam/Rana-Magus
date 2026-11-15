using UnityEngine;

public enum SurfaceType
{
    Floor,
    Wall,
    SelectType

}
public class Surface : MonoBehaviour
{
    public SurfaceType type = SurfaceType.SelectType;
}
