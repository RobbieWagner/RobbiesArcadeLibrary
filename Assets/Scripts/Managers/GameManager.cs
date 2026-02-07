using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RobbieWagnerGames.Utilities;
using UnityEngine;

namespace RobbieWagnerGames.ArcadeLibrary.Managers
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        [field: SerializeField] public List<GameConfigurationData> GameLibrary { get; private set; }
        public GameConfigurationData CurrentGame { get; private set; }
        public GameName emptyGame;

        public event Action<GameConfigurationData> OnSetGame;
        public event Action<GameConfigurationData> OnLoadGame;

        public bool isGameSet => CurrentGame != null && CurrentGame.gameName != GameName.NONE;

        protected override void Awake()
        {
            base.Awake();
            CurrentGame = GameLibrary.FirstOrDefault(g => g.gameName == emptyGame);
            
            StartCoroutine(LoadMainMenuCo());
        }

        private IEnumerator LoadMainMenuCo()
        {
            yield return new WaitForEndOfFrame();

            bool isGameSceneLoaded = false;
            foreach (GameConfigurationData game in GameLibrary)
            {
                if (SceneLoadManager.Instance.IsSceneLoaded(game.sceneName))
                {
                    isGameSceneLoaded = true;
                    break;
                }
            }
            
            if (!SceneLoadManager.Instance.IsSceneLoaded("MainMenu") && !isGameSceneLoaded)
                yield return SceneLoadManager.Instance.LoadSceneAdditive("MainMenu", false, null);
        }

        public IEnumerator LoadGame(GameName gameName)
        {
            GameConfigurationData game = GameLibrary.FirstOrDefault(x => x.gameName == gameName);

            if(game != null)
            {
                yield return SceneLoadManager.Instance.UnloadAllScenesExcept(new (){"MainScene"}, null, false);
                CurrentGame = game;
                OnSetGame?.Invoke(game);
                yield return SceneLoadManager.Instance.LoadSceneAdditive(game.sceneName, true, () => {OnLoadGame?.Invoke(game);});
            }
            else
                Debug.LogWarning($"Could not find a valid game for \"{gameName}\"");
        }
    }
}