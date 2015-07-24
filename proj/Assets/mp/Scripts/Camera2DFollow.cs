using System;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class Camera2DFollow : MonoBehaviour
    {
        //public Transform target;
		public Player2Controller target;
        //public float damping = 1;
        //public float lookAheadFactor = 3;
        //public float lookAheadReturnSpeed = 0.5f;
        //public float lookAheadMoveThreshold = 0.1f;

        //private float m_OffsetZ;
        //private Vector3 m_LastTargetPosition;
        //private Vector3 m_CurrentVelocity;
        //private Vector3 m_LookAheadPos;

		public Transform[] backgroundsNodes;
		public Vector2[] backgroundsRatios;

		public GameObject[] backgroundsBackgrounds;

		private Vector3 lastPos;

		public Vector2 stageSize = new Vector2 (20f, 10f);
		public Vector2 stagesOffset = new Vector2 (0f, 0f);
		// jak duzo jednosek pokazuje na ekranie
		Vector2 hms = new Vector2 ();

		public Vector2 targetStage = new Vector2 ();

		public Vector2 backgroundLimits = new Vector2 (-30, 30);

		Camera camera;

		void Awake(){
			//public GameObject[] backgroundsBackgrounds;
			for (int i = 0; i < backgroundsNodes.Length; ++i) {
				if( backgroundsNodes[i] == null ) continue;
				if( backgroundsBackgrounds[i] == null) continue;

				GameObject bckg = Instantiate<GameObject> (backgroundsBackgrounds[i]);
				bckg.transform.position = new Vector3(0f,0f,0f);
				bckg.transform.parent = backgroundsNodes[i];

				SpriteRenderer bckgSpriteRend = bckg.GetComponent<SpriteRenderer>();
				//print(bckgSpriteRend.bounds);
				//print(bckgSpriteRend.sprite.border);

				Vector3 bckgItemSize = bckgSpriteRend.bounds.extents;

				float bckgDist = bckgItemSize.x;
				int c = 1;

				while( bckgDist < backgroundLimits.y ){
					//c = 1;
					bckg = Instantiate<GameObject> (backgroundsBackgrounds[i]);
					bckg.transform.position = new Vector3(bckgDist + bckgItemSize.x ,0f,0f);
					bckg.transform.parent = backgroundsNodes[i];
					bckgDist = bckgItemSize.x + c * (bckgItemSize.x*2);
					c += 1;
				}

				bckgDist = -bckgItemSize.x;
				c = 1;

				while( bckgDist > backgroundLimits.x ){
					//c = 1;
					bckg = Instantiate<GameObject> (backgroundsBackgrounds[i]);
					bckg.transform.position = new Vector3(bckgDist - bckgItemSize.x ,0f,0f);
					bckg.transform.parent = backgroundsNodes[i];
					bckgDist = -bckgItemSize.x - c * (bckgItemSize.x*2);
					c+=1;
				}
			}
		}

        // Use this for initialization
        private void Start()
        {
			///aaa
			camera = GetComponent<Camera> ();

			hms.x = camera.orthographicSize * camera.aspect;
			hms.y = camera.orthographicSize;

			transform.position = new Vector3( target.transform.position.x, target.transform.position.y, transform.position.z );
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
			//Vector3 oldPos = transform.position;

			targetStage = getTargetStage ();

			Vector3 newPos = new Vector3( target.transform.position.x, target.transform.position.y, transform.position.z );

			if (target.isInState (Player2Controller.State.CLIMB_ROPE)) {
				newPos.y -= target.cameraTargetRopeDiffY;
			} else {
				newPos.y += target.cameraTargetNormalDiffY;
			}

			Vector3 res = fitToStage (targetStage, newPos);

			//Vector3 posDiff = res - transform.position;
			//if( posDiff.magnitude > 1.0f ) res

			transform.position = res;

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

			lastPos = transform.position;
        }

		Vector2 getTargetStage(){
			Vector2 targetStage = new Vector2 ();

			Vector3 targetPos = target.transform.position;
			if (target.isInState (Player2Controller.State.CLIMB_ROPE)) {
				targetPos.y -= target.cameraTargetRopeDiffY;
			} else {
				targetPos.y += target.cameraTargetNormalDiffY;
			}

			targetStage.x =  (targetPos.x - stagesOffset.x) / stageSize.x;
			targetStage.y =  (targetPos.y - stagesOffset.y) / stageSize.y;

			//if (targetStage.x > 0)
			targetStage.x = Mathf.Floor (targetStage.x);
			targetStage.y = Mathf.Floor (targetStage.y);

			return targetStage;
		}

		Vector4 getStageMinMax(Vector2 stage){
			Vector4 stageMinMax = new Vector4 ();

			//min
			stageMinMax.x = stagesOffset.x + stage.x * stageSize.x;
			stageMinMax.y = stagesOffset.y + stage.y * stageSize.y;

			//max
			stageMinMax.z = stageMinMax.x + stageSize.x;
			stageMinMax.w = stageMinMax.y + stageSize.y;

			return stageMinMax;
		}

		Vector4 getMinMaxPosInStage(Vector2 stage){
			Vector4 stageMinMax = getStageMinMax (stage);

			float _tmp;

			stageMinMax.x += hms.x;
			stageMinMax.y += hms.y;

			stageMinMax.z -= hms.x;
			stageMinMax.w -= hms.y;

			if (stageMinMax.x > stageMinMax.z) {
				_tmp = stageMinMax.x;
				stageMinMax.x = stageMinMax.z;
				stageMinMax.z = _tmp;
			}

			if (stageMinMax.y > stageMinMax.w) {
				_tmp = stageMinMax.y;
				stageMinMax.y = stageMinMax.w;
				stageMinMax.w = _tmp;
			}

			return stageMinMax;
		}

		Vector3 fitToStage (Vector2 stage, Vector3 pos){
			Vector4 minMaxPosInStage = getMinMaxPosInStage (stage);

			Vector2 stageCenter = getStageCenter (stage);

			if (hms.x*2.0f > stageSize.x) {
				pos.x = stageCenter.x;
			} else {
				if (pos.x < minMaxPosInStage.x)
					pos.x = minMaxPosInStage.x;
				if (pos.x > minMaxPosInStage.z)
					pos.x = minMaxPosInStage.z;
			}

			if (hms.y*2.0f > stageSize.y) {
				pos.y = stageCenter.y;
			} else {
				if (pos.y < minMaxPosInStage.y)
					pos.y = minMaxPosInStage.y;
				if (pos.y > minMaxPosInStage.w)
					pos.y = minMaxPosInStage.w;
			}

			Vector3 res = pos;

			return res;
		}

		Vector2 getStageCenter(Vector2 stage){
			Vector2 stageCenter = new Vector2 ();
			Vector4 stageMinMax = getStageMinMax (stage);

			stageCenter.x = stageMinMax.x + stageSize.x * 0.5f;
			stageCenter.y = stageMinMax.y + stageSize.y * 0.5f;

			return stageCenter;
		}
    }
}
