using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionBlending : MonoBehaviour 
{
	[SerializeField] Transform m_Target;
	[SerializeField] int m_DrawCount = 5;
	[SerializeField] float m_MotionDelta = 1f / 300f;
	[SerializeField] AnimationCurve m_TrailAlphaCurve;
	[SerializeField] Rigidbody m_TargetRB;

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
		m_TargetRB = m_Target.GetComponent<Rigidbody>();
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
	private void OnPostRender()
	{
		foreach (var l_M2D in m_MeshToDraw)
		{
			var l_OAng = m_TargetRB.angularVelocity * Mathf.Rad2Deg * m_MotionDelta * m_DrawCount;
			var l_fRot = l_M2D.m_Trans.eulerAngles;
			var l_iRot = l_fRot - l_OAng;

			float l_Ratio = 0f;
			float l_RatioDT = 1f / m_DrawCount;
			for (int i = 0; i < m_DrawCount; i++)
			{
				l_Ratio += l_RatioDT;
				var c = l_M2D.m_Material.color;
				c.a = m_TrailAlphaCurve.Evaluate(l_Ratio);
				l_M2D.m_Material.color = c;

				var l_Rot = l_fRot;
				l_Rot.y = Mathf.Lerp(l_iRot.y, l_fRot.y, l_Ratio);
				var l_Mtx = Matrix4x4.TRS(l_M2D.m_Trans.position, Quaternion.Euler(l_Rot), l_M2D.m_Trans.lossyScale);
				l_M2D.m_Material.SetPass(0);
				Graphics.DrawMeshNow(l_M2D.m_Mesh, l_Mtx);
			}
		}
	}
}
