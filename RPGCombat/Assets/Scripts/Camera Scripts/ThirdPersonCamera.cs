using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {

	public GameObject target;
	KnightController targetController;
	float cameraDistance = 5.0f;

	float mouseSensitivity = 200.0f;
	Vector3 offset;
	float hMouse, vMouse;
	float x = 0.0f, y = 0.0f;

	// Use this for initialization
	void Start () {
		targetController = target.GetComponent<KnightController> ();	// Initiate target controller

		//transform.position = new Vector3 (transform.position.x, target.transform.position.y, transform.position.z);
		transform.position = new Vector3 (target.transform.position.x, 
		                                  target.transform.position.y, 
		                                  target.transform.forward.z - cameraDistance);

		Vector3 angle = transform.eulerAngles;
		x = angle.x;
		y = angle.y;

		offset = (target.transform.position - transform.position).normalized * cameraDistance;
	}
	
	// Update is called once per frame
	void Update() {

	}

	void LateUpdate () {

		hMouse = Input.GetAxis ("Mouse X");
		vMouse = Input.GetAxis ("Mouse Y");

		y -= vMouse * mouseSensitivity * Time.deltaTime;
		if(y <= 5.0f)
		{
			y = 5.0f;
		}
		else if(y >= 89.0f)
		{
			y = 89.0f;
		}

		// Check rotate-able states
		if(targetController.CanRotate())
		{
			target.transform.Rotate (0, hMouse * mouseSensitivity * Time.deltaTime, 0);	// Rotate target left and right
		}

		// Position the camera behind the player
		Quaternion rotation = Quaternion.Euler (y, target.transform.eulerAngles.y, 0);
		transform.position = target.transform.position + (rotation * offset);

		// Top-down mode
		// transform.position = new Vector3 (target.transform.position.x, target.transform.position.y + 10, target.transform.position.z);

		transform.LookAt (new Vector3(target.transform.position.x, 
		                             target.transform.position.y + target.collider.bounds.extents.y, 
		                             target.transform.position.z));
	}
}
