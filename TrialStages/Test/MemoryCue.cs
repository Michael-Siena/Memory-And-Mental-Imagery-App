using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorSpaces;
using CustomExtensions;
using CustomDataTypes;
using TaskPhases.Test;
using FirebaseWebGL;

namespace TrialStages.Test
{
    public sealed class MemoryCue : TrialStage
    {
        public GameObject Panel
        {
            get => _panel;
            private set => _panel = value;
        }
        [SerializeField]
        private GameObject _panel;
        // Sprite/texture stuff
        private string spritePath;
        public GameObject TargetSprite
        {
            get => _targetSprite;
            private set => _targetSprite = value;
        }
        [SerializeField]
        private GameObject _targetSprite;
        private Texture2D originalColorTexture, currentTexture;
        private byte[] rawOriginalPixelData;
        private Color32[] originalRGBAColors;
        private LABColor[] originalLABColors;

        private TestTrialData trialData;

        private void Awake() => Panel = this.gameObject;
        
        protected override void OnEnable()
        {
            base.OnEnable();

            Panel.SetActive(true);
            TargetSprite.SetActive(true);

            trialData = TestPhase.BlockTrialData[TestPhase.TrialIdx];

            spritePath = (Global.SessionInfo.IsPractice) 
                ? $"{Global.SpritePath}/Practice" 
                : $"{Global.SpritePath}/MainTask";

            _targetSprite.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"{spritePath}/{trialData.TargetName}");

            // copy original texture and cache pixel colors in RGBA and LAB color spaces
            currentTexture = TargetSprite.GetComponent<SpriteRenderer>().sprite.texture;

            originalColorTexture = currentTexture.CopyTexture();
            rawOriginalPixelData = originalColorTexture.GetRawTextureData();

            originalRGBAColors = originalColorTexture.GetPixels32();
            originalLABColors = originalRGBAColors.ToLABPixels32();

            // Set to grayscale
            currentTexture.SetPixelData(rawOriginalPixelData, 0, 0);
            currentTexture.ToGrayscalePixels32(originalRGBAColors, originalLABColors);

            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                PostTrialData();
            }
        }

        protected override void Update() => base.Update();

        protected override void OnDisable()
        {
            base.OnDisable();

            Panel.SetActive(false);
            TargetSprite.SetActive(false);

            currentTexture.SetPixelData(rawOriginalPixelData, 0, 0);
        }

        private void PostTrialData()
        {
            string fullPath = (Global.SessionInfo.IsPractice)
                ? $"{Global.FirebasePath}/SID{Global.SessionInfo.SID}/Practice/TestPhase/Trial{TestPhase.TrialIdx}/TrialData"
                : $"{Global.FirebasePath}/SID{Global.SessionInfo.SID}/Block{Global.SessionInfo.Block}/TestPhase/Trial{TestPhase.TrialIdx}/TrialData";
            string data = $"{trialData.TargetName}, " +
                $"{trialData.StudiedHueRotation}, " +
                $"{trialData.ShownHueRotation}, " +
                $"{trialData.StudiedTargetPosition},  " +
                $"{(int)trialData.MemoryTestOrder}, " +
                $"{(int)trialData.StudiedPerspective}, " +
                $"{(int)trialData.TestedPerspective}, " +
                $"{(int)trialData.SwitchStatus}, " +
                $"{System.DateTime.Now}";
            FirebaseDatabase.PostJSON(fullPath, data, this.gameObject.name, "", "");
        }
    }
}