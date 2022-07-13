using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FidgitSpinner : MonoBehaviour
{
	[Header("Reference")]
	[SerializeField] Camera m_Cam;
	[SerializeField] Transform m_CamPivot;

	[Header("Config")]
	[SerializeField] float m_AngVel_Mul = 1;
	[SerializeField] float m_CamMove_Senstivity = 0.1f;

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

	Vector3 m_TouchOffset;
	Vector3 m_MouseDrag;

	float m_LastAngleDelta;

	#region Unity Callbacks
	private void Awake()
	{
		m_Trans = transform;
		m_Rigid = GetComponent<Rigidbody>();
		m_Rigid.maxAngularVelocity = 6E6F;
	}
	private void Update()
	{
		if (Input.GetMouseButtonDown(1))
			m_MouseDrag = Input.mousePosition;
		if (Input.GetMouseButton(1))
		{
			var l_MDelta = Input.mousePosition - m_MouseDrag;
			m_CamPivot.eulerAngles += (Vector3.up * l_MDelta.x * m_Cam.aspect + Vector3.left * l_MDelta.y) * m_CamMove_Senstivity;
			m_MouseDrag = Input.mousePosition;
		}
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
		//m_Rigid.angularVelocity = m_Trans.up * m_PointerDelta.magnitude * m_AngVel_Mul;
		m_Rigid.angularVelocity = m_Trans.up * m_LastAngleDelta * m_AngVel_Mul;
		m_PointerDelta = Vector2.zero;
	}
	#endregion

	#region Events
	public void OnMouseDown()
	{
		if (!Input.GetMouseButton(0) || Input.GetMouseButton(1)) return;
		m_IsSpinned = false;
		m_IsPointerDown = true;
		m_IsPointerExit = false;
		m_PointerDelta = Vector2.zero;
		m_PointerPosition = Input.mousePosition;
		Physics.Raycast(m_Cam.ScreenPointToRay(Input.mousePosition + Vector3.forward), out RaycastHit l_HitInfo);
		m_TouchOffset = l_HitInfo.point - m_Trans.position;
		m_Rigid.angularVelocity = Vector3.zero;
	}
	public void OnMouseDrag()
	{
		if (!Input.GetMouseButton(0) || Input.GetMouseButton(1)) return;
		m_PointerDelta = (Input.mousePosition - m_PointerPosition);
		m_PointerPosition = Input.mousePosition;

		Physics.Raycast(m_Cam.ScreenPointToRay(Input.mousePosition + Vector3.forward), out RaycastHit l_HitInfo);
		var l_DragOffset = l_HitInfo.point - m_Trans.position;
		m_LastAngleDelta = Vector3.SignedAngle(m_TouchOffset, l_DragOffset, Vector3.up);
		m_Trans.eulerAngles += Vector3.up * m_LastAngleDelta;
		m_TouchOffset = l_DragOffset;
	}
	public void OnMouseUp()
	{
		if (Input.GetMouseButton(1)) return;
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
