using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FortyWorks.SmarTrail
{
    public class PointTracer
    {
        public List<WayPoint> WayPoints;
        
        private readonly float _minVertexDistance;
        private readonly float _lifeTime;
            
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
            
            var lastNotedPoint = WayPoints.LastOrDefault();
            if (lastNotedPoint == null)
            {
                Note(transform);
                return;
            }
            
            var distanceFromLast = Vector3.Distance(transform.position, lastNotedPoint.MidPosition);
            if (distanceFromLast >= _minVertexDistance)
                Note(transform);
        }

        private void RemoveEolPoints()
        {
            WayPoints.RemoveAll(x => (Time.time - x.CreatedAt) >= _lifeTime);
        }

        private void Note(Transform transform)
        {
            WayPoints.Add(new WayPoint(transform.position, transform.forward));
        }
    }
}