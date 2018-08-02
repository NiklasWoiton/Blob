using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR;
using System.IO;
using Assets.LSL4Unity.Scripts;

public class VRHardwarePosesLogger : MonoBehaviour {

    public Vector3 headVector, leftControllerVector, rightControllerVector;
    Stopwatch watch;
    long startTimeInMilliseconds;
    List<string> headArray = new List<string>();
    List<string> leftArray = new List<string>();
    List<string> rightArray = new List<string>();
    List<string> timeArray = new List<string>();
    //string[] columns = new string[2];
    int counter = 0;

    //private LSLOutlet marker;

    // Use this for initialization
    void Start () {
        //marker = FindObjectOfType<LSLMarkerStream>();
        LSLTransformOutlet lslTransformOutlet = GetComponent<LSLTransformOutlet>();

        startTimeInMilliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        watch = Stopwatch.StartNew();
        headVector = InputTracking.GetLocalPosition(XRNode.Head);
        leftControllerVector = InputTracking.GetLocalPosition(XRNode.LeftHand);
        rightControllerVector = InputTracking.GetLocalPosition(XRNode.RightHand);

        print("Starting time:" + startTimeInMilliseconds);

    }
	
	// Update is called once per frame
	void Update () {
        //print("Position of head:" + headVector);
        //print("Position of left hand:" + leftControllerVector);
        //print("Position of right hand:" + rightControllerVector);
        // print("Elapsed time in ticks:" + watch.ElapsedTicks);

        string timeString = watch.ElapsedMilliseconds.ToString();
        string headString = headVector.ToString();
        string leftString = leftControllerVector.ToString();
        string rightString = rightControllerVector.ToString();

        headArray.Add(headString);
        timeArray.Add(timeString);
        leftArray.Add(leftString);
        rightArray.Add(rightString);
        counter += 1;

        if (counter == 10) //writes file after 100 data points are added to the list. for demo purposes
        {
            writeToList();
        }

        print(counter);


	}

    void writeToList()
    {
        string filePath = @"C:\test.csv";  //File Path goes here, adjust as needed
        string delimiter = ",";

        string[][] output = new string[][]{
                  headArray.ToArray(),
                  timeArray.ToArray()
               };
        int length = output.GetLength(0);
        StringBuilder sb = new StringBuilder();
        for (int index = 0; index < length; index++)
        {
            sb.AppendLine(string.Join(delimiter, output[index]));
        }
        //File.WriteAllText(filePath, sb.ToString());
        UnityEngine.Debug.Log(sb.ToString());
    }

 
}