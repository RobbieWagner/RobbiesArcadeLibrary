using System.Collections;
using RobbieWagnerGames.Managers;
using UnityEngine;

namespace RobbieWagnerGames.ArcadeLibrary.Torchlight
{
    public class TorchlightManager : ArcadeGameManager
    {
        protected override GameName game => GameName.TORCHLIGHT;

        protected override void Awake()
        {
            base.Awake();

            EnableControls();

            StartCoroutine(StartGame());
        }

        public override IEnumerator StartGame()
        {
            yield return base.StartGame();

            Torch.Instance.InitializeTorch();
        }
    }
}