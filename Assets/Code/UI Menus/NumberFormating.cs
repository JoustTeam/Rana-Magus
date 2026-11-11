using System;
using System.Collections.Generic;
using System.Linq;

//uses prefixes to format number so they fit better on screen
//eg. 1.000 -> 1K  1.000.000 -> 1M etc.
public static class NumberFormating
{
    //set prefixes for the formating
    private static readonly Dictionary<int, string> units = new Dictionary<int, string>()
    {
        {0,""},
        {1,"K"},
        {2,"M"},
        {3,"B"},
        {4,"T"}
    };

    private static readonly int CharA = Convert.ToInt32('a');

    public static string Format(double value)
    {
        if (value < 1d)
        {
            return "0";
        }

        var n = (int)Math.Log(value, 1000);
        var m = value / Math.Pow(1000, n);
        var unit = "";

        if (n < units.Count)
        {
            unit = units[n];
        }
        else
        {
            var unitInt = n - units.Count;
            var secondUnit = unitInt % 26;
            var firstUnit = unitInt / 26;
            
            unit = Convert.ToChar(firstUnit + CharA).ToString() + Convert.ToChar(secondUnit + CharA).ToString();
        }
        return (Math.Floor(m * 100)/100).ToString("0.##") + unit;
    }
}