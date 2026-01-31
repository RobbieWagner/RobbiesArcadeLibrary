using RobbieWagnerGames.Managers;
using RobbieWagnerGames.UI;
using UnityEngine;

namespace RobbieWagnerGames.ArcadeLibrary
{
    public class GameSelectionMenu : Menu
    {
        [Header("Game Selection")]
        [SerializeField] private Camera renderCamera;
        private GameSelectionButton currentHoveredButton;

        protected override void Update()
        {
            base.Update();

            Vector2 mousePosition = InputManager.Instance.Controls.UI.Point.ReadValue<Vector2>();
            Ray ray = renderCamera.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameSelectionButton button = hit.collider.GetComponent<GameSelectionButton>();
                if (button != null)
                {
                    if (currentHoveredButton != null && currentHoveredButton != button)
                        currentHoveredButton.OnLeave();
                    
                    if (currentHoveredButton != button)
                    {
                        button.OnHover();
                        currentHoveredButton = button;
                    }
                }
            }
            else
            {
                currentHoveredButton?.OnLeave();
                currentHoveredButton = null;
            }
        }
    }
}