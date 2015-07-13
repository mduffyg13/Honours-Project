using UnityEngine;

namespace UnitySampleAssets._2D
{

    public class Camera2DFollow : MonoBehaviour
    {

        public Transform target;
        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;

        private float offsetZ;
        private Vector3 lastTargetPosition;
        private Vector3 currentVelocity;
        private Vector3 lookAheadPos;
		public  float y_pos = 1.0f;
        // Use this for initialization
        private void Start()
        {
			//Find spawned player on level load
			target = GameObject.Find("Player(Clone)").transform;

            lastTargetPosition = target.position;
            offsetZ = (transform.position - target.position).z;
            transform.parent = null;
        }

        // Update is called once per frame
        private void Update()
        {
//			Debug.Log ("Camera Target: " + target.ToString ());
			if (target == null) {
				//Debug.Log("No Taget Found");

				target = GameObject.FindGameObjectWithTag("Player").transform;
				offsetZ = (transform.position - target.position).z;

			}
            // only update lookahead pos if accelerating or changed direction
            float xMoveDelta = (target.position - lastTargetPosition).x;

            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

            if (updateLookAheadTarget)
            {
                lookAheadPos = lookAheadFactor*Vector3.right*Mathf.Sign(xMoveDelta);
            }
            else
            {
                lookAheadPos = Vector3.MoveTowards(lookAheadPos, Vector3.zero, Time.deltaTime*lookAheadReturnSpeed);
            }

            Vector3 aheadTargetPos = target.position + lookAheadPos + Vector3.forward*offsetZ;
			Vector3 newnewPos = new Vector3 (aheadTargetPos.x, y_pos, aheadTargetPos.z);
			Vector3 newPos = Vector3.SmoothDamp(transform.position,newnewPos , ref currentVelocity, damping);
			//aheadTargetPos
            transform.position = newPos;

            lastTargetPosition = target.position;
        }
    }
}