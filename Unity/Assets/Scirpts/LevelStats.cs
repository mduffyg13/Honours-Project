//Class which handles the collection of level and player data and contain functionality for uploading to database.
//Also contains functionality which generates the next levels generation data.
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelStats :MonoBehaviour
{
	LevelLogic levelLogic;

	//UI
	GameObject screenUI;
	Text[] screenText;
	Text startText;
	Text control;
	Text currentLevel;
	Text currentTime;
	private int startPause = 100;
	private int startCounter = 0;
	bool isUiLoaded = false;


	//Database Access
	private string dbUrl = "http://honours.hostoi.com/";
		
		
	//Starting info from title screen
	public string name;
	public int difficulty;
	public string id;

	//End Level Info
	public float difficultyScale = 0.0f;
		
	//Give Up menu options
	public string levelGiveUpOption = "";

	//Level Generation Data==============================

	//Geometry
	public string Axiom = "AAAA";
	[HideInInspector]
	public const int
		gapMaxLength = 7;
	[HideInInspector]
	public int
		gapNumber = 5;
	[HideInInspector]
	public float
		gapAverageLength = 2.5f;

	//Enemy variables
	[HideInInspector]
	public float
		enemySpeed = 1.5f;
	[HideInInspector]
	public float
		enemyJumpPower = 100.0f;
	[HideInInspector]
	public float
		enemyJumpTimer = 100.0f;
	[HideInInspector]
	public float
		enemyShotTimer = 200.0f;
	[HideInInspector]
	public float
		enemyShotDifficulty = 0;

	//Enemy amounts
	[HideInInspector]
	public int
		enemyWalkNo = 5;
	[HideInInspector]
	public int
		enemyJumpNo = 3;
	[HideInInspector]
	public int
		enemyFlyNo = 2;
	[HideInInspector]
	public int
		enemyNumberOf = 0;
	[HideInInspector]
	public float
		startRange = 10.0f;


	//minimums/Maximums
	private float enemyJumpPowerMin = 100.0f;
	private float enemyJumpPowerMax = 400.0f;

	
	//Player stats==================================
	[HideInInspector]
	public int
		jumps = 0;
	[HideInInspector]
	public int deaths = 0;
	[HideInInspector]
	public int deathsByFall = 0;
	[HideInInspector]
	public int
		deathsByWalker = 0;
	[HideInInspector]
	public int
		deathsByJumper = 0;
	[HideInInspector]
	public int
		deathsByShooter = 0;
	[HideInInspector]
	public int
		killsTotal = 0;
	[HideInInspector]
	public int
		killsWalker = 0;
	[HideInInspector]
	public int
		killsJumper = 0;
	[HideInInspector]
	public int
		killsShooter = 0;
	public float levelTime = 0.0f;
	public float totalLevelTime = 0.0f;
	[HideInInspector]
	public float
		timeMovingRight = 0.0f;
	[HideInInspector]
	public float
		timeMovingLeft = 0.0f;
	[HideInInspector]
	public float
		challengeScale = 1.0f;
	public int previousChallenge = 3;
	[SerializeField]
	private  int
		levelNumber = 1;
	[SerializeField]
	private bool
		generate = false;
	bool menu = false;
	string level_status = "";

	private void Awake ()
	{
		DontDestroyOnLoad (this);

		id = PlayerPrefs.GetString ("id");
		name = PlayerPrefs.GetString ("name");
		difficulty = PlayerPrefs.GetInt ("difficulty");
		
		switch (difficulty) {
		case 2:
			difficultyScale = 3.0f;
			break;
		case 0:
			difficultyScale = 1.0f;
			break;
		default:
			difficultyScale = 2.0f;
			break;
		}

		levelLogic = GetComponent<LevelLogic> ();
	}

	private void Start ()
	{
		GenerateNextLevel ();
		GetIU ();
	}

	private void GetIU ()
	{

		GiveUpScreen = (GameObject)Resources.Load ("Menus/GiveUpScreen");
		LevelEndScreen = (GameObject)Resources.Load ("Menus/LevelEndScreen");
		screenUI = GameObject.Find ("Canvas");
		screenText = screenUI.GetComponentsInChildren<Text> ();
		
		for (int i = 0; i < screenText.Length; i++) {
			if (screenText [i].name == "Start Text") {
				startText = screenText [i];
			} else if (screenText [i].name == "Control") {
				control = screenText [i];
			} else if (screenText [i].name == "Level") {
				currentLevel = screenText [i];
			} else if (screenText [i].name == "Time") {
				currentTime = screenText [i];
			}
		}
		if (currentLevel != null) {
			currentLevel.text = "Level " + levelNumber.ToString ();
		}
		if (currentTime != null) {
			currentTime.text = "Time ";
		}

	
	}

	public void GenerateNextLevel ()
	{
		Axiom = GenerateRandomAxiom ();

		if (levelNumber == 1) {
			challengeScale *= difficultyScale;
			IncreaseAll ();
		}


		if (levelNumber > 1) {

			switch (previousChallenge) {
			case 1:
				challengeScale += 0.6f;
				IncreaseAll ();
				break;
			case 2:
				challengeScale += 0.4f;
								//Increase Minimu deaths
				IncreaseMinDeaths ();
				break;
			case 3:
				challengeScale += 0.2f;
				IncreaseAll ();


				break;
			case 4:
								//Select most deaths and reduce
				challengeScale -= 0.4f;
				ReduceMostDeaths ();
				break;
			case 5:
				challengeScale -= 0.6f;
				ReduceMostDeaths ();
				ReduceAll ();
				break;
			case 6:
				challengeScale -= 0.1f;
				IncreaseAll ();
				break;

				if (challengeScale > 4.8) {
					enemyShotDifficulty = 1;
				} else {
					enemyShotDifficulty = 0;
				}
		
			}

			jumps = 0;
			deaths = 0;
			deathsByFall = 0;
			deathsByWalker = 0;
			deathsByJumper = 0;
			deathsByShooter = 0;
			killsTotal = 0;
			killsWalker = 0;
			killsJumper = 0;
			killsShooter = 0;
			levelTime = 0.0f;
			timeMovingRight = 0.0f;
			timeMovingLeft = 0.0f;
			totalLevelTime = 0.0f;

		}
		enemyNumberOf = enemyWalkNo + enemyJumpNo + enemyFlyNo;
	}
			
	private void Update ()
	{
		levelTime = Time.timeSinceLevelLoad;

		if (Input.GetKey (KeyCode.Escape)) {
			Application.Quit ();		
			Debug.Log ("QUIT");
		}
		if (currentLevel == null) {
			GetIU ();
		}
		if (currentTime != null) {
			currentTime.text = "Time: " + ((int)levelTime).ToString ("D3");
		}
			
		if (!menu) {
			//TEXT UI
			startCounter++;
			if (startCounter < startPause) {
				string display = "Start \n Get to End!";
				Time.timeScale = 0.0f;
				startText.text = display;
		
			} else {
				startText.text = "";	
				Time.timeScale = 1.0f;
			}
			if (Input.GetKeyDown (KeyCode.X)) {
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
			if (currentLevel != null) {
				currentLevel.text = "Level " + levelNumber.ToString ();
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
		btnSelect.onClick.AddListener (() => btnOnclick ()); 
	}

	public void DisplayLevelEnd ()
	{
		menu = true;
		Time.timeScale = 0.0f;
		displayedMenu = (GameObject)Instantiate (LevelEndScreen, LevelEndScreen.transform.position, LevelEndScreen.transform.rotation);
		toggle_group = displayedMenu.GetComponentsInChildren<Toggle> ();

		btnSelect = displayedMenu.GetComponentInChildren<Button> ();
		text = btnSelect.GetComponentInChildren<Text> ();
		btnSelect.onClick.AddListener (() => btnOnclick ()); 
	}



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
				previousChallenge = 5;
				level_status = "Too Hard";
				totalLevelTime += Time.timeSinceLevelLoad;
				SaveData ();
				//increase level
				levelNumber++;
			
				//Generate next level
				GenerateNextLevel ();
				//destroy menu
				Destroy (displayedMenu);
				//SaveData
				isUiLoaded = false;
				levelLogic.new_level = true;
				Time.timeScale = 1.0f;
				Application.LoadLevel (Application.loadedLevelName);
			} else if (selected == "Level Broken") {
				Debug.Log ("Level Broken");
				previousChallenge = 6;
				level_status = "Level Broken";
				//increase level
				totalLevelTime += Time.timeSinceLevelLoad;
				SaveData ();
				levelNumber++;

				GenerateNextLevel ();
				Destroy (displayedMenu);
				isUiLoaded = false;
				levelLogic.new_level = true;
				Time.timeScale = 1.0f;
				Application.LoadLevel (Application.loadedLevelName);
			}

		} else {

			previousChallenge = int.Parse (selected);

			level_status = "Complete";
			totalLevelTime += Time.timeSinceLevelLoad;
			SaveData ();
			//increase level
			levelNumber++;
			//Generate next level
			GenerateNextLevel ();
			//destroy menu
			Destroy (displayedMenu);

			Time.timeScale = 1.0f;
			isUiLoaded = false;
			levelLogic.new_level = true;
			Application.LoadLevel (Application.loadedLevelName);
		}
	}

	//Database============================
	public void SaveData ()
	{
		StartCoroutine (SaveDatatoDB ());
	}
	
	IEnumerator SaveDatatoDB ()
	{
		string dbName = name;
	
		WWWForm form = new WWWForm ();

		form.AddField ("name", dbName);
		form.AddField ("axiom", Axiom);
		form.AddField ("experience", difficulty);
		form.AddField ("level-no", levelNumber.ToString ());

		form.AddField ("challenge-scale", challengeScale.ToString ());
		form.AddField ("gap-number", gapNumber.ToString ());
		form.AddField ("gap-average-length", gapAverageLength.ToString ());

		form.AddField ("enemy-speed", enemySpeed.ToString ());
		form.AddField ("enemy-jump-power", enemyJumpPower.ToString ());
		form.AddField ("enemy-jump-timer", enemyJumpTimer.ToString ());
		form.AddField ("enemy-shot-timer", enemyShotTimer.ToString ());
		form.AddField ("enemy-shot-difficulty", enemyShotDifficulty.ToString ());
		form.AddField ("enemy-walk-no", enemyWalkNo.ToString ());
		form.AddField ("enemy-jump-no", enemyJumpNo.ToString ());
		form.AddField ("enemy-shoot-no", enemyFlyNo.ToString ());
		form.AddField ("enemy-total-no", enemyNumberOf.ToString ());

		form.AddField ("jumps", jumps.ToString ());
		form.AddField ("deaths-total", deaths.ToString ());
		form.AddField ("deaths-fall", deathsByFall.ToString ());
		form.AddField ("deaths-walker", deathsByWalker.ToString ());
		form.AddField ("deaths-jumper", deathsByJumper.ToString ());
		form.AddField ("deaths-shooter", deathsByShooter.ToString ());

		form.AddField ("kills-total", killsTotal.ToString ());
		form.AddField ("kills-walker", killsWalker.ToString ());
		form.AddField ("kills-jumper", killsJumper.ToString ());
		form.AddField ("kills-shooter", killsShooter.ToString ());

		form.AddField ("time-total", totalLevelTime.ToString ());
		form.AddField ("time-moving-right", timeMovingRight.ToString ());
		form.AddField ("time_moving_left", timeMovingLeft.ToString ());

		form.AddField ("level-challenge", previousChallenge.ToString ());
		form.AddField ("level-status", level_status);
		form.AddField ("date-time", System.DateTime.UtcNow.ToString ());

		form.AddField ("play-id", id.ToString ());
		form.AddField ("attempt-time", levelTime.ToString ());

		WWW webRequest = new WWW (dbUrl + "SaveLevel.php", form);
		
		yield return webRequest;
		Debug.Log (webRequest.text.ToString ());
	}

	private void IncreaseMinDeaths ()
	{
		int[] deaths = {
			deathsByFall,
			deathsByJumper,
			deathsByShooter,
			deathsByWalker
		};
		int min = Mathf.Min (deaths);
		int result = 0;

		if (min == 0) {
			//Increase random factor
			Random.seed = enemyNumberOf;
			result = Random.Range (0, 4);
			
		} else {
			if (deathsByFall == min) {
				result = 0;
			} else if (deathsByJumper == min) {
				result = 1;
			} else if (deathsByShooter == min) {
				result = 2;
			} else if (deathsByWalker == min) {
				result = 3;
			}
		}
		
		switch (result) {
		case 0:
			//Increase gaps
			gapNumber += (int)challengeScale;
			if (gapAverageLength < gapMaxLength - 0.5f) {
				gapAverageLength += 0.2f;
			}
			break;
		case 1:
			enemyJumpNo += (int)challengeScale;
			enemyJumpPower += (challengeScale * 2);
			enemyJumpTimer -= challengeScale;
			break;
		case 2:
			enemyFlyNo += (int)Mathf.Abs (challengeScale);
			enemyShotTimer -= challengeScale;
			enemySpeed += (int)Mathf.Abs (challengeScale / 10);
			break;
		case 3:
			enemySpeed += (int)Mathf.Abs (challengeScale / 10);
			enemyWalkNo += (int)Mathf.Abs (challengeScale);
			break;
		}
	}
	
	private void ReduceMostDeaths ()
	{
		//Find highest cause of death
		int[] deaths = {
			deathsByFall,
			deathsByJumper,
			deathsByShooter,
			deathsByWalker
		};
		int max = Mathf.Max (deaths);
		
		if (deathsByFall == max) {
	
			gapNumber -= (int)challengeScale;
			if (gapAverageLength > 1.2f) {
				gapAverageLength -= 0.1f;
			}
		} else if (deathsByJumper == max) {
			enemyJumpNo -= (int)challengeScale;
			enemyJumpPower -= (challengeScale * 2);
			enemyJumpTimer += challengeScale;
		} else if (deathsByShooter == max) {
			enemyFlyNo -= (int)Mathf.Abs (challengeScale);
			enemyShotTimer += challengeScale;
			enemySpeed -= (int)Mathf.Abs (challengeScale / 10);
		} else if (deathsByWalker == max) {
			enemyWalkNo -= (int)Mathf.Abs (challengeScale);
			enemySpeed -= (int)Mathf.Abs (challengeScale / 10);
		}
	}
	
	private void ReduceAll ()
	{
		enemySpeed -= (int)Mathf.Abs (challengeScale / 10);
		enemyJumpNo -= (int)Mathf.Abs (challengeScale);
		enemyJumpPower -= (challengeScale * 2);
		enemyJumpTimer += challengeScale;
		
		enemyFlyNo -= (int)Mathf.Abs (challengeScale);
		enemyShotTimer += challengeScale;
		
		enemyWalkNo -= (int)Mathf.Abs (challengeScale);
		
		gapNumber -= (int)challengeScale;
		if (gapAverageLength > 1.2f) {
			gapAverageLength -= 0.1f;
		}
	}
	
	private void IncreaseAll ()
	{
		enemySpeed += (int)Mathf.Abs (challengeScale / 10);
		enemyJumpNo += (int)challengeScale - 1;
		enemyJumpPower += (challengeScale * 2);
		enemyJumpTimer -= challengeScale;
		
		enemyFlyNo += (int)Mathf.Abs (challengeScale) - 1;
		enemyShotTimer -= challengeScale;
		
		enemyWalkNo += (int)Mathf.Abs (challengeScale) - 1;

		gapNumber += (int)challengeScale - 1;
		if (gapAverageLength < gapMaxLength - 0.5f) {
			gapAverageLength += 0.2f;
		}
	}

	private string GenerateRandomAxiom ()
	{
		
		string tempAxiom = "";
		Random.seed = (int)(System.DateTime.Now.Second);
		
		for (int i = 0; i < 4; i++) {
			int result = (int)Random.Range (0.0f, 5.0f);

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
		return Axiom = tempAxiom;
	}
}
