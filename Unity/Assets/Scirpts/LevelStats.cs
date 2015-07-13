using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelStats :MonoBehaviour
{
		//UI
		GameObject screen_ui;
		Text[] screen_text;
		Text start_text;
		Text control;
		Text current_level;
		Text current_time;
		private int start_pause = 100;
		private int start_counter = 0;
		bool ui_loaded = false;
		//Database
		private string db_url = "http://honours.hostoi.com/";
		LevelLogic level_logic;
		//Starting info
		public string name;   //From title screen
		public int difficulty; //From title screen
		
		//End Level Info
		
		public float difficulty_scale = 0.0f;
		

		//Level Generation Data==============================
		//Geometry
		public string Axiom = "AAAA";
		[HideInInspector]
		public const int
				gap_max_length = 7;
		[HideInInspector]
		public int
				gap_number = 5;
		[HideInInspector]
		public float
				gap_average_length = 2.5f;
		//Enemy variables
		[HideInInspector]
		public float
				enemy_speed = 1.5f;
		[HideInInspector]
		public float
				enemy_jump_power = 100.0f;
		[HideInInspector]
		public float
				enemy_jump_timer = 100.0f;
		[HideInInspector]
		public float
				enemy_shot_timer = 200.0f;
		[HideInInspector]
		public float
				enemy_shot_difficulty = 0;
		//Enemy numbers
		[HideInInspector]
		public int
				enemy_walk_no = 5;
		[HideInInspector]
		public int
				enemy_jump_no = 3;
		[HideInInspector]
		public int
				enemy_fly_no = 2;
		[HideInInspector]
		public int
				enemyNumberOf = 0;
		[HideInInspector]
		public float
				start_range = 10.0f;


		//minimums/Maximums
		private float enemy_jump_power_min = 100.0f;
		private float enemy_jump_power_max = 400.0f;


		//Give Up menu options
		public string level_give_up = "";


		//Play stats==================================
		public int jumps = 0;
		public int deaths = 0;
		public int deaths_by_fall = 0;
		[HideInInspector]
		public int
				deaths_by_walker = 0;
		[HideInInspector]
		public int
				deaths_by_jumper = 0;
		[HideInInspector]
		public int
				deaths_by_shooter = 0;
		[HideInInspector]
		public int
				kills = 0;
		[HideInInspector]
		public int
				kills_walker = 0;
		[HideInInspector]
		public int
				kills_jumper = 0;
		[HideInInspector]
		public int
				kills_shooter = 0;
		public float level_time = 0.0f;
		public float total_level_time = 0.0f;
		[HideInInspector]
		public float
				time_moving_right = 0.0f;
		[HideInInspector]
		public float
				time_moving_left = 0.0f;
		[HideInInspector]
		public float
				challenge_scale = 1.0f;
		public int previous_challenge = 3;
		[SerializeField]
		private  int
				level_number = 1;
		[SerializeField]
		private bool
				generate = false;
		public string id;
		

		private void Awake ()
		{
				DontDestroyOnLoad (this);

				id = PlayerPrefs.GetString ("id");
				//Debug.Log ("Level Stats Awake: Axiom " + Axiom.ToString ());
				name = PlayerPrefs.GetString ("name");
				difficulty = PlayerPrefs.GetInt ("difficulty");
		
				switch (difficulty) {
				case 2:
						difficulty_scale = 3.0f;
						break;
				case 0:
						difficulty_scale = 1.0f;
						break;
				default:
						difficulty_scale = 2.0f;
						break;
				}
		
		
				
				level_logic = GetComponent<LevelLogic> ();
				
			
		}

		private void Start ()
		{
				GenerateNextLevel ();
				//Debug.Log ("LevelStats Start: Axiom " + Axiom.ToString ());
				GetIU ();
				
	
		}

		private void GetIU ()
		{

				GiveUpScreen = (GameObject)Resources.Load ("Menus/GiveUpScreen");
				LevelEndScreen = (GameObject)Resources.Load ("Menus/LevelEndScreen");
				screen_ui = GameObject.Find ("Canvas");
				screen_text = screen_ui.GetComponentsInChildren<Text> ();
		
				for (int i = 0; i < screen_text.Length; i++) {
						if (screen_text [i].name == "Start Text") {
								start_text = screen_text [i];
						} else if (screen_text [i].name == "Control") {
								//	Debug.Log("control text found");
								control = screen_text [i];
						} else if (screen_text [i].name == "Level") {
//								Debug.Log("level text found");
								current_level = screen_text [i];
						} else if (screen_text [i].name == "Time") {
								//								Debug.Log("level text found");
								current_time = screen_text [i];
				
						}
			
				}
				if (current_level != null) {
						//	Debug.Log("cur level: " + level_number.ToString ());
						current_level.text = "Level " + level_number.ToString ();
				}
				if (current_time != null) {
						//	Debug.Log("cur level: " + level_number.ToString ());
						current_time.text = "Time ";
				}

	
		}

		public void GenerateNextLevel ()
		{
				//Debug.Log ("Generate Level: " + level_number.ToString ());

				Axiom = RandomAxiom ();
				Debug.Log ("Generate Level: " + level_number.ToString () + " Axiom: " + Axiom.ToString ());
				if (level_number == 1) {
						//Axiom = RandomAxiom ();
						challenge_scale *= difficulty_scale;
						IncreaseAll ();
		
		
				}


				if (level_number > 1) {

						switch (previous_challenge) {
						case 1:
								challenge_scale += 0.6f;
								IncreaseAll ();
								break;
						case 2:
								challenge_scale += 0.4f;
								//Increase Minimu deaths
								IncreaseMinDeaths ();
								break;
						case 3:
								challenge_scale += 0.2f;
								IncreaseAll ();


								break;
						case 4:
								//Select most deaths and reduce
								challenge_scale -= 0.4f;
								ReduceMostDeaths ();
								break;
						case 5:
								challenge_scale -= 0.6f;
								ReduceMostDeaths ();
								ReduceAll ();
								break;
						case 6:
								challenge_scale -= 0.1f;
								IncreaseAll ();
								break;

								if (challenge_scale > 4.8) {
										enemy_shot_difficulty = 1;
								} else {
										enemy_shot_difficulty = 0;
								}
		
						}

						jumps = 0;
						deaths = 0;
						deaths_by_fall = 0;
						deaths_by_walker = 0;
						deaths_by_jumper = 0;
						deaths_by_shooter = 0;
						kills = 0;
						kills_walker = 0;
						kills_jumper = 0;
						kills_shooter = 0;
						level_time = 0.0f;
						time_moving_right = 0.0f;
						time_moving_left = 0.0f;
						total_level_time = 0.0f;
						;
				}
				
			

				//enemy_shot_difficulty+= difficulty_scale;

				//current_level.text = "Level" + level_number.ToString();
				enemyNumberOf = enemy_walk_no + enemy_jump_no + enemy_fly_no;
			
	
		}
		
		bool menu = false;

		private void Update ()
		{
				if (Input.GetKey (KeyCode.Escape)) {
						Application.Quit ();		
						Debug.Log ("QUIT");
				}
				//	total_level_time += level_time;
				if (current_level == null) {
//			Debug.Log("UI not loaded");	
						GetIU ();
						//GenerateNextLevel();
				}
				if (current_time != null) {
						//	Debug.Log("cur level: " + level_number.ToString ());
						current_time.text = "Time: " + ((int)level_time).ToString ("D3");
				}
			

				level_time = Time.timeSinceLevelLoad;
				if (!menu) {
						//TEXT UI
						start_counter++;
						if (start_counter < start_pause) {

								string display = "Start \n Get to End!";
								Time.timeScale = 0.0f;
								start_text.text = display;
		
						} else {
								start_text.text = "";	
								Time.timeScale = 1.0f;
						}
						if (Input.GetKeyDown (KeyCode.X)) {
				
								Debug.Log ("Give up button pushed");
								control.text = "Press X to Restart";
								menu = true;
								Time.timeScale = 0.0f;
								DisplayGiveUp ();
						}
				} else {
						if (Input.GetKeyDown (KeyCode.X)) {
								if (control != null) {
										control.text = "Press X to Give Up";
								}			
								menu = false;
								Time.timeScale = 1.0f;
								Destroy (displayedMenu);
						}
		
				}
				if (Input.GetKeyDown (KeyCode.Y)) {
						DisplayLevelEnd ();
				}
				if (Input.GetKeyDown (KeyCode.P)) {
						Debug.Log ("Cureent LEvel: " + level_number.ToString ());
						//SaveData ();
						if (current_level != null) {
								current_level.text = "Level " + level_number.ToString ();
						}
				}
		}

		//Level Menu======================================================
		GameObject GiveUpScreen;
		GameObject LevelEndScreen;
		GameObject displayedMenu;
		string selected = "";		
	
		//GiveUpScreen
		Button btnSelect;
		Toggle[] toggle_group;
		Text text;
	
		public void DisplayGiveUp ()
		{
				displayedMenu = (GameObject)Instantiate (GiveUpScreen, GiveUpScreen.transform.position, GiveUpScreen.transform.rotation);
				toggle_group = displayedMenu.GetComponentsInChildren<Toggle> ();
		
		
				btnSelect = displayedMenu.GetComponentInChildren<Button> ();
				text = btnSelect.GetComponentInChildren<Text> ();
				//text.text = "dngklsnglknbn";
				btnSelect.onClick.AddListener (() => btnOnclick ()); 
		}

		public void DisplayLevelEnd ()
		{
				Debug.Log ("DISPLAY LEVEL END");
				menu = true;
				Time.timeScale = 0.0f;
				displayedMenu = (GameObject)Instantiate (LevelEndScreen, LevelEndScreen.transform.position, LevelEndScreen.transform.rotation);
				toggle_group = displayedMenu.GetComponentsInChildren<Toggle> ();
		
		
				btnSelect = displayedMenu.GetComponentInChildren<Button> ();
				text = btnSelect.GetComponentInChildren<Text> ();
				//text.text = "dngklsnglknbn";
				btnSelect.onClick.AddListener (() => btnOnclick ()); 
		}

		string level_status = "";

		public void btnOnclick ()
		{
				for (int i = 0; i < toggle_group.Length; i++) {
						if (toggle_group [i].isOn) {
				
								selected = toggle_group [i].name;
						}
				}

			
				if (displayedMenu.name == "GiveUpScreen(Clone)") {
		
						if (selected == "Too Hard") {
								Debug.Log ("Too Hard");
								previous_challenge = 5;
								level_status = "Too Hard";
								total_level_time += Time.timeSinceLevelLoad;
								SaveData ();
								//increase level
								level_number++;
			
								//Generate next level
								GenerateNextLevel ();
								//destroy menu
								Destroy (displayedMenu);
								//SaveData
								ui_loaded = false;
								level_logic.new_level = true;
								Time.timeScale = 1.0f;
								Application.LoadLevel (Application.loadedLevelName);
						} else if (selected == "Level Broken") {
								Debug.Log ("Level Broken");
								previous_challenge = 6;
								level_status = "Level Broken";
								//increase level
								total_level_time += Time.timeSinceLevelLoad;
								SaveData ();
								level_number++;

								//Generate next level
								GenerateNextLevel ();
								//destroy menu
								Destroy (displayedMenu);
								//SaveData
								ui_loaded = false;
								level_logic.new_level = true;
								Time.timeScale = 1.0f;
								Application.LoadLevel (Application.loadedLevelName);
						}

				} else {


						previous_challenge = int.Parse (selected);

						level_status = "Complete";
						total_level_time += Time.timeSinceLevelLoad;
						SaveData ();
						//increase level
						level_number++;
		
						//Generate next level
						GenerateNextLevel ();
						//destroy menu
						Destroy (displayedMenu);
						//displayedMenu.
						//SaveData
						
						Time.timeScale = 1.0f;
						ui_loaded = false;
						level_logic.new_level = true;
						Application.LoadLevel (Application.loadedLevelName);

						Debug.Log ("After ui_loaded");
		
				}


				//Debug.Log ("Button pressed: " + previous_challenge.ToString ());
		}

		//Database============================
		public void SaveData ()
		{
				StartCoroutine (SaveDatatoDB ());
		}
	
		IEnumerator SaveDatatoDB ()
		{
				string dbName = name;

				//Axiom = "ABCD";
				Debug.Log ("SENDING FORM");
				

				WWWForm form = new WWWForm ();

				form.AddField ("name", dbName);
				form.AddField ("axiom", Axiom);
				form.AddField ("experience", difficulty);
				form.AddField ("level-no", level_number.ToString ());

				form.AddField ("challenge-scale", challenge_scale.ToString ());
				form.AddField ("gap-number", gap_number.ToString ());
				form.AddField ("gap-average-length", gap_average_length.ToString ());

				form.AddField ("enemy-speed", enemy_speed.ToString ());
				form.AddField ("enemy-jump-power", enemy_jump_power.ToString ());
				form.AddField ("enemy-jump-timer", enemy_jump_timer.ToString ());
				form.AddField ("enemy-shot-timer", enemy_shot_timer.ToString ());
				form.AddField ("enemy-shot-difficulty", enemy_shot_difficulty.ToString ());
				form.AddField ("enemy-walk-no", enemy_walk_no.ToString ());
				form.AddField ("enemy-jump-no", enemy_jump_no.ToString ());
				form.AddField ("enemy-shoot-no", enemy_fly_no.ToString ());
				form.AddField ("enemy-total-no", enemyNumberOf.ToString ());

				form.AddField ("jumps", jumps.ToString ());
				form.AddField ("deaths-total", deaths.ToString ());
				form.AddField ("deaths-fall", deaths_by_fall.ToString ());
				form.AddField ("deaths-walker", deaths_by_walker.ToString ());
				form.AddField ("deaths-jumper", deaths_by_jumper.ToString ());
				form.AddField ("deaths-shooter", deaths_by_shooter.ToString ());

				form.AddField ("kills-total", kills.ToString ());
				form.AddField ("kills-walker", kills_walker.ToString ());
				form.AddField ("kills-jumper", kills_jumper.ToString ());
				form.AddField ("kills-shooter", kills_shooter.ToString ());

				form.AddField ("time-total", total_level_time.ToString ());
				form.AddField ("time-moving-right", time_moving_right.ToString ());
				form.AddField ("time_moving_left", time_moving_left.ToString ());

				form.AddField ("level-challenge", previous_challenge.ToString ());
				form.AddField ("level-status", level_status);
				form.AddField ("date-time", System.DateTime.UtcNow.ToString ());

				form.AddField ("play-id", id.ToString ());
				form.AddField ("attempt-time", level_time.ToString ());

				WWW webRequest = new WWW (db_url + "SaveLevel.php", form);
		
				yield return webRequest;
				Debug.Log (webRequest.text.ToString ());
		}

		private void IncreaseMinDeaths ()
		{
				int[] deaths = {
			deaths_by_fall,
			deaths_by_jumper,
			deaths_by_shooter,
			deaths_by_walker
		};
				int min = Mathf.Min (deaths);
				int result = 0;
		
		
				if (min == 0) {
						//Increase random factor
						Random.seed = enemyNumberOf;
						result = Random.Range (0, 4);
			
				} else {
						if (deaths_by_fall == min) {
								result = 0;
						}
			else if (deaths_by_jumper == min) {
								result = 1;
						}
			else if (deaths_by_shooter == min) {
								result = 2;
						}
			else if (deaths_by_walker == min) {
								result = 3;
						}
				}
		
				switch (result) {
				case 0:
			//Increase gaps
						gap_number += (int)challenge_scale;
						if (gap_average_length < gap_max_length - 0.5f) {
								gap_average_length += 0.2f;
						}
						break;
				case 1:
						enemy_jump_no += (int)challenge_scale;
			enemy_jump_power += (challenge_scale*2);
						enemy_jump_timer -= challenge_scale;
						break;
				case 2:
						enemy_fly_no += (int)Mathf.Abs (challenge_scale);
						enemy_shot_timer -= challenge_scale;
			enemy_speed += (int)Mathf.Abs (challenge_scale/10);
						break;
				case 3:
			enemy_speed += (int)Mathf.Abs (challenge_scale/10);
						enemy_walk_no += (int)Mathf.Abs (challenge_scale);
						break;
				}
		}
	
		private void ReduceMostDeaths ()
		{
		
				//Find highest cause of death
				int[] deaths = {
			deaths_by_fall,
			deaths_by_jumper,
			deaths_by_shooter,
			deaths_by_walker
		};
				int max = Mathf.Max (deaths);
		
				if (deaths_by_fall == max) {
						//Reduce gaps
			
						gap_number -= (int)challenge_scale;
						if (gap_average_length > 1.2f) {
								gap_average_length -= 0.1f;
						}
				}
		
				else if (deaths_by_jumper == max) {
						enemy_jump_no -= (int)challenge_scale;
			enemy_jump_power -= (challenge_scale*2);
						enemy_jump_timer += challenge_scale;
				}
		
		else if (deaths_by_shooter == max) {
						enemy_fly_no -= (int)Mathf.Abs (challenge_scale);
						enemy_shot_timer += challenge_scale;
			enemy_speed -= (int)Mathf.Abs (challenge_scale/10);
				}
		
		else if (deaths_by_walker == max) {
						enemy_walk_no -= (int)Mathf.Abs (challenge_scale);
			enemy_speed -= (int)Mathf.Abs (challenge_scale/10);
				}
		
		
		}
	
		private void ReduceAll ()
		{
				enemy_speed -= (int)Mathf.Abs (challenge_scale/10);
				enemy_jump_no -= (int)Mathf.Abs (challenge_scale);
				enemy_jump_power -= (challenge_scale*2);
				enemy_jump_timer += challenge_scale;
		
				enemy_fly_no -= (int)Mathf.Abs (challenge_scale);
				enemy_shot_timer += challenge_scale;
		
				enemy_walk_no -= (int)Mathf.Abs (challenge_scale);
		
				gap_number -= (int)challenge_scale;
				if (gap_average_length > 1.2f) {
						gap_average_length -= 0.1f;
				}
		
		
		}
	
		private void IncreaseAll ()
		{
		
				enemy_speed += (int)Mathf.Abs (challenge_scale/10);
				enemy_jump_no += (int)challenge_scale-1;
		enemy_jump_power += (challenge_scale*2);
				enemy_jump_timer -= challenge_scale;
		
				enemy_fly_no += (int)Mathf.Abs (challenge_scale)-1;
				enemy_shot_timer -= challenge_scale;
		
				enemy_walk_no += (int)Mathf.Abs (challenge_scale)-1;
		
		
				gap_number += (int)challenge_scale-1;
				if (gap_average_length < gap_max_length - 0.5f) {
						gap_average_length += 0.2f;
				}
		}

		private string RandomAxiom ()
		{
		
				string tempAxiom = "";
				Random.seed = (int)(System.DateTime.Now.Second);
		
				for (int i = 0; i < 4; i++) {
						int result = (int)Random.Range (0.0f, 5.0f);
						//						Debug.Log ("I - " + ((int)Random.Range (0.0f, 5.0f)).ToString ());
						switch (result) {
						case 0:
								tempAxiom += "A";
								break;
						case 1:
								tempAxiom += "B";
								break;
						case 2:
								tempAxiom += "C";
								break;
						case 3:
								tempAxiom += "D";
								break;
						default:
								tempAxiom += "A";
								break;
						}
				}
		
		
				//Debug.Log ("Generated axiom: " + tempAxiom + " Time: " + Time.realtimeSinceStartup.ToString());
				return Axiom = tempAxiom;
		}

}
