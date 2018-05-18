﻿using UnityEngine;

namespace FortyWorks.SmarTrail
{
	public partial class SmarTrailRenderer : MonoBehaviour
	{
		[SerializeField] private Material[] _materials = { };
		[SerializeField] private float _time = 5f;
		[SerializeField] private AnimationCurve _width;
		[SerializeField] private float _minVertexDistance = 0.1f; 
		[SerializeField] private Gradient _color;
		[SerializeField] private Align _align = Align.Forward;
		[SerializeField] private bool _tracking = true;

		private PointTracer CreatePointTracer()
		{
			return new PointTracer(_minVertexDistance, _time);
		}

		private MeshBaker CreateMeshBaker()
		{
			return new MeshBaker(_width, _time, _color, _align);
		}
	}
	
	[ExecuteInEditMode]
	public partial class SmarTrailRenderer : MonoBehaviour
	{
		private MeshBaker _baker;
		private PointTracer _pointTracer;

		private MeshFilter _meshFilter;
		private MeshRenderer _meshRenderer;

		private void OnEnable()
		{
			SetupComponents();
   		}

		private void SetupComponents()
		{
			if (_baker == null)
                _baker = CreateMeshBaker();
			
			if (_pointTracer == null)
                _pointTracer = CreatePointTracer();
	
			_meshFilter = GetComponent<MeshFilter>();
			if (_meshFilter == null)
				gameObject.AddComponent<MeshFilter>();

			_meshRenderer = GetComponent<MeshRenderer>();
			if (_meshRenderer == null)
				gameObject.AddComponent<MeshRenderer>();
			
			_meshFilter.mesh = _baker.Mesh;
			_meshRenderer.materials = _materials;
		}

		private void OnDisable()
		{
			_baker.Mesh.Clear();
			_pointTracer.WayPoints.Clear();
		}

		public void SetTracking(bool value)
		{
			_tracking = value;
		}

		public void Update()
		{
			if (_baker == null || _pointTracer == null)
				SetupComponents();
			
            _pointTracer.Update(transform, _tracking);
			_baker.Bake(_pointTracer.WayPoints, transform.position);
		}
	}
}
