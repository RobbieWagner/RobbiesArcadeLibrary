using System;
using RobbieWagnerGames.Utilities;

namespace RobbieWagnerGames.ArcadeLibrary.RicochetWeb
{
    public class RicochetWebScoreManager : MonoBehaviourSingleton<RicochetWebScoreManager>
    {
        private int score = 0;
        public int Score
        {
            get { return score; }
            set 
            {
                if (score == value)
                    return;
                score = value; 
                OnScoreUpdated?.Invoke(score);
            }
        }
        public event Action<int> OnScoreUpdated;
    }
}