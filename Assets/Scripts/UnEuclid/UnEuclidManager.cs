using UnityEngine;

namespace RobbieWagnerGames.ArcadeLibrary
{
    public class UnEuclidManager : ArcadeGameManager
    {
        protected override void Awake()
        {
            base.Awake();

            EnableControls();
        }
    }
}