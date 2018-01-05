using UnityEngine;

namespace FortyWorks.SmarTrail
{
	public partial class SmarTrailRenderer : MonoBehaviour
	{
		[SerializeField] private Material[] _materials = { };
		[SerializeField] private float _time = 5f;
		[SerializeField] private AnimationCurve _width;
		[SerializeField] private float _minVertexDistance = 0.1f; 
		[SerializeField] private Gradient _color;

		private PointTracer CreatePointTracer()
		{
			return new PointTracer(_minVertexDistance, _time);
		}

		private MeshBaker CreatemMeshBaker()
		{
			return new MeshBaker(_width, _time, _color);
		}
	}
	
	public partial class SmarTrailRenderer : MonoBehaviour
	{
		private MeshBaker _baker;
		private PointTracer _pointTracer;

		private MeshFilter _meshFilter;
		private MeshRenderer _meshRenderer;
			
		public void Start()
		{
			_baker = CreatemMeshBaker();
			_pointTracer = CreatePointTracer();
			
			SetupComponents();

			_meshFilter.mesh = _baker.Mesh;
			_meshRenderer.materials = _materials;
		}

		private void SetupComponents()
		{
			GameObject go = new GameObject();
			_meshFilter = go.AddComponent<MeshFilter>();
			_meshRenderer = go.AddComponent<MeshRenderer>();
			go.transform.SetParent(transform, false);
		}

		public void Update()
		{
			_pointTracer.Update(transform);
			_baker.Bake(_pointTracer.WayPoints);
		}
	}
}
