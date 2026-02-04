using System;
using RobbieWagnerGames.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobbieWagnerGames.ArcadeLibrary
{
    public class GameSelectionDisplay : MonoBehaviourSingleton<GameSelectionDisplay>
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descText;
        [SerializeField] private Image icon;
        [SerializeField] private VerticalLayoutGroup display;

        private GameSelectionButton _currentButton;
        public GameSelectionButton currentButton
        {
            get
            {
                return _currentButton;
            }
            set
            {
                if (_currentButton == value)
                    return;
                _currentButton = value;
                UpdateVisual(value);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            UpdateVisual(null);
        }

        private void UpdateVisual(GameSelectionButton button)
        {
            if (button == null)
            {    
                display.gameObject.SetActive(false);
                return;
            }
            
            titleText.text = button.gameConfig.gameTitle;
            descText.text = button.gameConfig.gameDesc;
            icon.sprite = button.gameConfig.gameIcon;
            display.gameObject.SetActive(true);
        }
    }
}