//Class which loads enemy spawn points into finished level

using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{

	
	
		LevelStats level_stats;
		public GameObject enemy_spawn;
		private GameObject enemySpawnPoint;
		private Tile[,] levelMap;
		public Enemy.EnemyType[] enemy_order;
		public int walkers;
		public int jumpers;
		public int flyers;
		public float dynamic_range = 0.0f;

		void Awake ()
		{
			
				enemy_spawn = (GameObject)Resources.Load ("Enemy/EnemySpawn");
				level_stats = GameObject.Find ("LevelLogic").GetComponent<LevelStats> ();
		}

		public void MakeOrder ()
		{

				enemy_order = new Enemy.EnemyType[level_stats.enemyNumberOf];
				walkers = level_stats.enemy_walk_no;
				jumpers = level_stats.enemy_jump_no;
				flyers = level_stats.enemy_fly_no;

				for (int i = 0; i < level_stats.enemyNumberOf; i++) {
						int value = 0;

						//value represents the number of enmy types left to pick from
						if (walkers > 0) {
								value++;
						}
						if (jumpers > 0) {
								value++;
						}
						if (flyers > 0) {
								value++;
						}
					
						//Get random result to decide which to add
						Random.seed = i;
						int result = (int)(Random.value * 100) % value;
						
						switch (result) {
						case 0:
								if (walkers > 0) {
										enemy_order [i] = Enemy.EnemyType.Walker;
										walkers--;
								} else if (jumpers > 0) {
										enemy_order [i] = Enemy.EnemyType.Jumper;
										jumpers--;
								} else if (flyers > 0) {
										enemy_order [i] = Enemy.EnemyType.Shooter;
										flyers--;
								}
								break;
						case 1:
								if (jumpers > 0) {
										enemy_order [i] = Enemy.EnemyType.Jumper;
										jumpers--;
								} else if (flyers > 0) {
										enemy_order [i] = Enemy.EnemyType.Shooter;
										flyers--;
								} else if (walkers > 0) {
										enemy_order [i] = Enemy.EnemyType.Walker;
										walkers--;
								}
								break;
						case 2:

								if (flyers > 0) {
										enemy_order [i] = Enemy.EnemyType.Shooter;
										flyers--;
								} else if (walkers > 0) {
										enemy_order [i] = Enemy.EnemyType.Walker;
										walkers--;
								} else if (jumpers > 0) {
										enemy_order [i] = Enemy.EnemyType.Jumper;
										jumpers--;
								}
								break;
						}
				}
		}

		public void LoadMap (Tile[,] levelIn)
		{
				levelMap = levelIn;
		}

		

		public void LoadEnemySpawns ()
		{
				MakeOrder ();
				//if number of enemies less that potential spawn points, spawns must be spread out+++++++++++++++++++++++++++++
				int enemy_no = level_stats.enemyNumberOf;

				//the number of placed spawns
				int placed_spawns = 0;

				float frequency_counter;// = level_stats.spawn_inverse_frequency;
				//Debug.Log ("FOUND SPEED " + level_stats.enemySpeed.ToString ());
				int level_length = levelMap.GetLength (0);
				int level_height = levelMap.GetLength (1);

				
				int tiles_since_last = 0;

				dynamic_range = (level_length - 10) / level_stats.enemyNumberOf;
				frequency_counter = dynamic_range; 
				tiles_since_last = 0;
				int enemy_index = 0;

				int go_round = 0;
				while (placed_spawns < level_stats.enemyNumberOf) {
						go_round += 1;
						for (int i = 0; i < level_length; i++) {
								for (int j = 0; j< level_height; j++) {
								
					//Tile is valid for enemy spawn
					if (levelMap [i, j].isEnemySpawn ()) {
						//And tile is past the starting range

												if (i > level_stats.start_range + go_round && tiles_since_last <= 0) {
					

														if (enemy_index < enemy_order.Length) {
																enemySpawnPoint = (GameObject)Instantiate (enemy_spawn, new Vector3 (levelMap [i, j].tilePos.x, levelMap [i, j].tilePos.y, 0.0f), Quaternion.identity);
																enemySpawnPoint.GetComponent<EnemySpawnScript> ().type = enemy_order [enemy_index];
																
																placed_spawns++;
																tiles_since_last = (int)dynamic_range;
//																
																enemy_index++;
														}
											
														frequency_counter = 0;
											
												} else {
														tiles_since_last--; 	
												}
										}
								}
		
						}
						go_round += (int)dynamic_range / 3; 
				}
		}
}
