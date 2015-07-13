using UnityEngine;
using System.Collections;

public class PlayerMovement2D : MonoBehaviour
{
		private LevelLogic level_logic;
		private AudioSource audio_source;
		private AudioClip jump;
		private AudioClip enemy_death;
		LevelStats levelStats;
		private bool facingRight = true; // For determining which way the player is currently facing.


		[SerializeField]
		private LayerMask
				whatIsGround; // A mask determining what is ground to the character

		public bool grounded = true; // Whether or not the player is grounded.
		bool limit_jump = false;
		public float max_velocity = 10.0f;
		public float x_velocity = 0.0f;
		public float y_velocity = 0.0f;
		public float acceleration = 1.0f;
		public float drag = 0.2f;
		private float dir = 0.0f;
		public float jump_force = 20.0f;
		public float gravity = 3.0f;
		public bool wall_right = false;
		public bool wall_left = false;

		//public float terminal_velocity = 50.0f;
		public float kill_bounce = 10.0f;
		public float ray_length;
		public float ray_height;
		public float jump_timer = 0.2f;
		public float reduced_air_control = 2.0f;
		private Animator anim;
		private bool keyDown = false;

		private void Awake ()
		{
				jump = (AudioClip)Resources.Load ("SoundFX/player_jump");
				enemy_death = (AudioClip)Resources.Load ("SoundFX/enemy_death");
				levelStats = GameObject.Find ("LevelLogic").GetComponent<LevelStats> ();
				audio_source = GetComponent<AudioSource> ();
				ray_length = 0.3f; //GetComponent<Renderer> ().bounds.size.x / 2*5;
				ray_height = 0.40f;// (GetComponent<Renderer> ().bounds.size.y/2)*5;
				anim = GetComponent<Animator> ();
		}

		void Start ()
		{
				level_logic = GameObject.Find ("LevelLogic").GetComponent<LevelLogic> ();
		}

		private void FixedUpdate ()
		{

				//Debug.Log ("FIXED UPDATE");

				CastRays ();
				CheckFallDeath ();
				CheckHorizontalMovement ();
				CheckVerticalMovement ();
				Move ();

				//If not grounded, set jump animation
				if (!grounded) {
						anim.SetBool ("jumping", true);
						anim.SetBool ("moving", false);
						anim.SetBool ("idle", false);
						//If not moving set idle animation
				} else if (grounded && x_velocity != 0) {
						anim.SetBool ("moving", true);
						anim.SetBool ("jumping", false);
						anim.SetBool ("idle", false);
						//If moving set run animation
				} else {
						anim.SetBool ("idle", true);
						anim.SetBool ("jumping", false);
						anim.SetBool ("moving", false);
				}


		}

		private void CheckVerticalMovement ()
		{

				//If grounded, stop downwards movement
				if (grounded) {
						y_velocity = 0;
				} else {
						//If not grounded apply gravity
						y_velocity -= gravity;
				}

				if (Input.GetKey (KeyCode.UpArrow)) {
						//If grounded allow jump, eliminate holding jump button
						if (grounded && !keyDown) {
								//Apply vertical force
								y_velocity = jump_force;
								keyDown = true;
								levelStats.jumps++;

								//Play jump sound effect
								audio_source.PlayOneShot (jump, 10.0f);
						}
				} else {

						keyDown = false;
				}

		}

		private void CheckHorizontalMovement ()
		{
				if (Input.GetKey (KeyCode.RightArrow)) {

						if (grounded && !wall_right) {
								dir = 1.0f;
								if (!facingRight) {
										Flip ();
								}

								//If current velocity is under the maximum
								if (x_velocity < max_velocity) {
										//Apply acceleration each frame button is down
										x_velocity = x_velocity + acceleration;
								}

								levelStats.time_moving_right += Time.deltaTime;

						} else if (!grounded && !wall_right) {
								dir = 1.0f;

								if (x_velocity < max_velocity) {
										x_velocity = x_velocity + acceleration / reduced_air_control;
								}
								//moving = true;
								levelStats.time_moving_right += Time.deltaTime;
						}

				} else if (Input.GetKey (KeyCode.LeftArrow)) {

						if (grounded && !wall_left) {
								dir = -1.0f;
								if (facingRight) {
										Flip ();
								}
								if (x_velocity < max_velocity) {
										x_velocity = x_velocity + acceleration;
								}

								levelStats.time_moving_left += Time.deltaTime;

						} else if (!grounded && !wall_left) {
								dir = -1.0f;

								if (x_velocity < max_velocity) {
										x_velocity = x_velocity + acceleration / reduced_air_control;
								}
								levelStats.time_moving_right += Time.deltaTime;
						}
				} else {
						if (x_velocity > 0) {
								x_velocity = x_velocity - drag;
						} else {
								x_velocity = 0;
						}
				}

		}

		private void Move ()
		{
				//Calculate new position vector
				Vector3 newPos = new Vector3 (transform.position.x + x_velocity * Time.deltaTime * dir, transform.position.y + y_velocity * Time.deltaTime, transform.position.z);
				//Move to new position
				transform.position = newPos;

		}
		//Method to switch the players direction
		private void Flip ()
		{
				// Switch the way the player is labelled as facing.
				facingRight = !facingRight;

				// Multiply the player's x local scale by -1.
				Vector3 theScale = transform.localScale;
				theScale.x *= -1;
				transform.localScale = theScale;
		}

		int frame = 0;
		public	bool groundedR = true;
		public	bool groundedL = true;

		private void CastRays ()
		{
				float offsety = 0.0f;


				//Calculate Offsets
				float offsetx = x_velocity * Time.deltaTime;

				if (!grounded) {
						offsety = -y_velocity * Time.deltaTime;
				}

				//Numbe of rays to cast
				int no_of_rays = 8;
				//Single ray cast hit instance
				RaycastHit2D hit = new RaycastHit2D ();

				//Loop through each ray
				for (int i = 0; i < no_of_rays; i++) {

						//Check Right 1
						if (i == 0) {

								//Cast ray. with offset applied
								hit = Physics2D.Raycast (transform.position, Vector2.right, ray_length + offsetx, whatIsGround);

								if (hit.collider == null) {
										wall_right = false;
								} else if (hit.collider.tag == "Platform") {

										if (!wall_right) {
												Vector3 newPos = new Vector3 (hit.distance - ray_length, 0.0f, 0.0f);
												transform.Translate (newPos);
										}
										x_velocity = 0;
										wall_right = true;
								}
						}
						//Check Right 2
						if (i == 6) {
						
								Vector3 castPos = new Vector3 (transform.position.x, transform.position.y - (ray_height / 2), transform.position.z);

								hit = Physics2D.Raycast (castPos, Vector2.right, ray_length + offsetx, whatIsGround);

								if (hit.collider == null) {

										wall_right = false;
								} else if (hit.collider.tag == "Platform") {

										if (!wall_right) {
												Vector3 newPos = new Vector3 (hit.distance - ray_length, 0.0f, 0.0f);

												transform.Translate (newPos);
										}
										x_velocity = 0;
										wall_right = true;
								}
						}

						//Check Left
						if (i == 1) {
								hit = Physics2D.Raycast (transform.position, -Vector2.right, ray_length + offsetx, whatIsGround);
								if (hit.collider == null) {
										wall_left = false;
								} else if (hit.collider.tag == "Enemy") {
										Die ();
								} else if (hit.collider.tag == "Platform") {


										if (!wall_left) {
												Vector3 newPos = new Vector3 (-hit.distance + ray_length, 0.0f, 0.0f);
												Debug.Log ("MOVE TO" + newPos.ToString ());
												transform.Translate (newPos);
										}
										x_velocity = 0;
										wall_left = true;


								}
						}
						if (i == 7) {
								Vector3 castPos = new Vector3 (transform.position.x, transform.position.y - (ray_height / 2), transform.position.z);

								hit = Physics2D.Raycast (castPos, -Vector2.right, ray_length + offsetx, whatIsGround);

								if (hit.collider == null) {

										wall_left = false;
								} else if (hit.collider.tag == "Platform") {


										if (!wall_left) {
												Vector3 newPos = new Vector3 (-hit.distance + ray_length, 0.0f, 0.0f);
												Debug.Log ("MOVE TO" + newPos.ToString ());
												transform.Translate (newPos);
										}
										x_velocity = 0;
										wall_left = true;
		}
						}
						//Check Ceiling
						if (i == 2) {
								hit = Physics2D.Raycast (transform.position, Vector2.up, 0.5f, whatIsGround);

								if (hit.collider == null) {

								} else if (hit.collider.tag == "Enemy") {
										Debug.Log ("Enemy Hit");
										Die ();
								} else if (hit.collider.tag == "Platform") {

										y_velocity = 0;
								}

						}

						//Grounded Check
						if (i == 3) {

								Vector3 castPosL = new Vector3 (transform.position.x - 0.2f, transform.position.y, 0.0f);
								hit = Physics2D.Raycast (castPosL, -Vector2.up, ray_height + offsety, whatIsGround);


								if (hit.collider != null) {


										if (hit.collider.tag == "Platform") {

												Vector3 newPos = new Vector3 (0.0f, -hit.distance + ray_height, 0.0f);
												transform.Translate (newPos);
												groundedL = true;
												y_velocity = 0;
										}

								} else if (hit.collider == null) {

										groundedL = false;

								}
						}
						if (i == 4) {
								//Debug.Log ("GROUND CHECK1: " + frame.ToString() + ", " + offsety.ToString());
								Vector3 castPosR = new Vector3 (transform.position.x + 0.2f, transform.position.y, 0.0f);
								hit = Physics2D.Raycast (castPosR, -Vector2.up, ray_height + offsety, whatIsGround);

								if (hit.collider != null) {


										if (hit.collider.tag == "Platform") {

												Vector3 newPos = new Vector3 (0.0f, -hit.distance + ray_height, 0.0f);

												transform.Translate (newPos);

												groundedR = true;
												y_velocity = 0;

										}

								} else if (hit.collider == null) {
										//Debug.Log ("GROUND CHECK2" + frame.ToString() );
										//grounded = false;
										groundedR = false;
								}
						}
						if (!groundedL && !groundedR) {
								grounded = false;
						} else {
								grounded = true;
						}
				}

		}

		void OnTriggerEnter2D (Collider2D collider)
		{

				Debug.Log ("IN TRIGGGER" + collider.name);
				if (collider.name == "EndTrigger") {
						Debug.Log ("Level Finish Triggered");
						Application.LoadLevel ("TitleScreen");
				}
				if (collider.name == "EnemyKillCheck") {
						audio_source.PlayOneShot (enemy_death, 10.0f);
						//Debug.Log ("Enemy Killed by head");
						levelStats.kills++;
						y_velocity = 0;
						y_velocity += kill_bounce;
						//Debug.Log("Enemy Killed: " + collider.gameObject.transform.parent.gameObject.name);

						string enemyType = collider.gameObject.transform.parent.gameObject.name;
						switch (enemyType) {
						case "Walker":
								levelStats.kills_walker++;
								break;
						case "Jumper":
								levelStats.kills_jumper++;
								break;
						case "Shooter":
								levelStats.kills_shooter++;
								break;
						}

						Destroy (collider.gameObject.transform.parent.gameObject);

						//Application.LoadLevel("TitleScreen");

				}
				if (collider.tag == "bullet") {
						Debug.Log ("SHOT " + collider.name);
						Kill (Enemy.EnemyType.Shooter);
				}
				if (collider.tag == "finish") {
						Debug.Log ("LEVEL END");
						levelStats.DisplayLevelEnd ();
				}
		}

		private void CheckFallDeath ()
		{
				if (transform.position.y < -5) {
						levelStats.deaths_by_fall++;
						Die ();
				}

		}

		public void Kill (Enemy.EnemyType enemy)
		{
				//Increment player stat based on calling object
				switch (enemy) {
				case Enemy.EnemyType.Walker:
						levelStats.deaths_by_walker++;
						break;
				case Enemy.EnemyType.Jumper:
						levelStats.deaths_by_jumper++;
						break;
				case Enemy.EnemyType.Shooter:
						levelStats.deaths_by_shooter++;
						break;

				}
				Die ();

		}

		private void Die ()
		{
				levelStats.total_level_time += Time.timeSinceLevelLoad;
				levelStats.deaths++;
				Debug.Log ("Player Dead");
				//level_logic.Reload ();
				Application.LoadLevel (Application.loadedLevelName);
		}
}
