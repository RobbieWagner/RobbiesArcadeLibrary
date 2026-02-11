using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RobbieWagnerGames.AI;
using UnityEngine;
using UnityEngine.AI;

namespace RobbieWagnerGames.ArcadeLibrary.RicochetWeb
{
    /// <summary>
    /// Defines the base AI agent and helpful methods for pathfinding.
    /// Useful for simple AI agents or when controlling many agents.
    /// Can be inherited for custom behaviors.
    /// </summary>
    public class Fly : MonoBehaviour
    {
        [Header("Navigation Settings")]
        [SerializeField] protected float idleWaitTime = 3f;
        [SerializeField] protected float movementRange = 100f;
        
        public NavMeshAgent Agent { get; protected set; }
        public AIState CurrentState { get; protected set; } = AIState.None;

        protected float currentWaitTime;

        protected virtual void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            if (Agent == null)
            {
                Debug.LogError("NavMeshAgent component is missing!", this);
            }

            CurrentState = AIState.Idle;
        }

        protected virtual void Update()
        {
            UpdateState();
        }

        #region State Management
        protected virtual void UpdateState()
        {
            switch (CurrentState)
            {
                case AIState.Idle:
                    UpdateIdleState();
                    break;
                case AIState.Moving:
                    UpdateMovingState();
                    break;
            }
        }

        protected virtual void ChangeState(AIState newState)
        {
            if (newState == CurrentState) return;
            CurrentState = newState;
            OnStateChanged(newState);
        }

        protected virtual void OnStateChanged(AIState newState)
        {
            // Can be overridden for custom state change behavior
        }
        #endregion

        #region State Behaviors
        public virtual void GoIdle()
        {
            Agent.isStopped = true;
            ChangeState(AIState.Idle);
        }

        protected virtual void UpdateIdleState()
        {
            currentWaitTime += Time.deltaTime;
            if (currentWaitTime >= idleWaitTime)
            {
                currentWaitTime = 0;
                MoveToRandomSpot(movementRange);
            }
        }

        protected virtual void UpdateMovingState()
        {
            if (HasReachedDestination())
            {
                GoIdle();
            }
        }
        #endregion

        #region Navigation
        public virtual bool SetDestination(Vector3 destination)
        {
            Agent.isStopped = false;
            return Agent.SetDestination(destination);
        }

        public virtual bool MoveAgent(Vector3 destination)
        {
            ChangeState(AIState.Moving);
            bool success = SetDestination(destination);

            if (!success)
            {
                GoIdle();
                Debug.LogWarning("Failed to move agent to destination");
            }

            return success;
        }

        public virtual void MoveToRandomSpot(float range = 100f)
        {
            StartCoroutine(MoveToRandomSpotCoroutine(transform.position, range));
        }

        protected virtual IEnumerator MoveToRandomSpotCoroutine(Vector3 center, float range, int maxAttempts = 100, int attemptsBeforeYield = 10)
        {
            int attempts = 0;
            bool success = false;
            
            Debug.Log("find location");
            while (attempts < maxAttempts && !success)
            {
                attempts++;
                
                if (attempts % attemptsBeforeYield == 0)
                    yield return null;
                
                Vector3 randomDirection = (Vector3) (Random.insideUnitCircle * range) + (Vector3.forward * 1.5f);
                randomDirection += center;
                
                if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, range, NavMesh.AllAreas))
                {
                    Debug.Log("hello");
                    success = MoveAgent(hit.position);
                }
            }
            
            if (!success)
            {
                Debug.LogWarning($"Failed to find valid navigation position after {maxAttempts} attempts");
            }
        }

        protected virtual bool HasReachedDestination()
        {
            return !Agent.pathPending 
                   && Agent.remainingDistance <= Agent.stoppingDistance 
                   && (!Agent.hasPath || Agent.velocity.sqrMagnitude == 0f);
        }
        #endregion
    }
}