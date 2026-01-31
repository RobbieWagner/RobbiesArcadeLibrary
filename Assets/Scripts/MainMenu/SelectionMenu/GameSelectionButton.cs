using UnityEngine;

namespace RobbieWagnerGames.ArcadeLibrary
{
    public class GameSelectionButton :MonoBehaviour //Selectable, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnHover()
        {
            Debug.Log("enter");
        }

        public void OnLeave()
        {
            Debug.Log("exit");
        }
    }
}