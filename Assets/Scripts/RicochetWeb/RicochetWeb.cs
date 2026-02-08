using System;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.ArcadeLibrary.RicochetWeb
{
    public class RicochetWeb : MonoBehaviour
    {
        [Header("line")]
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private float thickness = 0.1f;
        [SerializeField] private PolygonCollider2D polyCollider;

        [Header("shooting")]
        [SerializeField] private int maxBounces = -1;
        [SerializeField] private float maxLength = -1;
        
        [SerializeField] private float fixedZPosition = 1.5f;
        
        private Vector2 currentDirection;
        private float currentSpeed;
        private int currentPosition => lineRenderer.positionCount - 1;
        private Vector3 currentPositionPosition => lineRenderer.GetPosition(currentPosition);
        public bool isWebLocked {get; private set;}
        public bool isShooting {get; private set;}
        public event Action OnWallCollision = null;

        public void ShootWeb(Vector3 start, Vector2 direction, float speed = 2)
        {
            Vector3 adjustedStart = new Vector3(start.x, start.y, fixedZPosition);
            
            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, adjustedStart);
            lineRenderer.positionCount++;
            lineRenderer.widthCurve = new AnimationCurve(new Keyframe(0, thickness));
            lineRenderer.useWorldSpace = true;

            currentDirection = direction.normalized;
            currentSpeed = speed;

            isShooting = true;
            UpdatePolygonCollider();
        }

        private void Update()
        {
            if (isShooting)
            {
                Vector3 newPos = currentPositionPosition + ((Vector3)currentDirection * Time.deltaTime * currentSpeed);
                newPos.z = fixedZPosition;
                
                lineRenderer.SetPosition(currentPosition, newPos);
                UpdatePolygonCollider();
            }
        }

        private void UpdatePolygonCollider()
        {
            if (lineRenderer.positionCount < 2) return;

            int pointCount = lineRenderer.positionCount;
            int polygonPointCount = pointCount * 2;
            
            List<Vector2> polygonPoints = new List<Vector2>(polygonPointCount);
            
            for (int i = 0; i < pointCount; i++)
            {
                Vector3 point3D = lineRenderer.GetPosition(i);
                Vector2 point = new Vector2(point3D.x, point3D.y);
                Vector2 perpendicular;
                
                if (i == 0 && pointCount > 1)
                {
                    Vector3 nextPoint3D = lineRenderer.GetPosition(1);
                    Vector2 nextPoint = new Vector2(nextPoint3D.x, nextPoint3D.y);
                    Vector2 direction = (nextPoint - point).normalized;
                    perpendicular = new Vector2(-direction.y, direction.x) * (thickness * 0.5f);
                }
                else if (i == pointCount - 1 && pointCount > 1)
                {
                    Vector3 prevPoint3D = lineRenderer.GetPosition(i - 1);
                    Vector2 prevPoint = new Vector2(prevPoint3D.x, prevPoint3D.y);
                    Vector2 direction = (point - prevPoint).normalized;
                    perpendicular = new Vector2(-direction.y, direction.x) * (thickness * 0.5f);
                }
                else if (pointCount > 2 && i > 0 && i < pointCount - 1)
                {
                    Vector3 prevPoint3D = lineRenderer.GetPosition(i - 1);
                    Vector3 nextPoint3D = lineRenderer.GetPosition(i + 1);
                    Vector2 prevPoint = new Vector2(prevPoint3D.x, prevPoint3D.y);
                    Vector2 nextPoint = new Vector2(nextPoint3D.x, nextPoint3D.y);
                    Vector2 dir1 = (point - prevPoint).normalized;
                    Vector2 dir2 = (nextPoint - point).normalized;
                    Vector2 averageDir = (dir1 + dir2).normalized;
                    perpendicular = new Vector2(-averageDir.y, averageDir.x) * (thickness * 0.5f);
                }
                else
                {
                    perpendicular = new Vector2(-currentDirection.y, currentDirection.x) * (thickness * 0.5f);
                }
                
                polygonPoints.Add(new Vector2(point.x + perpendicular.x, point.y + perpendicular.y));
            }
            
            for (int i = pointCount - 1; i >= 0; i--)
            {
                Vector3 point3D = lineRenderer.GetPosition(i);
                Vector2 point = new Vector2(point3D.x, point3D.y);
                Vector2 perpendicular;
                
                if (i == 0 && pointCount > 1)
                {
                    Vector3 nextPoint3D = lineRenderer.GetPosition(1);
                    Vector2 nextPoint = new Vector2(nextPoint3D.x, nextPoint3D.y);
                    Vector2 direction = (nextPoint - point).normalized;
                    perpendicular = new Vector2(-direction.y, direction.x) * (thickness * 0.5f);
                }
                else if (i == pointCount - 1 && pointCount > 1)
                {
                    Vector3 prevPoint3D = lineRenderer.GetPosition(i - 1);
                    Vector2 prevPoint = new Vector2(prevPoint3D.x, prevPoint3D.y);
                    Vector2 direction = (point - prevPoint).normalized;
                    perpendicular = new Vector2(-direction.y, direction.x) * (thickness * 0.5f);
                }
                else if (pointCount > 2 && i > 0 && i < pointCount - 1)
                {
                    Vector3 prevPoint3D = lineRenderer.GetPosition(i - 1);
                    Vector3 nextPoint3D = lineRenderer.GetPosition(i + 1);
                    Vector2 prevPoint = new Vector2(prevPoint3D.x, prevPoint3D.y);
                    Vector2 nextPoint = new Vector2(nextPoint3D.x, nextPoint3D.y);
                    Vector2 dir1 = (point - prevPoint).normalized;
                    Vector2 dir2 = (nextPoint - point).normalized;
                    Vector2 averageDir = (dir1 + dir2).normalized;
                    perpendicular = new Vector2(-averageDir.y, averageDir.x) * (thickness * 0.5f);
                }
                else
                {
                    perpendicular = new Vector2(-currentDirection.y, currentDirection.x) * (thickness * 0.5f);
                }
                
                polygonPoints.Add(new Vector2(point.x - perpendicular.x, point.y - perpendicular.y));
            }
            
            polyCollider.SetPath(0, polygonPoints.ToArray());
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.collider.gameObject.CompareTag("Wall"))
            {
                currentDirection = CalculateRicochetTrajectory(collision);
                lineRenderer.positionCount++;
                
                Vector3 lastPoint = lineRenderer.GetPosition(currentPosition - 1);
                lastPoint.z = fixedZPosition;
                lineRenderer.SetPosition(currentPosition - 1, lastPoint);
                
                OnWallCollision?.Invoke();
            }
        }

        private Vector2 CalculateRicochetTrajectory(Collision2D collision)
        {
            if (collision.contactCount > 0)
            {
                Vector2 normal = collision.GetContact(0).normal;
                
                Vector2 reflected = Vector2.Reflect(currentDirection, normal).normalized;
                return reflected;
            }
            
            throw new Exception("unable to calculate ricochet trajectory");
        }

        public void LockWeb()
        {
            isWebLocked = true;
            isShooting = false;
            UpdatePolygonCollider();
        }
    }
}