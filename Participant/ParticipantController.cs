using UnityEngine;

namespace Participant
{
	public class ParticipantController : MonoBehaviour
	{
		public CharacterController controller;

		public GameObject firstPerson, thirdPerson;
		public GameObject participant, participantBody;

		private Vector2 currMouse;

		private readonly float maxSpeed = 25f, acceleration = .3f; // originally, maxSpeed = 50f; acceleration = 0.5f
		private float currSpeed;
		private readonly float rotationSpeed = 100f;
		private Vector3 moveDirection;
		private readonly float forwardDistance = 6.75f;

		public Camera firstPersonCamera, thirdPersonCamera;
		private float currFOV;
		public float zoomRate = 40f;

		private void OnEnable()
		{
			if (thirdPerson.activeSelf)
			{
				transform.rotation = Quaternion.identity; // Disable rotations if thirdperson
				thirdPersonCamera.fieldOfView = Global.TPCamMaxFOV; // reset fov to default (max)
			}
		}

		private void Update()
		{
			// movement
			Move();

			// looking
			if (firstPerson.activeSelf)
			{
				// camera rotation if mousex moves
				if ((currMouse.x = Input.GetAxisRaw("Mouse X")) != 0f)
				{
					Rotate();
				}
			}
			else if (thirdPerson.activeSelf)
			{
				if ((currMouse.y = Input.GetAxisRaw("Mouse Y")) != 0f)
				{
					Zoom();  // Zoom camera in third-person
				}
			}
		}

		private void Move()
		{
			// Get raw inputs
			moveDirection.x = Input.GetAxisRaw("Horizontal");
			moveDirection.z = Input.GetAxisRaw("Vertical");

			moveDirection = transform.TransformDirection(moveDirection); // Convert direction from local to global space, so that the rotation can be applied using Move()
			currSpeed = (currSpeed < maxSpeed) ? currSpeed + acceleration : maxSpeed; // Accelerate if currentSpeed is less than maxSpeed

			controller.Move(moveDirection * currSpeed * Time.deltaTime);

			participantBody.transform.localPosition = (firstPerson.activeSelf) ? Vector3.forward * forwardDistance : new Vector3(0f, 0.55f, 0f); //offset position
		}

		private void Rotate() => transform.Rotate(new Vector3(0f, currMouse.x * rotationSpeed * Time.deltaTime, 0f));

		private void Zoom()
		{
			currFOV = thirdPersonCamera.fieldOfView;
			currFOV -= currMouse.y * zoomRate * Time.deltaTime;
			currFOV = Mathf.Clamp(currFOV, Global.TPCamMinFOV, Global.TPCamMaxFOV);
			thirdPersonCamera.fieldOfView = currFOV;
		}
    }
}