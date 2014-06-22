using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	
	public Texture2D menuBackground;

	struct MenuButton
	{
		Vector2 position;
		float width;
		float height;
		string title;

		public bool hovered;

		public MenuButton(Vector2 pos, float w, float h, string t)
		{
			position = pos;
			width = w;
			height = h;
			title = t;

			hovered = false;
		}

		public void RenderButton()
		{
			GUIStyle style = new GUIStyle ();
			style.fontSize = 48;
			style.fontStyle = FontStyle.Bold;
			style.alignment = TextAnchor.MiddleCenter;
			style.normal.textColor = new Color(0.5f, 0.0f, 0.0f);

			Rect rect = new Rect (position.x - width * 0.5f, position.y - height * 0.5f, width, height);
			Rect iRect = new Rect (rect.xMin, Screen.height - rect.yMax, width, height);

			if(iRect.Contains (Input.mousePosition))
			{
				GUI.color = Color.black;

				hovered = true;
			}
			else
			{
				GUI.color = Color.red;

				hovered = false;
			}

			GUI.Label (rect, title, style);
		}
	}

	MenuButton buttonStart;
	MenuButton buttonOptions;
	MenuButton buttonQuit;

	void Start()
	{
		buttonStart = new MenuButton (new Vector2 (Screen.width * 0.64f, Screen.height * 0.5f), 200, 50, "START");
		buttonOptions = new MenuButton (new Vector2 (Screen.width * 0.64f, Screen.height * 0.65f), 200, 50, "OPTIONS");
		buttonQuit = new MenuButton (new Vector2 (Screen.width * 0.64f, Screen.height * 0.8f), 200, 50, "QUIT");
	}

	void Update () {
		if(Input.GetMouseButtonUp (0))
		{
			if(buttonStart.hovered)
			{
				Application.LoadLevel("Home");
			}
			else if(buttonOptions.hovered)
			{

			}
			else if(buttonQuit.hovered)
			{
				Application.Quit ();
			}
		}
	}
	
	void OnGUI()
	{
		// Draw background
		GUI.skin.box.normal.background = menuBackground;
		GUI.Box (new Rect (0.0f, 0.0f, Screen.width, Screen.height), "");

		// Draw buttons
		buttonStart.RenderButton ();
		buttonOptions.RenderButton ();
		buttonQuit.RenderButton ();
	}

}