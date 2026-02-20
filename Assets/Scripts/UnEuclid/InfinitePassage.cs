using System;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.ArcadeLibrary.UnEuclid
{
    public class InfinitePassage : MonoBehaviour
    {
        [SerializeField] private List<Transform> transforms = new List<Transform>(); // assume length 3
        [SerializeField] private PassageTrigger upTrigger;
        [SerializeField] private Vector3 upTriggerOffset;
        [SerializeField] private PassageTrigger downTrigger;
        [SerializeField] private Vector3 downTriggerOffset;
        [SerializeField] private int currentTransformIndex = 1; // assume start in middle
        [SerializeField] private Vector3 objectOffset = Vector3.zero;

        private Vector3 initialPos = Vector3.zero;

        private void Awake()
        {
            initialPos = transforms[currentTransformIndex].position;

            transforms[0].position = initialPos - objectOffset;
            transforms[2].position = initialPos + objectOffset;
            
            downTrigger.onTriggered += HandleDownTrigger;
            upTrigger.onTriggered += HandleUpTrigger;

            downTrigger.transform.position = transforms[1].position + downTriggerOffset;
            upTrigger.transform.position = transforms[1].position + upTriggerOffset;
        }

        private void HandleDownTrigger(PassageTrigger trigger, Transform playerTransform)
        {
            if (playerTransform.position.y < trigger.transform.position.y)
                MovePassage((currentTransformIndex + 2) % 3); // (currentTransformIndex - 1 + 3) % 3
        }

        private void HandleUpTrigger(PassageTrigger trigger, Transform playerTransform)
        {
            if (Mathf.Abs(playerTransform.position.y - trigger.transform.position.y) < .25f) // because the root is at the players feet
                MovePassage((currentTransformIndex + 1) % 3);
            Debug.Log("triggered");
        }
        
        private void MovePassage(int newIndex)
        {
            currentTransformIndex = newIndex;

            transforms[(currentTransformIndex + 2) % 3].position = transforms[currentTransformIndex].position - objectOffset; // (currentTransformIndex - 1 + 3) % 3
            transforms[(currentTransformIndex + 1) % 3].position = transforms[currentTransformIndex].position + objectOffset;

            downTrigger.transform.position = transforms[currentTransformIndex].position + downTriggerOffset;
            upTrigger.transform.position = transforms[currentTransformIndex].position + upTriggerOffset;
        }
    }
}