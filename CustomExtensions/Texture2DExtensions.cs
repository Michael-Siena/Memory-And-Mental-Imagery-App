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
