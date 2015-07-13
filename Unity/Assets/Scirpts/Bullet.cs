using UnityEngine;
using System.Collections;

//Class which controls the movement and interactions of a bullet object
//attached to bullet gameObject

public class Bullet: MonoBehaviour
{

		private Vector3 currentPosition;
		private Vector3 direction;
		private float speed = 3.0f;
		private int alive_counter;
		private int aliver_time = 300;

		// Use this for initialization
		void Start ()
		{
				currentPosition = transform.position;
				alive_counter = aliver_time;
		}
	
		// Update is called once per frame
		void Update ()
		{
				transform.Translate (new Vector3 (direction.x * speed * Time.deltaTime, direction.y * speed * Time.deltaTime, 0.0f));

				if (alive_counter < 0) {
						Destroy (gameObject);
						alive_counter = aliver_time;
				}
				alive_counter--;
		}

		public void SetDir (Vector3 v3in)
		{
				direction = v3in;
		}
}
