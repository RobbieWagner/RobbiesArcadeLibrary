using RobbieWagnerGames.Utilities;
using UnityEngine;

namespace RobbieWagnerGames.ArcadeLibrary.Common
{
    public class CameraMouseInteraction : MonoBehaviourSingleton<CameraMouseInteraction>
    {
        [SerializeField] private Camera renderCamera;
        public Vector3 mousePosition {get; set;}
        public float xScaleMultiplier;
        public float xOffset;

        private void Update()
        {
            Vector2 mousePos = Input.mousePosition;
            Rect cameraViewportRect = renderCamera.rect;
            Rect cameraPixelRect = new Rect(
                cameraViewportRect.x * Screen.width,
                cameraViewportRect.y * Screen.height,
                cameraViewportRect.width * Screen.width,
                cameraViewportRect.height * Screen.height
            );
            
            Vector2 normalizedPos = new Vector2(
                (mousePos.x - cameraPixelRect.x) / cameraPixelRect.width,
                (mousePos.y - cameraPixelRect.y) / cameraPixelRect.height
            );
            
            float adjustedX = (normalizedPos.x - 0.5f) / renderCamera.aspect * xScaleMultiplier + 0.5f + xOffset;
            
            Vector3 viewportPoint = new Vector3(
                adjustedX,
                normalizedPos.y,
                renderCamera.nearClipPlane
            );
            
            mousePosition = renderCamera.ViewportToWorldPoint(viewportPoint);
        }
    }
}