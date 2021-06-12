using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using TMPro;

public class MeasurementTool : MonoBehaviour
{
    //I declare the Gaze Cursor (the pointer controlled by the head)
    public IMixedRealityCursor GazeCursor; 

    [Tooltip("Attach here the Measure prefab")]
    public GameObject measure;
    [Tooltip("Attach here the Measure Container object")]
    public GameObject measureContainer;
    [Tooltip("Attach here the material to be assigned to the line")]
    public Material lineMaterial;

    //I save the usefulVariables in a variable
    private UsefulVariables usefulVariables;

    private void Start()
    {
        //I initialise the completeMeasure at true
        usefulVariables = FindObjectOfType<UsefulVariables>();
        usefulVariables.completeMeasure = true;

        //I initialise the Gaze Cursor (the pointer controlled by the head)
        GazeCursor = CoreServices.InputSystem.GazeProvider.GazeCursor;
    }

    public void CreateNew()
    {
        //Firstly I reset the measure if needed

        //Declare the UsefulVariables object
        usefulVariables = FindObjectOfType<UsefulVariables>();

        //Enter only if you have already a measure
        if (usefulVariables.completeMeasure == false)
        {
            //I delete the last created measure
            Destroy(GameObject.Find("Measure " + usefulVariables.measureNumber));

            //I decrease of 1 unit the usefulVariables.measureNumber
            usefulVariables.measureNumber -= 1;

            if (gameObject.GetComponent<PointerHandler>() != null)
            {
                //I remove all the listeners
                gameObject.GetComponent<PointerHandler>().OnPointerClicked.RemoveAllListeners();

                Destroy(gameObject.GetComponent<PointerHandler>());
            }
        }

        //Variables Reset
        //I set at zero usefulVariables.clickMeasure again
        usefulVariables.clickMeasure = 0;

        //The complete measure is set at false because the click of the AirTap allow to make it true creating the measure
        usefulVariables.completeMeasure = false;


        //Secondly I add the listener for the click function

        //I declare the pointerHandler variable
        PointerHandler pointerHandler = gameObject.AddComponent<PointerHandler>();

        //Everytime I click, the event PlaceMeasureElements will be called
        pointerHandler.OnPointerClicked.AddListener((evt) => PlaceDot());

        // Make this a global input handler, otherwise this object will only receive events when it has input focus
        CoreServices.InputSystem.RegisterHandler<IMixedRealityPointerHandler>(pointerHandler);

        
        //Thirdly I create the measure in the MeasureContainer

        //I increase of 1 the counter of the measure
        usefulVariables.measureNumber += 1;

        //I instantiate the Measure 
        Instantiate(measure, measureContainer.transform);

        //I rename the Instantiated Measure based on the number in UsefulVariables
        GameObject.Find("Measure(Clone)").name = "Measure " + usefulVariables.measureNumber;

        //I resize at 0 the first ball, the second ball and the canvas so that they are not visible at first
        GameObject.Find("Measure " + usefulVariables.measureNumber).transform.GetChild(0).localScale = Vector3.zero; //First ball
        GameObject.Find("Measure " + usefulVariables.measureNumber).transform.GetChild(1).localScale = Vector3.zero; //Second ball
        GameObject.Find("Measure " + usefulVariables.measureNumber).transform.GetChild(2).localScale = Vector3.zero; //Canvas
    }

    public void PlaceDot()
    {
        // What is hitting the Ray in that precise moment
        RaycastHit hit;

        //I do this so that everytime I point the Ray toward the UI, it doesn't trigger the measure
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 20.0f))
        {
            if (hit.collider != null)
            {
                GameObject touchedObject = hit.transform.gameObject;

                //The layer correspondent to the UI is the number 5
                if (touchedObject.layer != 5)
                {
                    //Declare the UsefulVariables object
                    usefulVariables = FindObjectOfType<UsefulVariables>();

                    //I increase of 1 the counter of the measure click because I'm placing the first ball at this point
                    usefulVariables.clickMeasure += 1;

                    if (usefulVariables.clickMeasure == 1)
                    {
                        //I store in the variable the first position of the cursor
                        usefulVariables.point0 = GazeCursor.Position;

                        //I move the first ball for the measure
                        GameObject.Find("Measure " + usefulVariables.measureNumber).transform.GetChild(usefulVariables.clickMeasure - 1).position = usefulVariables.point0;

                        //I resize at 1 the first ball so that it is now visible
                        GameObject.Find("Measure " + usefulVariables.measureNumber).transform.GetChild(0).localScale = new Vector3(0.03f, 0.03f, 0.03f); //First ball
                    }

                    else if (usefulVariables.clickMeasure == 2)
                    {
                        //I store in the variable the second position of the cursor
                        usefulVariables.point1 = GazeCursor.Position;

                        //I move the second ball for the measure
                        GameObject.Find("Measure " + usefulVariables.measureNumber).transform.GetChild(usefulVariables.clickMeasure - 1).position = usefulVariables.point1;

                        //I resize at 1 the second ball so that it is now visible
                        GameObject.Find("Measure " + usefulVariables.measureNumber).transform.GetChild(1).localScale = new Vector3(0.03f, 0.03f, 0.03f); //Second ball

                        //Line
                        //I create the line between the two points of the measure
                        LineRenderer lineRenderer = GameObject.Find("Measure " + usefulVariables.measureNumber).AddComponent<LineRenderer>();
                        lineRenderer.SetPosition(0, usefulVariables.point0);
                        lineRenderer.SetPosition(1, usefulVariables.point1);

                        //I change the material of the line
                        lineRenderer.material = lineMaterial;

                        //I change the width of the line
                        lineRenderer.widthMultiplier = 0.01f;

                        //I remove all the listeners
                        gameObject.GetComponent<PointerHandler>().OnPointerClicked.RemoveAllListeners();

                        Destroy(gameObject.GetComponent<PointerHandler>());

                        //The completeMeasure is true because the user clicked twice to create the measure
                        usefulVariables.completeMeasure = true;


                        //I save in the Text of the Canvas the coordinates

                        //Coordinates of the two points created for the measure
                        Vector3 point0 = usefulVariables.point0;
                        Vector3 point1 = usefulVariables.point1;

                        //Distances x, y, z and d between the two points (order of the array x:[0], y:[1], z:[2], d:[3]) 
                        Vector3 distance;
                        distance.x = point1.x - point0.x; //in meters
                        distance.y = point1.y - point0.y; //in meters
                        distance.z = point1.z - point0.z; //in meters

                        //I print the coordinates on the canvas
                        GameObject.Find("Measure " + usefulVariables.measureNumber).GetComponentInChildren<TMP_Text>().text =
                            "Measure " + usefulVariables.measureNumber + "\n" +
                            "x = " + (float)System.Math.Round((point1.x - point0.x) * 100, 0) + " cm\n" +
                            "y = " + (float)System.Math.Round((point1.y - point0.y) * 100, 0) + " cm\n" +
                            "z = " + (float)System.Math.Round((point1.z - point0.z) * 100, 0) + " cm\n" +
                            "d = " + (float)System.Math.Round((distance.magnitude) * 100, 0) + " cm\n";
                    }
                }
            }
        }
    }

    public void DeleteLast()
    {
        //Declare the UsefulVariables object
        usefulVariables = FindObjectOfType<UsefulVariables>();

        //I delete the last created measure
        if (usefulVariables.measureNumber > 0)
        {
            Destroy(GameObject.Find("Measure " + usefulVariables.measureNumber));

            //I decrease of 1 unit the usefulVariables.measureNumber
            usefulVariables.measureNumber -= 1;

            if(gameObject.GetComponent<PointerHandler>() != null)
            {
                //I remove all the listeners
                gameObject.GetComponent<PointerHandler>().OnPointerClicked.RemoveAllListeners();

                Destroy(gameObject.GetComponent<PointerHandler>());

                usefulVariables.completeMeasure = false;
            }
        }
    }
    
    void DeleteAll()
    {
        //Declare the UsefulVariables object
        usefulVariables = FindObjectOfType<UsefulVariables>();

        for (int i = 0; i < measureContainer.transform.childCount; i++)
        {
            //I delete all the created measure
            Destroy(measureContainer.transform.GetChild(i).gameObject);
        }

        //I decrease at 0 the usefulVariables.measureNumber
        usefulVariables.measureNumber = 0;

        if (gameObject.GetComponent<PointerHandler>() != null)
        {
            //I remove all the listeners
            gameObject.GetComponent<PointerHandler>().OnPointerClicked.RemoveAllListeners();

            Destroy(gameObject.GetComponent<PointerHandler>());

            usefulVariables.completeMeasure = false;
        }
    }

    private void Update()
    {
        // What is hitting the Ray in that precise moment
        RaycastHit hit;

        //Declare the UsefulVariables object
        usefulVariables = FindObjectOfType<UsefulVariables>();

        //I do this so that everytime I point the Ray toward the UI, it doesn't trigger the measure
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 20.0f))
        {
            if (hit.collider != null)
            {
                GameObject touchedObject = hit.transform.gameObject;

                if (touchedObject.tag == "MeasurementTool" && usefulVariables.completeMeasure == true && touchedObject.transform.parent.GetChild(2).gameObject != null)
                {
                    GameObject canvas = touchedObject.transform.parent.GetChild(2).gameObject;

                    //Make the canvas appear when I touch the measure object 
                    canvas.transform.position = touchedObject.transform.position + ((Camera.main.transform.position - touchedObject.transform.position) / 10f);

                    //Make the canvas watching the camera
                    canvas.transform.LookAt(Camera.main.transform);

                    //I start the coroutine
                    StartCoroutine(MinimiseCanvas());

                    IEnumerator MinimiseCanvas()
                    {
                        //Scale the canvas at 0.005f when I touch the measure
                        canvas.transform.localScale = new Vector3(0.0005f, 0.0005f, 0.0005f);

                        yield return new WaitForSeconds(5);

                        //I wait f seconds and then I minimise again the canvas
                        if (canvas != null)
                        {
                            canvas.transform.localScale = Vector3.zero;
                        }

                    }
                }
            }
        }
    }
}