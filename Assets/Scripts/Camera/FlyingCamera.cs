using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingCamera : MonoBehaviour
{
	public float speed = 1.5f, sensivity = 1f;
	public bool smooth = false;
	public float smoothAmount = 2f;
	
	private float mouseX, mouseY;
	private float actualSpeed = 0f;

	private Rigidbody rbody;
	
	private void OnEnable()
	{
		rbody = GetComponent<Rigidbody>();
		rbody.isKinematic = false;
		rbody.useGravity = false;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
	
	private void Update()
	{
		mouseX += Input.GetAxis("Mouse X") * sensivity;
		mouseY += Input.GetAxis("Mouse Y") * sensivity;
		actualSpeed = Mathf.Lerp(actualSpeed, speed*(Input.GetKey(KeyCode.LeftShift) ? 2f : 1f), Time.deltaTime*5f);
		if (!smooth){
			transform.rotation = Quaternion.Euler(Vector3.up * mouseX);
			transform.Rotate(-mouseY, 0f, 0f);
		}else{
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Vector3.up * mouseX + Vector3.right * -mouseY ), Time.deltaTime * smoothAmount);
		}
		rbody.velocity = transform.TransformDirection(new Vector3(Input.GetAxis("Horizontal")*actualSpeed, ((Input.GetKey(KeyCode.Space) ? 1f : 0f)+(Input.GetKey(KeyCode.LeftControl) ? -1f : 0f))*actualSpeed, Input.GetAxis("Vertical")*actualSpeed));
		
	}
}
