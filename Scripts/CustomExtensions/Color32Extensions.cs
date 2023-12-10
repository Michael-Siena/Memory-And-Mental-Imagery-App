using UnityEngine;
using ColorSpaces;

namespace CustomExtensions
{
    public static class Color32Extensions
    {
        public static LABColor[] ToLABPixels32(this Color32[] colors)
        {
            LABColor[] LABcolors = new LABColor[colors.Length];
            for (uint i = 0; i < colors.Length; i++)
            {
                LABcolors[i] = LABColor.FromColor(colors[i]);
            }
            return LABcolors;
        }
    }
}
