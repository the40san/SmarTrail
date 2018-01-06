using UnityEngine;

namespace FortyWorks.SmarTrail
{
    public class Vertex
    {
        public Vector3 Point { get; private set; }
        
        public Vertex(Vector3 point)
        {
            Point = point;
        }
    }
}