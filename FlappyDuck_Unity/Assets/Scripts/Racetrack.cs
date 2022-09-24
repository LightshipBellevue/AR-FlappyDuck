using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racetrack : MonoBehaviour
{
    //These are the race track points in 3D space
    public List<Transform> PointList;
    public string Filepath = "MyTrack.json";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Save()
    {
        //todo: save the recording to a json file
    }

    public bool Load()
    {
        //todo: load the recording from a json file

        //by default, we fail to load.
        return false;
    }
}
