using UnityEngine;

namespace FortyWorks.SmarTrail
{
	public partial class SmarTrailRenderer : MonoBehaviour
	{
		[SerializeField] private Material[] _materials = { };
		[SerializeField] private float _time = 5f;
		[SerializeField] private AnimationCurve _widthCurve = AnimationCurve.Linear(0, 0, 1, 1);
		[SerializeField, Range(0, 1)] private float _widthMultiplier = 1.0f;
		[SerializeField] private float _minVertexDistance = 0.1f; 
		[SerializeField] private Gradient _color = new Gradient();
		[SerializeField] private Align _align = Align.Forward;
		[SerializeField] private bool _tracking = true;

		private PointTracer CreatePointTracer()
		{
			return new PointTracer(_minVertexDistance, _time);
		}

		private MeshBaker CreateMeshBaker()
		{
			return new MeshBaker(_widthCurve, _widthMultiplier, _color, _align);
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
				_meshFilter = gameObject.AddComponent<MeshFilter>();

			_meshRenderer = GetComponent<MeshRenderer>();
			if (_meshRenderer == null)
				_meshRenderer = gameObject.AddComponent<MeshRenderer>();
			
			_meshFilter.mesh = _baker.Mesh;
			_meshRenderer.materials = _materials;
		}

		private void OnDisable()
		{
			Clean();
    	}

		private void OnValidate()
		{
			Clean();
		}

		public void Clean()
		{
			_baker?.Dispose();
			_baker = null;
			_pointTracer?.Dispose();
			_pointTracer = null;
		}

		public void SetTracking(bool value)
		{
			_tracking = value;
		}

		private void Update()
		{
			if (_baker == null || _pointTracer == null)
				SetupComponents();
			
            _pointTracer.Update(transform, _tracking);
			_baker.Bake(_pointTracer.WayPoints, transform.position);
		}
	}
}
