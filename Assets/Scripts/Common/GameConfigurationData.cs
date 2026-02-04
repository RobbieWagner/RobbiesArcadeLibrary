using System;
using UnityEngine;

public enum GameName
{
    NONE = -1,
    TORCHLIGHT,
    RICOCHET_WEB,
    GAMBLING
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
