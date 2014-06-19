using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour {

	GUIStyle damageStyle;	// Damage style
	MKController controller;

	// Character stats
	float currentHealth;	// Current character health
	public float maxHealth;		// Maximum player health

	float damage;
	float damageTimer;
	
	// Use this for initialization
	void Start () {
		controller = GetComponent <MKController> ();

		damageStyle = new GUIStyle ();
		damageStyle.normal.textColor = Color.white;
		damageStyle.fontSize = 32;
		damageStyle.fontStyle = FontStyle.Bold;

		currentHealth = maxHealth;	// Initiate full health

		damage = 0.0f;
		damageTimer = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// GUI Update
	void OnGUI()
	{
		// Get screen location
		Camera cam = Camera.main;
		Vector3 pos = cam.WorldToScreenPoint (new Vector3(transform.position.x, 
		                                                  transform.position.y + collider.bounds.extents.y * 2.0f, 
		                                                  transform.position.z));

		// Direction to play
		Vector3 direction = transform.position - cam.transform.position;
		float angle = Vector3.Angle (direction, cam.transform.forward);

		if(angle < 90.0f)
		{
			RaycastHit hit;

			if(Physics.Raycast(cam.transform.position + cam.transform.up, direction.normalized, out hit, 20.0f))
			{
				//if(hit.collider.gameObject == cam)
				//{
					// Health bar
					// Check if character is alive
					if(currentHealth > 0)
					{
						Texture2D tex = new Texture2D (1, 1);
						tex.SetPixel (0, 0, Color.red);
						tex.Apply ();
						GUI.skin.box.normal.background = tex;
						GUI.Box (new Rect (pos.x - ((currentHealth / maxHealth) * 50.0f), Screen.height - pos.y - 15, 
						                   100.0f * (currentHealth / maxHealth), 10.0f), "");
					}

					// Display damage
					if(damage > 0.0f)
					{
						damageTimer += Time.deltaTime;	// Increment damage timer

						// Reset damage
						if (damageTimer > 2.0f)
						{
							damage = 0.0f;	// Set damage back to zero
						}
						else
						{
							// Display damage
							GUI.Label (new Rect(pos.x, Screen.height - pos.y + 50, 150, 130), damage.ToString (), damageStyle);
						}
					}
				//}
			}
		}
	}

	// Get current health
	public float GetHealth()
	{
		return currentHealth;
	}
	
	// Set current health
	public void SetHealth(float value)
	{
		// Taking damage
		if(value < currentHealth && currentHealth > 0.0f)
		{
			damage += (currentHealth - value);
			damageTimer = 0.0f;
		}

		// Health value can't be negative
		if(value < 0)
		{
			currentHealth = 0;
		}
		// Health can't be greater than max
		else if(value > maxHealth)
		{
			currentHealth = maxHealth;
		}
		else
		{
			currentHealth = value;
		}
	}
	
	// Get max health
	public float GetMaxHealth()
	{
		return maxHealth;
	}
	
	// Set max health
	public void SetMaxHealth(float value)
	{
		// Health can't be negative or zero
		if(value < 1)
		{
			maxHealth = 1;
		}
		else
		{
			maxHealth = value;
		}
		
		// Ensure current health doesn't exceed maximum health
		if(currentHealth > maxHealth)
		{
			currentHealth = maxHealth;
		}
	}
}