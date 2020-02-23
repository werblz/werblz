using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player_Controller : MonoBehaviour 
{

    private Rigidbody rb;

    private bool isGrounded = false;

    [Header("Motion")]
    [Tooltip("Is the player given a kick at start?")]
    [SerializeField]
    private bool initialThrustFlag = false;

    [Tooltip("How much kick to give player at start")]
    [SerializeField]
    private float initialThrustMult = 1.0f;

    [Header( "Indicators" )]
    [Tooltip("Show (or hide) vector indicators")]
    [SerializeField]
    private bool showIndicators = true;

    [Tooltip("GameObject of the Player Velocity Vector indicator")]
    [SerializeField]
    private GameObject indicatorPlayerVector = null;

    [Tooltip("How far ahead of player for velocity to look")]
    [SerializeField]
    private float playerPredictionDistance = 5.0f;

    [Tooltip("GameObject of the Joystick Vector indicator")]
    [SerializeField]
    private GameObject indicatorJoystickVector = null;

    [Tooltip("Distance for the Joystick Vector to look ahead")]
    [SerializeField]
    private float joystickPredictionDistance = 5.0f;

    [Tooltip("The LineRenderer that will draw the line between the vectors")]
    [SerializeField]
    LineRenderer vectorLineRenderer = null;

    [Tooltip("The LineRenderer of the resulting direction based on the two vector indicators")]
    [SerializeField]
    LineRenderer intendedLineRenderer = null;


    private float moveHorizontal = 0.0f;
    private float moveVertical = 0.0f;
	
    [Tooltip("Speed")]
    [SerializeField]
    private float thrustMultiplier = 20.0f;

    private Vector3 movement = new Vector3 (0.0f, 0.0f, 0.0f);

    private Vector3[] vectorPositions = new Vector3[ 3 ];
    private Vector3[] intendedPositions = new Vector3[ 2 ];









  // NEXT:
  //
  // Add a joy button press for visualizers. Show visuals only if the button is pressed.









  private void Start()
    {
		
        rb = GetComponent<Rigidbody>();
        rb.sleepThreshold = 1.0F;
	
    }





    void FixedUpdate()
    {

        // Not sure why this should ask if ThrustFlag is false. It seems it should ask if it's true
        // If I remember right, ThrustFlag adds a random thrust at start of play, so the player drops
        // at a random angle, not straight down.
        InitialImpulse();
        
        if (isGrounded)
        {
            moveHorizontal = Input.GetAxis("Horizontal");
            moveVertical = Input.GetAxis("Vertical");

            movement = new Vector3(moveHorizontal * thrustMultiplier, 0.0f, moveVertical * thrustMultiplier);
            rb.AddForce(movement);
        }


        // Place the vector object to indicate where the ball is heading.
        // Pass the Player Vector Indicator object
        // Pass the current rigidbody position
        // Pass the current rigidbody velocity
        PlaceVectorIndicator(indicatorPlayerVector, rb.transform.position, rb.velocity, playerPredictionDistance);

        // Place the Joystick Vector Indicator object
        // Pass the Joystick Vector object
        // Pass the Player Vector object so we can use its position as a start
        // Pass the current rigidbody position (may not actually need that)
        // Pass the joystick's two input floats
        // Pass a multiplier for how far we want that prediction to go from origin
        PlaceJoystickIndicator(indicatorJoystickVector, indicatorPlayerVector, rb.transform.position, moveHorizontal, moveVertical, joystickPredictionDistance);

        DrawVectorLine(rb, indicatorPlayerVector, indicatorJoystickVector);



    }

    void PlaceVectorIndicator( GameObject ind, Vector3 pos, Vector3 vel, float dist)
    {
        Vector3 newPos = pos + (vel * dist);
        newPos.y = 0.1f;
        ind.transform.localPosition = newPos;
    }

    void PlaceJoystickIndicator( GameObject joyInd, GameObject playerInd, Vector3 pos, float horiz, float vert, float dist)
    {
        Vector3 newJoyPos = playerInd.transform.localPosition;
        newJoyPos.x = newJoyPos.x + (horiz * dist);
        newJoyPos.y = 0.15f;
        newJoyPos.z = newJoyPos.z + (vert * dist);
        joyInd.transform.localPosition = newJoyPos;
    }

    void DrawVectorLine(Rigidbody obj1, GameObject obj2, GameObject obj3)
    {
    if (showIndicators)
    {
        // If showIndicators is true (flag in editor, later make it map to a joystick trigger)
        // Make sure both visualizer objects are active
        // Then make sure the 3 points in the line vectorPositions are set to the 3 objects

        obj2.SetActive( true );
        obj3.SetActive( true );

        vectorPositions[ 0 ] = obj1.transform.localPosition;
        vectorPositions[ 1 ] = obj2.transform.localPosition;
        vectorPositions[ 2 ] = obj3.transform.localPosition;

        intendedPositions[ 0 ] = vectorPositions[ 0 ]; // Set start point to the Player, the first object
        intendedPositions[ 1 ] = vectorPositions[ 2 ]; // Set end point to the JOYSTICK indicator, the 3rd object


    }
    else
    {
        // If showIndicators is false (flag in editor, later make it map to a joystick trigger)
        // Make sure both visualizers are inactive
        // Then make sure all 3 points of the line renderer's array are set to player so the line draws inside player and you don't see it.
        obj2.SetActive( false );
        obj3.SetActive( false );

        vectorPositions[ 0 ] = rb.transform.localPosition;
        vectorPositions[ 1 ] = rb.transform.localPosition;
        vectorPositions[ 2 ] = rb.transform.localPosition;

        intendedPositions[ 0 ] = rb.transform.localPosition;
        intendedPositions[ 1 ] = rb.transform.localPosition;
    }

    vectorLineRenderer.positionCount = 3;
    vectorLineRenderer.SetPositions( vectorPositions );

    intendedLineRenderer.positionCount = 2;
    intendedLineRenderer.SetPositions( intendedPositions );

  }



    void InitialImpulse()
    {
        if (initialThrustFlag == true)
        {
            float RandXThrust = UnityEngine.Random.value;
            Vector3 StartMovement = new Vector3((UnityEngine.Random.value - .5f), UnityEngine.Random.value, (UnityEngine.Random.value - .5f));
            rb.AddForce(StartMovement * initialThrustMult);
            initialThrustFlag = false;
        }

    }
    void OnCollisionEnter( Collision other )
    {
        if ( other.gameObject.tag == "Ground" )
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit( Collision other )
    {
        if ( other.gameObject.tag == "Ground" )
        {
            isGrounded = false;
        }
    }
    
}
