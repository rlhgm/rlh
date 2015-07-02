using System;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class Camera2DFollow : MonoBehaviour
    {
        public Transform target;
        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;

		public Transform[] backgroundsNodes;
		public Vector2[] backgroundsRatios;

		private Vector3 lastPos;

		Camera camera;
        // Use this for initialization
        private void Start()
        {
			camera = GetComponent<Camera> ();

			transform.position = new Vector3( target.position.x, target.position.y, transform.position.z );
            transform.parent = null;

			lastPos = transform.position;
        }


        // Update is called once per frame
        private void Update()
        {
//			print ("---------------------------------------");
//			//print (camera.orthographicSize);
//			float asp = (float)Screen.width / (float)Screen.height;
//			float aspInv = (float)Screen.height / (float)Screen.width;
//			float horzExtent = camera.orthographicSize * asp;
//			print ( Screen.width + " x " +  Screen.height + " aspect: " + asp + " " + aspInv + " " + horzExtent);
//			
//			print ("---------------------------------------");

			transform.position = new Vector3( target.position.x, target.position.y, transform.position.z );

			int numberOfBackgrounds = backgroundsNodes.Length;

			if (numberOfBackgrounds != backgroundsRatios.Length) {
				print ("numberOfBackgrounds != backgroundsRatios.GetLength ()");
				return;
			}

			for( int i = 0 ; i < numberOfBackgrounds ; ++i ){
				Vector3 pos = backgroundsNodes[i].position;
				pos.x = transform.position.x * backgroundsRatios[i].x;
				pos.y = transform.position.y * backgroundsRatios[i].y;
				backgroundsNodes[i].position = pos;
			}
        }
    }
}
