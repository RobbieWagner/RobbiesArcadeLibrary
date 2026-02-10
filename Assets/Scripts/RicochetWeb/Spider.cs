using System.Collections.Generic;
using RobbieWagnerGames.ArcadeLibrary.Common;
using RobbieWagnerGames.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RobbieWagnerGames.ArcadeLibrary.RicochetWeb
{
    public class Spider : MonoBehaviour
    {
        private enum BorderWallEdge
        {
            LEFT,
            RIGHT,
            TOP,
            BOTTOM    
        }

        [Header("Spider")]
        private Vector2 moveVector;
        private List<BorderWallEdge> borderWallEdges = new List<BorderWallEdge>();
        [SerializeField] private Vector2 borderWallMinPositions;
        [SerializeField] private Vector2 borderWallMaxPositions;
        [SerializeField] private float zPos;
        [SerializeField] private Rigidbody2D rb2d;
        public bool canMove { get; set; }
        [SerializeField] private float speed;
        [SerializeField] private float borderSnapThreshold = 0.1f;
        private List<BorderWallEdge> intentionallyRemovedEdges = new List<BorderWallEdge>();

        private void Awake()
        {
            transform.position = new Vector3(borderWallMinPositions.x, borderWallMinPositions.y, zPos);
            InputManager.Instance.EnableActionMap(ActionMapName.GAME);
            canMove = true;
        }

        private void Update()
        {
            UpdateBorderEdges();
            
            if (canMove)
                UpdateMovement();
        }

        private void UpdateBorderEdges()
        {
            borderWallEdges.Clear();
            
            float currentX = transform.position.x;
            float currentY = transform.position.y;
            
            if (Mathf.Abs(currentX - borderWallMinPositions.x) <= borderSnapThreshold)
            {
                if (!intentionallyRemovedEdges.Contains(BorderWallEdge.LEFT))
                {
                    borderWallEdges.Add(BorderWallEdge.LEFT);
                    currentX = borderWallMinPositions.x;
                }
            }
            if (Mathf.Abs(currentX - borderWallMaxPositions.x) <= borderSnapThreshold)
            {
                if (!intentionallyRemovedEdges.Contains(BorderWallEdge.RIGHT))
                {
                    borderWallEdges.Add(BorderWallEdge.RIGHT);
                    currentX = borderWallMaxPositions.x;
                }
            }
            
            if (Mathf.Abs(currentY - borderWallMinPositions.y) <= borderSnapThreshold)
            {
                if (!intentionallyRemovedEdges.Contains(BorderWallEdge.BOTTOM))
                {
                    borderWallEdges.Add(BorderWallEdge.BOTTOM);
                    currentY = borderWallMinPositions.y;
                }
            }
            if (Mathf.Abs(currentY - borderWallMaxPositions.y) <= borderSnapThreshold)
            {
                if (!intentionallyRemovedEdges.Contains(BorderWallEdge.TOP))
                {
                    borderWallEdges.Add(BorderWallEdge.TOP);
                    currentY = borderWallMaxPositions.y;
                }
            }
            
            transform.position = new Vector3(currentX, currentY, zPos);
            
            if (borderWallEdges.Count == 0)
                ForceOntoNearestBorder();
        }

        private void RemoveConflictingBorders(Vector2 movement)
        {
            intentionallyRemovedEdges.Clear();
            
            if (movement.x > 0)
            {
                borderWallEdges.Remove(BorderWallEdge.LEFT);
                intentionallyRemovedEdges.Add(BorderWallEdge.LEFT);
            }
            else if (movement.x < 0)
            {
                borderWallEdges.Remove(BorderWallEdge.RIGHT);
                intentionallyRemovedEdges.Add(BorderWallEdge.RIGHT);
            }
                
            if (movement.y > 0)
            {
                borderWallEdges.Remove(BorderWallEdge.BOTTOM);
                intentionallyRemovedEdges.Add(BorderWallEdge.BOTTOM);
            }
            else if (movement.y < 0)
            {
                borderWallEdges.Remove(BorderWallEdge.TOP);
                intentionallyRemovedEdges.Add(BorderWallEdge.TOP);
            }
        }

        private void ForceOntoNearestBorder()
        {
            float currentX = transform.position.x;
            float currentY = transform.position.y;
            
            float distToLeft = Mathf.Abs(currentX - borderWallMinPositions.x);
            float distToRight = Mathf.Abs(currentX - borderWallMaxPositions.x);
            float distToBottom = Mathf.Abs(currentY - borderWallMinPositions.y);
            float distToTop = Mathf.Abs(currentY - borderWallMaxPositions.y);
            
            float minDist = Mathf.Min(distToLeft, distToRight, distToBottom, distToTop);
            
            intentionallyRemovedEdges.Clear();
            
            if (minDist == distToLeft)
            {
                transform.position = new Vector3(borderWallMinPositions.x, currentY, zPos);
                borderWallEdges.Add(BorderWallEdge.LEFT);
            }
            else if (minDist == distToRight)
            {
                transform.position = new Vector3(borderWallMaxPositions.x, currentY, zPos);
                borderWallEdges.Add(BorderWallEdge.RIGHT);
            }
            else if (minDist == distToBottom)
            {
                transform.position = new Vector3(currentX, borderWallMinPositions.y, zPos);
                borderWallEdges.Add(BorderWallEdge.BOTTOM);
            }
            else
            {
                transform.position = new Vector3(currentX, borderWallMaxPositions.y, zPos);
                borderWallEdges.Add(BorderWallEdge.TOP);
            }
        }

        private void UpdateMovement()
        {
            moveVector = InputManager.Instance.Controls.GAME.Move.ReadValue<Vector2>();
            
            if (moveVector.magnitude < .1f) 
            {
                rb2d.linearVelocity = Vector2.zero;
                return;
            }

            Vector2 inputDirection = moveVector.normalized;
            
            bool canMoveHorizontally = borderWallEdges.Contains(BorderWallEdge.TOP) || borderWallEdges.Contains(BorderWallEdge.BOTTOM);
            bool canMoveVertically = borderWallEdges.Contains(BorderWallEdge.LEFT) || borderWallEdges.Contains(BorderWallEdge.RIGHT);
            
            Vector2 movement = Vector2.zero;
            
            if (canMoveHorizontally && Mathf.Abs(inputDirection.x) > 0)
                movement.x = Mathf.Sign(inputDirection.x);
            
            if (canMoveVertically && Mathf.Abs(inputDirection.y) > 0)
                movement.y = Mathf.Sign(inputDirection.y);
            
            if (movement.x != 0 && movement.y != 0)
            {
                if (Mathf.Abs(inputDirection.x) > Mathf.Abs(inputDirection.y))
                    movement.y = 0;
                else
                    movement.x = 0;
            }
            
            if (movement != Vector2.zero)
            {
                rb2d.linearVelocity = movement.normalized * speed;
                RemoveConflictingBorders(movement);
            }
            else
                rb2d.linearVelocity = Vector2.zero;
        }

        private void FixedUpdate()
        {
            KeepOnBorder();
        }

        private void KeepOnBorder()
        {
            float currentX = transform.position.x;
            float currentY = transform.position.y;
            
            if (borderWallEdges.Contains(BorderWallEdge.LEFT))
                currentX = borderWallMinPositions.x;
            else if (borderWallEdges.Contains(BorderWallEdge.RIGHT))
                currentX = borderWallMaxPositions.x;
                
            if (borderWallEdges.Contains(BorderWallEdge.BOTTOM))
                currentY = borderWallMinPositions.y;
            else if (borderWallEdges.Contains(BorderWallEdge.TOP))
                currentY = borderWallMaxPositions.y;
            
            transform.position = new Vector3(currentX, currentY, zPos);
            
            currentX = Mathf.Clamp(currentX, borderWallMinPositions.x, borderWallMaxPositions.x);
            currentY = Mathf.Clamp(currentY, borderWallMinPositions.y, borderWallMaxPositions.y);
            transform.position = new Vector3(currentX, currentY, zPos);
        }

    }
}