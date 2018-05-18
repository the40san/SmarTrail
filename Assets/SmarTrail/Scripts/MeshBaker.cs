using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FortyWorks.SmarTrail
{
    public class MeshBaker : IDisposable
    {
        private readonly AnimationCurve _widthCurve;
        private readonly float _widthMultiplier;
        private readonly Gradient _colorGradient;
        private readonly Align _align;

        public Mesh Mesh { get; private set; }

        public MeshBaker(AnimationCurve widthCurve, float widthMultiplier, Gradient colorGradient, Align align)
        {
            _widthCurve = widthCurve;
            _widthMultiplier = widthMultiplier;
            _colorGradient = colorGradient;
            _align = align;
            
            Mesh = new Mesh();
        }

        public void Bake(List<WayPoint> wayPoints, Vector3 offset)
        {
            Mesh.Clear();
            if (wayPoints.Count < 2) return;
            
            var vertices = CreateVertice(wayPoints, offset).Select(x => x.Point).ToArray();
            var uv = CreateUvMap(wayPoints.Count).ToArray();
            var triangles = CreateTriangles(wayPoints.Count).ToArray();
            var colors = CreateVertexColor(wayPoints).ToArray();
                
            Mesh.vertices = vertices;
            Mesh.uv = uv;
            Mesh.triangles = triangles;
            Mesh.colors = colors;
        }

        private IEnumerable<Color> CreateVertexColor(List<WayPoint> wayPoints)
        {
            return Enumerable.Range(0, wayPoints.Count).SelectMany(index =>
            {
                var color = _colorGradient.Evaluate(1 - ((float)index / wayPoints.Count));
                    
                return new[]
                {
                    color,
                    color 
                };
            });
        }
        
        private IEnumerable<int> CreateTriangles(int pointCount)
        {
            return Enumerable.Range(0, pointCount - 1).SelectMany(index => new int[]
            {
                index * 2,
                index * 2 + 1,
                index * 2 + 2,
                index * 2 + 3,
                index * 2 + 2,
                index * 2 + 1
            });
        }

        private IEnumerable<Vector2> CreateUvMap(int pointCount)
        {
            return Enumerable.Range(0, pointCount).SelectMany(index => new Vector2[]
            {
                new Vector2((float) index / pointCount, 0),
                new Vector2((float) index / pointCount, 1)
            });
        }

        private IEnumerable<Vertex> CreateVertice(List<WayPoint> wayPoints, Vector3 offset)
        {
            var maxWidth = _widthCurve.Evaluate(0) * _widthMultiplier;
            return Enumerable.Range(0, wayPoints.Count).SelectMany(index =>
            {
                var element = wayPoints.ElementAt(index);
                var width = _widthCurve.Evaluate(1 - ((float)index / wayPoints.Count)) * _widthMultiplier;

                return new[]
                {
                    new Vertex(element.ToBasePosition(width, maxWidth, _align) - offset),
                    new Vertex(element.ToForwardPosition(width, maxWidth, _align) - offset),
                };
            });
        }

        public void Dispose()
        {
            Mesh.Clear();
        }
    }
}