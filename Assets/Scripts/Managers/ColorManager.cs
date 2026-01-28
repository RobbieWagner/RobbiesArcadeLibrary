using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using RobbieWagnerGames.Utilities;
using UnityEngine;

namespace RobbieWagnerGames.ArcadeLibrary.Managers
{
    [Serializable]
    public class ColorData
    {
        public List<Color> colors;
        public Sprite colorPalette;
    }

    public class ColorManager : MonoBehaviourSingleton<ColorManager>
    {
        [SerializedDictionary("Game","Colors")] 
        public SerializedDictionary<GameName, ColorData> colorsByGame;

        public ColorData activeColorData => colorsByGame[GameManager.Instance.isGameSet ? GameManager.Instance.CurrentGame.gameName : currentGame];
        public List<Color> activeColorMapping => activeColorData.colors;

        private GameName _currentGame = GameName.NONE;
        public GameName currentGame
        {
            get{return _currentGame;}
            set
            {
                _currentGame = value;
                UpdateMaterialsWithNewPalette();
                OnColorSchemeChanged?.Invoke();
            }
        }
        public event Action OnColorSchemeChanged;

        protected override void Awake()
        {
            base.Awake();
            currentGame = GameName.NONE;
        }

        public Material uiMat;
        public Material renderMat;

        public void UpdateMaterialsWithNewPalette()
        {
            Texture2D paletteTexture = activeColorData.colorPalette.texture; 

            uiMat.SetTexture("_Palette", paletteTexture);
            renderMat.SetTexture("_Palette", paletteTexture);
        }

        public void SetPaletteTexture(Texture2D paletteTexture)
        {
            uiMat.SetTexture("_Palette", paletteTexture);
            renderMat.SetTexture("_Palette", paletteTexture);
        }

        public void SetFadeValue(float fadeValue)
        {
            uiMat.SetFloat("_Fade", fadeValue);
            renderMat.SetFloat("_Fade", fadeValue);
        }
    }
}