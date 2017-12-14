using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpBackForth : MonoBehaviour {

    public float speedForward;
    public float speedBackward;
    public float delayTurnAround;

    private bool movingForward = true;

    public GameObject startMarker;
    public GameObject endMarker;

    private Vector3 startPos;
    private Vector3 endPos;
    private float startTime;
    private float journeyLength;

    private bool isMoving;




	// Use this for initialization
	void Start () {
        this.endPos = this.endMarker.transform.position;
        this.startPos = (this.startMarker == null) ? this.endMarker.transform.position : this.gameObject.transform.position;

        // If there's no start marker game object assigned, movement will start from this game object's current position.
        if (startMarker == null)
        {
            print("No start marker game object. Defaulting to current player position for startPos.");
            this.startPos = this.gameObject.transform.position;
        }
	}
	
    void FixedUpdate()
    {
        if (isMoving)
        {
            if (Mathf.Abs(Vector3.Distance(this.transform.position, this.endPos)) < .01f)
            {
                // Reached the end of part of the journey.
                isMoving = false;

                // Do we need to move back to the original position, or are we done?
                if (movingForward)
                {
                    // Still need to move back to original position.
                    movingForward = false;
                    swapMarkers();
                    Invoke("startMoving", delayTurnAround);
                }
                else
                {
                    // We're done moving.
                    movingForward = true;
                    swapMarkers();
                }
            }
            else
            {
                // Move
                float currSpeed = movingForward ? speedForward : speedBackward;
                float distCovered = (Time.time - startTime) * currSpeed;
                float fracJourney = distCovered / journeyLength;
                this.transform.position = Vector3.Lerp(startPos, endPos, fracJourney);
            }
        }
    }

    // Only called from startMoving()
    private void resetLengthAndTime()
    {
        journeyLength = Vector3.Distance(startPos, endPos);
        startTime = Time.time;
    }

    private void swapMarkers()
    {
        Vector3 temp = startPos;
        startPos = endPos;
        endPos = startPos;
    }

    private void startMoving()
    {
        resetLengthAndTime();
        isMoving = true;
    }
}
