using RobbieWagnerGames.ArcadeLibrary.Managers;
using RobbieWagnerGames.Managers;
using RobbieWagnerGames.Utilities;
using UnityEngine;

namespace RobbieWagnerGames.ArcadeLibrary
{
    public class ArcadeGameManager : MonoBehaviourSingleton<ArcadeGameManager>
    {
        protected virtual GameName game => GameName.NONE;

        protected override void Awake()
        {
            base.Awake();

            ColorManager.Instance.currentGame = game;
        }

        protected virtual void EnableControls()
        {
            InputManager.Instance.EnableActionMap(ActionMapName.GAME);
        }
    }
}