using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameBlender : MonoBehaviour
{
	Renderer[] m_Renderers;
	private void Awake()
	{
		m_Renderers = GetComponentsInChildren<Renderer>();
	}
	private void OnPostRender()
	{
		//RenderTexture.active
	}
}
