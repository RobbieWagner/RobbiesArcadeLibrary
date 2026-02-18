using System;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.ArcadeLibrary.UnEuclid
{
    public class InfinitePassage : MonoBehaviour
    {
        [SerializeField] private List<Transform> transforms = new List<Transform>(); // assume length 3
        [SerializeField] private Collider upTrigger;
        [SerializeField] private Vector3 upTriggerOffset;
        [SerializeField] private Collider downTrigger;
        [SerializeField] private Vector3 downTriggerOffset;
        [SerializeField] private int currentTransformIndex = 1; // assume start in middle
        [SerializeField] private Vector3 objectOffset = Vector3.zero;

        private Vector3 initialPos = Vector3.zero;

        private void Awake()
        {
            initialPos = transforms[currentTransformIndex].position;

            transforms[0].position = initialPos - objectOffset;
            transforms[2].position = initialPos + objectOffset;
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log("hi");
            if (other.gameObject.CompareTag("Player"))
                UpdateTransforms(other.transform);
        }

        private void UpdateTransforms(Transform player)
        {
            Vector3 playerPos = player.position;

            if (playerPos.y > transforms[currentTransformIndex].position.y) // TODO: Extend to other axis (not just y)
            {
                Debug.Log("above");
            }
            else if (playerPos.y < transforms[currentTransformIndex].position.y)
            {
                Debug.Log("below");
            }
        }
    }
}