using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        [SerializeField] private Gradient flashGradient;
        
        [SerializeField] private float fixedZPosition = 1.5f;
        
        [Header("boundaries")]
        [SerializeField] private Vector2 fieldMinBounds = new Vector2(-5, -5);
        [SerializeField] private Vector2 fieldMaxBounds = new Vector2(5, 5);
        [SerializeField] private float boundaryOffset = 0.1f;
        
        private Vector2 currentDirection;
        private float currentSpeed;
        private float currentLength = 0f;
        private int currentBounceCount = 0;
        private int currentPosition => lineRenderer.positionCount - 1;
        private Vector3 currentPositionPosition => lineRenderer.GetPosition(currentPosition);
        
        private List<int> bouncePointIndices = new List<int>();
        private Coroutine flashCoroutine;
        
        public bool isWebLocked {get; private set;}
        public bool isShooting {get; private set;}
        public bool isCleaningUp {get; private set;}
        public event Action OnWallCollision = null;
        public event Action OnWebCleanupComplete = null;

        public void ShootWeb(Vector3 start, Vector2 direction, float speed = 2)
        {
            Vector3 adjustedStart = new Vector3(start.x, start.y, fixedZPosition);
            
            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, adjustedStart);
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(1, adjustedStart);
            lineRenderer.widthCurve = new AnimationCurve(new Keyframe(0, thickness));
            lineRenderer.useWorldSpace = true;
            
            lineRenderer.colorGradient = new Gradient();
            
            bouncePointIndices.Clear();
            
            currentDirection = direction.normalized;
            currentSpeed = speed;
            currentLength = 0f;
            currentBounceCount = 0;

            isShooting = true;
            isCleaningUp = false;
            isWebLocked = false;
            
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
                flashCoroutine = null;
            }
            
            UpdatePolygonCollider();
        }

        private void Update()
        {
            if (isShooting && !isCleaningUp)
            {
                float moveDistance = Time.deltaTime * currentSpeed;
                Vector3 newPos = currentPositionPosition;
                
                while (moveDistance > 0)
                {
                    float distanceToNextBoundary = GetDistanceToNextBoundary(newPos, currentDirection);
                    float stepDistance = Mathf.Min(moveDistance, distanceToNextBoundary);
                    
                    newPos += (Vector3)currentDirection * stepDistance;
                    newPos.z = fixedZPosition;
                    moveDistance -= stepDistance;
                    currentLength += stepDistance;
                    
                    if (distanceToNextBoundary <= stepDistance + 0.001f && moveDistance > 0)
                    {
                        currentBounceCount++;
                        bouncePointIndices.Add(currentPosition - 1);

                        if (maxBounces > 0 && currentBounceCount >= maxBounces)
                        {
                            LockWeb();
                            return;
                        }
                        
                        currentDirection = CalculateBoundaryRicochet(newPos, currentDirection);
                        OnWallCollision?.Invoke();
                        
                        lineRenderer.positionCount++;
                        lineRenderer.SetPosition(currentPosition - 1, newPos);
                    }
                }
                
                lineRenderer.SetPosition(currentPosition, newPos);
                
                if (maxLength > 0 && currentLength >= maxLength)
                {
                    StartCleanupProcess();
                    return;
                }
                
                UpdatePolygonCollider();
            }
        }

        private void StartCleanupProcess()
        {
            if (isCleaningUp) return;
            
            isShooting = false;
            isCleaningUp = true;
            
            flashCoroutine = StartCoroutine(CleanupHangingWeb());
        }

        private IEnumerator CleanupHangingWeb()
        {
            int lastBounceIndex = bouncePointIndices.Count;
            int totalPoints = lineRenderer.positionCount;
            
            if (totalPoints > lastBounceIndex + 1)
            {
                Gradient originalGradient = lineRenderer.colorGradient;
                Sequence flashSequence = DOTween.Sequence();
                
                for (int i = 0; i < 3; i++)
                {
                    flashSequence.Append(
                        DOTween.To(() => 1f, alpha => UpdateSegmentAlpha(lastBounceIndex + 1, totalPoints - 1, alpha), 0f, 0.1f));
                    flashSequence.Append(
                        DOTween.To(() => 0f, alpha => UpdateSegmentAlpha(lastBounceIndex + 1, totalPoints - 1, alpha), 1f, 0.1f));
                }
                
                flashSequence.OnComplete(() => { lineRenderer.colorGradient = originalGradient;});
                flashSequence.Play();
                yield return flashSequence.WaitForCompletion();
            }
            else
                yield return null;
            
            RemoveHangingWebPositions(lastBounceIndex);
            UpdatePolygonCollider();
            
            isWebLocked = true;
            isCleaningUp = false;
            
            OnWebCleanupComplete?.Invoke();
        }

        private void UpdateSegmentAlpha(int startIndex, int endIndex, float alpha)
        {
            float normalizedStart = (float) startIndex / (lineRenderer.positionCount - 1);
            float normalizedEnd = (float) endIndex / (lineRenderer.positionCount - 1);
            
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[3];
            alphaKeys[0] = new GradientAlphaKey(1f, 0f);
            alphaKeys[1] = new GradientAlphaKey(1f, Mathf.Clamp01(normalizedStart - 0.01f));
            alphaKeys[2] = new GradientAlphaKey(alpha, Mathf.Clamp01(normalizedEnd + 0.01f));
            
            GradientColorKey[] colorKeys = new GradientColorKey[2];
            colorKeys[0] = new GradientColorKey(lineRenderer.startColor, 0f);
            colorKeys[1] = new GradientColorKey(lineRenderer.endColor, 1f);
            
            Gradient fadeGradient = new Gradient();
            fadeGradient.SetKeys(colorKeys, alphaKeys);
            lineRenderer.colorGradient = fadeGradient;
        }

        private void RemoveHangingWebPositions(int lastValidIndex)
        {
            if (lastValidIndex + 1 < lineRenderer.positionCount)
                lineRenderer.positionCount = lastValidIndex + 1;
            currentLength = CalculateLineLength();
        }

        private float CalculateLineLength()
        {
            if (lineRenderer.positionCount < 2) return 0f;
            
            float length = 0f;
            for (int i = 1; i < lineRenderer.positionCount; i++)
            {
                length += Vector3.Distance(
                    lineRenderer.GetPosition(i - 1), 
                    lineRenderer.GetPosition(i)
                );
            }
            return length;
        }

        private float GetDistanceToNextBoundary(Vector3 currentPos, Vector2 direction)
        {
            if (direction == Vector2.zero) return Mathf.Infinity;
            
            float distance = Mathf.Infinity;
            
            if (direction.x > 0)
            {
                float distToRight = (fieldMaxBounds.x - boundaryOffset - currentPos.x) / direction.x;
                if (distToRight > 0 && distToRight < distance) distance = distToRight;
            }
            else if (direction.x < 0)
            {
                float distToLeft = (fieldMinBounds.x + boundaryOffset - currentPos.x) / direction.x;
                if (distToLeft > 0 && distToLeft < distance) distance = distToLeft;
            }
            
            if (direction.y > 0)
            {
                float distToTop = (fieldMaxBounds.y - boundaryOffset - currentPos.y) / direction.y;
                if (distToTop > 0 && distToTop < distance) distance = distToTop;
            }
            else if (direction.y < 0)
            {
                float distToBottom = (fieldMinBounds.y + boundaryOffset - currentPos.y) / direction.y;
                if (distToBottom > 0 && distToBottom < distance) distance = distToBottom;
            }
            
            return distance;
        }

        private Vector2 CalculateBoundaryRicochet(Vector3 position, Vector2 direction)
        {
            bool hitLeft = Mathf.Abs(position.x - (fieldMinBounds.x + boundaryOffset)) < 0.01f && direction.x < 0;
            bool hitRight = Mathf.Abs(position.x - (fieldMaxBounds.x - boundaryOffset)) < 0.01f && direction.x > 0;
            bool hitBottom = Mathf.Abs(position.y - (fieldMinBounds.y + boundaryOffset)) < 0.01f && direction.y < 0;
            bool hitTop = Mathf.Abs(position.y - (fieldMaxBounds.y - boundaryOffset)) < 0.01f && direction.y > 0;
            
            Vector2 newDirection = direction;
            
            if (hitLeft || hitRight)
                newDirection.x = -direction.x;
            if (hitBottom || hitTop)
                newDirection.y = -direction.y;
            
            if (newDirection.magnitude > 0)
                newDirection.Normalize();
            
            return newDirection;
        }

        private void UpdatePolygonCollider()
        {
            if (lineRenderer.positionCount < 2) 
            {
                polyCollider.pathCount = 0;
                return;
            }

            int pointCount = lineRenderer.positionCount;
            int polygonPointCount = pointCount * 2;
            
            List<Vector2> polygonPoints = new List<Vector2>(polygonPointCount);
            
            for (int i = 0; i < pointCount; i++)
            {
                Vector3 point3D = lineRenderer.GetPosition(i);
                Vector2 point = new Vector2(point3D.x, point3D.y);
                Vector2 perpendicular = GetPerpendicularAtPoint(i, pointCount);
                polygonPoints.Add(new Vector2(point.x + perpendicular.x, point.y + perpendicular.y));
            }
            
            for (int i = pointCount - 1; i >= 0; i--)
            {
                Vector3 point3D = lineRenderer.GetPosition(i);
                Vector2 point = new Vector2(point3D.x, point3D.y);
                Vector2 perpendicular = GetPerpendicularAtPoint(i, pointCount);
                polygonPoints.Add(new Vector2(point.x - perpendicular.x, point.y - perpendicular.y));
            }
            
            polyCollider.SetPath(0, polygonPoints.ToArray());
            polyCollider.enabled = true;
        }

        private Vector2 GetPerpendicularAtPoint(int index, int pointCount)
        {
            Vector3 point3D = lineRenderer.GetPosition(index);
            Vector2 point = new Vector2(point3D.x, point3D.y);
            
            if (pointCount > 1)
            {
                if (index == 0)
                {
                    Vector3 nextPoint3D = lineRenderer.GetPosition(1);
                    Vector2 nextPoint = new Vector2(nextPoint3D.x, nextPoint3D.y);
                    Vector2 direction = (nextPoint - point).normalized;
                    return new Vector2(-direction.y, direction.x) * (thickness * 0.5f);
                }
                else if (index == pointCount - 1)
                {
                    Vector3 prevPoint3D = lineRenderer.GetPosition(index - 1);
                    Vector2 prevPoint = new Vector2(prevPoint3D.x, prevPoint3D.y);
                    Vector2 direction = (point - prevPoint).normalized;
                    return new Vector2(-direction.y, direction.x) * (thickness * 0.5f);
                }
                else if (pointCount > 2)
                {
                    Vector3 prevPoint3D = lineRenderer.GetPosition(index - 1);
                    Vector3 nextPoint3D = lineRenderer.GetPosition(index + 1);
                    Vector2 prevPoint = new Vector2(prevPoint3D.x, prevPoint3D.y);
                    Vector2 nextPoint = new Vector2(nextPoint3D.x, nextPoint3D.y);
                    Vector2 dir1 = (point - prevPoint).normalized;
                    Vector2 dir2 = (nextPoint - point).normalized;
                    Vector2 averageDir = (dir1 + dir2).normalized;
                    return new Vector2(-averageDir.y, averageDir.x) * (thickness * 0.5f);
                }
            }
            
            return new Vector2(-currentDirection.y, currentDirection.x) * (thickness * 0.5f);
        }

        public void LockWeb()
        {
            if (isCleaningUp) return;
            
            isWebLocked = true;
            isShooting = false;
            UpdatePolygonCollider();
        }
    }
}