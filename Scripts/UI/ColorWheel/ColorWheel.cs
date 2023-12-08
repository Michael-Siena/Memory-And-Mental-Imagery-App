using UnityEngine;
using Coordinates;
using Debug = global::UnityEngine.Debug;

namespace UI.ColorWheel
{
    public class ColorWheel : MonoBehaviour
    {
        public GameObject Slider
        {
            get => _slider;
            set => _slider = value;
        }
        [SerializeField]
        private GameObject _slider;

        private readonly float acceleration = 1.04f, minSpeed = 10f, maxSpeed = 180f;
        private float rotationSpeed;

        private void Awake()
        {
            rotationSpeed = minSpeed;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.A))
            {
                rotationSpeed *= (rotationSpeed <= maxSpeed) ? acceleration : 1f;
                Slider.transform.RotateAround(Vector3.zero, Vector3.forward, rotationSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                rotationSpeed *= (rotationSpeed <= maxSpeed) ? acceleration : 1f;
                Slider.transform.RotateAround(Vector3.zero, Vector3.back, rotationSpeed * Time.deltaTime);
            }
            else
            {
                rotationSpeed = 10f;
            }
        }
    }
}
