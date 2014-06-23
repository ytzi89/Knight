using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour {

	AudioSource audio;

	public AudioClip music;
	float musicTimer;

	// Main character
	GameObject knight;
	KnightHealth knightHealth;

	// NPCs for gate triggers
	public GameObject MKone;
	MKController mkcOne;
	public GameObject MKtwo;
	MKController mkcTwo;
	public GameObject MKthree;
	MKController mkcThree;
	public GameObject MKfour;
	MKController mkcFour;
	public GameObject MKfive;
	MKController mkcFive;
	public GameObject MKsix;
	MKController mkcSix;

	// Boss variables
	public GameObject boss;
	MKController bossController;
	bool bossSpawned;

	// Gates
	public GameObject GateTwo;
	GateController gcTwo;
	public GameObject GateThree;
	GateController gcThree;
	public GameObject GateFour;
	GateController gcFour;

	bool gameEnd;
	float gameEndTimer;

	// Use this for initialization
	void Start () {

		knight = GameObject.FindGameObjectWithTag ("Knight");
		knightHealth = knight.GetComponent<KnightHealth> ();

		gameEnd = false;

		gameEndTimer = 0.0f;

		Screen.showCursor = false;	// Remove cursor
		Screen.lockCursor = true;	// Lock cursor

		InitAudio ();	// Initiate audio

		InitTriggers ();	// Initiate the triggers

	}
	
	// Update is called once per frame
	void Update () {

		if(!gameEnd && knightHealth.GetHealth() <= 0.0f)
		{
			gameEnd = true;
		}

		if(boss != null && !gameEnd)
		{
			if(boss.GetComponent<EnemyHealth>().GetHealth () <= 0.0f)
			{
				gameEnd = true;
			}
		}

		UpdateAudio ();	// Update the audio;

		if(gameEnd)
		{
			gameEndTimer += Time.deltaTime;

			// 5 seconds return to menu
			if(gameEndTimer >= 5.0f)
			{
				Application.LoadLevel("MainMenu");
			}
		}
		else
		{
			UpdateTriggers ();	// Update the triggers
		}

	}

	void OnGUI()
	{
		if(gameEnd)
		{
			GUIStyle style = new GUIStyle ();
			style.fontSize = 48;
			style.fontStyle = FontStyle.Bold;
			style.alignment = TextAnchor.MiddleCenter;

			if(knightHealth.GetHealth () <= 0.0f)
			{
				style.normal.textColor = Color.red;

				GUI.Label (new Rect(Screen.width * 0.5f, Screen.height * 0.5f, 100, 50), "GAME OVER");
			}
			else if(boss.GetComponent<EnemyHealth>().GetHealth () <= 0.0f)
			{
				style.normal.textColor = Color.green;

				GUI.Label(new Rect(Screen.width * 0.5f, Screen.height * 0.5f, 100, 50), "CONGRATULATIONS");
			}
		}
	}


	/*
	 * Audio
	 */
	
	void InitAudio()
	{
		audio = GetComponent<AudioSource>();
		
		musicTimer = 0.0f;
		audio.clip = music;
		audio.Play();
	}
	
	void UpdateAudio()
	{
		if(musicTimer >= music.length)
		{
			audio.Play();
			musicTimer = 0.0f;
		}
		else
		{
			musicTimer += Time.deltaTime;
		}
	}


	/*
	 * Triggers
	 */

	void InitTriggers()
	{
		mkcOne = MKone.GetComponent<MKController> ();
		mkcTwo = MKtwo.GetComponent<MKController> ();
		mkcThree = MKthree.GetComponent<MKController> ();
		mkcFour = MKfour.GetComponent<MKController> ();
		mkcFive = MKfive.GetComponent<MKController> ();
		mkcSix = MKsix.GetComponent<MKController> ();

		gcTwo = GateTwo.GetComponent<GateController> ();
		gcThree = GateThree.GetComponent<GateController> ();
		gcFour = GateFour.GetComponent<GateController> ();

		bossSpawned = false;
	}

	void UpdateTriggers()
	{
		// If the second gate hasn't been activated
		if(gcTwo != null && !gcTwo.Activated())
		{
			// Has the knight blocking the gate been defeated?
			if(mkcOne.IsDead ())
			{
				gcTwo.DropGate ();	// Activate the gate
			}
		}
		// Third gate hasn't been activated
		else if(gcThree != null && !gcThree.Activated ())
		{
			// Has the knight blocking the gate been defeated?
			if(mkcTwo.IsDead ())
			{
				gcThree.DropGate ();	// Activate the gate
			}
		}
		// Fourth gate hasn't been activated
		else if(gcFour != null && !gcFour.Activated ())
		{
			// Have the knights blocking the gate been defeated?
			if(mkcThree.IsDead () && mkcFour.IsDead ())
			{
				gcFour.DropGate ();	// Activate the gate
			}
		}
		else if(!bossSpawned)
		{
			// Have the final knights been defeated?
			if(mkcFive.IsDead () && mkcSix.IsDead ())
			{
				// Spawn the final boss
				boss = (GameObject)Instantiate (Resources.Load ("Prefabs/Enemies/Mirror Knight/Mirror Knight"));
				boss.transform.position = new Vector3 (-14.5f, 2.0f, -8.57f);

				bossController = boss.GetComponent<MKController>();

				// Set boss color
				bossController.SetColor(Color.red);

				// Set boss attributes
				bossController.scale = 1.5f;
				bossController.pAttackDamageBasic = 1.25f;
				bossController.pAttackDamageHeavy = 1.25f;

				bossSpawned = true;
			}
		}
	}
}