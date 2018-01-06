using UnityEngine;

namespace FortyWorks.SmarTrail
{
    public class WayPoint
    {
        public Vector3 MidPosition { get; private set; }
        public Vector3 Forward { get; private set; }
        
        public float CreatedAt { get; private set; }

        public WayPoint(Vector3 midPosition, Vector3 forward)
        {
            MidPosition = midPosition;
            Forward = forward;
            CreatedAt = Time.time;
        }

        public Vector3 ToForwardPosition(float width)
        {
            return MidPosition + Forward * (width / 2f);
        }

        public Vector3 ToBasePosition(float width)
        {
            return MidPosition - Forward * (width / 2f);
        }
    }
}