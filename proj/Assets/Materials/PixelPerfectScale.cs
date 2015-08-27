using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PixelPerfectScale : MonoBehaviour
{
	public int screenVerticalPixels = 768;

	public bool preferUncropped = true;

	public float screenPixelsY = 0;
	
	public bool currentCropped = false;

	public float screenRatio;
	public float ratio;

	public Camera mainCamera = null;
	public Camera touchCamera = null;

	void Update()
	{
		if(screenPixelsY != (float)Screen.height || currentCropped != preferUncropped)
		{
			screenPixelsY = (float)Screen.height;
			currentCropped = preferUncropped;

			screenRatio = screenPixelsY/screenVerticalPixels;
			//ratio;

			if(preferUncropped)
			{
				ratio = Mathf.Floor(screenRatio)/screenRatio;
			}
			else
			{
				ratio = Mathf.Ceil(screenRatio)/screenRatio;
			}

			transform.localScale = Vector3.one*ratio;

			if( touchCamera ){
				touchCamera.orthographicSize = mainCamera.orthographicSize * screenRatio;
			}
		}
	}
}
