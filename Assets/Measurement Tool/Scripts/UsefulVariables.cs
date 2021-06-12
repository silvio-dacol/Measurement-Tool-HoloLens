using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsefulVariables : MonoBehaviour
{
    //Variable which counts the number of measures in that section
    public int measureNumber;

    //Variable counting when to exit from the actual measure (because I need to place two balls and the line)
    public int clickMeasure;

    //Vectors containing the position of the two points of the measure
    public Vector3 point0;
    public Vector3 point1;

    //I create this variable to not enter in the ResetMeasure if I have completed a Measure
    public bool completeMeasure = true;


    void Start()
    {
        //I don't want this gameObject to be destroyed when changing scenes because I need it for the length of the experiment
        DontDestroyOnLoad(gameObject);
    }
}
