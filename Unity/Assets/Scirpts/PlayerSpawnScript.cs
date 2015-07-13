using UnityEngine;
using System.Collections;

public class PlayerSpawnScript : MonoBehaviour {


	public GameObject player;
	
	void Awake(){

		player = (GameObject)Instantiate((GameObject)Resources.Load("Player/Player"), gameObject.transform.position, Quaternion.identity);

	}


}
