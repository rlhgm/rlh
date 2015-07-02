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

		public Transform backgroundNearNode;
		public Transform backgroundFarNode;

		public Vector2 backgroundNearRatio = new Vector2(0.1f,0.05f);
		public Vector2 backgroundFarRatio = new Vector2(0.3f,0.15f);

		private Vector3 lastPos;

		Camera camera;
        // Use this for initialization
        private void Start()
        {
			camera = GetComponent<Camera> ();

			print ("---------------------------------------");
			print (camera.orthographicSize);
			float horzExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
			print (horzExtent);

			print ("---------------------------------------");
			//m_LastTargetPosition = target.position;
            //m_OffsetZ = (transform.position - target.position).z;

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

			if( backgroundNearNode ){
				Vector3 pos = backgroundNearNode.position;
				pos.x = transform.position.x * backgroundNearRatio.x;
				pos.y = transform.position.y * backgroundNearRatio.y;
				backgroundNearNode.position = pos;
			}

			if( backgroundFarNode ){
				Vector3 pos = backgroundFarNode.position;
				pos.x = transform.position.x * backgroundFarRatio.x;
				pos.y = transform.position.y * backgroundFarRatio.y;
				backgroundFarNode.position = pos;
			}

//            // only update lookahead pos if accelerating or changed direction
//            float xMoveDelta = (target.position - m_LastTargetPosition).x;
//
//            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;
//
//            if (updateLookAheadTarget)
//            {
//                m_LookAheadPos = lookAheadFactor*Vector3.right*Mathf.Sign(xMoveDelta);
//            }
//            else
//            {
//                m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime*lookAheadReturnSpeed);
//            }
//
//            Vector3 aheadTargetPos = target.position + m_LookAheadPos + Vector3.forward*m_OffsetZ;
//            Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);
//
//            transform.position = newPos;
//
//            m_LastTargetPosition = target.position;
        }
    }
}
