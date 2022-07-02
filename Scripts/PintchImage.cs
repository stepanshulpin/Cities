using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PintchImage : MonoBehaviour {


    public float scaleSpeed = 1.0f;
    public float maxScale = 3f;
    public float minScale = 0.25f;
    // Update is called once per frame
    void Update () {
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPoes = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPoes = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltamag = (touchZeroPrevPoes - touchOnePrevPoes).magnitude;
            float touchDeltamag = (touchZero.position - touchOne.position).magnitude;
            
            Vector3 newScale = transform.localScale * scaleSpeed * touchDeltamag / prevTouchDeltamag;
            if (newScale.x > maxScale) transform.localScale = new Vector3(maxScale, maxScale, maxScale);
            else if (newScale.x < minScale) transform.localScale = new Vector3(minScale, minScale, minScale);
            else transform.localScale = newScale;
        }
	}
}
