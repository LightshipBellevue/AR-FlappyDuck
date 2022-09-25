using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racetrack : MonoBehaviour
{
    //These are the race track points in 3D space
    public List<Vector3> PointList;
    public string Filepath = "MyTrack.json";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetStartLocation()
    {
        if(PointList.Count < 0)
            return PointList[0];
        return Vector3.zero;
    }

    public Vector3 GetFinishLocation()
    {
        if (PointList.Count < 0)
            return PointList[PointList.Count - 1];
        return Vector3.zero;
    }

    public bool Save()
    {
        //todo: save the recording to a json file
        return false;
    }

    public bool Load()
    {
        //todo: load the recording from a json file
        LoadFakeData();
        return true;

        //by default, we fail to load.
        //return false;
    }

    void LoadFakeData()
    {
        PointList.Clear();

        for(int a=0; a<500; a++)
        {
            PointList.Add(new Vector3(a, 0, 0));
        }
    }
}
