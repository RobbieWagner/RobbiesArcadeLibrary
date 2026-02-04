using System.Linq;
using RobbieWagnerGames.ArcadeLibrary.Managers;
using UnityEngine;

namespace RobbieWagnerGames.ArcadeLibrary
{
    public class GameSelectionButton : MonoBehaviour //Selectable, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("button details")]
        private bool isHovering = false;
        private Quaternion initialRotation;
        public GameName gameName;
        public GameObject buttonObj;
        [SerializeField] private Transform box;

        public GameConfigurationData gameConfig {get; set;}

        private void Awake()
        {
            initialRotation = box.rotation;
            gameConfig = GameManager.Instance.GameLibrary.First(g => g.gameName == gameName);
        }

        public void OnHover()
        {
            isHovering = true;
        }

        public void OnLeave()
        {
            // Debug.Log("exit");
            isHovering = false;
        }

        private void Update()
        {
            if (isHovering)
                RotateCube();
            else
                box.rotation = initialRotation;
        }

        private void RotateCube()
        {
            Vector3 rotationDirection = Random.insideUnitSphere/2;
            rotationDirection = new Vector3(Mathf.Abs(rotationDirection.x), Mathf.Abs(rotationDirection.y), Mathf.Abs(rotationDirection.z));
            box.Rotate(rotationDirection, Space.World);
        }
    }
}