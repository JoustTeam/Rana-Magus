using UnityEngine;

public static class GridFunctions //stores the grid layout reference for other scripts to call on without a reference
{
    public static GridLayout grid;

    static GridFunctions() //finds the grid upon initialisation (assuming there is only 1 grid object)
    {
        grid = GameObject.FindFirstObjectByType<GridLayout>();
    }
}
