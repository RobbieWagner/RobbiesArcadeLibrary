using UnityEngine;

namespace RobbieWagnerGames.ArcadeLibrary.UnEuclid
{
	public class PortalTextureSetup : MonoBehaviour {

		[SerializeField] private Camera cam;
		[SerializeField] private Material portalMat;

		private void Awake () 
		{
			if (cam.targetTexture != null)
				cam.targetTexture.Release();
			cam.targetTexture = new RenderTexture(160, 144, 24);
			portalMat.mainTexture = cam.targetTexture;
		}
		
	}
}