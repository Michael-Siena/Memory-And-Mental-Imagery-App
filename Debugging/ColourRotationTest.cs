using UnityEngine.Profiling;
using UnityEngine;
using UnityEngine.UI;
using Coordinates;
using ColorSpaces;
using EditorTools;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Text;
using FirebaseWebGL;
using System.Collections.Generic;
using Debug = global::UnityEngine.Debug;

namespace Debugging
{
    public class ColourRotationTest : MonoBehaviour
    {
        public GameObject Target;
        public Slider Slider;
        public struct Data
        {
            public float Mean;
            public float StDev;
            public float[] Raw;
        }

        private Data data = new Data();
        private Texture2D originalColorTexture, currentTexture;
        private byte[] rawOriginalPixelData;
        private Color32[] originalRGBAColors;
        private LABColor[] originalLABColors;
        private float rotation = 0f;
        private const float MAX_DEGREES = 360f;
        private readonly ScreenshotTaker screenshotTaker = new ScreenshotTaker();
        private readonly Stopwatch sw = new Stopwatch();
        private int iHueRot = 0;
        private readonly float[] hueRots = new float[360];

        private void Awake()
        {
            Application.targetFrameRate = 30;

            currentTexture = Target.GetComponent<SpriteRenderer>().sprite.texture;

            // copy original texture and cache pixel colors in RGBA and LAB color spaces
            originalColorTexture = CopyTexture(currentTexture);
            rawOriginalPixelData = originalColorTexture.GetRawTextureData();

            originalRGBAColors = originalColorTexture.GetPixels32();
            originalLABColors = PixelColorsToLAB(originalRGBAColors);
        }

        private void Update()
        {
            // test here
            if (iHueRot < hueRots.Length)
            {
                sw.Start();

                // reload original texture
                ResetTexture();

                rotation++;

                RotatePixelColors(currentTexture, originalRGBAColors, originalLABColors, Mathf.Abs(rotation));
                screenshotTaker.TryCapture(iHueRot.ToString() + "XYZ.png");

                // update slider
                Slider.value = rotation * 0.00277777784503996372222900390625f;

                sw.Stop();
                hueRots[iHueRot] = sw.ElapsedMilliseconds;
                sw.Reset();
            }
            // send results to db
            else if (iHueRot == hueRots.Length)
            {
                data.Mean = Mean(hueRots);
                data.StDev = StDev(hueRots);
                data.Raw = hueRots;

                //SendToDB(data, Global.FirebasePath);

                Slider.interactable = true;
                Slider.value = 0f;

                if (Application.platform != RuntimePlatform.WebGLPlayer)
                {
                    //Application.Quit();
                }
            }
            else
            {
                // reload original texture
                ResetTexture();

                RotatePixelColors(currentTexture, originalRGBAColors, originalLABColors, rotation);
            }

            if (iHueRot <= hueRots.Length)
            {
                iHueRot++;
            }
        }

        private void OnApplicationQuit()
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                ResetTexture();

                WriteToLog();
            }
        }

        private void WriteToLog()
        {
            string timeFinished = System.DateTime.Now.ToString("dd-mm-yyyy hh-mm-ss-tt");

            string baseDir = Application.dataPath;
            string savePath = Path.GetFullPath(Path.Combine(baseDir, "..\\Logs"));

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            string toWrite = $"{timeFinished}\nPerformance for {hueRots.Length} hue rotations\n\nMean: {Mean(hueRots)}ms\nSD: {StDev(hueRots)}ms\n\nRaw data";
            for (int i = 0; i < hueRots.Length; i++)
            {
                toWrite += $"\n Rotation num {i}: {hueRots[i]}ms";
            }

            File.WriteAllText($"{savePath}\\log {timeFinished}.txt", toWrite);
            Debug.LogError("Log written to " + savePath);
        }

        private float Mean(float[] vals)
        {
            float sum = 0f;
            for (int i = 0; i < vals.Length; i++)
            {
                sum += vals[i];
            }

            return sum / vals.Length;
        }

        private float StDev(float[] vals)
        {
            float mean = Mean(vals);
            float sumOfSquaresOfDiffereces = vals.Select(val => (val - mean) * (val - mean)).Sum();

            return Mathf.Sqrt(sumOfSquaresOfDiffereces / vals.Length);
        }

        private LABColor[] PixelColorsToLAB(Color32[] colors)
        {
            LABColor[] LABcolors = new LABColor[colors.Length];
            for (uint i = 0; i < colors.Length; i++)
            {
                LABcolors[i] = LABColor.FromColor(colors[i]);
            }

            return LABcolors;
        }

        private void ResetTexture() => currentTexture.SetPixelData(rawOriginalPixelData, 0, 0);

        private Texture2D CopyTexture(Texture2D original)
        {
            var copy = new Texture2D(original.width, original.height, TextureFormat.RGBA32, false);
            Graphics.CopyTexture(original, copy); // doesn't put as much pressure on garbage collector as Get/SetPixels

            return copy;
        }

        private void UpdatePixelColors(Texture2D t, Color32[] newColors)
        {
            t.SetPixels32(0, 0, t.width, t.height, newColors);
            t.Apply();
        }

        public void OnSliderValueChange() => rotation = Slider.value * MAX_DEGREES;

        // For each opaque pixel, convert to grayscale RGBA using lightness value from LAB color space
        private void ConvertToGrayscale(Texture2D t, Color32[] RGBAColors, LABColor[] LABColors)
        {
            // convert to grayscale
            Color32[] grayscale = new Color32[LABColors.Length];
            for (int i = 0; i < LABColors.Length; i++)
            {
                if (LABColors[i].a != 0)
                {
                    LABColor temp = new LABColor(LABColors[i].l, 0f, 0f);//LABColors[i];
                    //temp.a = temp.b = 0f; // we only want L dimension of LAB color space so set A & B to 0

                    grayscale[i] = temp.ToColor();
                }
                grayscale[i].a = RGBAColors[i].a; // use original alpha
            }

            UpdatePixelColors(t, grayscale);
        }

        private PolarCoord GetPolarCoordinatesFromPixels(float h, float v)
        {
            // Get radius
            float centerHoriz = Screen.width / 2f; //use fullscreen width for comparison to MATLAB version
            float centerVert = Screen.height / 2f; //use fullscreen height for comparison to MATLAB version
            float distH = h - centerHoriz;
            float distV = v - centerVert;

            float radius = Mathf.Sqrt(distH * distH + distV * distV) + Mathf.Epsilon;

            // Get angle in degrees
            float degrees = Mathf.Acos(distH / radius) / Mathf.PI * 180f;

            // correct angle depending on quadrant
            if (distH == 0f & distV > 0f)
            {
                degrees = 90f;
            }
            else if (distH == 0 & distV < 0f)
            {
                degrees = 270f;
            }
            else if (distH > 0f & distV == 0f)
            {
                degrees = 0f;
            }
            else if (distH < 0f & distV == 0f)
            {
                degrees = 180f;
            }
            else if ((distH < 0f & distV < 0f) | (distH > 0f & distV < 0f))
            {
                degrees -= 360f;
                degrees = Mathf.Abs(degrees);
            }

            float radians = PolarCoord.DegreesToRadians(degrees);
            return new PolarCoord(radius, radians);
        }

        // Rotate color of every opaque pixel in texture
        private void RotatePixelColors(Texture2D t, Color32[] RGBAColors, LABColor[] LABColors, float angle)
        {
            // OPTIMISATIONS
            // =============
            // Theta calculation:
            //     originally calculated as 2f * Mathf.PI * (angle / 360f).
            // My approximation (accurate to 8 decimal places):
            // (1) Floating multiplication is faster than division, and division can be made into multiplication if the divisor is known ahead of time,
            //     so precompute the invariant multiplication of (1 / 360f)
            // (2) Divsion has been eliminated but multiplication is still costly.
            //     Since all remaining terms are multiplied and the order in which they are done so does not matter, we can multiply all literal values
            //     ahead of time to reduce the number of multiplcation operations (was: 2f * Mathf.PI * 0.00277777784503996372222900390625f)
            // (3) Multiplying the angle by our new precomputed constant value will give a very close approximation of theta!
            //
            // Rotation matrix:
            // Was originally a 2D array, which I flattened to a 1D aray as the .NET runtime has specialised instructions for working with the latter.
            // These do not involve expensive method calls as found in the inner loops of multidimensional arrays. 1D arrays also offer better locality
            // in memory and have less allocation / deallocation overhead
            //
            // LABColorspace conversion:
            // See LABColor struct for comments.
            float theta = angle * 0.01745329342f;
            var flatRotationMatrix = new float[] { Mathf.Cos(theta), -Mathf.Sin(theta), Mathf.Sin(theta), Mathf.Cos(theta) };

            Color32[] rotatedColors = new Color32[RGBAColors.Length];
            for (int i = 0; i < rotatedColors.Length; i++)
            {
                if (RGBAColors[i].a != 0f)
                {
                    LABColor rotatedLAB = new LABColor(LABColors[i].l, // use unrotated lightness value to hold percieved brightness constant
                                                       (flatRotationMatrix[0] * LABColors[i].a) + (flatRotationMatrix[1] * LABColors[i].b),
                                                       (flatRotationMatrix[2] * LABColors[i].a) + (flatRotationMatrix[3] * LABColors[i].b));
                    rotatedColors[i] = LABColor.ToColor(rotatedLAB);
                    rotatedColors[i].a = RGBAColors[i].a; // use original alpha 
                }
                else
                {
                    rotatedColors[i] = RGBAColors[i]; // we can just use the original pixels, since they are transparent!
                }
            }

            UpdatePixelColors(t, rotatedColors);
        }

        /*
        private void SendToDB(Data data, string firebasePath)
        {
            var sb = new StringBuilder($"Mean: {data.Mean}ms, StDev: {data.StDev}ms");
            for(int i = 0; i < data.Raw.Length; i++)
            {
                sb.Append($", Iter {i}: {data.Raw[i]}ms");
            }
            string formattedData = sb.ToString();

            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                FirebaseDatabase.PostJSON($"{firebasePath}/SID{Global.ID}/Performance", formattedData, this.gameObject.name, "", "");
                Debug.Log("Sent to DB: " + formattedData);
            }
            else
            {
                Debug.Log("Data was not sent to the database - invalid runtime platform.");
            }
        }
        */
    }
}