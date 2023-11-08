using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorSpaces;

namespace CustomExtensions
{
    public static class Texture2DExtensions
    {
        public static Texture2D CopyTexture(this Texture2D original)
        {
            var copy = new Texture2D(original.width, original.height, TextureFormat.RGBA32, false);
            Graphics.CopyTexture(original, copy); // doesn't put as much pressure on garbage collector as Get/SetPixels

            return copy;
        }

        public static void UpdateColorPixels32(this Texture2D t, Color32[] newCols)
        {
            t.SetPixels32(0, 0, t.width, t.height, newCols);
            t.Apply();
        }

        // convert to grayscale
        public static void ToGrayscalePixels32(this Texture2D t, Color32[] cols, LABColor[] labCols)
        {
            Color32[] grayscale = new Color32[labCols.Length];
            for (int i = 0; i < labCols.Length; i++)
            {
                LABColor temp = new LABColor(labCols[i].l, 0f, 0f); // we only want l dimension of LAB color space so set a & b to 0
                grayscale[i] = temp.ToColor();
                grayscale[i].a = cols[i].a; // use original alpha
            }

            t.UpdateColorPixels32(grayscale);
        }

        // Rotate color of every opaque pixel in texture
        // OPTIMISATIONS 
        // =============
        // Theta calculation:
        //     originally calculated as 2f * Mathf.PI * (angle / 360f).
        //
        // My approximation, uses constant folding (accurate to 8 decimal places):
        // (1) Floating multiplication is faster than division, and division can be made into multiplication if the divisor is known ahead of time,
        //     so precompute the invariant multiplication of (1 / 360f)
        // (2) Divsion has been eliminated but multiplication is still costly.
        //     Since all remaining terms are multiplied and the order in which they are done so does not matter, we can multiply all literal values
        //     ahead of time to reduce the number of multiplcation operations (was: 2f * Mathf.PI * 0.00277777784503996372222900390625f)
        // (3) Multiplying the angle by our new precomputed constant value will give a very close approximation of theta!
        //
        // Rotation matrix:
        // Was originally a 2D array, which I flattened to a 1D aray as the .NET runtime has specialised instructions for working with the latter.
        // These do not involve expensive method calls as found in the inner loops of multidimensional arrays. 1D arrays also offer better cache locality
        // and have less allocation / deallocation overhead
        //
        // LABColorspace conversion:
        // See LABColor struct for comments.
        public static void RotatePixelHues32(this Texture2D t, Color32[] cols, LABColor[] labCols, float angle)
        {
            float theta = angle * 0.01745329342f;
            float[] flatRotationMatrix = new float[] { Mathf.Cos(theta), -Mathf.Sin(theta), Mathf.Sin(theta), Mathf.Cos(theta) };
            Color32[] rotatedCols = new Color32[cols.Length];
            for (int i = 0; i < rotatedCols.Length; i++)
            {
                if (cols[i].a != 0f)
                {
                    LABColor rotatedLAB = new LABColor(
                        labCols[i].l, // use unrotated lightness value to hold percieved brightness constant
                        flatRotationMatrix[0] * labCols[i].a + flatRotationMatrix[1] * labCols[i].b,
                        flatRotationMatrix[2] * labCols[i].a + flatRotationMatrix[3] * labCols[i].b);
                    rotatedCols[i] = rotatedLAB.ToColor();
                    rotatedCols[i].a = cols[i].a; // use original alpha from Color32
                }
                else
                {
                    rotatedCols[i] = cols[i]; // we can just use the original pixels, since they are transparent!
                }
            }

            t.UpdateColorPixels32(rotatedCols);
        }
    }
}