using System;
using UnityEngine;

public enum GameName
{
    NONE = -1,
    TORCHLIGHT,
    RICOCHET_WEB
}

[Serializable]
public class GameConfigurationData
{
    public GameName gameName;
    public string sceneName;
    public Sprite gameIcon;
    public Sprite gamePalette;
    [TextArea] public string gameDesc; 
}
