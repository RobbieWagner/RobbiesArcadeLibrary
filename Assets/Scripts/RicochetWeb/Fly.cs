using System.Collections;
using DG.Tweening;
using RobbieWagnerGames.AI;
using UnityEngine;
using UnityEngine.AI;

namespace RobbieWagnerGames.ArcadeLibrary.RicochetWeb
{
    public class Fly : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [Header("Navigation")]
        [SerializeField] protected float idleWaitTime = 3f;
        [SerializeField] protected float movementRange = 100f;
        
        public NavMeshAgent Agent { get; protected set; }
        public AIState CurrentState { get; protected set; } = AIState.None;
        private Coroutine agentCoroutine;
        public bool isCaptured {get; private set;}

        protected float currentWaitTime;

        protected virtual void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            if (Agent == null)
            {
                Debug.LogError("NavMeshAgent component is missing!", this);
            }
            Agent.updateRotation = false;
            Agent.enabled = false;

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
                if (!isCaptured)
                    MoveToRandomSpot(movementRange);
            }
        }

        protected virtual void UpdateMovingState()
        {
            if (HasReachedDestination())
                GoIdle();
        }
        #endregion

        #region Navigation
        public virtual bool SetDestination(Vector3 destination)
        {
            Agent.enabled = true;
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
            agentCoroutine = StartCoroutine(MoveToRandomSpotCoroutine(transform.position, range));
        }

        protected virtual IEnumerator MoveToRandomSpotCoroutine(Vector3 center, float range, int maxAttempts = 100, int attemptsBeforeYield = 10)
        {
            int attempts = 0;
            bool success = false;
            
            while (attempts < maxAttempts && !success)
            {
                attempts++;
                
                if (attempts % attemptsBeforeYield == 0)
                    yield return null;
                
                Vector2 randomCircle = Random.insideUnitCircle * range;
                
                Vector3 randomPosition = new Vector3(center.x + randomCircle.x,center.y + randomCircle.y, 1.5f);
                
                if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, range, NavMesh.AllAreas))
                    success = MoveAgent(hit.position);
            }
            
            if (!success)
                Debug.LogWarning($"Failed to find valid navigation position after {maxAttempts} attempts");
            
            agentCoroutine = null;
        }

        protected virtual bool HasReachedDestination()
        {
            return !Agent.pathPending 
                   && Agent.remainingDistance <= Agent.stoppingDistance 
                   && (!Agent.hasPath || Agent.velocity.sqrMagnitude == 0f);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Hazard"))
                CaptureFly();
        }
        #endregion

        #region Capture
        private void CaptureFly()
        {
            if (!isCaptured)
            {
                if (agentCoroutine != null)
                {
                    StopCoroutine(agentCoroutine);
                    agentCoroutine = null;
                }
                Agent.destination = transform.position;
                ChangeState(AIState.Idle);
                isCaptured = true;

                Sequence dieOnCaptureSequence = DOTween.Sequence();

                for (int i = 0; i < 4; i++)
                {
                    dieOnCaptureSequence.Append(spriteRenderer.DOColor(Color.clear, .1f));
                    dieOnCaptureSequence.Append(spriteRenderer.DOColor(Color.white, .1f));
                }
                dieOnCaptureSequence.OnComplete(() => {FlyManager.Instance.DestroyFly(this, FlyDestructionReason.POINT);});
                dieOnCaptureSequence.Play();
            }
        }
        #endregion
    }
}