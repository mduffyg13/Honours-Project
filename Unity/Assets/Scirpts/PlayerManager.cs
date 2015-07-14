//Class which loads both player spawn point and the level end trigger

using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{

		GameObject playerSpawn;
		GameObject endTrigger;
		private Tile[,] levelMap;
		private GameObject playerSpawnPoint;
		private GameObject endTriggerPoint;

		void Awake ()
		{
				playerSpawn = (GameObject)Resources.Load ("Player/PlayerSpawn");
				endTrigger = (GameObject)Resources.Load ("Triggers/EndTrigger");
		}

		public void LoadMap (Tile[,] levelIn)
		{
				levelMap = levelIn;
		}

		public void LoadPlayerSpawn ()
		{
				bool placed = false;
				int level_length = levelMap.GetLength (0);
				int level_height = levelMap.GetLength (1);
		
				for (int i = 0; i < level_length; i++) {
						for (int j = 0; j< level_height; j++) {
								if (levelMap [i, j].IsPlayerSpawn ()) {
										if (!placed) {
												playerSpawnPoint = (GameObject)Instantiate (playerSpawn, new Vector3 (levelMap [i, j].tilePos.x, levelMap [i, j].tilePos.y, 0.0f), Quaternion.identity);
												placed = true;
										}
								}
						}
				}
		}

		public void LoadEndPoint ()
		{
				bool endPlaced = false;
				int level_length = levelMap.GetLength (0);
				int level_height = levelMap.GetLength (1);

				for (int i = level_length -1; i > 0; i--) {
						for (int j = 0; j< level_height; j++) {
								if (levelMap [i, j].IsEndPoint () && endPlaced == false) {
										endTriggerPoint = (GameObject)Instantiate (endTrigger, new Vector3 (levelMap [i, j].tilePos.x, levelMap [i, j].tilePos.y, 0.0f), Quaternion.identity);
										endPlaced = true;
								}
						}
				}
		}
}
