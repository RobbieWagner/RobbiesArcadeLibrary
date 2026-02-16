using System.Collections.Generic;
using NavMeshPlus.Extensions;
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

            SpawnFly();
        }

        public void SpawnFly() => SpawnFly(new Vector2(Random.Range(minBounds.x, maxBounds.x), Random.Range(minBounds.y, maxBounds.y)));

        public void SpawnFly(Vector2 position)
        {
            Debug.Log(position);
            Fly fly = Instantiate(flyPrefab, transform);
            fly.transform.position = new Vector3(Mathf.Clamp(position.x, minBounds.x, maxBounds.x), Mathf.Clamp(position.y, minBounds.y, maxBounds.y), 1.5f);

            flies.Add(fly);
        }

        public void DestroyAllFlies(FlyDestructionReason destructionReason = FlyDestructionReason.CLEANUP)
        {
            foreach (Fly fly in flies)
                DestroyFly(fly, destructionReason);
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