//Class which controls the movement and interactions of a bullet object
//attached to bullet gameObject

using UnityEngine;
using System.Collections;

public class Bullet: MonoBehaviour{

	private Vector3 currentPosition;
	private Vector3 direction;
	private float speed = 3.0f;
	private int alive_counter;
	private int alive_time = 300;

	// Use this for initialization
	void Start (){
				
		//Set current postion to inital spawn position
		currentPosition = transform.position;
				
		//Start active time counter
		alive_counter = alive_time;
	
	}
	
	// Update is called once per frame
	void Update (){

		//Move bullet object
		transform.Translate (new Vector3 (direction.x * speed * Time.deltaTime, direction.y * speed * Time.deltaTime, 0.0f));

		if (alive_counter < 0) {
			Destroy (gameObject);
		}

		alive_counter--;

	}
	
	public void SetDir (Vector3 spawnDirectionIn){

		direction = spawnDirectionIn;

	}

}
