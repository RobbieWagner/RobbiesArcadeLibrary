using System.Collections.Generic;
using System.Linq;
using RobbieWagnerGames.Managers;
using RobbieWagnerGames.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace RobbieWagnerGames.ArcadeLibrary
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private List<MenuTab> tabs;
        [SerializeField] private int defaultTabIndex;
        [SerializeField] private bool openOnAwake;
        
        private void Awake()
        {
            if (openOnAwake)
                OpenDefaultMenu();
            
            InputManager.Instance.Controls.UI.NavigateTabs.performed += SwitchTab;
        }

        public void OpenDefaultMenu()
        {
            Menu.activeMenu = tabs[defaultTabIndex].menu;
            InputManager.Instance.EnableActionMap(ActionMapName.UI);
        }
        
        private void SwitchTab(InputAction.CallbackContext context)
        {
            float direction = context.ReadValue<float>();
            int currentMenuIndex = tabs.IndexOf(tabs.FirstOrDefault(t => t.menu == Menu.activeMenu));
            int newMenuIndex = -1;

            if (currentMenuIndex > -1)
            {
                if (direction > 0f)
                    newMenuIndex = (currentMenuIndex + 1) % tabs.Count;
                else if (direction < 0f)
                {
                    newMenuIndex = currentMenuIndex - 1;
                    if (currentMenuIndex == -1) newMenuIndex = tabs.Count - 1; 
                }
                else
                    Debug.LogWarning("Could not determine direction based on input");
            }
            else
                Debug.LogWarning("Could not find active menu in array");

            foreach (ColorAdaptedText text in tabs.Select(t => t.menuText))
                text.colorIndex = 1;

            if (newMenuIndex != -1)
            {
                Menu.activeMenu = tabs[newMenuIndex].menu;
                tabs[newMenuIndex].menuText.colorIndex = 0;
            }
            else
                Debug.LogWarning("Could not locate new tab");
        }
    }
}