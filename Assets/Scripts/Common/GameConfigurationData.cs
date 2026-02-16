using System;
using UnityEngine;

namespace RobbieWagnerGames.ArcadeLibrary
{
    public enum GameName
    {
        NONE = -1,
        TORCHLIGHT,
        RICOCHET_WEB,
        GAMBLING,
        UNEUCLID
    }

    [Serializable]
    public class GameConfigurationData
    {
        public GameName gameName;
        public string gameTitle;
        public string sceneName;
        public Sprite gameIcon;
        [TextArea] public string gameDesc; 
    }
}