using System;
using UnityEngine;
using UnityEngine.UI;

namespace RobbieWagnerGames.ArcadeLibrary.Torchlight
{
    public class SnuffTimer : MonoBehaviour
    {
        [SerializeField] private Slider timeSlider;

        private void Awake()
        {
            Torch.OnInitializeTorch += InitializeSnuffTimer;
        }

        private void InitializeSnuffTimer(Torch torch)
        {
            timeSlider.maxValue = torch.snuffTimer.duration;
            timeSlider.minValue = 0;
            timeSlider.value = torch.snuffTimer.timerValue;

            torch.snuffTimer.OnTimerUpdate += UpdateSnuffTimer;
        }

        private void UpdateSnuffTimer(float timerValue)
        {
            timeSlider.value = Torch.Instance.snuffTimer.duration - timerValue;
        }
    }
}