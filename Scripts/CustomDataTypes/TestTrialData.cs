using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomDataTypes
{
    public sealed class TestTrialData
    {
        public string TargetName { get; set; }
        public float StudiedHueRotation { get; set; } // in degrees
        public float ShownHueRotation { get; set; } // random degree
        public Vector3 StudiedTargetPosition { get; set; }
        public MemoryTestOrderType MemoryTestOrder { get; set; }
        public PerspectiveType StudiedPerspective { get; set; }
        public PerspectiveType TestedPerspective { get; set; }
        public SwitchStatusType SwitchStatus { get; set; }
    }
}