using UnityEngine;
using System;

namespace Rendering
{
	public class Billboard : MonoBehaviour
	{
		public GameObject firstPersonCam, thirdPersonCam;

		// We hide these as these are only for the current instance of this class
		private GameObject spriteObj;
		private string spriteType;
		private SpriteRenderer spriteRenderer;

		private void Awake()
		{
			// cache this stuff
			spriteObj = this.gameObject;
			spriteType = this.spriteObj.name;
			spriteRenderer = this.GetComponent<SpriteRenderer>();
		}

        private void LateUpdate()
		{
			if (firstPersonCam.activeSelf)
			{
				// Reposition external landmark further away from arena boundary to avoid clipping issues
				switch (spriteType)
				{
					case "Target":
						spriteObj.transform.position = new Vector3(spriteObj.transform.position.x, Global.ObjectHeight, spriteObj.transform.position.z);
						break;
					case "North":
						spriteObj.transform.position = new Vector3((Global.WallDistanceFromOrigin + Global.DistanceFromWall), Global.TPPOrientationCueHeight, 0f);
						break;
					case "South":
						spriteObj.transform.position = new Vector3((Global.WallDistanceFromOrigin + Global.DistanceFromWall) * -1f, Global.TPPOrientationCueHeight, 0f);
						break;
					case "East":
						spriteObj.transform.position = new Vector3(0f, Global.TPPOrientationCueHeight, (Global.WallDistanceFromOrigin + Global.DistanceFromWall) * -1f);
						break;
					case "West":
						spriteObj.transform.position = new Vector3(0f, Global.TPPOrientationCueHeight, (Global.WallDistanceFromOrigin + Global.DistanceFromWall));
						break;
				}

				// Occlude sprites within arena based on depth in the z-buffer
				spriteRenderer.sortingOrder = (spriteType == "Target")
					? Mathf.RoundToInt(spriteObj.transform.position.y * 100f) * -1
					: -490;

				// Rotate all sprites to face camera at all times (i.e., billboard!)
				spriteObj.transform.LookAt(new Vector3(firstPersonCam.transform.position.x, spriteObj.transform.position.y, firstPersonCam.transform.position.z));
			}
			else if (thirdPersonCam.activeSelf)
			{
				// Face camera without rotations in the correct orientations
				switch (spriteType)
				{
					// Targets
					case "Target":
						spriteObj.transform.eulerAngles = new Vector3(90f, Global.ObjectHeight, 0f);
						break;
					// External Landmarks
					case "North":
						spriteObj.transform.eulerAngles = new Vector3(90f, 90f, 0f);
						spriteObj.transform.position = new Vector3(Global.WallDistanceFromOrigin, 1f, 0f);
						break;
					case "South":
						spriteObj.transform.eulerAngles = new Vector3(-90f, 90f, 0f);
						spriteObj.transform.position = new Vector3(Global.WallDistanceFromOrigin * -1f, 1f, 0f);
						break;
					case "East":
						spriteObj.transform.eulerAngles = new Vector3(90f, -90f, 90f);
						spriteObj.transform.position = new Vector3(0f, 1f, Global.WallDistanceFromOrigin * -1f);
						break;
					case "West":
						spriteObj.transform.eulerAngles = new Vector3(90f, 90f, 90f);
						spriteObj.transform.position = new Vector3(0f, 1f, Global.WallDistanceFromOrigin);
						break;
				}
			}

			spriteRenderer.flipX = (firstPersonCam.activeSelf);
		}
	}
}