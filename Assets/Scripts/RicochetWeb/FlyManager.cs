using System.Collections.Generic;
using RobbieWagnerGames.Utilities;
using UnityEngine;

namespace RobbieWagnerGames.ArcadeLibrary.RicochetWeb
{
    public enum FlyDestructionReason
    {
        NONE = -1,
        CLEANUP,
        POINT,
        OTHER
    }
    public class FlyManager : MonoBehaviourSingleton<FlyManager>
    {
        [SerializeField] private Fly flyPrefab;
        private List<Fly> flies = new List<Fly>();
        [SerializeField] private Vector2 minBounds;
        [SerializeField] private Vector2 maxBounds;

        protected override void Awake()
        {
            base.Awake();

            SpawnFly(Vector2.zero);
        }

        public void SpawnFly(Vector2 position)
        {
            Fly fly = Instantiate(flyPrefab, transform);
            fly.transform.position = new Vector3(position.x, position.y, 1.5f);

            flies.Add(fly);
        }

        public void DestroyFly(Fly fly, FlyDestructionReason destructionReason)
        {
            bool success = flies.Remove(fly);
            if (success)
                Destroy(fly.gameObject);

            if (destructionReason == FlyDestructionReason.POINT)
                RicochetWebScoreManager.Instance.Score++;
        }


    }
}