using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {


    Vector3 lastFramePosition;
    Vector3 currFramePosition;
    // Use this for initialization
    void Start () {
        currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        currFramePosition.z = 0;
        lastFramePosition.z = 0;
    }
	
	// Update is called once per frame
	void Update () {
        currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currFramePosition.z = 0;


        UpdateCameraMovement();


        //save mouse position don't use curr frame pos due to the possibility of a moved camera
        lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastFramePosition.z = 0;

    }

    void UpdateCameraMovement()
    {
        // Handle screen panning
        if (Input.GetMouseButton(2))
        { // Right or Middle Mouse Button

            Vector3 diff = lastFramePosition - currFramePosition;
            Camera.main.transform.Translate(diff);

        }

        Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel");

        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 3f, 25f);
    }
}
