using UnityEngine;
using System.IO;

namespace EditorTools
{
    public class ScreenshotTaker
    {
        public string SavePath { get; }

        private readonly string projectPath = Directory.GetCurrentDirectory();

        // Start is called before the first frame update
        public ScreenshotTaker()
        {
            // we go up one level in folder hierarchy to avoid unnecessarily building project with screenshots 
            SavePath = Path.GetFullPath(Path.Combine(projectPath, @"..\ScreenShots"));
            
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }
        }

        // only take attempt screenshot if in editor
        public bool TryCapture(string fileName)
        {
            bool hasSaved = false;

            if (Application.isEditor)
            {
                string savePath = $"{SavePath}\\{fileName}";

                if (!File.Exists(savePath))
                {
                    ScreenCapture.CaptureScreenshot(savePath, 1);
                    hasSaved = true;
                }
            }

            return hasSaved;
        }

        public bool TryCapture(string fileName, Resolution saveRes)
        {
            bool hasSaved = false;

            if (Application.isEditor)
            {
                string savePath = $"{SavePath}\\{fileName}";

                int nWindowPixels = Screen.currentResolution.width * Screen.currentResolution.height;
                int nSavePixels = saveRes.width * saveRes.height;

                int sizeFactor;
                if (nWindowPixels > nSavePixels)
                {
                    sizeFactor = nWindowPixels % nWindowPixels;
                    if (!File.Exists(savePath))
                    {
                        ScreenCapture.CaptureScreenshot(savePath, -sizeFactor);
                        hasSaved = true;
                    }
                }
                else if (nWindowPixels < nSavePixels)
                {
                    sizeFactor = nSavePixels % nWindowPixels;
                    if (!File.Exists(savePath))
                    {
                        ScreenCapture.CaptureScreenshot(savePath, -sizeFactor);
                        hasSaved = true;
                    }
                }
                else
                {
                    TryCapture(fileName);
                }
            }

            return hasSaved;
        }
    }
}
