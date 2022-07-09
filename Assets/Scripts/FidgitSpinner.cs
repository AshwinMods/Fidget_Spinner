using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FidgitSpinner : MonoBehaviour
{
	[Header("Config")]
	[SerializeField] float m_AngVel_Mul = 1;

	[Header("Runtime Vars")]
	[SerializeField] bool m_IsSpinned;
	[SerializeField] bool m_IsPointerDown;
	[SerializeField] bool m_IsPointerExit;
	[SerializeField] Vector3 m_PointerDelta;
	[SerializeField] Vector3 m_PointerPosition;

	[Header("Dev")]
	[SerializeField] bool m_ForceAddSpin;
	[SerializeField] float m_ForceSpinVelocity;
	[Space]
	[SerializeField] int m_TargetFPS = 30;

	Transform m_Trans;
	Rigidbody m_Rigid;

	#region Unity Callbacks
	private void Awake()
	{
		m_Trans = transform;
		m_Rigid = GetComponent<Rigidbody>();
	}
	private void Update()
	{
	}
	private void OnValidate()
	{
		if (m_ForceAddSpin)
		{
			m_ForceAddSpin = false;
			m_PointerDelta = Vector3.right * (m_ForceSpinVelocity + m_Rigid.angularVelocity.magnitude);
			Spin();
		}
		Application.targetFrameRate = m_TargetFPS;
	}
	#endregion

	#region Private Func
	void Spin()
	{
		m_IsSpinned = true;
		m_Rigid.angularVelocity = m_Trans.up * m_PointerDelta.magnitude * m_AngVel_Mul;
		m_PointerDelta = Vector2.zero;
	}
	#endregion

	#region Events
	public void OnMouseDown()
	{
		m_IsSpinned = false;
		m_IsPointerDown = true;
		m_IsPointerExit = false;
		m_PointerDelta = Vector2.zero;
		m_PointerPosition = Input.mousePosition;
	}
	public void OnMouseDrag()
	{
		m_PointerDelta = (Input.mousePosition - m_PointerPosition);
		m_PointerPosition = Input.mousePosition;
	}
	public void OnMouseUp()
	{
		m_IsPointerDown = false;
		if (!m_IsSpinned)
			Spin();
	}
	public void OnMouseExit()
	{
		m_IsPointerExit = true;
		if (!m_IsSpinned)
			Spin();
	}
	#endregion
}
