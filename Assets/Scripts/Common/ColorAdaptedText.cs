using System;
using RobbieWagnerGames.ArcadeLibrary.Managers;
using TMPro;
using UnityEngine;

namespace RobbieWagnerGames.ArcadeLibrary
{
    public class ColorAdaptedText : MonoBehaviour
    {
        public TextMeshProUGUI text;
        [SerializeField][Range(0,3)] private int _colorIndex;
        public int colorIndex
        {
            get {return _colorIndex;}
            set
            {
                _colorIndex = Math.Clamp(value, 0, 3);
                text.color = ColorManager.Instance.activeColorMapping[_colorIndex];
            }
        }

        private void Awake()
        {
            GameManager.Instance.OnLoadGame += RefreshText;
            ColorManager.Instance.OnColorSchemeChanged += RefreshText;

            text.color = ColorManager.Instance.activeColorMapping[colorIndex];
        }

        private void RefreshText()
        {
            text.color = ColorManager.Instance.activeColorMapping[colorIndex];
        }

        private void RefreshText(GameConfigurationData data)
        {
            text.color = ColorManager.Instance.colorsByGame[data.gameName].colors[colorIndex];
        }
    }
}