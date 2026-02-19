using System;
using UnityEngine;

namespace RobbieWagnerGames.ArcadeLibrary.UnEuclid
{
    public class PassageTrigger : MonoBehaviour
    {
        public event Action<PassageTrigger, Transform> onTriggered = null;
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
                onTriggered?.Invoke(this, other.transform);
        }
    }
}