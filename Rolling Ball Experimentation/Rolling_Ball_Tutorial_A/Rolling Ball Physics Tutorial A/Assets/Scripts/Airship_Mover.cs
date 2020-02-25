using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Airship_Mover : MonoBehaviour {

    [SerializeField]
    private Vector3 airshipStartPosition = Vector3.zero;

    [SerializeField]
    private float cityRadius = 500.0f;

    [SerializeField]
    public GameObject rotator = null;

    public float yOffset = 390.0f;

    public float xOffset = 500.0f;

    [SerializeField]
    public float airshipHeight = 10.0f;

    // Use this for initialization..
    void Start () {

        PlaceShip();
		
	}


    public void PlaceShip()
    {
        // Just offset in the X
        // No. Let the GameManager do this.
        //float xOffset = (Random.value * cityRadius * 2.0f) - cityRadius;
        // Not using yOffset ecuase GameManager wants to place it vertically, so it can
        // set each ship in its own vertical lane
        //transform.localPosition = new Vector3(xOffset, 0.0f, 0.0f);
        
        // For now, do not let the ships  rotate themselves
        //rotator.transform.eulerAngles = new Vector3(0.0f, Random.value * 360.0f, 0.0f);
        enabled = true;
    }
}
