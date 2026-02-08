using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.ArcadeLibrary.RicochetWeb
{
    public class WebShooter : MonoBehaviour
    {
        [SerializeField] private RicochetWeb webPrefab;
        private List<RicochetWeb> webs = new List<RicochetWeb>();
        [SerializeField] private float defaultSpeed;

        private void Awake()
        {
            RicochetWeb web = Instantiate(webPrefab);
            web.transform.position = transform.position;
            web.ShootWeb(transform.position, Random.insideUnitCircle, defaultSpeed);
            webs.Add(web);
        }
    }
}