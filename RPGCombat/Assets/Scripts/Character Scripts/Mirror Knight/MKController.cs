using UnityEngine;
using System.Collections;

public class MKController : EnemyController {
	
	// State variables
	// TODO: Put these in the EnemyController base file
	public enum EnemyState { Free, Blocking, Attacking, RunningAttack, Flinching, Dodging, Dead };	// Enemy state enumeration
	public EnemyState enemyState;	// Enemy state variable
	
	Animator anim;	// Animator component
	
	EnemyHealth enemyHealth;	// Health and stamina script

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

	// Target variables
	GameObject target;					// Target game object
	KnightController targetController;	// Target's controller class

	// Character customization variables
	public float scale;
	public float pMoveSpeed;
	public float pTurnSpeed;
	public float pAttackDamageBasic;
	public float pAttackDamageHeavy;
	public Color pColor;
	
	// Movement variables
	float moveSpeed;		// Movement speed
	float hMovement;		// Horizontal movement
	float vMovement;		// Vertical movement
	Vector2 movement;		// Movement vector
	Vector2 dodgeDirection;	// Dodge vector
	Vector3 destination;	// Character destination
	float turnSpeed;		// Character's turn speed
	
	// Attack variables
	AttackType attackType;						// Class attack type variable
	float attackDamageBasic;					// Basic attack damage
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
	int attack2StateHash = Animator.StringToHash ("Base Layer.Attack2");
	
	// Timers
	float dodgeTimer;	// Dodge timer for applied force
	float attackTimer;	// Attack timer for exiting state and halting movement
	float blockTimer;	// Block timer for blocked attack recovery
	float flinchTimer;	// Flinch timer for exiting flinch state
	
	// Action times
	float timeDodge;			// Time it takes to dodge
	float timeFlinch;			// Time it takes to flinch
	float timeBasicAttack;		// Time it takes for basic attack
	float timeHeavyAttack;		// Time it takes for heavy attack
	float timeBlockedAttack;	// Time it takes to block an attack
	float timeInvincible;		// Time the character is invincible while dodging

	// AI timers
	float aiTimerAttack;		// Time to recover after performing an action
	float aiDelayAttack;		// Time to delay before character can attack again
	
	// State-specific variables
	bool attackRegistered;				// Used in the attack state logic to enable a delay timer after attack has registered
	bool attackAfterDodge;				// Used in the dodge state to transition smoothly from dodge to attack
	bool secondAttack;					// Used to test if the attack combo is on its second attack
	Vector3 runningAttackDestination;	// Used to determine the destination of the running attack
	float strafeDirection;				// Direction to strafe in free state

	// Sight variables
	float sightRadius;			// How far can the character see?
	float enableSightRadius;	// How far can the character see while inactive
	float sightAngle;			// What is the character's field of view when inactive

	// Active variable
	bool isActive;			// Determines whether or not the AI is enabled

	// Use this for initialization
	void Start () {
		
		anim = GetComponent<Animator> ();	// Get Animator component
		
		enemyHealth = GetComponent<EnemyHealth> ();	// Get EnemyHealth script component

		audioSource = GetComponent<AudioSource> ();

		// Character scale
		transform.localScale *= scale;

		target = GameObject.FindGameObjectWithTag ("Knight");		// Find 'Knight' game object
		targetController = target.GetComponent<KnightController>();	// Get the target's controller component
		
		enemyState = EnemyState.Free;	// Initiate player state to free

		// Audio variables
		audioTimer = 0.0f;

		// Movement variables
		moveSpeed = 160000.0f;					// Character movement speed
		hMovement = 0.0f;						// Horizontal movement
		vMovement = 0.0f;						// Vertical movement
		movement = Vector2.zero;				// Movement vector
		dodgeDirection = Vector2.zero;			// Dodge vector
		destination = transform.position;		// Character destination
		runningAttackDestination = destination;	// Character's running attack destination
		turnSpeed = 120.0f;						// Character's turn speed
		
		// Attacking variables
		attackType = AttackType.Basic;	// Initate attack type to  basic
		attackDamageBasic = 25.0f;		// Set basic damage
		attackDamageHeavy = 40.0f;		// Set heavy damage
		attackRange = 5.0f;				// Attack range set to 3
		attackAngle = 120.0f;			// 120 degree attack angle
		
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

		// AI timers
		aiTimerAttack = 2.0f;
		aiDelayAttack = 3.0f;	// Delay attack 3.0 seconds
		
		// Action times
		timeDodge = 0.4f;
		timeFlinch = 0.8f;
		timeBasicAttack = 0.65f;
		timeHeavyAttack = 1.1f;
		timeBlockedAttack = 0.3f;
		timeInvincible = 0.25f;
		
		// State-specific variables
		attackRegistered = false;
		attackAfterDodge = false;
		secondAttack = false;
		strafeDirection = 0.0f;	// Initiate no strafing

		// Sight variables
		sightRadius = 20.0f;		// Sight radius is 20 when active
		enableSightRadius = 10.0f;	// Sight radius is 10 when inactive
		sightAngle = 90.0f;			// Sight angle is 45 degrees in either direction

		// Scaling variables
		moveSpeed *= pMoveSpeed;
		turnSpeed *= pTurnSpeed;
		attackDamageBasic *= pAttackDamageBasic;
		attackDamageHeavy *= pAttackDamageHeavy;
		attackRange *= scale;

		SetColor (pColor);
	}
	
	// Update is called once per frame
	void Update () {

		if(!isActive)
		{
			EnableAI();
		}

		// Handle states and AI if character is active
		if(isActive)
		{
			AITimers ();	// Handle AI timers
			AILogic ();	// Handle AI decision making
			
			// State handler
			switch(enemyState)
			{
			case  EnemyState.Free:
				FreeLogic();
				break;
			case EnemyState.Blocking:
				BlockingLogic();
				break;
			case EnemyState.Attacking:
				AttackingLogic();
				break;
			case EnemyState.RunningAttack:
				RunningAttackLogic();
				break;
			case EnemyState.Dodging:
				DodgingLogic();
				break;
			case EnemyState.Flinching:
				FlinchingLogic();
				break;
			case EnemyState.Dead:
				DeadLogic();
				break;
			}
		}
		
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
				if(isSprinting)
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
			ChangeToState(EnemyState.Free);
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
		
		// TODO: Enable character turning for the first half of the attack
		
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
			// Attack
			else
			{
				// Determine random attack time
				aiDelayAttack = Random.Range (2.5f, 4.0f);	// 2.5 - 4 seconds

				// Determine attack damage
				float currentDamage = 0.0f;

				if(attackType == AttackType.Basic)
				{
					// Audio
					PlaySound(audioSwingBasic);

					currentDamage = attackDamageBasic;
				}
				else if(attackType == AttackType.Heavy)
				{
					// Audio
					PlaySound(audioSwingOverhead);

					currentDamage = attackDamageHeavy;
				}

				// Register attack
				attackRegistered = true;	// Attack was registered 

				GameObject[] knights = GameObject.FindGameObjectsWithTag("Knight");
				foreach(GameObject knight in knights)
				{
					Vector3 direction = knight.transform.position - transform.position;	// Get the direction to the enemy
					float angle = Vector3.Angle(direction, transform.forward);	// Get the angle to the enemy
					
					// If the enemy is within the attack FOV and within range of attack
					if(angle < attackAngle * 0.5 && (knight.transform.position - transform.position).magnitude <= attackRange)
					{
						RaycastHit hit;
						
						// Check if the 
						if(Physics.Raycast (transform.position + transform.up, direction.normalized, out hit, attackRange))
						{
							// Target isn't being obstructed by an object
							if(hit.collider.gameObject == knight)
							{
								Debug.Log ("Knight hit!");
								
								knight.GetComponent<KnightController>().TakeHit (transform.position, currentDamage, attackType);
							}
						}
					}
				}
			}
		}
		// .3 second delay before automatic change to free state
		else if(attackType == AttackType.Basic && attackTimer >= timeBasicAttack + 0.45f)
		{
			// Check if combo is active
			if(continueAttack)
			{
				ChangeToState (EnemyState.Attacking);
				
				anim.SetBool (continueAttackHash, continueAttack);	// Change to next attack
			}
			else
				ChangeToState (EnemyState.Free);	// Refer back to free state
		}
		// .5 second delay before atomatic change to free state
		else if(attackType == AttackType.Heavy && attackTimer >= timeHeavyAttack + 0.45f)
		{
			ChangeToState (EnemyState.Free);
		}
	}

	// Running attack state logic
	void RunningAttackLogic()
	{
		SetMovement (0.0f, 1.0f);	// Set movement forward

		movement *= 6.0f;

		// Move the character
		rigidbody.AddRelativeForce (new Vector3 (movement.x, 0.0f, movement.y) * Time.deltaTime * moveSpeed);

		if(DistanceTo (runningAttackDestination) < attackRange * 0.66f)
		{
			ChangeToState (EnemyState.Attacking);
			SetMovement (0.0f, 0.0f);
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
			ChangeToState (EnemyState.Attacking);
		}
		// Dodge complete after
		else if(dodgeTimer > timeDodge + 0.15f)
		{
			ChangeToState (EnemyState.Free);	// Change to free state
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

		flinchTimer += Time.deltaTime;
		
		if(flinchTimer > timeFlinch + 0.3f)
		{
			ChangeToState (EnemyState.Free);	// Change to free state
		}
	}
	
	// Dead state logic
	void DeadLogic()
	{
		if(isDying && anim.GetCurrentAnimatorStateInfo (0).nameHash == dyingStateHash)
		{
			// Audio
			PlaySound (audioDying);

			isDying = false;	// Avoid death loop
		}
	}
	
	
	/* * * * * * * * * * * * * * * * * State Handling * * * * * * * * * * * * * * * * */
	
	/*
	 * ChangeToState
	 * 
	 * Set variables for state change.
	 */
	void ChangeToState(EnemyState state)
	{
		// Audio timer
		audioTimer = 0.0f;
		audioSource.clip = null;

		switch(state)
		{
		case  EnemyState.Free:
			FreeStateChange();
			break;
		case EnemyState.Blocking:
			BlockingStateChange();
			break;
		case EnemyState.Attacking:
			AttackingStateChange();
			break;
		case EnemyState.RunningAttack:
			RunningAttackStateChange();
			break;
		case EnemyState.Dodging:
			DodgingStateChange();
			break;
		case EnemyState.Flinching:
			FlinchingStateChange();
			break;
		case EnemyState.Dead:
			DeadStateChange();
			break;
		}
	}
	
	// Change to free state
	void FreeStateChange()
	{
		enemyState = EnemyState.Free;
		
		isAttacking = false;
		isDodging = false;
		isFlinching = false;
		isBlocking = false;
		blockedAttack = false;

		// Set strafe
		strafeDirection = Random.Range (-1, 2);
	}
	
	// Change to blocking state
	void BlockingStateChange()
	{
		enemyState = EnemyState.Blocking;
		
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
		enemyState = EnemyState.Attacking;

		isAttacking = true;
		isDodging = false;
		isSprinting = false;
		isFlinching = false;
		isBlocking = false;
		blockedAttack = false;
		
		attackRegistered = false;
		secondAttack = false;
		
		attackTimer = 0.0f;

		aiDelayAttack = Random.Range (1.5f, 3.0f);
	}

	// Change to running attack state
	void RunningAttackStateChange()
	{
		enemyState = EnemyState.RunningAttack;

		isAttacking = false;
		isDodging = false;
		isSprinting = true;
		isFlinching = false;
		isBlocking = false;
		blockedAttack = false;

		movingForward = true;
		isIdle = false;

		runningAttackDestination = target.transform.position;	// Set destination to current target position
		transform.LookAt (runningAttackDestination);	// Look at destination
	}
	
	// Change to dodging state
	void DodgingStateChange()
	{
		enemyState = EnemyState.Dodging;
		
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
		enemyState = EnemyState.Flinching;
		
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
		enemyState = EnemyState.Dead;
		
		isDying = true;
		isDead = true;
		isFlinching = false;
		isAttacking = false;
		isDodging = false;
		isSprinting = false;
		isBlocking = false;
		blockedAttack = false;
	}


	/* * * * * * * * * * * * * * * AI Logic * * * * * * * * * * * * * * * * * * * */

	/*
	 * AITimers
	 * 
	 * This method handles the AI timers that determine when actions can be performed.
	 */
	void AITimers()
	{
		if(enemyState != EnemyState.Attacking)
		{
			aiTimerAttack += Time.deltaTime;
		}
		else
		{
			aiTimerAttack = 0.0f;
		}
	}

	/*
	 * AILogic
	 * 
	 * This method handles the decision making for the character.
	 */
	void AILogic()
	{
		CanSee ();	// Check if player can be seen and set desination

		// Set destination to running attack destination if in running attack state
		if(enemyState == EnemyState.RunningAttack)
		{
			destination = runningAttackDestination;
		}

		// Face the destination
		if(CanRotate ())
		{
			Quaternion q = Quaternion.LookRotation ((new Vector3(destination.x, transform.position.y, destination.z)) - transform.position);

			transform.rotation = Quaternion.RotateTowards (transform.rotation, q, turnSpeed * Time.deltaTime);

			//transform.LookAt (new Vector3(destination.x, transform.position.y, destination.z));
		}

		// Handle AI for specific states
		switch(enemyState)
		{
		case EnemyState.Free:
			AIFree ();
			break;
		case EnemyState.Attacking:
			AIAttacking();
			break;
		case EnemyState.RunningAttack:
			AIRunningAttack();
			break;
		case EnemyState.Blocking:
			AIBlocking ();
			break;
		case EnemyState.Dodging:
			AIDodging ();
			break;
		}
	}

	// AI while free
	void AIFree()
	{
		// TODO: Move towards player until 2.0f
		// TODO: Wait until magnitude is greater than 4.0f to move again
		if((destination - transform.position).magnitude > attackRange * 0.5f)
		{
			SetMovement (0.0f, 1.0f);	// Moving forward towards destination
		}
		else
		{
			// SetMovement (0.0f, 0.0f);	// Stop moving
			SetMovement (strafeDirection, 0.0f);	// Strafe
		}

		// Attack if within range && rested
		if(DistanceTo (target.transform.position) < attackRange * 0.66f && aiTimerAttack > aiDelayAttack)
		{
			ChangeToState (EnemyState.Attacking);
		}
	}

	// AI while attacking
	void AIAttacking()
	{
		if(attackType == AttackType.Basic && attackTimer > timeBasicAttack)
		{
			if(!secondAttack)
				secondAttack = anim.GetCurrentAnimatorStateInfo(0).nameHash == attack2StateHash;

			if(DistanceTo (target.transform.position) < attackRange && !secondAttack)
				continueAttack = true;
			else if(secondAttack)
				continueAttack = false;
		}
		else if(attackType == AttackType.Heavy && attackTimer > timeHeavyAttack)
		{

		}
	}

	// AI running attack
	void AIRunningAttack()
	{

	}

	// AI while blocking
	void AIBlocking()
	{

	}

	// AI while dodging
	void AIDodging()
	{

	}
	
	
	/* * * * * * * * * * * * * * * Helper Functions * * * * * * * * * * * * * * * */

	/*
	 * SetColor
	 * 
	 * This method sets the character's armor color.
	 */
	public void SetColor(Color color)
	{
		transform.GetChild (1).renderer.material.color = color;
	}

	/*
	 * IsDead
	 * 
	 * Is the character dead?
	 */
	public bool IsDead()
	{
		return isDead;
	}

	/* DistanceTo
	 * 
	 * Find the distance to an object.
 */
	float DistanceTo(Vector3 pos)
	{
		return (pos - transform.position).magnitude;
	}

	/*
	 * SetMovement
	 * 
	 * This method sets the movement variables
	 */
	void SetMovement(float x, float y)
	{
		hMovement = x;
		vMovement = y;
		movement = new Vector2 (x, y);
	}

	/*
	 * TakeHit
	 * 
	 * This method handles logic from being being attacked.
	 */
	public override void TakeHit(Vector3 origin, float damage, AttackType type)
	{
		// If character is not yet active, become active
		if(!isActive)
		{
			isActive = true;
		}

		// Handle flinch-capable states
		switch(enemyState)
		{
		case EnemyState.Free:
			// Character can always take damage in free state
			TakeDamage (damage);
			break;
		case EnemyState.Blocking:
			// Is the character blocking in the direction of the attack?
			if(CanBlock(origin))
			{
				// Character can only lose stamina when not already blocking an attack
				if(!blockedAttack)
					blockedAttack = true;
			}
			// Character facing the wrong direction
			else
			{
				TakeDamage (damage);
			}
			break;
		case EnemyState.Attacking:
			// Character can always take damage while attacking
			TakeDamage (damage);
			break;
		case EnemyState.Dodging:
			// Is the character invincible?
			if(dodgeTimer > timeInvincible)
				TakeDamage (damage);
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
	 * TakeDamage
	 * 
	 * This method handles any damage dealt to the player.
	 */
	void TakeDamage(float damage)
	{
		// Audio
		PlaySound (audioFlinch);

		enemyHealth.SetHealth (enemyHealth.GetHealth () - damage);

		// Check death state
		if (enemyHealth.GetHealth () <= 0.0f)
			ChangeToState (EnemyState.Dead);

		Debug.Log (damage + " damage received... " + enemyHealth.GetHealth () + " health remaining.");
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
		// Increase turn speed for attack state
		if(enemyState == EnemyState.Attacking)
		{
			turnSpeed = 360.0f;
		}
		else
		{
			turnSpeed = 120.0f;
		}

		bool canRotate = false;	// Initiate false
		
		// Handle rotatable states
		switch(enemyState)
		{
		case EnemyState.Free:
			// Character can always rotate in free state
			canRotate = true;
			break;
		case EnemyState.Attacking:
			// Character can rotate briefly at the beginning of the attack
			if(attackType == AttackType.Basic && attackTimer <= 0.3f)
				canRotate = true;
			else if(attackType == AttackType.Heavy && attackTimer <= 0.5f)
				canRotate = true;
			else
				canRotate = false;
			break;
		case EnemyState.Blocking:
			// Character can rotate if not recovering from an attack
			if(!blockedAttack)
				canRotate = true;
			else
				canRotate = false;
			break;
		case EnemyState.Dodging:
			// Character can always rotate while dodging
			canRotate = true;
			break;
		case EnemyState.Flinching:
			// Character can never rotate while flinching
			canRotate = false;
			break;
		case EnemyState.Dead:
			// Character can never rotate while flinching
			canRotate = false;
			break;
		}
		
		return canRotate;
	}

	/*
	 * CanSee
	 * 
	 * This method uses raycasting to find the target's location.
	 * The target's location is used to move the character and set
	 * attack destinations.
	 */
	bool CanSee()
	{
		Vector3 direction = target.transform.position - transform.position;

		RaycastHit hit;

		// 360 degree FOV initially
		if(Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, sightRadius))
		{
			if(hit.collider.gameObject == target)
			{
				destination = target.transform.position;

				return true;
			}
		}

		return false;
	}

	// Get target
	public GameObject GetTarget()
	{
		return target;
	}

	/*
	 * CanEnableAI
	 * 
	 * This method is used to enable the AI if the player is within the character's
	 * field of view or is too close to the character.
	 */
	void EnableAI()
	{
		Vector3 direction = target.transform.position - transform.position;
		float angle = Vector3.Angle (direction, transform.forward);

		// If the target is within the field of view
		if(angle < sightAngle * 0.5)
		{
			RaycastHit hit;

			// Check if the target is within sight range and not obstructed by other objects
			if(Physics.Raycast (transform.position + transform.up, direction.normalized, out hit, enableSightRadius))
			{
				if(hit.collider.gameObject == target)
				{
					Debug.Log ("Knight spotted!");

					isActive = true;
				}
			}
		}
		// If the target is within 1 unit from the character and no objects are obstructing it
		else if(DistanceTo(target.transform.position) < 1.0f)
		{
			if(CanSee ())
			{
				isActive = true;
			}
		}
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
