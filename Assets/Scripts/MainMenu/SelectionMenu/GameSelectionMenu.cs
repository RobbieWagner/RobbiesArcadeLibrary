using System.Collections.Generic;
using RobbieWagnerGames.ArcadeLibrary.Managers;
using RobbieWagnerGames.Managers;
using RobbieWagnerGames.UI;
using UnityEngine;

namespace RobbieWagnerGames.ArcadeLibrary
{
    public class GameSelectionMenu : Menu
    {
        [Header("Game Selection")]
        [SerializeField] private Camera renderCamera;
        [SerializeField] private LayerMask buttonLayerMask = -1;
        [SerializeField] private Vector2 rayOffset;
        [SerializeField] private float raycastScale;
        
        private GameSelectionButton currentHoveredButton;
        [SerializeField] private List<GameSelectionButton> gameSelectionButtons = new List<GameSelectionButton>();

        [SerializeField] private GameSelectionDisplay gameSelectionDisplayPrefab;

        protected override void Update()
        {
            base.Update();

            Vector2 mousePosition = InputManager.Instance.Controls.UI.Point.ReadValue<Vector2>();
            Ray ray = new Ray(renderCamera.ScreenToWorldPoint(mousePosition), renderCamera.transform.forward);
            ray.origin *= raycastScale; 
            ray.origin += (Vector3) rayOffset;
            
            // Debug.DrawLine(ray.origin, ray.direction * 25 + ray.origin, Color.red, 1);
            // Debug.Log(ray.origin);
            
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, buttonLayerMask))
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
                        ColorManager.Instance.currentGame = button.game;
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