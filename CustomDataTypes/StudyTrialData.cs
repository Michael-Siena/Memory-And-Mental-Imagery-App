using UnityEngine;

namespace CustomDataTypes
{
    public sealed class StudyTrialData
    {
        public string TargetName { get; set; }
        public float TargetHueRotation { get; set; } // in degrees
        public Vector3 TargetPosition { get; set; }
        public PerspectiveType Perspective { get; set; }
    }
}