using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsFrameRenderer : MonoBehaviour
{
	[SerializeField] Transform m_Target;

	public struct MeshToDraw
	{
		public MeshToDraw(Mesh a_Mesh, Transform a_Trans, Material a_Material)
		{
			m_Mesh = a_Mesh;
			m_Trans = a_Trans;
			m_Material = a_Material;
		}
		public Mesh m_Mesh;
		public Transform m_Trans;
		public Material m_Material;
	}
	List<MeshToDraw> m_MeshToDraw = new List<MeshToDraw>();
	List<List<Matrix4x4>> m_PhysicsFrameMtxList = new List<List<Matrix4x4>>();
	private void Awake()
	{
		foreach (Transform l_Trans in m_Target)
		{
			var l_MeshFilter = l_Trans.GetComponent<MeshFilter>();
			if (l_MeshFilter)
			{
				var l_MeshRend = l_Trans.GetComponent<MeshRenderer>();
				m_MeshToDraw.Add(new MeshToDraw(l_MeshFilter.sharedMesh, l_Trans, l_MeshRend.sharedMaterial));
				m_PhysicsFrameMtxList.Add(new List<Matrix4x4>());
				l_MeshRend.enabled = false;
			}
		}
	}
	private void FixedUpdate()
	{
		int l_Index = 0;
		foreach (var l_M2D in m_MeshToDraw)
			m_PhysicsFrameMtxList[l_Index++].Add(l_M2D.m_Trans.localToWorldMatrix);
	}
	private void OnPostRender()
	{
		int l_ObjIndex = 0;
		foreach (var l_M2D in m_MeshToDraw)
		{
			var l_PhxMtx = m_PhysicsFrameMtxList[l_ObjIndex++];
			foreach (var l_Mtx in l_PhxMtx)
			{
				l_M2D.m_Material.SetPass(0);
				Graphics.DrawMeshNow(l_M2D.m_Mesh, l_Mtx);
			}
			l_PhxMtx.Clear();
		}
	}
}
