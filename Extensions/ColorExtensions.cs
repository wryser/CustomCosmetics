using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CustomCosmetics.Extensions
{
    public static class ColorExtensions
    {
        public static Color parseColor(string sourceString)
        {
            if (sourceString == null || sourceString == "" || sourceString == "$")
            {
                return Color.black;
            }
            string outString;
            Color outColor;
            string[] splitString;

            // Trim extranious parenthesis
            outString = sourceString.Replace("(", string.Empty);
            outString = outString.Replace(")", string.Empty);
            outString = outString.Replace("RGBA", string.Empty);

            // Split delimted values into an array
            splitString = outString.Split(",");

            // Build new Vector3 from array elements
            float x;
            float y;
            float z;
            float.TryParse(splitString[0], out x);
            float.TryParse(splitString[1], out y);
            float.TryParse(splitString[2], out z);
            outColor.r = x;
            outColor.g = y;
            outColor.b = z;
            outColor.a = 1f;

            return outColor;
        }
    }
}
