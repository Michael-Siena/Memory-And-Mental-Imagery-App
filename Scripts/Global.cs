using System;
using System.Collections;
using System.Collections.Generic;
using Questionnaires;
using CustomDataTypes;

public static class Global
{
    public struct SessionInformation
    {
        public int SID;
        public TaskOrderType TaskOrder;
        public QuestionnaireOrderType QuestionnaireOrder;
        public DateTime StartTime;
        public DateTime FinishTime;
        public int Block;
        public bool IsPractice;
    }
    public static SessionInformation SessionInfo;
    public static int NBlocks { get => 10; }
    public static float TPCameraHeight { get => 55f; }
    public static float TPCamMinFOV { get => 10f; } // originally 40f
    public static float TPCamMaxFOV { get => 75f; }
    public static float WallDistanceFromOrigin { get => 25f; }
    public static float DistanceFromWall { get => 7.5f; }
    public static float WallHeight { get => 4f; }
    public static float ObjectHeight { get => 2.1f; } // 1.75
    public static float TPPOrientationCueHeight { get => WallHeight; }
    public static string FirebasePath = "MainTask";
    public static string InstructionsPath { get => "Images/Instructions"; }
    public static string DataPath { get => "Data/SubjectDataFiles"; }
    public static string SpritePath { get => "Sprites/Targets"; }
}