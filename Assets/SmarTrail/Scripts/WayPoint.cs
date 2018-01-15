using System;
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

        public Vector3 ToForwardPosition(float width, float maxWidth, Align align)
        {
            switch (align)
            {
                case Align.Base:
                    return MidPosition + Forward * (-maxWidth / 2f + width);
                    
                case Align.Center:
                    return MidPosition + Forward * (width / 2);
                    
                case Align.Forward:
                    return MidPosition + Forward * (maxWidth / 2f);
            }
            
            throw new ArgumentException("should not reach here");
        }

        public Vector3 ToBasePosition(float width, float maxWidth, Align align)
        {
            switch (align)
            {
                case Align.Forward:
                    return MidPosition + Forward * (maxWidth / 2f - width); 
                    
                case Align.Center:
                    return MidPosition - Forward * (width / 2f);
                    
                case Align.Base:
                    return MidPosition - Forward * (maxWidth / 2f);
            }
            
            throw new ArgumentException("should not reach here");
        }
    }
}