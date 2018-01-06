﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FortyWorks.SmarTrail
{
    public class MeshBaker
    {
        private readonly AnimationCurve _widthCurve;
        private readonly float _lifeTime;
        private readonly Gradient _colorGradient;

        public Mesh Mesh { get; private set; }

        public MeshBaker(AnimationCurve widthCurve, float lifeTime, Gradient colorGradient)
        {
            _widthCurve = widthCurve;
            _lifeTime = lifeTime;
            _colorGradient = colorGradient;
            Mesh = new Mesh();
        }

        public void Bake(List<WayPoint> wayPoints, Vector3 currentPosition)
        {
            Mesh.Clear();
            if (wayPoints.Count < 2) return;
            
            var vertices = CreateVertice(wayPoints, currentPosition).Select(x => x.Point).ToArray();
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
                var element = wayPoints.ElementAt(index);
                var lerp = ClampLifeTime(element);
                var color = _colorGradient.Evaluate(lerp);
                    
                return new[]
                {
                    color,
                    color 
                };
            });
        }

        private float ClampLifeTime(WayPoint wayPoint)
        {
            return Mathf.Clamp(
                (Time.time - wayPoint.CreatedAt) / _lifeTime,
                0f, 
                1f
            );
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

        private IEnumerable<Vertex> CreateVertice(List<WayPoint> wayPoints, Vector3 currentPosition)
        {
            return wayPoints.SelectMany(element =>
            {
                var lerp = ClampLifeTime(element);
                var width = _widthCurve.Evaluate(lerp);

                return new[]
                {
                    new Vertex(element.ToBasePosition(width)),
                    new Vertex(element.ToForwardPosition(width)),
                };
            });
        }
    }
}