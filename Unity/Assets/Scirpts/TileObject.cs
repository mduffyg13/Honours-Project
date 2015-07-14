//Script attached to tile gameObject

using UnityEngine;
using System.Collections;

public class TileObject : MonoBehaviour
{
	
	//Attached tile script
	private Tile tile;
	
	//Position in array
	public int xpos;
	public int ypos;
	public bool isSpriteSet = false;
	private SpriteRenderer spriteRenderer;
	private BoxCollider2D boxCollider;
	
	// Use this for initialization
	void Start (){
		
		spriteRenderer = GetComponent<SpriteRenderer> ();
		boxCollider = GetComponent<BoxCollider2D> ();
		
	}
	
	// Update is called once per frame
	void Update (){
		
		if (tile.isSpriteSet == false) {
			switch (tile.tileType) {
			case Tile.TileType.Platform:
				gameObject.layer = 8;
				if (tile.platformPos == Tile.PlatformPos.Middle) {
					spriteRenderer.sprite = Resources.Load<Sprite> ("Tiles/Ground");
				}
				if (tile.platformPos == Tile.PlatformPos.Center) {
					spriteRenderer.sprite = Resources.Load<Sprite> ("Tiles/grassMid");
				}
				tile.isSpriteSet = true;
				break;
			case Tile.TileType.Sky:
				spriteRenderer.sprite = Resources.Load<Sprite> ("Tiles/Sky_Clear");
				tile.isSpriteSet = true;
				
				boxCollider.enabled = false;
				break;
			}
		}
	}
	
	public void SetTile (Tile tileIn){
		tile = tileIn;
	}
}
