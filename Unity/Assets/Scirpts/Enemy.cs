//Class which is attached to the enemy gameObject
//Alters the appearance and behaviour of enemyObject based on 
//each instances EnemyType variable.

using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]

public class Enemy : MonoBehaviour
{	

	LevelStats level_stats;
	private AudioSource audio_source;
	private AudioClip shotFX;
	private AudioClip frogJumpFX;
	private Animator anim;
	private PlayerMovement2D player;
	private bool wall = false;
	private bool edge = false;

	//walker
	private float speed = 3.0f;
	private float move = -1.0f;

	//jumper
	private float jump_power = 250.0f;
	private float jump_timer = 100.0f;
	private float jump_counter; 

	//Shooter
	private float shot_radius = 0.3f;
	private float	shot_timer = 100.0f;
	private Transform groundCheck;
	private float groundedRadius = 0.2f;
	private bool grounded = false;
	public LayerMask	whatIsGround;
	public LayerMask	whatIsPlayer;
		
	//Pause enemy movement when shooting
	private int shot_pause = 10;
	private bool pause = false;

	//Current movement direction of enemy gameObject
	public enum MoveDirection
	{
		Left,
		Right
	}

	public enum EnemyType
	{
		Walker,
		Jumper,
		Shooter,
		Null // Null is required for generation method
	}
	
	public EnemyType enemyType;
	private MoveDirection moveDirection;
	private Vector2 enemyPos;
	private Vector3 edgeCheckLeft;
	private Vector3 edgeCheckRight;
	private Vector3 checkDistanceLeft;
	private Vector3 checkDistanceRight;

	void Awake ()
	{
		audio_source = GetComponent<AudioSource> ();
		shotFX = (AudioClip)Resources.Load ("SoundFX/enemy_shoot");
		frogJumpFX = (AudioClip)Resources.Load ("SoundFX/frog_jump");
		level_stats = GameObject.Find ("LevelLogic").GetComponent<LevelStats> ();
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerMovement2D> ();
		anim = GetComponent<Animator> ();			
	}

	void Start ()
	{

		speed = level_stats.enemySpeed;
		jump_power = level_stats.enemyJumpPower;
		jump_timer = level_stats.enemyJumpTimer;
		shot_timer = level_stats.enemyShotTimer;
		
		moveDirection = MoveDirection.Left;
		groundCheck = transform.Find ("GroundCheck");
		edgeCheckLeft = transform.Find ("EdgeCheckLeft").position;
		edgeCheckRight = transform.Find ("EdgeCheckRight").position;
		checkDistanceLeft = edgeCheckLeft - transform.position;
		checkDistanceRight = edgeCheckRight - transform.position;
		jump_counter = jump_timer;
		
		switch (enemyType) {
			
		case EnemyType.Walker:
			gameObject.name = "Walker";
			anim.SetBool ("walker", true);
			anim.SetBool ("flyer", false);
			anim.SetBool ("jumper_idle", false);
			anim.SetBool ("jumper_jump", false);
			Transform killcheck = transform.Find ("EnemyKillCheck");
			Vector3 newPos2 = new Vector3 (0.0f, 0.24f, 0.0f); 
			killcheck.Translate (newPos2);
			break;
		case EnemyType.Jumper:
			gameObject.name = "Jumper";
			anim.SetBool ("walker", false);
			anim.SetBool ("flyer", false);
			anim.SetBool ("jumper_idle", true);
			anim.SetBool ("jumper_jump", false);
			break;
		case EnemyType.Shooter:
			gameObject.name = "Shooter";
			anim.SetBool ("walker", false);
			anim.SetBool ("flyer", true);
			anim.SetBool ("jumper_idle", false);
			anim.SetBool ("jumper_jump", false);
			Vector3 newPos = new Vector3 (transform.position.x, transform.position.y + 1.0f, transform.position.z);
			transform.Translate (newPos);
			break;
		}
				
	}

	public void SetType (EnemyType typeIn)
	{
		this.enemyType = typeIn;
	}

	private void FixedUpdate ()
	{
		switch (enemyType) {
		case EnemyType.Walker:
			CheckCollisions ();
			Move_Walker ();
			break;
		case EnemyType.Jumper:
			CheckCollisions ();
			Move_Jumper ();
			break;
		case EnemyType.Shooter:
			//CheckCollisions();
			Move_Shooter ();
			break;
		}

		if (transform.position.y < -5.0f) { 
			Destroy (gameObject);
		}
	}

	private void CheckCollisions ()
	{
		RaycastHit2D hit = new RaycastHit2D ();
		
		if (moveDirection == MoveDirection.Left) {
			
			hit = new RaycastHit2D ();
			for (int i = 0; i < 3; i++) {
				
				if (i == 0) {
					
					GetComponent<Collider2D> ().enabled = false;
					hit = Physics2D.Raycast (transform.position + checkDistanceLeft, -Vector2.up, 0.5f);
					GetComponent<Collider2D> ().enabled = true;
					
					if (hit.collider == null) {
						edge = true;
					}
				} else if (i == 1) {
					
					GetComponent<Collider2D> ().enabled = false;
					hit = Physics2D.Raycast (transform.position + checkDistanceLeft, -Vector2.right, 0.1f);
					GetComponent<Collider2D> ().enabled = true;
					if (hit.collider != null) {
						
						if (hit.collider.tag == "Platform") {
							wall = true;
						}
						if (hit.collider.tag == "Enemy") {
							wall = true;
						}
						if (hit.collider.tag == "Player") {
							//Kill player
							player.Kill (this.enemyType);
						}
					}
				} else if (i == 2) {
					
					GetComponent<Collider2D> ().enabled = false;
					hit = Physics2D.Raycast (transform.position + checkDistanceRight, Vector2.right, 0.1f);
					GetComponent<Collider2D> ().enabled = true;
					if (hit.collider != null) {
						
						if (hit.collider.tag == "Player") {
							Debug.Log ("HIT: " + hit.collider.name);
							player.Kill (this.enemyType);
						}
					}
				}
				
				if (edge || wall) {
					switchDirection ();
				}
			}
		} else if (moveDirection == MoveDirection.Right) {
			
			hit = new RaycastHit2D ();

			for (int i = 0; i < 3; i++) {
				
				//Check downwards
				if (i == 0) {
					
					GetComponent<Collider2D> ().enabled = false;
					hit = Physics2D.Raycast (transform.position + checkDistanceRight, -Vector2.up, 0.5f);
					GetComponent<Collider2D> ().enabled = true;
					
					if (hit.collider == null) {
						
						edge = true;
					}
				}
				//Check to right
				else if (i == 1) {
					
					GetComponent<Collider2D> ().enabled = false;
					hit = Physics2D.Raycast (transform.position + checkDistanceRight, Vector2.right, 0.1f);
					GetComponent<Collider2D> ().enabled = true;
							
					if (hit.collider != null) {
						
						if (hit.collider.tag == "Platform") {		
							wall = true;
						} 
						if (hit.collider.tag == "Enemy") {
							wall = true;
						}
						if (hit.collider.tag == "Player") {
							player.Kill (this.enemyType);
						}
					}
				} else if (i == 2) {
					
					GetComponent<Collider2D> ().enabled = false;
					hit = Physics2D.Raycast (transform.position + checkDistanceLeft, -Vector2.right, 0.1f);
					GetComponent<Collider2D> ().enabled = true;
					
					
					if (hit.collider != null) {
						
						if (hit.collider.tag == "Player") {
							
							player.Kill (this.enemyType);
						}
					}
				}
			}
			if (edge || wall) {
				switchDirection ();
			}		
		}
	}

	private void Move_Walker ()
	{
		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		grounded = Physics2D.OverlapCircle (groundCheck.position, groundedRadius, whatIsGround);
				
		if (grounded) {
			GetComponent<Rigidbody2D> ().velocity = new Vector2 (move * speed, GetComponent<Rigidbody2D> ().velocity.y);
		} 
	}

	private void Move_Jumper ()
	{
	
		grounded = Physics2D.OverlapCircle (groundCheck.position, groundedRadius, whatIsGround);
		if (!grounded) {
			anim.SetBool ("jumper_idle", false);
			anim.SetBool ("jumper_jump", true);
		} else {
			anim.SetBool ("jumper_idle", true);
			anim.SetBool ("jumper_jump", false);
		}		
		jump_timer--;
		
		if (jump_timer < 0) {
			audio_source.PlayOneShot (frogJumpFX, 10.0f);
			GetComponent<Rigidbody2D> ().AddForce (new Vector2 (move * jump_power, jump_power));
			jump_timer = jump_counter;
		}
	}

	private void switchDirection ()
	{
		move *= -1;

		if (moveDirection == MoveDirection.Left) {
			moveDirection = MoveDirection.Right;
		} else {
			moveDirection = MoveDirection.Left;
		}

		wall = false;
		edge = false;

		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	private void Move_Shooter ()
	{
		RaycastHit2D hit = new RaycastHit2D ();
		Vector3 newPos = new Vector3 ();
		GetComponent<Rigidbody2D> ().isKinematic = true;

		if (pause) {
			shot_pause--;
			if (shot_pause <= 0) {
				pause = false;
				shot_pause = 10;
			}
		}


		if (moveDirection == MoveDirection.Left && !pause) {
			newPos = new Vector3 (-speed * Time.deltaTime, 0.0f, 0.0f);
		} 


		transform.Translate (newPos);
		shot_timer--;	

		if (shot_timer < 0) {
			pause = true;
			shot_timer = level_stats.enemyShotTimer;
			Shoot ();
		}


		hit = new RaycastHit2D ();
		for (int i = 0; i < 2; i++) {
		
			if (i == 0) {
			
				GetComponent<Collider2D> ().enabled = false;
				hit = Physics2D.Raycast (transform.position + checkDistanceLeft, -Vector2.right, 0.1f);
				GetComponent<Collider2D> ().enabled = true;
			
				if (hit.collider != null) {

					if (hit.collider.tag == "Player") {
						player.Kill (EnemyType.Shooter);
					}
				}
			} else if (i == 1) {
				GetComponent<Collider2D> ().enabled = false;
				hit = Physics2D.Raycast (transform.position + checkDistanceRight, Vector2.right, 0.1f);
				GetComponent<Collider2D> ().enabled = true;
				if (hit.collider != null) {
				
					if (hit.collider.tag == "Player") {
						Debug.Log ("HIT: " + hit.collider.name);
						player.Kill (EnemyType.Shooter);
					}
				}
			}
		}	
	}
	
	void Shoot ()
	{

		int shots = 4;

		if (level_stats.enemyShotDifficulty == 1) {
			shots = 8;
		}
				
		GameObject[] bullets = new GameObject[shots];
		Vector3[] shot_spawn = new Vector3[shots];
		Vector3[] shot_dir = new Vector3[shots];
		audio_source.PlayOneShot (shotFX, 10.0f);

		//Calculate spawn points surrounding enemy at moment of firing
		shot_spawn [0] = new Vector3 (transform.position.x, transform.position.y + shot_radius, 0.0f);
		shot_spawn [1] = new Vector3 (transform.position.x + shot_radius, transform.position.y, 0.0f);
		shot_spawn [2] = new Vector3 (transform.position.x, transform.position.y - shot_radius, 0.0f);
		shot_spawn [3] = new Vector3 (transform.position.x - shot_radius, transform.position.y, 0.0f);

		//Direction vectors for each spawn point
		shot_dir [0] = Vector3.up;
		shot_dir [1] = Vector3.right;
		shot_dir [2] = -Vector3.up;
		shot_dir [3] = -Vector3.right;
			
		if (level_stats.enemyShotDifficulty == 1) {

			shot_spawn [4] = new Vector3 (transform.position.x + shot_radius, transform.position.y + shot_radius, 0.0f);
			shot_spawn [5] = new Vector3 (transform.position.x + shot_radius, transform.position.y - shot_radius, 0.0f);
			shot_spawn [6] = new Vector3 (transform.position.x - shot_radius, transform.position.y - shot_radius, 0.0f);
			shot_spawn [7] = new Vector3 (transform.position.x - shot_radius, transform.position.y + shot_radius, 0.0f);

			
			shot_dir [4] = new Vector3 (1.0f, 1.0f, 0.0f);
			shot_dir [5] = new Vector3 (1.0f, -1.0f, 0.0f);
			shot_dir [6] = new Vector3 (-1.0f, -1.0f, 0.0f);
			shot_dir [7] = new Vector3 (-1.0f, 1.0f, 0.0f);
		}
				
		for (int i = 0; i < shots; i++) {
			//Create each bullet and pass information
			bullets [i] = Instantiate ((GameObject)Resources.Load ("Bullet/BulletPrefab"), shot_spawn [i], Quaternion.identity) as GameObject;
			bullets [i].GetComponent<Bullet> ().SetDir (shot_dir [i]);
				
		}
	}
}
