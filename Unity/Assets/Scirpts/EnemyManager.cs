//Class which loads enemy spawn points into finished level, also handles the random distribution of enmy types.

using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{	
	private LevelStats level_stats;
	private GameObject enemy_spawn;
	private GameObject enemySpawnPoint;
	private Tile[,] levelMap;
	private Enemy.EnemyType[] enemy_order;
	private int noOfWalkers;
	private int noOfJumpers;
	private int noOfFlyers;
	private float dynamic_range = 0.0f;

	void Awake ()
	{
		enemy_spawn = (GameObject)Resources.Load ("Enemy/EnemySpawn");
		level_stats = GameObject.Find ("LevelLogic").GetComponent<LevelStats> ();
	}
	
	public void LoadMap (Tile[,] levelIn)
	{
		levelMap = levelIn;
	}

	//Use collection of enemy types to place spawn points in the generated level.
	public void LoadEnemySpawns ()
	{
		MakeOrder ();
		//if number of enemies less that potential spawn points, spawns must be spread out+++++++++++++++++++++++++++++
		int enemy_no = level_stats.enemyNumberOf;

		int placed_spawns = 0;
		int level_length = levelMap.GetLength (0);
		int level_height = levelMap.GetLength (1);
		int tiles_since_last = 0;

		dynamic_range = (level_length - 10) / level_stats.enemyNumberOf;
		int enemy_index = 0;
		int levelLoopOffset = 0;

		while (placed_spawns < level_stats.enemyNumberOf) {

			levelLoopOffset += 1;

			for (int i = 0; i < level_length; i++) {
				for (int j = 0; j< level_height; j++) {
					if (levelMap [i, j].isEnemySpawn ()) {

						//Place spawn if current horizontal tile is past the starting range plus offset
						//AND number of tiles has passed between spawn placement
						if (i > level_stats.start_range + levelLoopOffset && tiles_since_last <= 0) {
					
							if (enemy_index < enemy_order.Length) {
								enemySpawnPoint = (GameObject)Instantiate (enemy_spawn, new Vector3 (levelMap [i, j].tilePos.x, levelMap [i, j].tilePos.y, 0.0f), Quaternion.identity);
								enemySpawnPoint.GetComponent<EnemySpawnScript> ().type = enemy_order [enemy_index];
																
								placed_spawns++;
								tiles_since_last = (int)dynamic_range;																
								enemy_index++;
							}
						} else {
							tiles_since_last--; 	
						}
					}
				}
			}
			levelLoopOffset += (int)dynamic_range / 3; 
		}
	}

	//Create collection of enemy types to spawn based on enemy amounts from level stats.
	private void MakeOrder ()
	{
		enemy_order = new Enemy.EnemyType[level_stats.enemyNumberOf];
		noOfWalkers = level_stats.enemy_walk_no;
		noOfJumpers = level_stats.enemy_jump_no;
		noOfFlyers = level_stats.enemy_fly_no;

		for (int i = 0; i < level_stats.enemyNumberOf; i++) {
			
			//Generate controlled random result to decide which enemy type to add to collection.
			Random.seed = i;
			int result = (int)(Random.value * 100) % 3;

			switch (result) {
			case 0:
				if (noOfWalkers > 0) {
					enemy_order [i] = Enemy.EnemyType.Walker;
					noOfWalkers--;
				} else if (noOfJumpers > 0) {
					enemy_order [i] = Enemy.EnemyType.Jumper;
					noOfJumpers--;
				} else if (noOfFlyers > 0) {
					enemy_order [i] = Enemy.EnemyType.Shooter;
					noOfFlyers--;
				}
				break;
			case 1:
				if (noOfJumpers > 0) {
					enemy_order [i] = Enemy.EnemyType.Jumper;
					noOfJumpers--;
				} else if (noOfFlyers > 0) {
					enemy_order [i] = Enemy.EnemyType.Shooter;
					noOfFlyers--;
				} else if (noOfWalkers > 0) {
					enemy_order [i] = Enemy.EnemyType.Walker;
					noOfWalkers--;
				}
				break;
			case 2:
				
				if (noOfFlyers > 0) {
					enemy_order [i] = Enemy.EnemyType.Shooter;
					noOfFlyers--;
				} else if (noOfWalkers > 0) {
					enemy_order [i] = Enemy.EnemyType.Walker;
					noOfWalkers--;
				} else if (noOfJumpers > 0) {
					enemy_order [i] = Enemy.EnemyType.Jumper;
					noOfJumpers--;
				}
				break;
			}
		}
	}
}
