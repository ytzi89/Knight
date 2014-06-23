using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	
	public Texture2D menuBackground;
	public Texture2D optionsMenu;

	AudioSource audio;

	public AudioClip menuMusic;
	float musicTimer;

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
	MenuButton buttonBack;

	enum MenuState{Main, Options};
	MenuState menuState;

	void Start()
	{
		Screen.showCursor = true;
		Screen.lockCursor = false;

		audio = GetComponent<AudioSource> ();

		menuState = MenuState.Main;

		musicTimer = 0.0f;
		audio.clip = menuMusic;
		audio.Play ();

		buttonStart = new MenuButton (new Vector2 (Screen.width * 0.64f, Screen.height * 0.5f), 200, 50, "START");
		buttonOptions = new MenuButton (new Vector2 (Screen.width * 0.64f, Screen.height * 0.65f), 200, 50, "OPTIONS");
		buttonQuit = new MenuButton (new Vector2 (Screen.width * 0.64f, Screen.height * 0.8f), 200, 50, "QUIT");
		buttonBack = new MenuButton(new Vector2(Screen.width - 100, Screen.height - 50), 200, 50, "BACK");
	}

	void Update () {

		switch(menuState)
		{
		case MenuState.Main:
			if(Input.GetMouseButtonUp (0))
			{
				if(buttonStart.hovered)
				{
					Application.LoadLevel("IntroCinematic");
				}
				else if(buttonOptions.hovered)
				{
						menuState = MenuState.Options;
				}
				else if(buttonQuit.hovered)
				{
					Application.Quit ();
				}
			}
			break;
		case MenuState.Options:
			if(Input.GetMouseButtonUp (0))
			{
				if(buttonBack.hovered)
				{
						menuState = MenuState.Main;
				}
			}
			break;
		}
		musicTimer += Time.deltaTime;

		if(musicTimer >= menuMusic.length)
		{
			musicTimer = 0.0f;
			audio.Play ();
		}
	}
	
	void OnGUI()
	{
		switch(menuState)
		{
		case MenuState.Main:
			// Draw background
			GUI.skin.box.normal.background = menuBackground;
			GUI.Box (new Rect (0.0f, 0.0f, Screen.width, Screen.height), "");
			
			// Draw buttons
			buttonStart.RenderButton ();
			buttonOptions.RenderButton ();
			buttonQuit.RenderButton ();
			break;
		case MenuState.Options:
			// Draw background
			GUI.skin.box.normal.background = optionsMenu;
			GUI.Box (new Rect(0.0f, 0.0f, Screen.width, Screen.height), "");

			// Draw buttons
			buttonBack.RenderButton();
			break;
		}
	}

}