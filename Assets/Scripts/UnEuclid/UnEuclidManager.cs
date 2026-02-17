using UnityEngine;

namespace RobbieWagnerGames.ArcadeLibrary
{
    public class UnEuclidManager : ArcadeGameManager
    {

        protected override GameName game => GameName.UNEUCLID;

        protected override void Awake()
        {
            base.Awake();

            EnableControls();
        }
    }
}