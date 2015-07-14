//Class which represents the Tile data structure which is used in the generation process,
//used by the tile gameObject to decide its state based on position in level array.
//Contains rules which dictate cellular automata process.
using UnityEngine;
using System.Collections;

public class Tile
{
	
	public enum TileType
	{
		
		Platform,
		Sky, 
		Null //Null required for initialising tiles
		
	}
	
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
	public TileType tileType;
	private Tile[] tileNeighbours;
	public int state = 0;
	private int alive = 1;
	private int dead = 0;
	public Vector2 tilePos;			
	private bool isEnemySpawn;		
	private bool isPlayerSpawn;		
	private bool isEndPoint;			
	private bool isAtEdge;			
	public bool isSpriteSet = false;
	
	public Tile ()
	{
		tileType = TileType.Null;
		platformPos = PlatformPos.Middle;
		tileNeighbours = new Tile [8];
		isEnemySpawn = false;
		isPlayerSpawn = false;
		isAtEdge = false;
		isEndPoint = false;
	}
	
	public bool IsEnemySpawn ()
	{
		return isEnemySpawn;
	}
	
	public bool IsPlayerSpawn ()
	{
		return isPlayerSpawn;
	}
	
	public bool IsEndPoint ()
	{	
		return isEndPoint;
	}
	
	public void SetNeighbours (Tile tileDown, Tile tileLeft, Tile tileUp, Tile tileRight,
	                           Tile tileDownLeft, Tile tileUpLeft, Tile tileUpRight, Tile tileDownRight)
	{
		
		tileNeighbours [(int)Direction.Down] = tileDown; 
		tileNeighbours [(int)Direction.Left] = tileLeft;
		tileNeighbours [(int)Direction.Up] = tileUp; 
		tileNeighbours [(int)Direction.Right] = tileRight; 
		tileNeighbours [(int)Direction.Down_Left] = tileDownLeft; 
		tileNeighbours [(int)Direction.Up_Left] = tileUpLeft;  
		tileNeighbours [(int)Direction.Up_Right] = tileUpRight; 
		tileNeighbours [(int)Direction.Down_Right] = tileDownRight; 
		
		for (int i = 0; i < tileNeighbours.Length; i++) {
			if (tileNeighbours [i] == null) {
				isAtEdge = true;
			}		
		}
		
		SetSprite ();
	}
	
	private void SetSprite ()
	{
		//If tile is a platform
		if (state == 1) {
			//If no tile above, make sprite grass edge
			if (tileNeighbours [(int)Direction.Up] == null || tileNeighbours [(int)Direction.Up].state == 0) {
				platformPos = PlatformPos.Center;
			}
			//if tile above a platform, make sprite middle sprite
			if (tileNeighbours [(int)Direction.Up].state == alive) {
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
		if (!isAtEdge) {
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
			} 
			
			EndSpawnRule ();
			PlayerSpawnRule ();	
		}
		SetSprite ();

	}
	
	private void VerticalRangeRule ()
	{
		if (state == 1) {
			if (tileNeighbours [(int)Direction.Down].state == alive &&
				tileNeighbours [(int)Direction.Right].state == alive &&
				tileNeighbours [(int)Direction.Down_Right].state == alive &&
				tileNeighbours [(int)Direction.Up].state == dead &&
				tileNeighbours [(int)Direction.Left].state == dead &&
				tileNeighbours [(int)Direction.Up_Left].state == dead &&
				tileNeighbours [(int)Direction.Up_Right].state == dead &&
				tileNeighbours [(int)Direction.Down_Left].state == dead) {
				state = dead;
			}
		}
	}
	
	//If tile dead and surrounded by alive on above and below rows, make alive
	private void SurroundedRule ()
	{
		if (state == 0) {
			if (tileNeighbours [(int)Direction.Down].state == alive &&
				tileNeighbours [(int)Direction.Up].state == alive &&
				tileNeighbours [(int)Direction.Left].state == alive &&
				tileNeighbours [(int)Direction.Right].state == alive &&
				tileNeighbours [(int)Direction.Down_Left].state == alive &&
				tileNeighbours [(int)Direction.Down_Right].state == alive &&
				tileNeighbours [(int)Direction.Up_Left].state == alive &&
				tileNeighbours [(int)Direction.Up_Right].state == alive) {
				state = 1;
			}	
		}
	}
	
	//If tile dead and surrounded by alive on above and below rows, make alive
	private void SingleGapRule ()
	{
		if (state == 0) {
			if (tileNeighbours [(int)Direction.Down].state == alive &&
			    
				tileNeighbours [(int)Direction.Left].state == alive &&
				tileNeighbours [(int)Direction.Right].state == alive &&
				tileNeighbours [(int)Direction.Down_Left].state == alive &&
				tileNeighbours [(int)Direction.Down_Right].state == alive) {
				state = 1;
				isSpriteSet = false;
			}				
		}
	}
	
	private void UpperPlatformExtendSingle ()
	{
		if (state == 0) {
			if (tileNeighbours [(int)Direction.Down].state == dead &&
			    
				tileNeighbours [(int)Direction.Left].state == alive &&
				tileNeighbours [(int)Direction.Right].state == alive &&
				tileNeighbours [(int)Direction.Down_Left].state == dead &&
				tileNeighbours [(int)Direction.Down_Right].state == dead) {
				state = alive;
				isSpriteSet = false;
			}	
			
		}
		if (state == 1) {
			if (tileNeighbours [(int)Direction.Down].state == dead &&
			    
				tileNeighbours [(int)Direction.Left].state == dead &&
				tileNeighbours [(int)Direction.Right].state == dead &&
				tileNeighbours [(int)Direction.Down_Left].state == dead &&
				tileNeighbours [(int)Direction.Down_Right].state == dead) {
				state = dead;
				isSpriteSet = false;
			}	
			
		}
	}
	
	private void UpperAccessRule ()
	{
		if (state == 0) {
			if (tileNeighbours [(int)Direction.Down].state == alive &&
				tileNeighbours [(int)Direction.Up].state == dead &&
				tileNeighbours [(int)Direction.Left].state == dead &&
				tileNeighbours [(int)Direction.Right].state == dead &&
				tileNeighbours [(int)Direction.Down_Left].state == alive &&
				tileNeighbours [(int)Direction.Down_Right].state == dead &&
				tileNeighbours [(int)Direction.Up_Left].state == dead &&
				tileNeighbours [(int)Direction.Up_Right].state == alive) {
				state = alive;
				isSpriteSet = false;
			}	
			
		}
	}
	
	//If tile is surrounded by only 3 alive tiles (all below) make enemy spawn point possible
	private void EnemySpawnRule ()
	{
		if (this.state == 0) {
			if (tileNeighbours [0].state == 1 &&
				tileNeighbours [4].state == 1 &&
				tileNeighbours [7].state == 1 &&
				tileNeighbours [1].state == 0 &&
				tileNeighbours [3].state == 0 && 
				tileNeighbours [2].state == 0 && 
				tileNeighbours [5].state == 0 &&
				tileNeighbours [6].state == 0) {
				isEnemySpawn = true;

			}
		} 
	}
	
	private void PlayerSpawnRule ()
	{
		if (this.state == 0) {
			if (tileNeighbours [0].state == 1) {
				isPlayerSpawn = true;
			}
		}
	}
	
	private void EndSpawnRule ()
	{
		if (this.state == 0) {
			if (tileNeighbours [0].state == 1) {
				isEndPoint = true;
			}
		}
	}
}
