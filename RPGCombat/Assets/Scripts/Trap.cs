using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour {
	public float delayTime;

	// Use this for initialization
	void Start () {
		StartCoroutine (Go ());

	}

	IEnumerator Go()
	{
		while(true)
		{
			animation.Play();
			yield return new WaitForSeconds(3f);
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
