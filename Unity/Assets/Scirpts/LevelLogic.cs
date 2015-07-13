using UnityEngine;
using System.Collections;

public class LevelLogic: MonoBehaviour
{

		
	
		private AudioSource audio_source;
		private AudioClip music;
		private LevelManager levelManager;
		private EnemyManager enemyManager;
		private PlayerManager playerManager;
		private Tile[,] levelMap;
		bool isTemporary = true;
		bool reStart = true;
		public bool new_level = true;

		void Awake ()
		{
				//On level load/reload, if more than one object exists with this script, destroy new one
				if (GameObject.FindObjectsOfType (typeof(LevelLogic)).Length > 1 && isTemporary)
						Destroy (gameObject);
				else {
						isTemporary = false;
						DontDestroyOnLoad (gameObject);
				}
		}
	
		void Start ()
		{
				audio_source = GetComponent<AudioSource> ();	

				if (new_level) {
						SelectMusic ();
						new_level = false;
				}

				levelManager = GameObject.Find ("LevelLogic").GetComponent<LevelManager> ();
				enemyManager = GameObject.Find ("LevelLogic").GetComponent<EnemyManager> ();
				playerManager = GameObject.Find ("LevelLogic").GetComponent<PlayerManager> ();

				//Form level
				levelManager.InitLevelMap ();
				//Update Each Tile in map - includeds asigning values through CA
				
				levelManager.UpdateGapsAndPlatforms ();
				levelManager.UpdateLevelMap ();
				//Draw level
				levelMap = levelManager.GetLevelMap ();
				levelManager.DrawLevelMap ();

				//Get level map tile array
				//levelMap = levelManager.GetLevelMap ();

				enemyManager.LoadMap (levelMap);
				playerManager.LoadMap (levelMap);

						
				//Player must be loaded after level for raycasting to work
				playerManager.LoadPlayerSpawn ();
				playerManager.LoadEndPoint ();
				enemyManager.LoadEnemySpawns ();	

			
		}

		

		// Update is called once per frame
		void Update ()
		{
				if (GameObject.FindGameObjectWithTag ("Player") == null) {
						//	Debug.Log ("restart?");
						Start ();
						//Reload();
				}
		}

		private void SelectMusic ()
		{

				Random.seed = System.DateTime.Now.Second;
				int result = Random.Range (0, 6);

				//Select random track
				switch (result) {
				case 0:
						music = (AudioClip)Resources.Load ("Music/megaman - cutman");
						break;
				case 1:
						music = (AudioClip)Resources.Load ("Music/megman - bombsman");
						break;
				case 2:
						music = (AudioClip)Resources.Load ("Music/megman - gutsman");
						break;
				case 3:
						music = (AudioClip)Resources.Load ("Music/megaman - elecman");
						break;
				case 4:
						music = (AudioClip)Resources.Load ("Music/megaman - iceman");
						break;
				case 5:
						music = (AudioClip)Resources.Load ("Music/megman - fireman");
						break;
				}

				//Play on loop
				audio_source.clip = music;
				audio_source.loop = true;
				audio_source.Play ();
		
		}
}
