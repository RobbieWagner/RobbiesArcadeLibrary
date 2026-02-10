using System.Collections.Generic;
using RobbieWagnerGames.ArcadeLibrary.Common;
using RobbieWagnerGames.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RobbieWagnerGames.ArcadeLibrary.RicochetWeb
{
    public class WebShooter : MonoBehaviour
    {
        [Header("Web Shooting")]
        [SerializeField] private RicochetWeb webPrefab;
        private List<RicochetWeb> webs = new List<RicochetWeb>();
        [SerializeField] private float defaultWebSpeed;

        [SerializeField] private Vector2 webShooterMinBounds;
        [SerializeField] private Vector2 webShooterMaxBounds;

        private void Awake()
        {
            InputManager.Instance.Controls.RICOCHET_WEB.ShootWeb.performed += ShootWeb;
            
            InputManager.Instance.EnableActionMap(ActionMapName.RICOCHET_WEB);
        }

        private void ShootWeb(InputAction.CallbackContext context)
        {
            Vector3 mousePos = CameraMouseInteraction.Instance.mousePosition;
            if (mousePos.x < webShooterMinBounds.x || mousePos.x > webShooterMaxBounds.x
                || mousePos.y < webShooterMinBounds.y || mousePos.y > webShooterMaxBounds.y)
                return;
            Debug.Log(mousePos);
            mousePos = new Vector3(mousePos.x, mousePos.y, 1.5f);
            Vector2 direction = (mousePos - transform.position).normalized;

            RicochetWeb web = Instantiate(webPrefab);
            web.transform.position = transform.position;
            
            web.ShootWeb(transform.position, direction, defaultWebSpeed);
            webs.Add(web);
        }

                
        private void OnDestroy()
        {
            InputManager.Instance.Controls.RICOCHET_WEB.ShootWeb.performed -= ShootWeb;
        }
    }
}