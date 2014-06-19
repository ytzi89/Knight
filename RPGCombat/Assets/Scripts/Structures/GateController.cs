using UnityEngine;
using System.Collections;

public class GateController : MonoBehaviour {

	float dropSpeed;	// Speed at which the gate drops
	float dropDistance;	// Distance at which the gate has dropped
	float ySize;		// height of the gate

	bool isDropping;	// Is the gate dropping?

	// Use this for initialization
	void Start () {
		dropSpeed = 1.0f;
		dropDistance = 0.0f;
		ySize = collider.bounds.extents.y * 2.0f;

		isDropping = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(isDropping)
		{
			dropDistance += dropSpeed * Time.deltaTime;

			if(dropDistance >= ySize)
			{
				Object.Destroy (this.gameObject);
			}
			else
			{
				transform.Translate (new Vector3(0.0f, -dropSpeed * Time.deltaTime, 0.0f));
			}
		}
	}

	public void DropGate()
	{
		isDropping = true;
	}

	public bool Activated()
	{
		return isDropping;
	}
}
