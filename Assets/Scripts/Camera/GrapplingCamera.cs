using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingCamera : MonoBehaviour
{
    public float speed = 300f, sensivity = 1f;
	public bool smooth = false;
	public float smoothAmount = 2f;

    public LineRenderer line;
	
	private float mouseX, mouseY;
	private float actualSpeed = 0f;

    private Rigidbody rbody;
    private Vector3 grapplingPos;
	
	private void OnEnable()
	{
        rbody = GetComponent<Rigidbody>();
		rbody.useGravity = true;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
	
	private void Update()
	{
		mouseX += Input.GetAxis("Mouse X") * sensivity;
		mouseY += Input.GetAxis("Mouse Y") * sensivity;
		if (!smooth){
			transform.rotation = Quaternion.Euler(Vector3.up * mouseX);
			transform.Rotate(-mouseY, 0f, 0f);
		}else{
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Vector3.up * mouseX + Vector3.right * -mouseY ), Time.deltaTime * smoothAmount);
		}
		if (Input.GetKeyDown(KeyCode.Mouse0)){
            RaycastHit hit;
            if (Physics.Raycast(transform.position+transform.forward*2f, transform.forward, out hit, 3000f)){
                grapplingPos = FloatingOrigin.Apply(hit.point);
            }
            rbody.isKinematic = false;
        }
        if (Input.GetKey(KeyCode.Mouse0) && grapplingPos != Vector3.zero){
            Vector3 dir = (FloatingOrigin.Invert(grapplingPos)-transform.position).normalized;
            rbody.velocity = Vector3.Lerp(rbody.velocity, dir*speed, Time.deltaTime*.5f);
            line.SetPositions(new Vector3[]{line.transform.position, FloatingOrigin.Invert(grapplingPos)});
        }else{
            line.SetPositions(new Vector3[]{line.transform.position, line.transform.position});
            grapplingPos = Vector3.zero;
        }

	}
}
