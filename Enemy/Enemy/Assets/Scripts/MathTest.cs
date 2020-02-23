using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathTest : MonoBehaviour {

  // Use this for initialization
  [SerializeField]
  private float yAngle = 0.0f;
  [SerializeField]
  private float radAngle = 0.0f;
  [SerializeField]
  private float cosYAngle = 0.0f;

  private Quaternion qAngle = Quaternion.identity; // Init quaternion version of the angle we want from transform.localRotation
  private Vector3 angleVector = Vector3.zero; // Init Vector3 so we can use the Y angle in degrees

  MathTest me = null;
  private void Start()
  {
    me = GetComponent<MathTest>();
  }

  // Update is called once per frame
  void Update () {

    qAngle = me.transform.localRotation; // Grab the transform's localRotation in its native Quaternion
    angleVector = qAngle.eulerAngles; // Now we have degree angle Vector3 in angleVector

    // OK. We now have YAngle correct. All this trouble above just to get the angle.
    // So in my home code, is it that the angle value is in Quaterion? That may do it.
    yAngle = angleVector.y;

    // Now, is the cos correct?
    // Yes. Finally. What happened, I believe, is that Cos was demanding a RADIAN angle, when I was passing it a DEG angle.
    // So before taking the angle I found (in degrees) by comparing the positions of the two objects, convert it to RAD before finding the SIN and COSINE and that should work

    radAngle = yAngle * Mathf.Deg2Rad;

    cosYAngle = Mathf.Cos( radAngle ); // FIXED!
	}
}
