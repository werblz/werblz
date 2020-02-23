using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Pointer : MonoBehaviour {

    [Tooltip("GameObject that is parent to the direction pointer")]
    [SerializeField]
    private GameObject pointerParent = null;

    [Tooltip("GameObject that is parent of the radial indicator sprite")]
    [SerializeField]
    private GameObject circleIndicator = null;

    [Tooltip("SpriteRenderer of the racial indicator circle")]
    [SerializeField]
    public SpriteRenderer circleSprite = null;

    [Tooltip("Do we show the direction and attack radius indicators?")]
    [SerializeField]
    private bool showIndicators = false;

    [Tooltip("Enemy script so we can use its data")]
    [SerializeField]
    private Enemy_Controller enemy = null; // Reference in the SCRIPT of the ball, not the ball itself or the rigidbody. Need a variable "lerpedAngle" from there

    Vector3 pointerAngle = new Vector3(0.0f, 0.0f, 0.0f);

    [Tooltip("The color of the radius ring if it is not an attacker. (ie: invisible)")]
    [SerializeField]
    private Color goneColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);

    [Tooltip("The color of the radius when it is counting down to attack mode. (ie: is inactive)")]
    [SerializeField]
    private Color passiveColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);

    [Tooltip("The color the radius rung goes just a bit before it becomes active, as a warning window.")]
    [SerializeField]
    private Color warningColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);

    [Tooltip("The color of the radial ring when it is primed to attack")]
    [SerializeField]
    private Color attackColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);

    [Tooltip("The color of the radial ring if it is an exploder (kill must also be true)")]
    [SerializeField]
    public Color exploderColor = new Color(0.0f, 0.0f, 0.0f, 0.0f); // Public so the Rigidbody can access it by direct reference

    [Tooltip("The color we turn the ring during an explosion. Currently this is very fast. One frame. And it will go away in favor of VFX")]
    [SerializeField]
    public Color boomColor = new Color(0.0f, 0.0f, 0.0f, 0.0f); // Public so the Rigidbody can access it by direct reference


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {


        if ( !enemy.kill )
        {
            circleSprite.color = goneColor;
        }
        else
        {
            if ( enemy.attackCountDownTimer < 0.0f )
            {
                circleSprite.color = attackColor;
            }
            else if ( enemy.attackCountDownTimer < .3f )
            {
                circleSprite.color = warningColor;
            }
            else
            {
                if (enemy.explode)
                {
                    // Here, have a seconde sprite (shaped like a deathstar exploding ring) that is animated outward rapidly
                    // Put an Animator on it, and the trigger "Explode" triggers a rapid expansion, then back to idle which is smaller than the ball
                    // Then here:
                    // exploderAnimator.SetTrigger("Explode");
                    circleSprite.color = exploderColor;
                }
                else
                {
                    circleSprite.color = passiveColor;

                }

            }
        }
        
        if (showIndicators)
        {
            pointerParent.SetActive(true);
            circleIndicator.SetActive(true);


            pointerParent.transform.localPosition = enemy.transform.localPosition; // Put the pointer exactly where the ball is
            circleIndicator.transform.localPosition = new Vector3(enemy.transform.localPosition.x, (enemy.transform.localPosition.y - 0.4f), enemy.transform.localPosition.z); // Put the pointer exactly where the ball is

            // Get enemy ball's rotation
            pointerAngle = new Vector3(0.0f, enemy.lerpedAngle, 0.0f);
            pointerParent.transform.localRotation = Quaternion.Euler(pointerAngle * Mathf.Rad2Deg);

            circleIndicator.transform.localScale = new Vector3(enemy.attackDistance * 2.0f, 1.0f, enemy.attackDistance * 2.0f);
        }
        else
        {
            pointerParent.SetActive(false);
            circleIndicator.SetActive(false);

        }

        
        
    }
}
