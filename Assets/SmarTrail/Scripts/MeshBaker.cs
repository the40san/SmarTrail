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
        private readonly int _subdivision;

        public Mesh Mesh { get; private set; }

        public MeshBaker(
            AnimationCurve widthCurve, 
            float widthMultiplier, 
            Gradient colorGradient, 
            Align align, 
            int subdivision)
        {
            _widthCurve = widthCurve;
            _widthMultiplier = widthMultiplier;
            _colorGradient = colorGradient;
            _align = align;
            _subdivision = subdivision;
            
            Mesh = new Mesh();
        }

        public void Bake(WayPoint[] wayPoints, Transform transform)
        {
            Mesh.Clear();
            if (wayPoints.Length < 2) return;

            var offsetToCancel = transform.position;
            var rotationToCancel = transform.rotation;

            var vertices = CreateVertice(wayPoints, offsetToCancel).Select(x => Quaternion.Inverse(rotationToCancel) * x.Point).ToArray();
            var uv = CreateUvMap(vertices.Length / 2).ToArray();
            var triangles = CreateTriangles(vertices.Length / 2).ToArray();
            var colors = CreateVertexColor(vertices.Length / 2).ToArray();

            Mesh.vertices = vertices;
            Mesh.uv = uv;
            Mesh.triangles = triangles;
            Mesh.colors = colors;
        }

        private IEnumerable<Color> CreateVertexColor(int pointCount)
        {
            return Enumerable.Range(0, pointCount).SelectMany(index =>
            {
                var color = _colorGradient.Evaluate(1 - ((float)index / pointCount));
                    
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

        private IEnumerable<Vertex> CreateVertice(WayPoint[] wayPoints, Vector3 offset)
        {
            var maxWidth = _widthCurve.Evaluate(0) * _widthMultiplier;
            var subdivided = CreateSubdividedWayPoints(wayPoints);

            return Enumerable.Range(0, subdivided.Length).SelectMany(index =>
            {
                var element = subdivided.ElementAt(index);
                var width = _widthCurve.Evaluate(1 - ((float) index / subdivided.Length)) * _widthMultiplier;

                return new[]
                {
                    new Vertex(element.ToBasePosition(width, maxWidth, _align) - offset),
                    new Vertex(element.ToForwardPosition(width, maxWidth, _align) - offset),
                };
            });
        }

        private WayPoint[] CreateSubdividedWayPoints(WayPoint[] baseWayPoints)
        {
            //CatMull-Rom needs at least 4 sample points.
            if (baseWayPoints.Length < 4)
                return baseWayPoints;

            var subdivider = new CatmullRomDivider(
                baseWayPoints.ToArray(),
                _subdivision
            );

            return subdivider.SubDivide();
        }

        public void Dispose()
        {
            Mesh.Clear();
        }
    }
}