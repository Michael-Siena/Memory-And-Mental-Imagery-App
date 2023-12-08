using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ColorSpaces;

public interface IRotatesHue
{
    Texture2D OriginalColorTexture { get; }
    Texture2D CurrentTexture { get; }
    byte[] RawOriginalPixelData { get; }
    Color32[] OriginalRGBAColors { get; }
    LABColor[] OriginalLABColors { get; }
}