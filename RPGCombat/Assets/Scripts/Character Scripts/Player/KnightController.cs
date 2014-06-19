using UnityEngine;
using System.Collections;

// TODO: Put this declaration in another file
public enum AttackType { Basic, Heavy };	// Attack type enumeration

public class KnightController : MonoBehaviour {

	// State variables
	public enum PlayerState { Free, Blocking, Attacking, Flinching, Dodging, Dead };	// Player state enumeration
	public PlayerState playerState;	// Player state variable

	Animator anim;	// Animator component

	KnightHealth knightHealth;	// Health and stamina script

	AudioSource audioSource;

	// Audio variables
	public AudioClip audioWalking;
	public AudioClip audioSprinting;
	public AudioClip audioSwingBasic;
	public AudioClip audioSwingOverhead;
	public AudioClip audioBlock;
	public AudioClip audioDodge;
	public AudioClip audioFlinch;
	public AudioClip audioDying;

	// Audio timer
	float audioTimer;

	// Movement variables
	float moveSpeed;		// Movement speed
	float hMovement;		// Horizontal movement
	float vMovement;		// Vertical movement
	Vector2 movement;		// Movement vector
	Vector2 dodgeDirection;	// Dodge vector

	// Attack variables
	AttackType attackType;						// Class attack type variable
	float attackDamageBasic;					// Base attack damage
	float attackDamageHeavy;					// Heavy attack damage
	float attackRange;							// Attack range
	float attackAngle;							// Angle of basic attack

	// Block variables
	float blockAngle;	// Angle for blocking

	// Conditional variables
	bool isIdle;			// Character is idle
	bool movingForward;		// Character is moving forward
	bool isBlocking;		// Character is blocking
	bool isAttacking;		// Character is attacking
	bool isDodging;			// Character is dodging
	bool isFlinching;		// Character is flinching
	bool isDying;			// Character is dying
	bool isDead;			// Character is dead
	bool isSprinting;		// Character is sprinting
	bool blockedAttack;		// Character blocked an attack
	bool continueAttack;	// Character is combo attacking

	// Animator parameters
	int xVelocityHash = Animator.StringToHash ("xVelocity");
	int zVelocityHash = Animator.StringToHash ("zVelocity");
	int isIdleHash = Animator.StringToHash ("isIdle");
	int movingForwardHash = Animator.StringToHash ("movingForward");
	int isBlockingHash = Animator.StringToHash ("isBlocking");
	int isAttackingHash = Animator.StringToHash ("isAttacking");
	int isDodgingHash = Animator.StringToHash ("isDodging");
	int isFlinchingHash = Animator.StringToHash ("isFlinching");
	int isDyingHash = Animator.StringToHash ("isDying");
	int isDeadHash = Animator.StringToHash ("isDead");
	int isSprintingHash = Animator.StringToHash ("isSprinting");
	int blockedAttackHash = Animator.StringToHash ("blockedAttack");
	int continueAttackHash = Animator.StringToHash ("continueAttack");

	// Animator states
	int dyingStateHash = Animator.StringToHash ("Base Layer.Dying");

	// Timers
	float dodgeTimer;			// Dodge timer for applied force
	float attackTimer;			// Attack timer for exiting state and halting movement
	float blockTimer;			// Block timer for blocked attack recovery
	float flinchTimer;			// Flinch timer for exiting flinch state
	float emptyStaminaTimer;	// Time to delay stamina regeneration after it runs out

	// Action times
	float timeDodge;			// Time it takes to dodge
	float timeFlinch;			// Time it takes to flinch
	float timeBasicAttack;		// Time it takes for basic attack
	float timeHeavyAttack;		// Time it takes for heavy attack
	float timeBlockedAttack;	// Time it takes to block an attack
	float timeInvincible;		// Time the character is invincible while dodging
	float timeStaminaDelay;		// Time before stamina is regenerated again.

	// State-specific variables
	bool attackRegistered;	// Used in the attack state logic to enable a delay timer after attack has registered
	bool attackAfterDodge;	// Used in the dodge state to transition smoothly from dodge to attack

	// Use this for initialization
	void Start () {

		anim = GetComponent<Animator> ();	// Get Animator component

		knightHealth = GetComponent<KnightHealth> ();	// Get KnightHealth script component

		audioSource = GetComponent<AudioSource> ();		// Get Audio Source component

		playerState = PlayerState.Free;	// Initiate player state to free

		Physics.gravity = new Vector3 (0.0f, Physics.gravity.y * 4.0f, 0.0f);

		// Audio variables
		audioTimer = 0.0f;

		// Movement variables
		moveSpeed = 240000.0f;			// Character movement speed
		hMovement = 0.0f;				// Horizontal movement
		vMovement = 0.0f;				// Vertical movement
		movement = Vector2.zero;		// Movement vector
		dodgeDirection = Vector2.zero;	// Dodge vector

		// Attacking variables
		attackType = AttackType.Basic;	// Initate attack type to  basic
		attackDamageBasic = 25.0f;		// Set basic damage
		attackDamageHeavy = 40.0f;		// Set heavy damage
		attackRange = 4.0f;				// Attack range set to 3
		attackAngle = 60.0f;			// 60 degree attack angle

		// Blocking variables
		blockAngle = 90.0f;				// 90 degree blocking angle

		// Conditional variables
		isIdle = true;					// Character is idle
		movingForward = true;			// Character moving forward
		isBlocking = false;				// Character not blocking
		isAttacking = false;			// Character not attacking
		isDodging = false;				// Character not dodging
		isFlinching = false;			// Character not flinching
		isDying = false;				// Character not dying
		isDead = false;					// Character not dead
		isSprinting = false;			// Character not sprinting
		blockedAttack = false;			// Character not recovering from block
		continueAttack = false;			// Character not combo attacking

		// Timers
		dodgeTimer = 0.0f;
		attackTimer = 0.0f;
		blockTimer = 0.0f;
		flinchTimer = 0.0f;
		emptyStaminaTimer = 1.0f;

		// Action times
		timeDodge = 0.4f;
		timeFlinch = 0.8f;
		timeBasicAttack = 0.45f;
		timeHeavyAttack = 0.75f;
		timeBlockedAttack = 0.3f;
		timeInvincible = 0.25f;
		timeStaminaDelay = 1.0f;

		// State-specific variables
		attackRegistered = false;
		attackAfterDodge = false;
	}
	
	// Update is called once per frame
	void Update () {

		UserInput ();	// Handle keyboard and mouse input

		// State handler
		switch(playerState)
		{
		case  PlayerState.Free:
			FreeLogic();
			break;
		case PlayerState.Blocking:
			BlockingLogic();
			break;
		case PlayerState.Attacking:
			AttackingLogic();
			break;
		case PlayerState.Dodging:
			DodgingLogic();
			break;
		case PlayerState.Flinching:
			FlinchingLogic();
			break;
		case PlayerState.Dead:
			DeadLogic();
			break;
		}

		RegenerateStamina ();	// Regenerate stamina

		SetAnimatorParameters ();	// Set parameters for the animator component

		Step ();
	}

	/* * * * * * * * * * * * * * * * * Logic Handling * * * * * * * * * * * * * * * * */

	// Free state logic
	void FreeLogic()
	{
		// Character is moving
		if(movement.magnitude > 0)
		{
			isIdle = false;	// Character no longer idle

			// Check movement direction
			if(vMovement >= 0.0f)
			{
				movingForward = true;	// Not moving backwards

				// Is the player sprinting?
				if(isSprinting && knightHealth.GetStamina () > 0.0f)
				{
					// Audio
					if(audioSource.clip != audioSprinting || audioTimer >= audioWalking.length)
					{
						audioSource.clip = audioSprinting;
						audioSource.Play ();

						audioTimer = 0.0f;
					}
					else
					{
						audioTimer += Time.deltaTime;
					}

					movement *= 2.0f;	// Increase movement speed

					// Reduce stamina while sprinting
					knightHealth.SetStamina (knightHealth.GetStamina () - (15.0f * Time.deltaTime));
				}
				// Isn't sprinting
				else
				{
					// Audio
					if(audioSource.clip != audioWalking || audioTimer >= audioWalking.length)
					{
						audioSource.clip = audioWalking;
						audioSource.Play ();

						audioTimer = 0.0f;
					}
					else
					{
						audioTimer += Time.deltaTime;
					}

					isSprinting = false;
				}
			}
			else if(vMovement < 0.0f)
			{
				// Audio
				if(audioSource.clip != audioWalking || audioTimer >= audioWalking.length)
				{
					audioSource.clip = audioWalking;
					audioSource.Play ();

					audioTimer = 0.0f;
				}
				else
				{
					audioTimer += Time.deltaTime;
				}

				movingForward = false;	// Moving backwards
				isSprinting = false;	// Can't sprint backwards
			}
		}
		// Player isn't moving
		else
		{
			audioSource.Stop ();	// Stop audio
			audioSource.clip = null;

			isIdle = true;
			movingForward = true;	// Default for idle state
			isSprinting = false;
		}

		// Move the character
		rigidbody.AddRelativeForce (new Vector3 (movement.x, 0.0f, movement.y) * Time.deltaTime * moveSpeed);
	}

	// Blocking state logic
	void BlockingLogic()
	{
		// TODO: If hit with heavy attack by opponent, apply backwards force to the player during block duration

		// Check if the player is blocking an attack
		if(blockedAttack)
		{
			// Audio
			if(blockTimer == 0.0f)
			{
				PlaySound (audioBlock);
			}
			blockTimer += Time.deltaTime;	// Increase block timer by update time
		}

		if(blockTimer >= timeBlockedAttack)
		{
			blockedAttack = false;
			blockTimer = 0.0f;
		}

		// Change state if the player isn't blocking
		if(!isBlocking && !blockedAttack)
		{
			ChangeToState(PlayerState.Free);
		}
	}

	// Attacking state logic
	void AttackingLogic()
	{
		// Execute on state execution
		if(attackTimer == 0.0f)
		{
			continueAttack = false;	// Combo initially disabled

			anim.SetBool (continueAttackHash, continueAttack);	// Disable combo animation
		}

		attackTimer += Time.deltaTime;

		// If the attack has not yet registered with opponents
		if(!attackRegistered)
		{
			// Basic attack time
			if(attackType == AttackType.Basic && attackTimer < timeBasicAttack)
			{
				// Do nothing
			}
			// Heavy attack time
			else if(attackType == AttackType.Heavy && attackTimer < timeHeavyAttack)
			{
				// Do nothing
			}
			// Register attack
			else
			{
				float currentDamage = 0.0f;	// How much damage does this attack do?

				// Reduce stamina
				if(attackType == AttackType.Basic)
				{
					// Audio
					PlaySound (audioSwingBasic);

					knightHealth.SetStamina (knightHealth.GetStamina () - 25.0f);
					currentDamage = attackDamageBasic;
				}
				// Reduce more stamina for heavy attack
				else if(attackType == AttackType.Heavy)
				{
					// Audio
					PlaySound (audioSwingOverhead);

					knightHealth.SetStamina (knightHealth.GetStamina () - 35.0f);
					currentDamage = attackDamageHeavy;
				}

				attackRegistered = true;	// Attack was registered

				GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
				foreach(GameObject enemy in enemies)
				{
					Vector3 direction = enemy.transform.position - transform.position;	// Get the direction to the enemy
					float angle = Vector3.Angle(direction, transform.forward);	// Get the angle to the enemy

					// If the enemy is within the attack FOV and within range of attack
					if(angle < attackAngle * 0.5 && (enemy.transform.position - transform.position).magnitude <= attackRange)
					{
						RaycastHit hit;

						// Check if the 
						if(Physics.Raycast (transform.position + transform.up, direction.normalized, out hit, attackRange))
						{
							// Target isn't being obstructed by an object
							if(hit.collider.gameObject == enemy)
							{
								Debug.Log ("Enemy hit!");

								enemy.GetComponent<EnemyController>().TakeHit (transform.position, currentDamage, attackType);
							}
						}
					}
				}

			}
		}
		// .3 second delay before automatic change to free state
		else if(attackType == AttackType.Basic && attackTimer >= timeBasicAttack + 0.3f)
		{
			// Check if combo is active
			if(continueAttack)
			{
				ChangeToState (PlayerState.Attacking);

				anim.SetBool (continueAttackHash, continueAttack);	// Change to next attack
			}
			else
				ChangeToState (PlayerState.Free);	// Refer back to free state
		}
		// .5 second delay before atomatic change to free state
		else if(attackType == AttackType.Heavy && attackTimer >= timeHeavyAttack + 0.3f)
		{
			ChangeToState (PlayerState.Free);
		}
	}

	// Dodging state logic
	void DodgingLogic()
	{
		// Reduce stamina upon entry
		if(dodgeTimer == 0.0f)
		{
			// Audio
			PlaySound (audioDodge);

			attackAfterDodge = false;	// Dodge to attack initially disabled

			knightHealth.SetStamina (knightHealth.GetStamina () - 15.0f);
		}

		dodgeTimer += Time.deltaTime;	// Increase dodge timer

		// Set directional movement for dodge animation
		hMovement = dodgeDirection.x;
		vMovement = dodgeDirection.y;
		
		// Enable physics for dodge and avoid sliding
		if(dodgeTimer <= timeDodge)
		{
			// Add force relative to the character's rotation
			rigidbody.AddRelativeForce (new Vector3(dodgeDirection.x, 0.0f, dodgeDirection.y) 
			                            * 4.0f * Time.deltaTime * moveSpeed);
		}
		// Transition to attack if player attacked during dodge
		else if(dodgeTimer > timeDodge + 0.15f && attackAfterDodge == true)
		{
			ChangeToState (PlayerState.Attacking);
		}
		// Dodge complete after
		else if(dodgeTimer > timeDodge + 0.15f)
		{
			ChangeToState (PlayerState.Free);	// Change to free state
		}


	}

	// Flinching state logic
	void FlinchingLogic()
	{
		// Audio
		if(flinchTimer == 0.0f)
		{
			PlaySound (audioFlinch);
		}

		// Check if character is dead
		if (knightHealth.GetHealth () <= 0.0f)
			ChangeToState (PlayerState.Dead);

		flinchTimer += Time.deltaTime;

		if(flinchTimer > timeFlinch + 0.3f)
		{
			ChangeToState (PlayerState.Free);	// Change to free state
		}
	}

	// Dead state logic
	void DeadLogic()
	{
		if(isDying && anim.GetCurrentAnimatorStateInfo(0).nameHash == dyingStateHash)
		{
			// Audio
			PlaySound (audioDying);

			isDying = false;
		}
	}


	/* * * * * * * * * * * * * * * * * State Handling * * * * * * * * * * * * * * * * */

	/*
	 * ChangeToState
	 * 
	 * Set variables for state change.
	 */
	void ChangeToState(PlayerState state)
	{
		// Audio timer
		audioTimer = 0.0f;
		audioSource.clip = null;

		switch(state)
		{
		case  PlayerState.Free:
			FreeStateChange();
			break;
		case PlayerState.Blocking:
			BlockingStateChange();
			break;
		case PlayerState.Attacking:
			AttackingStateChange();
			break;
		case PlayerState.Dodging:
			DodgingStateChange();
			break;
		case PlayerState.Flinching:
			FlinchingStateChange();
			break;
		case PlayerState.Dead:
			DeadStateChange();
			break;
		}
	}

	// Change to free state
	void FreeStateChange()
	{
		playerState = PlayerState.Free;

		isAttacking = false;
		isDodging = false;
		isFlinching = false;
		isBlocking = false;
		blockedAttack = false;
	}

	// Change to blocking state
	void BlockingStateChange()
	{
		playerState = PlayerState.Blocking;

		isAttacking = false;
		isDodging = false;
		isSprinting = false;
		isFlinching = false;
		isBlocking = true;
		blockedAttack = false;

		blockTimer = 0.0f;
	}

	// Change to attacking state
	void AttackingStateChange()
	{
		playerState = PlayerState.Attacking;

		isAttacking = true;
		isDodging = false;
		isSprinting = false;
		isFlinching = false;
		isBlocking = false;
		blockedAttack = false;

		attackRegistered = false;

		attackTimer = 0.0f;
	}

	// Change to dodging state
	void DodgingStateChange()
	{
		playerState = PlayerState.Dodging;

		isAttacking = false;
		isDodging = true;
		isFlinching = false;
		isBlocking = false;
		blockedAttack = false;

		dodgeTimer = 0.0f;
	}

	// Change to flinching state
	void FlinchingStateChange()
	{
		playerState = PlayerState.Flinching;

		isAttacking = false;
		isDodging = false;
		isSprinting = false;
		isFlinching = true;
		isBlocking = false;
		blockedAttack = false;

		flinchTimer = 0.0f;
	}

	void DeadStateChange()
	{
		playerState = PlayerState.Dead;

		isDying = true;
		isDead = true;
		isFlinching = false;
		isAttacking = false;
		isDodging = false;
		isSprinting = false;
		isBlocking = false;
		blockedAttack = false;
	}


	/* * * * * * * * * * * * * * * User Input Handling * * * * * * * * * * * * * * */

	/*
	 * UserInput
	 * 
	 * This method handles user input from the mouse and keyboard.
	 */
	void UserInput()
	{
		// Movement
		hMovement = Input.GetAxis ("Horizontal");
		vMovement = Input.GetAxis ("Vertical");
		movement = new Vector2 (hMovement, vMovement);

		// Attack on Left-Click
		if(Input.GetMouseButtonDown (0))
		{
			AttackInput ();	// Handle state change
		}

		// Block on Right-Mouse hold
		if(Input.GetMouseButton (1))
		{
			BlockingInput();	// Handle state change
		}
		// Right-Mouse is up
		else
		{
			isBlocking = false;
		}

		// Sprint on Left-Shift
		if(Input.GetKey (KeyCode.LeftShift))
		{
			isSprinting = true;
		}
		// Left-Shift is up
		else
		{
			isSprinting = false;
		}

		// Dodge on Space Bar
		if(Input.GetKeyDown (KeyCode.Space))
		{
			DodgeInput();	// Handle state change
		}

		// F key to test flinching
		if(Input.GetKeyDown (KeyCode.F))
		{
			TakeHit (transform.forward, 15.0f, AttackType.Basic);
		}
	}

	// Can the character attack
	void AttackInput()
	{
		// If player is already attacking, we don't need to determine the attack type
		if(playerState != PlayerState.Attacking)
		{
			// Determine attack type
			if (isSprinting)
				attackType = AttackType.Heavy;
			else
				attackType = AttackType.Basic;
		}

		// Character must have stamina to attack
		if (knightHealth.GetStamina () > 0.0f)
		{
			// Handle attack-capable states
			switch(playerState)
			{
			case PlayerState.Free:
				// Character can always attack in free state
				ChangeToState (PlayerState.Attacking);
				break;
			case PlayerState.Blocking:
				// Check if character is recovering from block
				if(!blockedAttack)
					ChangeToState (PlayerState.Attacking);
				break;
			case PlayerState.Dodging:
				// Check if dodge is complete
				if(dodgeTimer > timeDodge * 0.5f)
					attackAfterDodge = true;
				break;
			case PlayerState.Flinching:
				// Check if flinch is complete
				if(flinchTimer > timeFlinch)
					ChangeToState (PlayerState.Attacking);
				break;
			case PlayerState.Attacking:
				//Check timer against attack type and trigger combo if capable.
				if(attackType == AttackType.Basic && attackTimer > timeBasicAttack * 0.5f)
					continueAttack = true;
				break;
			}
		}
	}

	// Can the character block
	void BlockingInput()
	{
		// Character must have stamina to block
		if(knightHealth.GetStamina () > 0.0f)
		{
			// Handle block-capable states
			switch(playerState)
			{
			case PlayerState.Free:
				// Character can always block in free state
				ChangeToState (PlayerState.Blocking);
				break;
			case PlayerState.Dodging:
				// Check if dodge is complete
				if(dodgeTimer > timeDodge)
					ChangeToState (PlayerState.Blocking);
				break;
			case PlayerState.Flinching:
				// Check if flinch is complete
				if(flinchTimer > timeFlinch)
					ChangeToState (PlayerState.Blocking);
				break;
			case PlayerState.Attacking:
				if(attackType == AttackType.Basic && attackTimer > timeBasicAttack + 0.15f)
					ChangeToState (PlayerState.Blocking);
				else if(attackType == AttackType.Heavy && attackTimer > timeHeavyAttack + 0.15f)
					ChangeToState(PlayerState.Blocking);
				break;
			}
		}
	}

	// Can the character dodge
	void DodgeInput()
	{
		// Character must have stamina to dodge
		if(knightHealth.GetStamina () > 0.0f)
		{
			dodgeDirection = movement.normalized;	// Determine dodge direction

			if(movement.magnitude == 0.0f)
			{
				dodgeDirection = new Vector2(0.0f, -1.0f);
			}

			// Handle dodge-capable states
			switch(playerState)
			{
			case PlayerState.Free:
				// Character can always dodge in free state
				ChangeToState(PlayerState.Dodging);
				break;
			case PlayerState.Blocking:
				if(!blockedAttack)
					ChangeToState(PlayerState.Dodging);
				break;
			case PlayerState.Dodging:
				// Check if current dodge is complete
				if(dodgeTimer > timeDodge)
					ChangeToState (PlayerState.Dodging);
				break;
			case PlayerState.Flinching:
				// Check if flinch is complete
				if(flinchTimer > timeFlinch)
					ChangeToState (PlayerState.Dodging);
				break;
			case PlayerState.Attacking:
				if(attackType == AttackType.Basic && attackTimer > timeBasicAttack + 0.15f)
					ChangeToState (PlayerState.Dodging);
				else if(attackType == AttackType.Heavy && attackTimer > timeHeavyAttack + 0.15f)
					ChangeToState(PlayerState.Dodging);
				break;
			}
		}
	}


	/* * * * * * * * * * * * * * * Helper Functions * * * * * * * * * * * * * * * */

	/*
	 * TakeHit
	 * 
	 * This method handles logic from being being attacked.
	 */
	public void TakeHit(Vector3 origin, float damage, AttackType type)
	{
		// Handle flinch-capable states
		switch(playerState)
		{
		case PlayerState.Free:
			// Character can always take damage in free state
			TakeDamage (damage);
			ChangeToState (PlayerState.Flinching);
			break;
		case PlayerState.Blocking:
			// Is the character blocking in the direction of the attack?
			if(CanBlock(origin))
			{
				// Character can only lose stamina when not already blocking an attack
				if(!blockedAttack)
				{
					// Reduce stamina depending on attack type
					// 35.0f for basic
					if(type == AttackType.Basic)
						knightHealth.SetStamina (knightHealth.GetStamina () - 45.0f);
					// 55.0f for heavy
					else if(type == AttackType.Heavy)
					{
						knightHealth.SetStamina (knightHealth.GetStamina () - 80.0f);
						// TODO: Create state for character sliding back on heavy block
					}
					// Character has stamina remaining
					if(knightHealth.GetStamina () > 0.0f)
						blockedAttack = true;
					// Character flinches if the block wastes all of his stamina
					else
						ChangeToState (PlayerState.Flinching);
				}
			}
			// Character facing the wrong direction
			else
			{
				TakeDamage (damage);
				ChangeToState (PlayerState.Flinching);
			}
			break;
		case PlayerState.Attacking:
			// Character can always take damage while attacking
			TakeDamage (damage);
			ChangeToState (PlayerState.Flinching);
			break;
		case PlayerState.Dodging:
			// Is the character invincible?
			if(dodgeTimer > timeInvincible)
			{
				TakeDamage (damage);
				// TODO: Instead of flinching, knock character on back
				ChangeToState(PlayerState.Flinching);
			}
			break;
		}
	}

	// Can the character block the attack?
	bool CanBlock(Vector3 origin)
	{
		Vector3 direction = origin - transform.position;			// Direction to attack origin
		float angle = Vector3.Angle (direction, transform.forward);	// Angle of origin from character

		// Compare angle to block angle
		if(angle < blockAngle * 0.5f)
			return true;
		else
			return false;
	}

	/*
	 * RegenerateStamina
	 * 
	 * This method controls the player's stamina regeneration.
	 */
	void RegenerateStamina()
	{
		// If there is no delay on the stamina
		if(emptyStaminaTimer >= timeStaminaDelay)
		{
			// Regenerate stamina in free state
			if(playerState == PlayerState.Free && isSprinting == false)
			{
				knightHealth.SetStamina (knightHealth.GetStamina() + (25.0f * Time.deltaTime));
			}
			// Regenerate stamina at 1/3 the rate in Blocking state
			else if(playerState == PlayerState.Blocking)
			{
				knightHealth.SetStamina (knightHealth.GetStamina () + (5.0f * Time.deltaTime));
			}
		}
		else
		{
			emptyStaminaTimer += Time.deltaTime;
		}
	}

	/*
	 * EmptyStaminaDelay
	 * 
	 * This method begins a timer that delays stamina regneration.
	 */
	public void EmptyStaminaDelay()
	{
		emptyStaminaTimer = 0.0f;
	}

	/*
	 * TakeDamage
	 * 
	 * This method handles any damage dealt to the player.
	 */
	void TakeDamage(float damage)
	{
		knightHealth.SetHealth (knightHealth.GetHealth () - damage);

		Debug.Log ("Knight hit with " + damage + " damage... " + knightHealth.GetHealth () + " remaining.");
	}

	/*
	 * SetAnimatorParameters
	 * 
	 * This method sets all of the parameters for the animator component.
	 */
	void SetAnimatorParameters()
	{
		anim.SetFloat (xVelocityHash, hMovement);
		anim.SetFloat (zVelocityHash, vMovement);
		anim.SetBool (isIdleHash, isIdle);
		anim.SetBool (movingForwardHash, movingForward);
		anim.SetBool (isBlockingHash, isBlocking);
		anim.SetBool (isAttackingHash, isAttacking);
		anim.SetBool (isDodgingHash, isDodging);
		anim.SetBool (isSprintingHash, isSprinting);
		anim.SetBool (isFlinchingHash, isFlinching);
		anim.SetBool (isDyingHash, isDying);
		anim.SetBool (isDeadHash, isDead);
		anim.SetBool (blockedAttackHash, blockedAttack);
		//anim.SetBool (continueAttackHash, continueAttack);	// Special case: we are setting this value in the attack state logic
	}

	/*
	 * CanRotate
	 * 
	 * This method determines when the character can rotate.
	 */
	public bool CanRotate()
	{
		bool canRotate = false;	// Initiate false

		// Handle rotatable states
		switch(playerState)
		{
		case PlayerState.Free:
			// Character can always rotate in free state
			canRotate = true;
			break;
		case PlayerState.Attacking:
			// Character can rotate briefly at the beginning of the attack
			if(attackType == AttackType.Basic && attackTimer <= 0.3f)
				canRotate = true;
			else if(attackType == AttackType.Heavy && attackTimer <= 0.5f)
				canRotate = true;
			else
				canRotate = false;
			break;
		case PlayerState.Blocking:
			// Character can rotate if not recovering from an attack
			if(!blockedAttack)
				canRotate = true;
			else
				canRotate = false;
			break;
		case PlayerState.Dodging:
			// Character can always rotate while dodging
			canRotate = true;
			break;
		case PlayerState.Flinching:
			// Character can never rotate while flinching
			canRotate = false;
			break;
		case PlayerState.Dead:
			// Character can never rotate while flinching
			canRotate = false;
			break;
		}

		return canRotate;
	}

	/* * * * * * * * * * * * * * * Audio Functions * * * * * * * * * * * * * * * */

	/*
	 * PlaySound
	 * 
	 * This function handles variables for sound transition.
	 */
	void PlaySound(AudioClip clip)
	{
		audioSource.Stop ();			// Stop current audio
		audioSource.clip = clip;		// Set current clip to clip
		audioSource.PlayOneShot (clip);	// Play clip
	}


	/* * * * * * * * * * * * * * * Movement Functions * * * * * * * * * * * * * * * */

	/*
	 * Step
	 * 
	 * This function handles walking up steps.
	 */
	void Step()
	{
		RaycastHit hit;

		if(Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z),
		                   new Vector3(rigidbody.velocity.x, 0.0f, rigidbody.velocity.z), out hit, 0.25f))
		{
			if(Physics.Raycast (new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z),
			                    new Vector3(rigidbody.velocity.x, 0.0f, rigidbody.velocity.z), out hit, 0.25f))
			{
				rigidbody.AddRelativeForce(new Vector3(0.0f, 100000.0f, 0.0f));
			}
		}
	}
}
