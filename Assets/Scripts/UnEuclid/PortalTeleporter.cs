using UnityEngine;

namespace RobbieWagnerGames.ArcadeLibrary.UnEuclid
{
    public class PortalTeleporter : MonoBehaviour 
    {
        public Transform player;
        public Transform receiver;
        public CharacterController characterController;

        private bool playerIsOverlapping = false;

        private void Update () 
        {
            if (playerIsOverlapping)
            {
                Vector3 portalToPlayer = player.position - transform.position;
                float dotProduct = Vector3.Dot(transform.up, portalToPlayer);

                if (dotProduct < 0f)
                {
                    float rotationDiff = -Quaternion.Angle(transform.rotation, receiver.rotation);
                    Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToPlayer;
                    
                    if (characterController != null)
                    {
                        characterController.enabled = false;
                        player.position = receiver.position + positionOffset;
                        player.Rotate(Vector3.up, rotationDiff);
                        characterController.enabled = true;
                    }
                    else
                    {
                        player.position = receiver.position + positionOffset;
                        player.Rotate(Vector3.up, rotationDiff);
                    }

                    playerIsOverlapping = false;
                }
            }
        }

        private void OnTriggerEnter (Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerIsOverlapping = true;
                if (characterController == null)
                    characterController = other.GetComponent<CharacterController>();
            }
        }

        private void OnTriggerExit (Collider other)
        {
            if (other.CompareTag("Player"))
                playerIsOverlapping = false;
        }
    }
}