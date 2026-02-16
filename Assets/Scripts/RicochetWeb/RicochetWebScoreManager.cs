using System;
using RobbieWagnerGames.Utilities;
using TMPro;
using UnityEngine;

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

                scoreText.text = score.ToString("D4");
            }
        }
        public event Action<int> OnScoreUpdated;

        [SerializeField] private TextMeshProUGUI scoreText;
    }
}