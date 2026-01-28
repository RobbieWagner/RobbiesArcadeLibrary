using System;
using System.Collections.Generic;
using System.Linq;
using RobbieWagnerGames.Utilities;
using UnityEngine;

namespace RobbieWagnerGames.ArcadeLibrary.Managers
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        [field: SerializeField] public List<GameConfigurationData> GameLibrary { get; private set; }
        [field: SerializeField] public GameConfigurationData CurrentGame { get; private set; }

        public event Action<GameConfigurationData> OnSetGame;
        public event Action<GameConfigurationData> OnLoadGame;

        protected override void Awake()
        {
            base.Awake();
            SceneLoadManager.Instance.LoadSceneAdditive("MainMenu", true, .25f);
        }

        public void LoadGame(GameName gameName)
        {
            GameConfigurationData game = GameLibrary.FirstOrDefault(x => x.gameName == gameName);

            if(game != null)
                SceneLoadManager.Instance.UnloadAllScenesExcept(new (){"MainScene"}, () => {OnLoadGameScene(game);});
            else
                Debug.LogWarning($"Could not find a valid game for \"{gameName}\"");
        }

        public void OnLoadGameScene(GameConfigurationData game)
        {
            SceneLoadManager.Instance.LoadSceneAdditive(game.sceneName, true, 1, () => {OnLoadGame?.Invoke(game);});
            CurrentGame = game;
            OnSetGame?.Invoke(game);
        }
    }
}