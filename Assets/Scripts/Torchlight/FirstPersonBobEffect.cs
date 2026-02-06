using UnityEngine;
using System;
using NUnit.Framework;

namespace RobbieWagnerGames.FirstPerson
{
    [Serializable]
    public struct BobEffectSettings
    {
        public float bobSpeed;
        public float bobStrength;
        public float sideBobStrength;
        public float stepDelay;
        public float returnSpeed;
    }

    public class FirstPersonBobEffect : MonoBehaviour
    {
        public event Action<bool> OnBobRequested;
        
        [Header("Bob Settings")]
        [SerializeField] private BobEffectSettings bobSettings = new BobEffectSettings()
        {
            bobSpeed = 10f,
            bobStrength = 0.05f,
            sideBobStrength = 0.025f,
            stepDelay = 0.2f,
            returnSpeed = 5f
        };

        private float bobTimer = 0f;
        private Vector3 originalPosition;
        private bool isBobbing = false;
        private float groundTypeMultiplier = 1f;
        private float stepTimer = 0f;

        private void Start()
        {
            originalPosition = transform.localPosition;
        }

        private void Update()
        {
            if (!isBobbing)
            {
                ReturnToRestPosition();
                return;
            }

            UpdateBobEffect();
        }

        public void SetBobActive(bool active)
        {
            if(active == isBobbing) 
                return;
            
            OnBobRequested?.Invoke(active);
            
            if (active)
                StartBobEffect();
            else
                StopBobEffect();
        }

        private void StartBobEffect()
        {
            isBobbing = true;
            bobTimer = 0f;
            stepTimer = 0f;
        }

        private void StopBobEffect()
        {
            isBobbing = false;
        }

        private void UpdateBobEffect()
        {
            stepTimer += Time.deltaTime;
            if (stepTimer < bobSettings.stepDelay)
                return;
            
            bobTimer += Time.deltaTime * bobSettings.bobSpeed * groundTypeMultiplier;
            
            float verticalBob = Mathf.Sin(bobTimer) * bobSettings.bobStrength;
            float horizontalBob = Mathf.Sin(bobTimer * 2) * bobSettings.sideBobStrength;
            
            Vector3 bobPosition = new Vector3(horizontalBob,verticalBob,0 );

            transform.localPosition = originalPosition + bobPosition;
        }

        private void ReturnToRestPosition()
        {
            if (Vector3.Distance(transform.localPosition, originalPosition) > 0.001f)
            {
                transform.localPosition = Vector3.Lerp(
                    transform.localPosition, 
                    originalPosition, 
                    Time.deltaTime * bobSettings.returnSpeed
                );
            }
        }

        public BobEffectSettings GetBobSettings() => bobSettings;

        public void SetBobSettings(BobEffectSettings newSettings)
        {
            bobSettings = newSettings;
        }
    }
}