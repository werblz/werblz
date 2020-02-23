using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Follow : MonoBehaviour {

    [SerializeField]
    private GameObject player = null;

    [SerializeField]
    private GameObject mainCameraMover = null;

    [SerializeField]
    private Vector3 cameraOffset = new Vector3(0.0f, 0.0f, 0.0f);

    [SerializeField]
    private Vector3 rot = new Vector3(0.0f, 0.0f, 0.0f);

    [SerializeField]
    private float cameraDistance = 0.0f;

    private Vector3 camPos = new Vector3(0.0f, 0.0f, 0.0f);

    // Use this for initialization
    void Start()
    {


    }
	
	// Update is called once per frame
	void Update ()
    {
        
        // Place Camera_Parent where player is, plus offset
        transform.localPosition = player.transform.localPosition + cameraOffset;

        // Move camera mover object back a distance
        camPos.z = cameraDistance;
        mainCameraMover.transform.localPosition = camPos;

        // Rotate the Camera Parent
        transform.localRotation = Quaternion.Euler(rot);
	}
}
