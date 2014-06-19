using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour {

	AudioSource audio;

	public AudioClip music;
	float musicTimer;

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

	// Use this for initialization
	void Start () {

		InitAudio ();	// Initiate audio

		InitTriggers ();	// Initiate the triggers

	}
	
	// Update is called once per frame
	void Update () {

		UpdateAudio ();	// Update the audio;

		UpdateTriggers ();	// Update the triggers

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