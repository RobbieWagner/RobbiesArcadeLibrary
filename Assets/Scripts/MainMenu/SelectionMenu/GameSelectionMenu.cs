using System.Collections.Generic;
using RobbieWagnerGames.ArcadeLibrary.Managers;
using RobbieWagnerGames.Managers;
using RobbieWagnerGames.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RobbieWagnerGames.ArcadeLibrary
{
    public class GameSelectionMenu : Menu
    {
        [Header("Game Selection")]
        [SerializeField] private Camera renderCamera;
        [SerializeField] private LayerMask buttonLayerMask = -1;
        [SerializeField] private float rayLength = 25f;
        
        private GameSelectionButton currentHoveredButton;
        [SerializeField] private List<GameSelectionButton> gameSelectionButtons = new List<GameSelectionButton>();

        [Header("Raycast Adjustments")]
        [SerializeField] private float xScaleMultiplier = 1f; // Adjust this to fix X scaling
        [SerializeField] private float xOffset = 0f; // Fine-tune X position

        protected override void Awake()
        {
            base.Awake();

            InputManager.Instance.Controls.UI.Click.performed += SelectCurrentButton;
        }

        protected override void Update()
        {
            base.Update();

            Vector2 mousePosition = Input.mousePosition;
            Rect cameraViewportRect = renderCamera.rect;
            Rect cameraPixelRect = new Rect(
                cameraViewportRect.x * Screen.width,
                cameraViewportRect.y * Screen.height,
                cameraViewportRect.width * Screen.width,
                cameraViewportRect.height * Screen.height
            );
            
            if (!cameraPixelRect.Contains(mousePosition))
            {
                if(currentHoveredButton != null)
                {
                    currentHoveredButton.OnLeave();
                    currentHoveredButton = null;
                    GameSelectionDisplay.Instance.currentButton = currentHoveredButton;
                }
                return;
            }
            
            Vector2 normalizedPos = new Vector2(
                (mousePosition.x - cameraPixelRect.x) / cameraPixelRect.width,
                (mousePosition.y - cameraPixelRect.y) / cameraPixelRect.height
            );
            
            // Apply X scale adjustment
            float adjustedX = (normalizedPos.x - 0.5f) / renderCamera.aspect * xScaleMultiplier + 0.5f + xOffset;
            
            Vector3 viewportPoint = new Vector3(
                adjustedX,
                normalizedPos.y,
                renderCamera.nearClipPlane
            );
            
            Vector3 rayOrigin = renderCamera.ViewportToWorldPoint(viewportPoint);
            Vector3 rayDirection = renderCamera.transform.forward;
            Ray ray = new Ray(rayOrigin, rayDirection);
            
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * rayLength, Color.red, Time.deltaTime);
            
            if (Physics.Raycast(ray, out RaycastHit hit, rayLength, buttonLayerMask))
            {
                GameSelectionButton button = hit.collider.GetComponent<GameSelectionButton>();
                if (button != null)
                {   
                    if (currentHoveredButton != button)
                    {
                        if(currentHoveredButton != null)
                            currentHoveredButton.OnLeave();
                        button.OnHover();
                        currentHoveredButton = button;
                        ColorManager.Instance.currentGame = button.gameName;
                        GameSelectionDisplay.Instance.currentButton = currentHoveredButton;
                    }
                }
            }
            else if(currentHoveredButton != null)
            {
                currentHoveredButton.OnLeave();
                currentHoveredButton = null;
                GameSelectionDisplay.Instance.currentButton = currentHoveredButton;
            }
        }

        private void SelectCurrentButton(InputAction.CallbackContext context)
        {
            if (currentHoveredButton != null)
            {
                PromptData promptData = new PromptData()
                {
                    title = currentHoveredButton.gameConfig.gameTitle,
                    description = currentHoveredButton.gameConfig.gameDesc, 
                    confirmButtonText = "Play",
                    cancelButtonText = "Go Back",
                    confirmButtonColor = ColorManager.Instance.activeColorData.colors[2],
                    cancelButtonColor = ColorManager.Instance.activeColorData.colors[3],
                    OnConfirm = () => GameManager.Instance.LoadGame(currentHoveredButton.gameName)
                };
                PromptManager.Instance.ShowPrompt(promptData);
            }
        }

        public override void Close()
        {
            base.Close();
            foreach(GameSelectionButton button in gameSelectionButtons)
                button.gameObject.SetActive(false);
        }

        public override void Open()
        {
            base.Open();
            foreach(GameSelectionButton button in gameSelectionButtons)
                button.gameObject.SetActive(true);
        }
    }
}