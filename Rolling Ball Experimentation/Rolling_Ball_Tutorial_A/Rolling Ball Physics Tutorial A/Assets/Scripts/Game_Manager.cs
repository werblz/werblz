using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class Game_Manager : MonoBehaviour {

    [Header("Objects")]

    [SerializeField]
    private GameObject padObject = null;

    [SerializeField]
    private GameObject[] buildingObject = null;

    [SerializeField]
    private GameObject stationObject = null;

    [SerializeField]
    private GameObject homeObject = null;

    [SerializeField]
    private Taxi_Controller taxi = null;

    [SerializeField]
    private Airship_Mover airShipObject = null;

    private Airship_Mover[] airShip = null;

    [Header("City Setup")]
    [SerializeField]
    public int numPads = 10;

    [SerializeField]
    public int numStations = 8;

    [SerializeField]
    private float maxPadDistance = 500.0f;
    [SerializeField]
    private float minAllowableDistanceBetweenPads = 10.0f;

    [SerializeField]
    private float maxBuildingXZScale = 5.0f;
    [SerializeField]
    private float maxBuildingXZScaleOffset = 3.0f;
    [SerializeField]
    private float maxBuildingYScale = 30.0f;
    [SerializeField]
    private float maxBuildingYScaleOffset = 20.0f;
    [SerializeField]
    private Texture2D layoutTexture = null;
    [SerializeField]
    private float layoutWiggleMultiplier = 1.0f;
    [SerializeField]
    private float layoutWiggleHeightMultiplier = 1.0f;
    [SerializeField]
    private float minBuildingHeight = 1.0f;
    [SerializeField]
    private float gasPadScale = 5.0f;
    [SerializeField]
    private float landingPadScale = 10.0f;
    [SerializeField]
    private int numAirships = 10;
    [SerializeField]
    private float airshipRadius = 250.0f;

    
    // NEXT^^^ Control how many airships are instantiated
    // and when, and set them flying, each with a different anim speed. I may have to also
    // build into each animator a start delay so they don't all come in at once



    [Header("Scoring")]

    [SerializeField]
    public float cash = 100.0f; // SAVE

    [SerializeField]
    public float fare = 20.0f;

    [SerializeField]
    private float standardFare = 10.0f; // SAVE (upgrade?)

    [SerializeField]
    public float fareDistanceMultiplier = 0.05f; // This is the multiplier for the distance between taxi and next target, for fare calculation

    [SerializeField]
    public float fareDrain = 0.005f; // SAVE (upgrade?)

    [SerializeField]
    public float tip = 10.0f;

    [SerializeField]
    public float standardTip = 10.00f;

    [SerializeField]
    public float tipDrain = 1.0f; // SAVE (upgrade?) Tip drain for now happens only due to collision


    // THESE BOTH NOW WORK PER PHYSICS FRAME! THIS IS BAD!
    // MAKE THEM WORK BY time.deltaTime PER SECOND! That way I don't have to do TWO calculations
    // For different costs for gas at home and station
    [SerializeField]
    public float gasCost = 0.04f; // SAVE (upgrade?)
    [SerializeField]
    public float homePadGasCost = 0.02f; // SAVE (upgrade?) 20 if filled up during NextShift at half cost per "litre"
    [SerializeField]
    public float gasFillRate = 2.0f; // SAVE
    [SerializeField]
    public float homePadGasFillRate = 4.0f;

    [SerializeField]
    public float damageRepairCost = 0.02f; // SAVE (upgrade?)
    [SerializeField]
    public float homePadDamageRepairCost = 0.01f; // SAVE (upgrade?) Half of damage at gas pad
    [SerializeField]
    public float damageRepairRate = 0.1f; // SAVE
    [SerializeField]
    public float homePadDamageRepairRate = 0.2f; // SAVE

    [SerializeField] 
    public float crashDeductible = 500.0f; // SAVE

    [HideInInspector]
    public GameObject[] pads = null;
    private GameObject[] buildings = null;
    [HideInInspector]
    public GameObject[] stations = null;
    [HideInInspector]
    public GameObject homeBldg = null;

    [HideInInspector]
    public int numPadsLandedOn = 0;

    public GameObject summaryTextParent = null;
    public TextMeshPro cashText = null;
    public TextMeshPro fareText = null;
    public TextMeshPro tipText = null;

    [SerializeField]
    private TextMeshPro summaryText = null;
    [SerializeField]
    private TextMeshPro summaryWinsNumbersText = null;
    [SerializeField]
    private TextMeshPro summaryLossesNumbersText = null;
    [SerializeField]
    private TextMeshPro summaryCashText = null;

    // These are to keep track of stats for this shift only
    private float faresThisShift = 0.0f;
    private float tipsThisShift = 0.0f;
    public float gasCostThisShift = 0.0f; // Public because Taxi has to write to it
    public float repairsCostThisShift = 0.0f; // Public becuase Taxi has to write to it
    public float gasCostHome = 0.0f;
    public float repairsCostHome = 0.0f;
    private int numPadsThisShift = 0;

    [HideInInspector]
    public int nextPad = 0;

    public int numBuildingsInGrid = 0;

    private float buildingBuffer = 0.0f; // Buffer between max building sizes

    private float gridSize = 0.0f; // Size of grid which will be based on building XZ size

    private float gridCellSize = 0.0f; // Will be the size of each building cell

    private float tallestBuilding = 0.0f;

    [Header("Game Behavior")]
    [SerializeField]
    public bool uiIsUp = false; // This is for when I put up a UI panel. I need my non-UI update loops to just RETURN so I can perform UI panel work
    [SerializeField]
    private GameObject radarPanel = null;
    [SerializeField]
    public UI_Panel_Controller panelController = null;
    [SerializeField]
    public GameObject panel = null;
    [SerializeField]
    public TextMeshPro panelText = null;
    [SerializeField]
    public GameObject titleScreen = null;


    // SAVE ALL THESE
    public bool hasHomePad = false;
    public bool hasRadarPad = false;
    public bool hasRadarStation = false;
    public bool hasStrafe = false;
    public bool hasTurbo = false;
    public bool hasTank = false;
    public bool hasControl = false;

    [Header("Upgrades")]
    [SerializeField]
    public bool upgradesAvailable = false;

    [SerializeField]
    public GameObject[] upgrades = null;

    [SerializeField]
    public GameObject[] shiftDialogs = null;

    [SerializeField]
    public GameObject crashDialog = null;

    private GameObject[] shiftDialogInstances = null;

    [Header("Debug Stuff")]
    [SerializeField]
    public bool debugOn = false;
    [SerializeField]
    private int numDebugPads = 2;

    // Game stats
    [HideInInspector]
    public int shift = 1;

    // Use this for initialization
    void Start()
    {
        radarPanel.SetActive(false);

        Screen.SetResolution(1200, 800, false);

        
        upgradesAvailable = false;


        
        // TEST ONLY - FOR QUICK LEVELING! A debug only feature
        if (debugOn)
        {
            numPads = numDebugPads;
            Debug.Log("<color=red>DEBUG IS ON! THERE ARE ONLY TWO PADS THIS SHIFT!!!</color>");
        }

        //Start out clean
        cash = 100.0f;
        fare = 0.0f;

        faresThisShift = 0.0f;
        tipsThisShift = 0.0f;
        gasCostThisShift = 0.0f;
        repairsCostThisShift = 0.0f;
        gasCostHome = 0.0f;
        repairsCostHome = 0.0f;


        PopulateBuildingGrid(maxPadDistance);
        PopulatePads(numPads);
        PopulateGasStations(numStations);
        PopulateHomeBase();
        PopulateAirships(numAirships);


        RefreshScore();

        ///////////////////////////////////////////// HERE put code like this: pads[nextPad].Pad_Manager.isLit = true;
        Beam(0, true);
        taxi.taxiMovedToInitialLocation = false;

        NewFare(standardFare, 0); // Set first fare of the shift based on the distance between car and pad 0

        titleScreen.SetActive(false);
        radarPanel.SetActive(true);

        PutUiUp(shift, false);

    }



    private void LateUpdate()
    {
        hasControl = taxi.hasControl; // This is stupid. I have to tell Game Manager the taxi is in control or not just so the radar can know it.
        
    }

    private void FixedUpdate()
    {
        if ( !uiIsUp )
        {
            fare = fare - fareDrain;
            if (fare < 0.0f)
            {
                fare = 0.0f;
            }
        }
        
        RefreshScore();

    }


    void PopulateBuildingGrid(float maxPadDistance)
    {

        numBuildingsInGrid = 0; // Have to keep track of this as an index into the array, which is linear, while building a grid, which is 2d

        buildingBuffer = 0.0f; // Buffer between max building sizes

        gridSize = maxPadDistance / (maxBuildingXZScale + maxBuildingXZScaleOffset + buildingBuffer);

        gridCellSize = maxBuildingXZScale + maxBuildingXZScaleOffset;

        Vector3 bLoc = new Vector3(0.0f, 0.0f, 0.0f);

        Array.Resize(ref buildings, (int)((gridSize+1) * (gridSize+1))); // Resize Array to be grid x grid, which should be the max number of buildings BEFORE circle-culling
        //Debug.Log("<color=yellow> ****************** </color> GridSize = " + gridSize);
        //Debug.Log("<color=yellow> ****************** </color> GridCellSize = " + gridCellSize);


        for (int x = 0; x < gridSize; x++)  // Typical X, Y loop for placing things in a grid
        {
            for (int z = 0; z < gridSize; z++)
            {

                
                // Get a random scale to use later after placement
                Vector3 padScale = BuildingRandomScale();

                // buildingX and Y = the loop index plus the gridCellSize, or the size of each "square" in the grid. 
                float bx = x * gridCellSize;
                float bz = z * gridCellSize;

                bLoc = new Vector3((maxPadDistance / 2.0f) - bx, 0.0f, (maxPadDistance / 2.0f) - bz); // Ah yes. This goes from grid corner. We want 0,0 to be the CENTER of the grid



                // HERE - Check to see if bLoc is in the circle.
                if (HypotenuseDistance(bLoc) < (maxPadDistance / 2.0f))
                {
                    // Now that we have bLoc, place the building
                    int buildingChoice = (int)(UnityEngine.Random.value * 3.0f);
                    buildings[numBuildingsInGrid] = GameObject.Instantiate(buildingObject[buildingChoice]); // Instantiate building in array with index numBuildingsInGrid

                    // Wiggle it a little, so they aren't so gridded
                    bLoc.x = bLoc.x + UnityEngine.Random.value * maxBuildingXZScaleOffset;
                    bLoc.z = bLoc.z + UnityEngine.Random.value * maxBuildingXZScaleOffset;

                    Vector4 wiggleValue = TextureWiggle(bLoc);
                    //Debug.Log("<color=white>********</color><color=blue>*******</color> Vector 4 here is " + wiggleValue.ToString());

                    // Separate out the values I need. 
                    bLoc.x = bLoc.x + ((wiggleValue.x - 0.5f) * layoutWiggleMultiplier); // X
                    bLoc.y = 1.0f;// bLoc.y + (wiggleValue.y * layoutWiggleHeightMultiplier); // Height
                    bLoc.z = bLoc.z + ((wiggleValue.z - 0.5f) * layoutWiggleMultiplier); // Z
                    float rotY = wiggleValue.w; // .w is the first of a Vector4, so I will use that as rotation
                       

                    // Apply location and scale
                    buildings[numBuildingsInGrid].transform.position = bLoc;

                    padScale.y = padScale.y + ( (wiggleValue.y * layoutWiggleHeightMultiplier) + minBuildingHeight);
                    buildings[numBuildingsInGrid].transform.localScale = padScale;

                    // Now get that rotation from the texture. First, store the roatation pieces
                    Vector3 bldgRot = new Vector3(
                        buildings[numBuildingsInGrid].transform.localEulerAngles.x,
                        buildings[numBuildingsInGrid].transform.localEulerAngles.y,
                        buildings[numBuildingsInGrid].transform.localEulerAngles.z);

                    bldgRot.y = rotY * 360.0f; // Move the w of the returned Vector4 to the Y rotation * 360 because the value returned is 0-1

                    // Now put those rotations into the building
                    buildings[numBuildingsInGrid].transform.rotation = Quaternion.Euler(bldgRot); // Wow. Quaternion conversion is always hard to rememeber!

                    // Turn on building
                    buildings[numBuildingsInGrid].SetActive(true);

                    // THIS WILL FIND THE TALLEST BUILDING
                    // Find the height (scale) of this building.
                    float heightCheck = buildings[numBuildingsInGrid].transform.localScale.y;

                    // If it is taller than any before, record that height
                    if (heightCheck > tallestBuilding)
                    {
                        tallestBuilding = heightCheck;
                    }
                    numBuildingsInGrid++; // Increase array index
                }
            }
        }
        Debug.Log("<color=blue*******************</color> HEIGHT IS " + tallestBuilding);
        Debug.Log("<color=yellow>********************</color>  NUMBER OF BUILDINGS MADE: " + numBuildingsInGrid);
        //Debug.Log("<color=red>******************* Coordinates of the middle-most building:</color>" + buildings[(int)numBuildingsInGrid / 2].transform.position);
    }

    // This is slightly non-standard. I pass a Vector3 for the location of the building, and this method finds the pixel at the corresponding
    // coordinate of a four channel Texture2D. Then it returns the Vector3 PLUS one value for Y rotation, maiking it return a Vector 4
    private Vector4 TextureWiggle(Vector3 location)
    {
        // First, get the 0-1 decimal length across array I'm populating, per x and y
        float percentX = location.x / maxPadDistance + 0.5f; // decimal from 0-1 along the array side, then add .5 to offset it from corner
        float percentZ = location.z / maxPadDistance + 0.5f; // "

        // Next, find the pixel location in the texture that corresponds to the location in the 2D array of building locations
        int textureLocX = (int)(percentX * layoutTexture.width);
        int textureLocZ = (int)(percentZ * layoutTexture.height);

        // Get the pixel value at that texture location
        Color pixelValue = layoutTexture.GetPixel(textureLocX, textureLocZ);
        //Debug.Log("<color=yellow>****</color><color=red>****</color> Pixel Value at coord is " + pixelValue.ToString());

        // Now get the RED channel value, and nudge the building in the X direction
        float nudgeX = pixelValue.r;
        // Now get the GREEN channel value, and nudge the buliding in the Z direction
        float nudgeY = pixelValue.g;
        // Now get the BLUE channel value, and nudge the building in the Y direction (height)
        float nudgeZ = pixelValue.b;
        // Now get the ALPHA channel value, and nudge the building in the Y direction (height)
        float nudgeRotY = pixelValue.a;

        //Debug.Log("<color=red>****</color><color=cyan>****</color> Returning " + nudgeX + ", " + nudgeY + ", " + nudgeZ + ", " + nudgeRotY);
        return new Vector4 (nudgeX, nudgeY, nudgeZ, nudgeRotY); // DUE to the weirdness of Vector4, w, x, y, z, w comes first, so that
        // will be the rotation. That way, x, y and z can correspond properly to a location
    }


    private void Beam(int padNum, bool flag)
    {
        Pad_Manager pm = (Pad_Manager)pads[padNum].GetComponent(typeof(Pad_Manager));
        
        // Pass the bool for whether the beam is on or off to the pad manager
        // But also pass the pad num, so I can put the right texture on
        
        pm.LightBeam(flag, padNum);
        

    }


    // Advanc to the next pad. If last pad, GoToNextShift
    public void Advance()
    {
        AddFare(fare, tip);

        Beam(nextPad, false);
        //pads[nextPad].SetActive(false); // Now turn the WHOLE pad off. Because I don't want to make pads visible until they are the target

        nextPad++;

        if (nextPad >= numPads)
        {
            //Debug.LogWarning("<color=red>LEVEL FINISHED! </color><color=black>You just hit pad # " + (nextPad - 1) + "</color>");
            // Go to next shift passing crashed == false, because everything's normal.
            GoToNextShift(false);
            return;
        }
        else
        {
            //Debug.LogWarning("<color=yellow>*************</color><color=red> MOVING ON TO PAD " + nextPad + "</color>");

            
            Beam(nextPad, true);
        }


        NewFare(standardFare, nextPad);
        RefreshScore();
        
    }

    // GoToNextShift now takes a bool whether you've crashed or not. That then gets passed to PutUiUp
    void GoToNextShift(bool crashed)
    {
        if (crashed)
        {
            Debug.Log("\n\n");
            Debug.Log("\n\n");
            Debug.Log("\n\n");
            Debug.Log("\n\n");
            Debug.Log("                               CRASHED! BUT YOU ARE MOVING ON! BYGONES! ");
            // Remove deducitble

        }

        //taxi.cameraFollow = false;
        shift++;
        
        RefreshScore();

        /* Reset all variables required to start next shift
         * 
            ON TAXI_CONTROLLER:
            0) Delete pads[x]
            1) numPads = same for now, but that will either increase with shifts to max 9 OR be randomized between 4-9 per shift to shake things up.
            2) numPadsLandedOn = 0
            3) nextPad = 0
            4) PopulatePads(numPads) - 
            5) Beam(0,true) - Turn on first beam
            
            ON GAME_MANAGER:
            0) isGrounded = false;
            1) isAtPad = false;
            2) gas = maxGas - Fill er up
            3) hasGas = true
            4) hasControl = true
            5) damage = 0
            6) taxiMovedToInitialLocation = false - This is that LateUpdate function that moves the taxi at the start of the level to pad 0, but not afterwards.
           
         */

        // Destroy existing pads from previousl shift
        for (int i = 0; i < numPads; i++)
        {
            Destroy(pads[i], 0.0f); // Destroy each pad
        }

        // Num Pads

        // First, record the previous shift's number of pads, for the Summary
        numPadsThisShift = numPads;

        // - Perhaps? //numPads = (int)(UnityEngine.Random.value * 5.0f) + 4; // FOR NOW make pads random between 4 and 9
        if (numPads < 9)
        {
            numPads++;
        }

        // If we are past level 8, set a random number for the pads you have to land on for each subsequent shift.
        if (numPads > 8)
        {
            numPads = (int)(UnityEngine.Random.value * 6.0f) + 3;
        }

        // TEST ONLY - FOR QUICK LEVELING! A debug only feature
        if (debugOn)
        {
            numPads = numDebugPads;
            Debug.Log("<color=red>DEBUG IS ON! THERE ARE ONLY TWO PADS THIS SHIFT!!!</color>");
        }

        

        // Decrease landing pad size per shift, to a minimum of 6.0
        landingPadScale = landingPadScale - .1f;
        if (landingPadScale < 6.0f)
        {
            landingPadScale = 6.0f;
        }

        // Reset Next Pad to 0
        nextPad = 0;

        // Populate new pads
        PopulatePads(numPads);

        // Turn on first beam
        Beam(0, true);

        // TAXI:

        // isGrounded = false
        //taxi.isGrounded = false; - So far this is not a public variable, and may not need to be. isGrounded only happens if the taxi hits ground during game

        // isAtPad = false
        //taxi.isAtPad = false; - Also may not be needed, as this may happen on Update

        // Control
        taxi.hasControl = true; // May not be needed??
        taxi.isCrashing = false;

        // Calculate costs at home. Use those costs just below to subtract from cash
        gasCostHome += (taxi.maxGas - taxi.gas) * homePadGasCost; // Accumulated every shift
        repairsCostHome += taxi.damage * homePadDamageRepairCost + 0.0f;

        // First, calculate fill-up at home pad
        cash -= gasCostHome;
        Debug.Log("<color=purple>Max Gas = " + taxi.maxGas + "; Gas = " + taxi.gas + "</color>");
        Debug.Log("<color=purple>Gas Filled: " + (taxi.maxGas - taxi.gas) + "; at a cost of " + homePadGasCost + " per; Total cost: " + (taxi.gas * homePadGasCost) + "</color>");
        // Fill 'er up
        taxi.gas = taxi.maxGas;


        // First, calculate damage repair at home pad
        cash -= repairsCostHome;
        Debug.Log("<color=purple>Max Damage = " + taxi.maxDamage + "; Damage = " + taxi.damage + "</color>");
        Debug.Log("<color=purple>Damage repaired: " + taxi.damage + "; at a cost of " + homePadDamageRepairCost + " per; Total cost: " + (taxi.damage * homePadDamageRepairCost) + "</color>");
        // Repari all damage
        taxi.damage = 0.0f;

        // Add up costs for this shift.
        // This should be accumulated in Advance() as it adds up after every fare - faresThisShift = 0.0f;
        // This should be accumulated in Advance() as it adds up after every fare - tipsThisShift = 0.0f;
        // This should calculated during gasup - gasCostHome = 0.0f;
        // This should be calculated during repairs - repairsCostHome = 0.0f;


        CalculateShiftCosts();

        // Set taxi up to move to initiallocation
        taxi.taxiMovedToInitialLocation = false;
        taxi.cameraFollow = true;
        NewFare(standardFare, 0); // If this is the start of a new shift, set to 0, so code knows to use the Gas Station as starting point, and not the next pad as 0

        // Now determine if there are available upgrades, and make them show up too.
        // For now we just assume there are, and make the panel show up
        upgradesAvailable = true; // LET's let the UP Panel decide, since it searches for upgrades

        PutUiUp(shift, crashed);
    }



    void PopulatePads(int maxPads)
    {
        Array.Resize(ref pads, maxPads);

        // Further down I check to see if the next pad is too close to the previous pad. However, this is an unchecked variable that may exceed that distance 
        // and cause an infinite loop. So here I check that max (city radius) and if it's greater than that, I halve it. Not perfect, but should work.
        if (minAllowableDistanceBetweenPads > (maxPadDistance / 2.0f))
        {
            minAllowableDistanceBetweenPads = maxPadDistance / 4.0f;
        }

        for (int i = 0; i < maxPads; i++)
        {
            // We need this to keep track of the last random building so we don't choose the same one twice, unlikely as that would be. (It is not impossible)
            int lastRand = 0;

            // Array those pads. First, instantiate it
            pads[i] = GameObject.Instantiate(padObject); // Make a padd

            // Choose a random building.
            //Debug.Log("<color=blue>********</color> We have " + numBuildingsInGrid + " buildings in the array.");
            int randBuild = (int)(UnityEngine.Random.value * numBuildingsInGrid); // Grab a random building in the building array

            // Tom Thompson taught me a bitchin' way to never get the same random twice. Find it and put it here.
            while (randBuild == lastRand)
            {
                Debug.Log("<color=red> ********************** THE NEXT TO IMPOSSIBLE HAS HAPPENED! You chose the same one twice!");
                randBuild++;
                // There is a tiny chance this may be larger than the array, so:
                if (randBuild > buildings.Length-1)
                {
                    randBuild = randBuild - 2; // So if randBuild is equal to the size of the buildings array minus one, you are at the final element. Go back TWO
                }
            }
            // Now keep track of that randBuild in lastRand so we can check
            lastRand = randBuild;



            // Put pad at loc of same array building
            Vector3 padLoc = new Vector3(buildings[randBuild].transform.position.x,
                buildings[randBuild].transform.localScale.y + 2.0f,
                buildings[randBuild].transform.position.z);



            // Random Scale based on the building it is attached to.
            Vector3 padScale = new Vector3(buildings[randBuild].transform.localScale.x, 2.0f, buildings[randBuild].transform.localScale.z);

            // Set the padScale to landingPadScale for x and z, but the height of the building for y.
            padScale = new Vector3(landingPadScale, buildings[randBuild].transform.localScale.y, landingPadScale);

            pads[i].transform.localScale = new Vector3 ( landingPadScale, 2.0f, landingPadScale);

            // Then we scale the building to fit the pad, not vice versa like before.
            buildings[randBuild].transform.localScale = padScale; // THIS SETS THE BUILDING THE PAD IS ON TO THE SCALE THE PAD IS SET TO FOR THIS SHIFT

            // Apply properties
            pads[i].transform.position = padLoc;
            //pads[i].transform.localScale = padScale; // This used to set the pad scale based on the building. But now we do the opposite. 
            // We scale the building to the set pad scale.

            // Put the beam exactly where the pad is, but make it scale .98 xz and what it already is y.
            pads[i].SetActive(true);

            Pad_Manager pm = (Pad_Manager)pads[i].GetComponent(typeof(Pad_Manager));
            pm.padNumber = i;
            pm.lightNumber = i;
            Debug.Log("<color=black>         LIGHT COLOR = " + pm.lightNumber + "</color>");
            // Turn off the beam on this pad
            Beam(i, false); ;


        }

        

    }

    // I HAVE TO DO A CONFLICT CHECK! I must assure no pad of any kind is on another pad of any kind
    void PopulateGasStations(int maxStations)
    {
        Vector3 stationLoc = new Vector3(0.0f, 0.0f, 0.0f);
        
        Array.Resize(ref stations, maxStations);

        for (int i = 0; i < maxStations; i++)
        {
            // We need this to keep track of the last random building so we don't choose the same one twice, unlikely as that would be. (It is not impossible)
            int lastRand = -1;

            // Array those pads. First, instantiate it
            stations[i] = GameObject.Instantiate(stationObject); // Make a station

            // Choose a random building.
            //Debug.Log("<color=blue>********</color> We have " + numBuildingsInGrid + " buildings in the array.");
            int randBuild = (int)(UnityEngine.Random.value * numBuildingsInGrid); // Grab a random building in the building array

            // Tom Thompson taught me a bitchin' way to never get the same random twice. Find it and put it here.
            if (randBuild == lastRand)
            {
                Debug.Log("<color=red> ********************** THE NEXT TO IMPOSSIBLE HAS HAPPENED! You chose the same one twice!");
                randBuild++;
                // There is a tiny chance this may be larger than the array, so:
                if (randBuild > buildings.Length - 1)
                {
                    randBuild = randBuild - 2; // So if randBuild is equal to the size of the buildings array minus one, you are at the final element. Go back TWO
                }
            }
            // Now keep track of that randBuild in lastRand so we can check


            ///////////////////////////////////
            /// Here, put a test to ensure the building isn't one with a pad on it.



            // Passed all the tests, and now store the last rand so we don't repeat ourselves.



            // But first, if this is the first pass through the loop, this is station 0, and that should always be in the center of the building array, or home
            // This may go away later if I establish a home base you have to leave from and return to at end of shift
            // ONCE I GET HOME BASE WORKING, REMOVE THIS BIT!!!
            if ( i == 0)
            {
                randBuild = (int)(numBuildingsInGrid / 2.0f) - (int)(gridCellSize * 2.0f);
                //Debug.Log("<color=cyan>*******</color> OVERRIDING INDEX 0, so building is " + randBuild);
                //Debug.Log("<color=blue>*******</color> BUILDING COORDS ARE " + buildings[randBuild].transform.position.x + ", " + buildings[randBuild].transform.position.x);
            }


            // First, scale the pad to the Shift's scale value
            stations[i].transform.localScale = new Vector3(gasPadScale, gasPadScale, gasPadScale);

            // Put pad at loc of same array building -- Don't forget to add the y position now that the building is not only scaling but being NUDGED in the Y by texture displacement
            stationLoc = new Vector3(buildings[randBuild].transform.position.x,
                buildings[randBuild].transform.localScale.y + 1.0f,
                buildings[randBuild].transform.position.z);
            
            //Debug.Log("<color=cyan>************************* </color> Gas Station " + i + " is on Building number " + randBuild);

            // Random Scale based on the building it is attached to.
            float tmpScale = buildings[randBuild].transform.localScale.x / 2.0f;
            Vector3 stationScale = new Vector3(tmpScale, tmpScale, tmpScale); // THIS IS POINTLESS NOW. I AM NOW SCALING THE BUILDING TO THE GAS PAD SCALE FOR THIS SHIFT
            stationScale = new Vector3(gasPadScale * 2.0f, buildings[randBuild].transform.localScale.y, gasPadScale * 2.0f);
            //Debug.Log("<color=red> GAS PAD SCALE IS " + stationScale + "</color>");

            // NOW INSTEAD, SET THE BUILDING SCALE TO THE CURRENT SHIFT'S GAS PAD SCALE
            buildings[randBuild].transform.localScale = stationScale;


            // Apply properties
            stations[i].transform.position = stationLoc;
            //stations[i].transform.localScale = stationScale; // We no longer want to set the station scale to building scale, but to a variable we set up for each shift
            
            



            // Put the beam exactly where the pad is, but make it scale .98 xz and what it already is y.
            stations[i].SetActive(true);

            lastRand = randBuild;
        }
    }



    private void PopulateHomeBase()
    {
        // This code assumes, rightly, that the middle element in the buildings array is near the center
        // of the city. For reasons of texture tweaking it won't always be dead-center, but we don't care
        // about that. Close enough.
        // Get the index number half-way through the array
        int centerBuilding = numBuildingsInGrid / 2;

        // Instantiate a Home Base based on the one in the scene. Yes, this already exists, but instantiate it anyway
        // so it works just like pads and gas stations
        homeBldg = GameObject.Instantiate( homeObject );

        // For convenience, home base should scale same as gas stations
        homeBldg.transform.localScale = new Vector3(gasPadScale, gasPadScale, gasPadScale);

        // Put pad at loc of same array building -- Don't forget to add the y position now that the building is not only scaling but being NUDGED in the Y by texture displacement
        Vector3 homeLoc = new Vector3(buildings[centerBuilding].transform.position.x,
            buildings[centerBuilding].transform.localScale.y + 1.0f,
            buildings[centerBuilding].transform.position.z);

        // Place the Home Base object where the center-most building is.
        homeBldg.transform.position = homeLoc;

        Vector3 homeScale = new Vector3(gasPadScale * 2.0f, buildings[centerBuilding].transform.localScale.y, gasPadScale * 2.0f);

        Debug.Log("<color=red> HOME PAD SCALE IS " + homeScale + "</color>");
        // Now scale the building to fit the home pad, same as we do with the other pads
        buildings[centerBuilding].transform.localScale = homeScale;

        homeBldg.SetActive(true);

    }



    private Vector3 BuildingRandomLoc()
    {
        return new Vector3(UnityEngine.Random.value * maxPadDistance - (maxPadDistance / 2.0f),
              0.0f,
            UnityEngine.Random.value * maxPadDistance - (maxPadDistance / 2.0f));
    }

    private Vector3 BuildingRandomScale()
    {
        float padScaleXZ = (UnityEngine.Random.value * maxBuildingXZScale) + maxBuildingXZScaleOffset;
        float padScaleY = (UnityEngine.Random.value * maxBuildingYScale) + maxBuildingYScaleOffset;
        Vector3 padScale = new Vector3(padScaleXZ, padScaleY, padScaleXZ);

        return padScale;
    }


    private void PopulateAirships(int numShips)
    {
        
        // Make the array the right size
        Array.Resize(ref airShip, numShips+1);
        Debug.Log("ARRAY SIZE IS " + airShip.Length);

        // Start laneY at the airshipObject's yOffset, then let the loop increment
        // No. Now, start laneY with the tallest building height, which I now check for,
        // and add one height.
        float laneY = tallestBuilding + airShipObject.airshipHeight;

        for (int i = 0; i< numShips; i++)
        {
            // Start with the airship's yOffset. Place the first ship there..
            // Here I use the yOffset from the MAIN prefab, rather than the instantiated ones
            // Because, well... I haven't instantiated any yet

            
            
            // Make this ship. Instantiating it will run its Start, which will place and rotate it
            // After that, its animation event will restart it at a new rotation and position each time
            airShip[i] = Instantiate(airShipObject);

            // The x offset is the maxPadDistance (city radius)
            float myX = (UnityEngine.Random.value * airshipRadius) - (airshipRadius / 2.0f);
            // Start it at its height, but where it was placed by the airship's start
            airShip[i].transform.position = new Vector3(
                myX, 
                laneY,
                airShip[i].transform.position.z);

            // Next, add the airshipHeight each time, so the next ship is one lane higher.
            laneY += airShip[i].airshipHeight;
        }
    }



    private float HypotenuseDistance(Vector3 location)
    {
        float distance = Mathf.Sqrt((location.x * location.x) + (location.z * location.z));
        //Debug.Log("<color=blue>X = " + location.x + ", Z = " + location.z + ", and Distance = " + distance + "</color>");

        return distance;
    }


    private void AddFare(float fareLeft, float tipsLeft)
    {
        cash = cash + fareLeft + tipsLeft;
        faresThisShift += fareLeft;
        tipsThisShift += tipsLeft;
    }

    private void NewFare(float newFare, int nextPad)
    {
        float fareDistance = 0.0f;

        if ( nextPad == 0 )
        {
            fareDistance = Vector3.Distance(pads[nextPad].transform.position, homeBldg.transform.position);
        }
        else
        {
            // Make fare a standardFare with some multiplier for distance to target from car's current position
            fareDistance = Vector3.Distance(pads[nextPad].transform.position, taxi.transform.position);
        }

        fare = newFare + fareDistance * fareDistanceMultiplier;
        tip = standardTip;
        // Then multiply it by newFare, which is passed here
          
    }

    private void CalculateShiftCosts()
    {
        summaryText.text = "Shift Summary:\nFares:\nTips:\nGas Cost:\nRepairs Cost:\nHome Gas Cost:\nHome Repairs Cost:\n<color=white>CASH:</color>";
        summaryWinsNumbersText.text = (shift - 1).ToString() + "\n" +
            numPadsThisShift.ToString() + "\n" +
            "\u00A7 " + faresThisShift.ToString("F2") + "\n" +
            "\u00A7 " + tipsThisShift.ToString("F2");
        summaryLossesNumbersText.text = "<color=red>\u00A7 " + gasCostThisShift.ToString("F2") + "</color>\n" +
            "<color=red>\u00A7 " + repairsCostThisShift.ToString("F2") + "</color>\n" +
            "<color=red>\u00A7 " + gasCostHome.ToString("F2") + "</color>\n" +
            "<color=red>\u00A7 " + repairsCostHome.ToString("F2") + "</color>\n";
        summaryCashText.text = "\u00A7 " + cash.ToString("F2");

        // Now that we've set up the text, let's reset those shift values
        faresThisShift = 0.0f;
        tipsThisShift = 0.0f;
        gasCostThisShift = 0.0f;
        repairsCostThisShift = 0.0f;
        gasCostHome = 0.0f;
        repairsCostHome = 0.0f;


    }

    private void RefreshScore()
    {
        string tmpCashText = "Cash: \u00A7 " + cash.ToString("F2");
        cashText.text = tmpCashText;
        string tmpFareText = "Fare: \u00A7 " + fare.ToString("F2");
        fareText.text = tmpFareText;
        string tmpTipText = "Tip: \u00A7 " + tip.ToString("F2");
        tipText.text = tmpTipText;
    }

    

    public void PutUiUp(int shift, bool crashed)
    {
        Debug.Log("<color=yellow>***********</color> <color=white> UI UP!</color>");
        taxi.hasControl = false;
        taxi.isCrashing = false;
        uiIsUp = true;
        taxi.cameraFollow = false; // My aim here is to make the camera snap to taxi if a UI panel is up, rather than waiting for a follow which often does not happen

        // All of this right here? What say we automate it? No need for cases
        // For the first couple, yes, but then after that, we should have a randomizer
        if (!crashed)
        {
            switch (shift)
            {
                case 1:

                    // Turn off the shift summary text, as we have no summary yet. This is the start
                    summaryTextParent.SetActive(false);
                    // Call PugDialog, which is on UI_Panel_Controller
                    panelController.PutDialog(0);
                    break;
                /*
                case 2:
                    // Turn on summary text hereafter
                    summaryTextParent.SetActive(true);
                    panelController.RemoveDialog(panelController.myDialog);
                    panelController.PutDialog(1);
                    break;
                case 3:
                    // Turn on summary text hereafter
                    summaryTextParent.SetActive(true);
                    panelController.RemoveDialog(panelController.myDialog);
                    panelController.PutDialog(2);
                    break;
                case 4:
                    // Turn on summary text hereafter
                    summaryTextParent.SetActive(true);
                    panelController.RemoveDialog(panelController.myDialog);
                    panelController.PutDialog(3);
                    break;

                // EVENTUALLY DEFAULT will bring up a generic panel prefab
                // that has a script ON it that randomizes aphorisms. For now, however, just
                // Keep bringing up #4 because that's the last one we've made.
                */
                default:
                    summaryTextParent.SetActive(true);
                    panelController.RemoveDialog(panelController.myDialog);
                    if (shift < shiftDialogs.Length)
                    {
                        panelController.PutDialog(shift - 1);
                    }
                    else
                    {
                        // Later create a prefab that has an Aphoristic Randomizer
                        // and call that here
                        panelController.PutDialog(shiftDialogs.Length - 1);
                    }

                    break;

            }



        }
        else
        {
            Debug.Log("PUT UP A SPECIAL CRASH DIALOG INSTEAD!");
            // Turn off the shift summary text, as we have no summary yet. This is the start
            summaryTextParent.SetActive(false);
            panelController.RemoveDialog(panelController.myDialog);
            // Call PugDialog, which is on UI_Panel_Controller
            panelController.PutDialog(-1);
        }


        panel.SetActive(true);
    }

    public void PullUiDown()
    {
        Debug.Log("<color=yellow>***********</color> <color=black> UI DOWN!</color>");
        taxi.hasControl = true;
        taxi.isCrashing = false;
        uiIsUp = false;
        panel.SetActive(false);
        taxi.cameraFollow = true; // I hope this works. It should make the camera smooth-follow after UI comes down
    }


    public void RestartShift()
    {
        // Since Taxi might be crashing here, let's restore the drag values before recovering
        taxi.rb.angularDrag = taxi.defaultAngularDrag;
        taxi.rb.drag = taxi.defaultDrag;

        cash -= 500.0f;
        Debug.Log("<color=yellow> ********************* RESTART SHIFT ****************</color>");
        // GoToNextShift but pass crashed == true, so it can put up a special crash dialog and take deductible
        GoToNextShift(true);

        
        // To restart game instead, use this: (But don't)
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }

}
