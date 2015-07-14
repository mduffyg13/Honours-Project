using UnityEngine;
using System.Collections;

//using System;

public class LevelManager : MonoBehaviour
{
		LevelStats level_stats;

		//Input for generation
		
		private string axiom;
		private int	gap_number;
		[SerializeField]
		int
				gap_number_Actual;
		[SerializeField]
		private float
				gap_average_length;
		[SerializeField]
		private float
				gap_average_length_Actual;
		[SerializeField]
		private int
				plaform_number_Actual;

		// Object that we use to create our map contai
		private GameObject tileParent;
		private string lSystem;
		private int pattern_horizontal = 15;
		private int pattern_vertical = 5;
		private int pattern_per_section = 4;
		private int level_sections = 4;
		private Tile[,] full_level;
		private GapPlatformManager gapPlaformManager;
		
		void  Start ()
		{
				level_stats = gameObject.GetComponent<LevelStats> ();
		}

		public void UpdateGapsAndPlatforms ()
		{

				gapPlaformManager.LoadMap (full_level);

				//gapPlaformManager.findAverage ();
		

				gapPlaformManager.RemoveSingleSpaceGaps ();
				gapPlaformManager.DrillUp ();

				full_level = gapPlaformManager.GetUpdatedMap ();
				gapPlaformManager.LoadMap (full_level);
				gapPlaformManager.RegisterPlatforms ();
				gapPlaformManager.RemoveSingleSpacePlaforms ();

				full_level = gapPlaformManager.GetUpdatedMap ();
				gapPlaformManager.LoadMap (full_level);

				gapPlaformManager.ReduceToMax ();
				full_level = gapPlaformManager.GetUpdatedMap ();



				plaform_number_Actual = gapPlaformManager.GetTotalPlatforms ();
				gap_average_length_Actual = gapPlaformManager.gap_average;
				gap_number_Actual = gapPlaformManager.GetTotalGaps ();

				
				//Reduce number of gaps
				if (gap_number_Actual > level_stats.gapNumber) {
				
						gapPlaformManager.ReduceGaps (level_stats.gapNumber);
						full_level = gapPlaformManager.GetUpdatedMap ();
						plaform_number_Actual = gapPlaformManager.GetTotalPlatforms ();
						gap_average_length_Actual = gapPlaformManager.gap_average;
						gap_number_Actual = gapPlaformManager.GetTotalGaps ();
				}

				//Increase number of gaps
				if (gap_number_Actual < level_stats.gapNumber) {
						gapPlaformManager.IncreaseGaps (level_stats.gapNumber);
				}

				//Find average gap length in level 
				gap_average_length_Actual = gapPlaformManager.findAverage ();

				//If actual average is less than target, increase average
				if (gap_average_length_Actual < gap_average_length) {
						
						//Call until actual has risen above target
						while (gap_average_length_Actual < gap_average_length) {
								gapPlaformManager.IncreaseAverage ();
								gap_average_length_Actual = gapPlaformManager.findAverage ();
						}
						//If actual average is greater than target, decrease average
				} else if (gap_average_length_Actual > gap_average_length) {

						//Call until actual has fallen below target
						while (gap_average_length_Actual > gap_average_length) {
								gapPlaformManager.ReduceAverage ();
								gap_average_length_Actual = gapPlaformManager.findAverage ();
				
						}
				}

				
				gap_average_length_Actual = gapPlaformManager.findAverage ();
				full_level = gapPlaformManager.GetUpdatedMap ();
		}

		
	
		//Get current level of tiles
		public Tile[,] GetLevelMap ()
		{
				return full_level;
		}

		private void InitGenValues ()
		{
				axiom = level_stats.Axiom;
				gap_number = level_stats.gapNumber;
				gap_average_length = level_stats.gapAverageLength;
		}
	
		//Create level
		public void InitLevelMap ()
		{
				InitGenValues ();
				gapPlaformManager = new GapPlatformManager ();
				//platformManager = new PlatformManager ();
		
				//level size based on number of and dimensions of pattern
				full_level = new Tile[pattern_horizontal * level_sections * pattern_per_section, pattern_vertical];
		
				// Create an empty object with the name "Map" to act as the parent for our tiles 
				tileParent = new GameObject ();
				tileParent.name = "Map";

	
				InitFullLevel ();
				CreateLSystemString ();
				GeneratePatternsUsingLSyem ();
				LoadNeighbours ();
			
		
		
		}
		//Initialise the array of tiles objects
		private void InitFullLevel ()
		{
				// For each row, cycle through an entire column
				for (int i = 0; i < full_level.GetLength(0); i++) {
						for (int j = 0; j < full_level.GetLength(1); j++) {
								
								//A tiles position in array
								float x_pos = i * 0.5f;
								float y_pos = j * 1.0f;

								// Instance tile object and send position in array
								full_level [i, j] = new Tile ();
								full_level [i, j].tilePos = new Vector2 (x_pos, y_pos);
						}
				}
		}
		//Run update on each tile object in array
		public void UpdateLevelMap ()
		{
				int level_length = full_level.GetLength (0);
				int level_height = full_level.GetLength (1);
		
				//Go up each column, left to right
				for (int i =0; i < level_length; i++) {
						for (int j =0; j < level_height; j++) {
								//Update each tile
								full_level [i, j].UpdateTile ();
						}
				}		
				//re load neibours
				LoadNeighbours ();
		}


		//Draw level map
		public void DrawLevelMap ()
		{
				GameObject tilePrefab = (GameObject)Resources.Load ("Tiles/Tile");

				// Loop structure is the same as the loops above ^^^
				for (int i = 0; i < full_level.GetLength(0); i++) {
						for (int j = 0; j < full_level.GetLength(1); j++) {
				
								if (full_level [i, j].state == 0) {
										full_level [i, j].tileType = Tile.TileType.Sky;
					
								} else if (full_level [i, j].state == 1) {
										full_level [i, j].tileType = Tile.TileType.Platform;
								}

								
								// If the tile is null, we know not to try to draw a sprite for it
								if (full_level [i, j].tileType != Tile.TileType.Null) {
							
										GameObject tileObject;
										tileObject = (GameObject)Instantiate (tilePrefab, full_level [i, j].tilePos, Quaternion.identity);
										//Add tile information to GameObject
										tileObject.GetComponent<TileObject> ().SetTile (full_level [i, j]);
										//Add GameObject to parent Map GameObject 
										tileObject.transform.parent = tileParent.transform;
								}
						}
				}

		}

		private void LoadNeighbours ()
		{
				int level_length = full_level.GetLength (0);
				int level_height = full_level.GetLength (1);
		
				//			Debug.Log ("Level length: " + level_length);
		
				int iLevel_length = level_length - 1;
				int iLevel_height = level_height - 1;
		
				//CORNER TILES
				//bottomleft
				full_level [0, 0].SetNeighbours (null, null, full_level [0, 1], full_level [1, 0], null, null, full_level [1, 1], null);
				//topleft
				full_level [0, iLevel_height].SetNeighbours (full_level [0, iLevel_height - 1], null, null, full_level [1, iLevel_height], null, null, null, full_level [1, iLevel_height - 1]);
				//bottomright
				full_level [iLevel_length, 0].SetNeighbours (null, full_level [iLevel_length - 1, 0], full_level [iLevel_length, 1], null, null, full_level [iLevel_length - 1, 1], null, null);
				//topright
				full_level [iLevel_length, iLevel_height].SetNeighbours (full_level [iLevel_length, iLevel_height - 1], full_level [iLevel_length - 1, iLevel_height], null, null, full_level [iLevel_length - 1, iLevel_height - 1], null, null, null);
		
		
		
		
		
				//Go up each column, left to right
				for (int i =0; i < level_length; i++) {
						for (int j =0; j < level_height; j++) {
				
								//LEFT EDGE TILES			
								if (i == 0 && j != 0 && j != iLevel_height) {
										//Set non-edge tiles
										full_level [i, j].SetNeighbours (
						//Straight 
						full_level [i, j - 1], //0
						null,
						full_level [i, j + 1], //2
						full_level [i + 1, j], //3
						//Diagonals
						null,
						null,
						full_level [i + 1, j + 1], //6
						full_level [i + 1, j - 1] //7
										);
								} 
				//Right EDGE TILES
				else if (i == iLevel_length && j != 0 && j != iLevel_height) {
										full_level [i, j].SetNeighbours (
						//Straight 
						full_level [i, j - 1], //0
						full_level [i - 1, j], //1
						full_level [i, j + 1], //2
						null,
						//Diagonals
						full_level [i - 1, j - 1], //4
						full_level [i - 1, j + 1], //5
						null,
						null
										);
								}
				//TOP EDGE TILES
				else if (j == iLevel_height && i != 0 && i != iLevel_length) {
										full_level [i, j].SetNeighbours (
						//Straight 
						full_level [i, j - 1], //0
						full_level [i - 1, j], //1
						null,
						full_level [i + 1, j], //3
						//Diagonals
						full_level [i - 1, j - 1],//4
						null,
						null,
						full_level [i + 1, j - 1]//7
										);
					
								}
				//BOTTOM EDGE TILE
				else if (j == 0 && i != 0 && i != iLevel_length) {
										//Set non-edge tiles
										full_level [i, j].SetNeighbours (
						//Straight 
						null,
						full_level [i - 1, j], //1
						full_level [i, j + 1], //2
						full_level [i + 1, j],//3
						//Diagonals
						null,
						full_level [i - 1, j + 1], //5
						full_level [i + 1, j + 1], //6
						null
										);
					
					
					
								} else {
										//Eliminate corners
										if (i != 0 && i != iLevel_length && j != 0 && j != iLevel_height) {
												//Set non-edge tiles
												

												full_level [i, j].SetNeighbours (
							//Straight 
							full_level [i, j - 1],
							full_level [i - 1, j],
							full_level [i, j + 1],
							full_level [i + 1, j],
							//Diagonals
							full_level [i - 1, j - 1],
							full_level [i - 1, j + 1],
							full_level [i + 1, j + 1],
							full_level [i + 1, j - 1]
												);



										}
					
								}
				
				
						}
				}
		}
	
		//Function set - Construct level layout using l-system==============================
		private void CreateLSystemString ()
		{
				Debug.Log ("Create L-system using: " + axiom.ToString ());
				string temp = "";
				string result = "";

				//The number of iterations to run 
				int iterations = 2;

				//Replacement rules { A= AB, B=BC, C=AA}

				//Get axiom for use
				temp = axiom;
				for (int j = 0; j < iterations; j++) {
						result = "";
						for (int i = 0; i < temp.Length; i++) {
								
								//Replace char in result
								switch (temp [i]) {
								case 'A':
										result += "AB";
										break;
								case 'B':
										result += "DC";
										break;
								case 'C':
										result += "BA";
										break;
								case 'D':
										result += "CD";
										break;
								}
						}
						//Make previous result new axiom
						temp = result;
						//Return 16 char result
						lSystem = result;
				}
		}
	
		private void GeneratePatternsUsingLSyem ()
		{
				//Get 16 char L system result
				string patternstring = lSystem;
				
				//Go through each char in string
				for (int i = 0; i < patternstring.Length; i++) {
						
						//Call pattern generation method, pass place as parameter.
						//Add 1 to account for indexing
						switch (patternstring [i]) {
				
						case 'A':
								GenAPattern (i + 1);
								break;
						case 'B':
								GenBPattern (i + 1);
								break;
						case 'C':
								GenCPattern (i + 1);
								break;
						case 'D':
								GenDPattern (i + 1);
								break;
				
						}
				}
		}
	
		private void GenAPattern (int x_placement)
		{
				for (int i = 0; i < pattern_horizontal; i++) {
						
						//Calculate x position in full level array
						int place = (x_placement - 1) * pattern_horizontal + i;
						//Set y position affected by this function
						int j = 0;

						//Controled seed, can recreate level
						Random.seed = (int)place;

						//Calculate probabilty of tile switch
						int random_value = (int)(Random.value * 10) * i;
						int tile_state = random_value % 4;

						//If condition met, switch tile state
						if (tile_state == 0 || tile_state == 1) {
						
								full_level [place, j].SwitchState ();
						}
				}



				//int pos = (x_placement - 1) * pattern_horizontal + i;
		}
	
		private void GenBPattern (int x_placement)
		{
				for (int i = 0; i < pattern_horizontal; i++) {
						for (int j = 0; j < pattern_vertical; j++) {
				
								if (j == 0 || j == 1) {
										
										int place = (x_placement - 1) * pattern_horizontal + i + j;
										Random.seed = (int)place;
										int random_value = (int)(Random.value * 10) * i;
										int tile_state = random_value % 4;

										//int tile_state = (int)Random.Range (0.0f, 9.0f);
										int pos = (x_placement - 1) * pattern_horizontal + i;
										//Debug.Log ("Random value: " + random_value);			
										if (tile_state == 0 || tile_state == 1) {
												//						Debug.Log ("switched: " + Random.value * 100);
												full_level [pos, j].SwitchState ();
										}
								}
						}
				}
		}
	
		private void GenCPattern (int x_placement)
		{
				for (int i = 0; i < pattern_horizontal; i++) {
						for (int j = 0; j < pattern_vertical; j++) {
								if (j == 0 || j == 1 || j == 2) {
										


										int seed = (x_placement - 1) * pattern_horizontal + i + j;
										Random.seed = (int)seed;
										int random_value = (int)(Random.value * 10) * i;
										int tile_state = random_value % 4;


										
										int pos = (x_placement - 1) * pattern_horizontal + i;

										if (tile_state == 0 || tile_state == 1) {
											
												full_level [pos, j].SwitchState ();
										}
								}
						}
				}
		}

		private void GenDPattern (int x_placement)
		{
//		Debug.Log ("GEN D");
				for (int i = 0; i < pattern_horizontal; i++) {
						for (int j = 0; j < pattern_vertical; j++) {
								if (j == 3 || j == 1) {
					
					
					
										int seed = (x_placement - 1) * pattern_horizontal + i + j;
										Random.seed = (int)seed;
										int random_value = (int)(Random.value * 10) * i;
										int tile_state = random_value % 4;
					
					
					
										int pos = (x_placement - 1) * pattern_horizontal + i;
					
										if (tile_state == 0 || tile_state == 1) {
						
												full_level [pos, j].SwitchState ();
										}
								}
						}
				}
		}
		//===================================================================
		
}