using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionBlur : MonoBehaviour 
{
	[SerializeField] Transform m_Target;
	[SerializeField] int m_MaxDrawCount = 5;

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
	private void Update()
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
			if (l_PhxMtx.Count > m_MaxDrawCount)
				//l_PhxMtx.RemoveRange(0, l_PhxMtx.Count - m_MaxDrawCount);
				while (l_PhxMtx.Count > m_MaxDrawCount)
					l_PhxMtx.RemoveAt(0);
			float l_AlphaDT = 1f / l_PhxMtx.Count;
			float l_Alpha = 0f;
			//foreach (var l_Mtx in l_PhxMtx)
			for (int i = 0; i < l_PhxMtx.Count; i++)
			{
				var l_Mtx = l_PhxMtx[i];
				l_M2D.m_Material.SetPass(0);
				var c = l_M2D.m_Material.color;
				c.a = Mathf.Lerp(0, 1, (l_Alpha += l_AlphaDT));
				l_M2D.m_Material.color = c;
				Graphics.DrawMeshNow(l_M2D.m_Mesh, l_Mtx);
			}
		}
	}
}
