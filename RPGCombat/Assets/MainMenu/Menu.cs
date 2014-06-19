using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	public Texture2D background;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		GUI.skin.box.normal.background = background;
		GUI.Box(new Rect(0, 0, Screen.width, Screen.height),"");
	}
}
