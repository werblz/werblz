using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airship_Event_Driver : MonoBehaviour {

    // See, this is where I find Unity dumb.
    // I have to create a script on the object that has the Animator so the Animation can
    // call an event on it, which can restart the animation when the animation is over.
    // But that same event also has to call the paretnt's Airship_Mover script method
    // PlaceShip() so it can restet the ship's location and rotation
    //  In order to do this, I have to have the animator's script reference the parent

    // I call this an event driver only because this script has to be on the animator
    // for me to call an event, even if the event is going to call something from the parent
    // script (and also trigger in this object, so fair's fair.)

    // We get this so we can call its reset function to start a new ship at a different
    // location/rotation
    [SerializeField]
    private Airship_Mover mover = null;

    private Animator anim = null;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();

        // Random starts with a given speed, then ADDS a multipler on the same times the random num 0-3
        anim.speed = 0.1f + (Random.value * 3.0f * 0.1f); // .01 is a good base. Now alter it by random

    }



    public void RestartAnim()
    {
        anim.speed = (Random.value * 30.0f) * 0.01f; // .01 is a good base. Now alter it by random
        mover.PlaceShip();
        anim.SetTrigger("Start");
        

    }
}
