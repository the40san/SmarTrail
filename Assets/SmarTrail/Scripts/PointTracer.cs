using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FortyWorks.SmarTrail
{
    public class PointTracer : IDisposable
    {
        public List<WayPoint> WayPoints;
        
        private readonly float _minVertexDistance;
        private readonly float _lifeTime;
        private WayPoint _lastPointNotedByDistance;
            
        public PointTracer(float minVertexDistance,
            float lifeTime)
        {
            WayPoints = new List<WayPoint>();

            _minVertexDistance = minVertexDistance;
            _lifeTime = lifeTime;
        }
        
        public void Update(Transform transform, bool tracking)
        {
            RemoveEolPoints();

            if (tracking == false) return;
            
            if (_lastPointNotedByDistance == null)
            {
                NoteAsRelay(transform);
                return;
            }
            
            UpdateLastPoint(transform);
            
            var distanceFromLast = Vector3.Distance(transform.position, _lastPointNotedByDistance.MidPosition);
            if (distanceFromLast >= _minVertexDistance)
                NoteAsRelay(transform);
        }

        private void RemoveEolPoints()
        {
            WayPoints.RemoveAll(x => (Time.time - x.CreatedAt) >= _lifeTime);
        }

        private void NoteAsRelay(Transform transform)
        {
            var waypoint = new WayPoint(transform.position, transform.forward, transform.right);
            
            _lastPointNotedByDistance = waypoint;
            WayPoints.Add(waypoint);
        }

        private void UpdateLastPoint(Transform transform)
        {
            if (WayPoints.Count == 0)
                return;
                
            WayPoints[WayPoints.Count - 1] = new WayPoint(transform.position, transform.forward, transform.right);
        }

        public void Dispose()
        {
            WayPoints.Clear();
        }
    }
}