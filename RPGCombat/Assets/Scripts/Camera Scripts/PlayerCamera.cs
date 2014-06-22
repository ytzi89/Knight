using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {

	public GameObject target;
	public float sensitivity = 5.0f;
	Vector3 offset;

	public float cameraDistance = 5.0f;

	void Start()
	{
		Vector3 targetEuler = target.transform.eulerAngles;
		transform.rotation = Quaternion.Euler(targetEuler.x, targetEuler.y + 90.0f, targetEuler.z);
	}

	void LateUpdate()
	{


		transform.LookAt (target.transform);
	}
	
}
