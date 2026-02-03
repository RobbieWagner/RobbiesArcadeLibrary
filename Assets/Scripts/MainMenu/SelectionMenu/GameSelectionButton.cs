using UnityEngine;

namespace RobbieWagnerGames.ArcadeLibrary
{
    public class GameSelectionButton : MonoBehaviour //Selectable, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("button details")]
        private bool isHovering = false;
        private Quaternion initialRotation;
        public GameName game;
        public GameObject buttonObj;
        [SerializeField] private Transform box;

        [Header("game info")]
        public string titleText;
        public string descText;
        public Sprite gameIcon;

        private void Awake()
        {
            initialRotation = box.rotation;
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