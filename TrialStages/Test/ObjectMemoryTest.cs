// unity namespaces
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Rand = UnityEngine.Random;
// custom namespaces
using ColorSpaces;
using FirebaseWebGL;
using CustomExtensions;
using CustomDataTypes;
using TaskPhases.Test;

namespace TrialStages.Test
{ 
    public sealed class ObjectMemoryTest : TrialStage, ICollectsData<float>, IRotatesHue
    {
        public GameObject Panel
        {
            get => _panel;
            private set => _panel = value;
        }
        [SerializeField]
        private GameObject _panel;

        public GameObject TargetSprite
        {
            get => _targetSprite;
            private set => _targetSprite = value;
        }
        [SerializeField]
        private GameObject _targetSprite;

        public Texture2D OriginalColorTexture { get; private set; }
        public Texture2D CurrentTexture { get; private set; }
        public byte[] RawOriginalPixelData { get; private set; }
        public Color32[] OriginalRGBAColors { get; private set; }
        public LABColor[] OriginalLABColors { get; private set; }

        // Color wheel stuff
        private GameObject slider;
        private readonly float acceleration = 1.04f, minSpeed = 10f, maxSpeed = 180f;
        private float rotationSpeed;
        private float rotation;

        private TestTrialData trialData;

        private void Awake() => slider = Panel.transform.Find("Slider").gameObject;

        protected override void OnEnable()
        {
            trialData = TestPhase.BlockTrialData[TestPhase.TrialIdx];

            Panel.SetActive(true);
            TargetSprite.SetActive(true);

            // Cache original texture data in Color32 and LAB colorspaces
            CurrentTexture = TargetSprite.GetComponent<SpriteRenderer>().sprite.texture;
            OriginalColorTexture = CurrentTexture.CopyTexture();
            RawOriginalPixelData = OriginalColorTexture.GetRawTextureData();
            OriginalRGBAColors = OriginalColorTexture.GetPixels32();
            OriginalLABColors = OriginalRGBAColors.ToLABPixels32();
            
            // Reset initial rotation to 0 degrees, then randomly rotate target texture hue
            slider.transform.SetPositionAndRotation(new Vector3(0f, 46f, 0f), Quaternion.Euler(Vector3.zero));
            rotation = trialData.ShownHueRotation;
            slider.transform.RotateAround(Vector3.zero, Vector3.forward, rotation);

            CurrentTexture.SetPixelData<byte>(RawOriginalPixelData, 0, 0);
            CurrentTexture.RotatePixelHues32(OriginalRGBAColors, OriginalLABColors, rotation);

            rotationSpeed = minSpeed;
            
            base.OnEnable();
        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetKey(KeyCode.A))
            {
                // Update rotation and slider
                rotationSpeed *= (rotationSpeed <= maxSpeed) ? acceleration : 1f;
                slider.transform.RotateAround(Vector3.zero, Vector3.forward, rotationSpeed * Time.deltaTime);

                rotation = slider.transform.rotation.eulerAngles.z;

                // Rotate target texture hue
                CurrentTexture.SetPixelData<byte>(RawOriginalPixelData, 0, 0);
                CurrentTexture.RotatePixelHues32(OriginalRGBAColors, OriginalLABColors, rotation);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                // Update rotation and slider
                rotationSpeed *= (rotationSpeed <= maxSpeed) ? acceleration : 1f;
                slider.transform.RotateAround(Vector3.zero, Vector3.back, rotationSpeed * Time.deltaTime);

                rotation = slider.transform.rotation.eulerAngles.z;

                // Rotate target texture hue
                CurrentTexture.SetPixelData<byte>(RawOriginalPixelData, 0, 0);
                CurrentTexture.RotatePixelHues32(OriginalRGBAColors, OriginalLABColors, rotation);
            }
            else if (Input.GetKeyUp(KeyCode.Space) || TimeLeft <= 0f)
            {
                var HueRotationData = new ResponseData<float>(rotation, Duration - TimeLeft);
                Debug.Log($"{TestPhase.TrialIdx}, {this.gameObject.name}: {HueRotationData.ToString()}");

                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    CollectData(HueRotationData);
                }

                this.gameObject.SetActive(false);
            }
            else
            {
                rotationSpeed = minSpeed;
            }
        }

        protected override void OnDisable()
        {
            Input.ResetInputAxes();

            base.OnDisable();
            
            Panel.SetActive(false);
            TargetSprite.SetActive(false);
        }

        public void CollectData(ResponseData<float> responseData)
        {
            string fullPath = (Global.SessionInfo.IsPractice)
                 ? $"{Global.FirebasePath}/SID{Global.SessionInfo.SID}/Practice/TestPhase/Trial{TestPhase.TrialIdx}/ObjectMemoryPerformance"
                 : $"{Global.FirebasePath}/SID{Global.SessionInfo.SID}/Block{Global.SessionInfo.Block}/TestPhase/Trial{TestPhase.TrialIdx}/ObjectMemoryPerformance";
            FirebaseDatabase.PostJSON(fullPath, responseData.ToString(), this.gameObject.name, "", "");
        }
    }
}