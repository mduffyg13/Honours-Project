using UnityEngine;
using System.Collections;

public class Tile
{


		// An enum for assigning possible tile type
		public enum TileType
		{
				Platform,
				Sky, 
				Null //Null required for initialising tiles
		}

		//Contains the current tile's type
		public TileType tileType;

		//Enum used for assigning tile neighbours to array
		private enum Direction
		{
				Down,
				Left,
				Up,
				Right,
				Down_Left,
				Up_Left,
				Up_Right,
				Down_Right
		}

		//Used to assign correct sprite to tile object
		public enum PlatformPos
		{
				Middle,
				Center,
				Right,
				Left
		}
		public PlatformPos platformPos;

		private Tile[] tile_neighbours;
		
		public int state = 0;
		private int alive = 1;
		private int dead = 0;
	
		public Vector2 tilePos;			// The current position of a tile of full level
		private bool enemySpawn;		//Can an enemy spawn on this tile?
		private bool playerSpawn;		//Can the Player spawn on this tile
		private bool endPoint;			//Can level end spawn here
		private bool atEdge;			//Is this tile at edge of level
		public bool isSpriteSet = false;

		// Empty contructor for initalising object
		public Tile ()
		{
				tileType = TileType.Null;
				platformPos = PlatformPos.Middle;
				tile_neighbours = new Tile [8];
				enemySpawn = false;
				playerSpawn = false;
				atEdge = false;
				endPoint = false;
		}

		public bool isEnemySpawn ()
		{
				return enemySpawn;
		}

		public bool isPlayerSpawn ()
		{
				return playerSpawn;
		}

		public bool isEndSpawn ()
		{
				return endPoint;
		}

		public void SetNeighbours (Tile tileDown, Tile tileLeft, Tile tileUp, Tile tileRight,
	                          Tile tileDownLeft, Tile tileUpLeft, Tile tileUpRight, Tile tileDownRight)
		{
			
				tile_neighbours [(int)Direction.Down] = tileDown; 
				tile_neighbours [(int)Direction.Left] = tileLeft;
				tile_neighbours [(int)Direction.Up] = tileUp; 
				tile_neighbours [(int)Direction.Right] = tileRight; 
				tile_neighbours [(int)Direction.Down_Left] = tileDownLeft; 
				tile_neighbours [(int)Direction.Up_Left] = tileUpLeft;  
				tile_neighbours [(int)Direction.Up_Right] = tileUpRight; 
				tile_neighbours [(int)Direction.Down_Right] = tileDownRight; 

				for (int i = 0; i < tile_neighbours.Length; i++) {
						if (tile_neighbours [i] == null) {
								atEdge = true;
						}		
				}
			
				SetSprite ();
		}

		private void SetSprite ()
		{
				//If tile a platform
				if (state == 1) {
						//If no tile above, make sprite grass edge
						if (tile_neighbours [(int)Direction.Up] == null || tile_neighbours [(int)Direction.Up].state == 0) {
								platformPos = PlatformPos.Center;
						}
						//if tile above a platform, make sprite middle sprite
						if (tile_neighbours [(int)Direction.Up].state == alive) {
								platformPos = PlatformPos.Middle;
						}
				}
	
		}

		public void SwitchState ()
		{
				if (state == 0) {
						state = 1;
				} else {
						state = 0;	
				}
		}

		public void UpdateTile ()
		{
				if (!atEdge) {
						EnemySpawnRule ();
						VerticalRangeRule ();
						SurroundedRule ();
						
						if (this.tilePos.y > 0) {
								SingleGapRule ();
						}

						if (this.tilePos.y == 3) {
								UpperPlatformExtendSingle ();	
						} 
						if (this.tilePos.y == 2) {
								UpperAccessRule ();
								//UpperPlatformExtendSingle ();	
						} 



						
						EndSpawnRule ();
						PlayerSpawnRule ();	
				}
				

				if (this.tilePos.x == 0 ||
						this.tilePos.x == 1 ||
						this.tilePos.x == 2 ||
						this.tilePos.x == 3) {
						//	PlayerSpawnRule ();		
				} 


			
				SetSprite ();
		}

		private void VerticalRangeRule ()
		{
		
				if (state == 1) {
						if (tile_neighbours [(int)Direction.Down].state == alive &&
								tile_neighbours [(int)Direction.Right].state == alive &&
								tile_neighbours [(int)Direction.Down_Right].state == alive &&
			   
								tile_neighbours [(int)Direction.Up].state == dead &&
								tile_neighbours [(int)Direction.Left].state == dead &&
								tile_neighbours [(int)Direction.Up_Left].state == dead &&
								tile_neighbours [(int)Direction.Up_Right].state == dead &&
								tile_neighbours [(int)Direction.Down_Left].state == dead) {
								state = dead;
						}
				}
		
		}

		private void SurroundedRule ()
		{
				//If tile dead and surrounded by alive on above and below rows, make alive
				if (state == 0) {
						if (tile_neighbours [(int)Direction.Down].state == alive &&
								tile_neighbours [(int)Direction.Up].state == alive &&
								tile_neighbours [(int)Direction.Left].state == alive &&
								tile_neighbours [(int)Direction.Right].state == alive &&
								tile_neighbours [(int)Direction.Down_Left].state == alive &&
								tile_neighbours [(int)Direction.Down_Right].state == alive &&
								tile_neighbours [(int)Direction.Up_Left].state == alive &&
								tile_neighbours [(int)Direction.Up_Right].state == alive) {
								state = 1;
						}	
		
		
				}
		}

		private void SingleGapRule ()
		{
				//If tile dead and surrounded by alive on above and below rows, make alive
				if (state == 0) {
						if (tile_neighbours [(int)Direction.Down].state == alive &&

								tile_neighbours [(int)Direction.Left].state == alive &&
								tile_neighbours [(int)Direction.Right].state == alive &&
								tile_neighbours [(int)Direction.Down_Left].state == alive &&
								tile_neighbours [(int)Direction.Down_Right].state == alive) {
								state = 1;
								isSpriteSet = false;
						}	
			
			
				}
		}

		private void UpperPlatformExtendSingle ()
		{
				if (state == 0) {
						if (tile_neighbours [(int)Direction.Down].state == dead &&
			    
								tile_neighbours [(int)Direction.Left].state == alive &&
								tile_neighbours [(int)Direction.Right].state == alive &&
								tile_neighbours [(int)Direction.Down_Left].state == dead &&
								tile_neighbours [(int)Direction.Down_Right].state == dead) {
								state = alive;
								isSpriteSet = false;
						}	
		
				}
				if (state == 1) {
						if (tile_neighbours [(int)Direction.Down].state == dead &&
			    
								tile_neighbours [(int)Direction.Left].state == dead &&
								tile_neighbours [(int)Direction.Right].state == dead &&
								tile_neighbours [(int)Direction.Down_Left].state == dead &&
								tile_neighbours [(int)Direction.Down_Right].state == dead) {
								state = dead;
								isSpriteSet = false;
						}	
			
				}
		}

		private void UpperAccessRule ()
		{
				if (state == 0) {
						if (tile_neighbours [(int)Direction.Down].state == alive &&
								tile_neighbours [(int)Direction.Up].state == dead &&
								tile_neighbours [(int)Direction.Left].state == dead &&
								tile_neighbours [(int)Direction.Right].state == dead &&
								tile_neighbours [(int)Direction.Down_Left].state == alive &&
								tile_neighbours [(int)Direction.Down_Right].state == dead &&
								tile_neighbours [(int)Direction.Up_Left].state == dead &&
								tile_neighbours [(int)Direction.Up_Right].state == alive) {
								state = alive;
								isSpriteSet = false;
						}	
					
				}
		}

		private void RuleFour ()
		{
				if (state == 0) {
						if (tile_neighbours [0] == null) {
								state = 1;
						}		
				}
		}

		//If tile is surrounded by only 3 alive tiles (all below) make enemy spawn point possible
		private void EnemySpawnRule ()
		{
				if (this.state == 0) {
						if (tile_neighbours [0].state == 1 &&
								tile_neighbours [4].state == 1 &&
								tile_neighbours [7].state == 1 &&
								tile_neighbours [1].state == 0 &&
								tile_neighbours [3].state == 0 && 
								tile_neighbours [2].state == 0 && 
								tile_neighbours [5].state == 0 &&
								tile_neighbours [6].state == 0) {
								enemySpawn = true;
								//Debug.Log("ENEMY TILE!!! " + tilePos.x + " " + tilePos.y);
						}
				} 
		}

		private void PlayerSpawnRule ()
		{
				if (this.state == 0) {
						if (tile_neighbours [0].state == 1) {
								playerSpawn = true;
						}
				}
		}

		private void EndSpawnRule ()
		{
				if (this.state == 0) {
						if (tile_neighbours [0].state == 1) {
								endPoint = true;
						}
				}
		}
}
