//Script attached to enemy spawn gameObject

using UnityEngine;
using System.Collections;

public class EnemySpawnScript : MonoBehaviour
{
		private float spawn_range = 10.0f;
		public Enemy.EnemyType enemyType;
		private GameObject enemy;
		private GameObject player;
		
		void Awake ()
		{
				player = GameObject.FindGameObjectWithTag ("Player");
		}

		// Update is called once per frame
		void Update ()
		{
				if (transform.position.x - player.transform.position.x < spawn_range) {
						Spawn ();		
				}
		}

		private void Spawn ()
		{
			enemy = (GameObject)Instantiate ((GameObject)Resources.Load ("Enemy/Enemy"), gameObject.transform.position, Quaternion.identity);
			enemy.GetComponent<Enemy> ().SetType (enemyType);
			Destroy (gameObject);
		}
	
		
}
