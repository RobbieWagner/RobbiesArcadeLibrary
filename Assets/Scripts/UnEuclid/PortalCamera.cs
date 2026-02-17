using UnityEngine;

namespace RobbieWagnerGames.ArcadeLibrary.UnEuclid
{
	public class PortalCamera : MonoBehaviour 
	{
		public Transform playerCamera;
		public Transform portal;
		public Transform otherPortal;
		
		private void LateUpdate () 
		{
			// Calculate the relative position and rotation of the player camera to the other portal
			Vector3 playerOffsetFromOtherPortal = otherPortal.InverseTransformPoint(playerCamera.position);

			// Mirror the position through the portal
			Vector3 newCameraPosition = portal.TransformPoint(playerOffsetFromOtherPortal);
			transform.position = newCameraPosition;

			// Calculate the relative rotation
			Quaternion relativeRotation = Quaternion.Inverse(otherPortal.rotation) * playerCamera.rotation;
			Quaternion newCameraRotation = portal.rotation * relativeRotation;
			transform.rotation = newCameraRotation;
		}
	}
}