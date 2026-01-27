using System.Collections.Generic;
using System.Linq;
using RobbieWagnerGames.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace RobbieWagnerGames.ArcadeLibrary.Managers
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        [field: SerializeField] public List<GameConfigurationData> GameLibrary { get; private set; }
        [field: SerializeField] public GameConfigurationData CurrentGame { get; private set; }

        protected override void Awake()
        {
            SceneLoadManager.Instance.LoadSceneAdditive("MainMenu", true, .25f);
        }

        public void LoadGame(GameName gameName)
        {
            GameConfigurationData game = GameLibrary.FirstOrDefault(x => x.gameName == gameName);

            if(game != null)
            {
                SceneLoadManager.Instance.UnloadAllScenesExcept(new (){"MainScene"}, () => {OnLoadGame(game);});
            }
            else
                Debug.LogWarning($"Could not find a valid game for \"{gameName}\"");
        }

        public void OnLoadGame(GameConfigurationData game)
        {
            SceneLoadManager.Instance.LoadSceneAdditive(game.sceneName);
            CurrentGame = game;
        }
    }
}