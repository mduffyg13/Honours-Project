using UnityEngine;
using System.Collections;

public class EnemySpawnScript : MonoBehaviour
{

//Attached to enemy spawn object
		public int numberOf;
		public int cooldown;
		private float spawn_range = 10.0f;
		public Enemy.EnemyType type;
		private GameObject enemy;
		private GameObject player;
		private float p_dist;

		void Awake ()
		{
				player = GameObject.FindGameObjectWithTag ("Player");

		}
		// Update is called once per frame
		void Update ()
		{
				p_dist = transform.position.x - player.transform.position.x;
				if (transform.position.x - player.transform.position.x < spawn_range) {
						Spawn ();		
				}
		}

		private void Spawn ()
		{

				enemy = (GameObject)Instantiate ((GameObject)Resources.Load ("Enemy/Enemy"), gameObject.transform.position, Quaternion.identity);
				enemy.GetComponent<Enemy> ().SetType (type);
				Destroy (gameObject);
		}
	
		
}
