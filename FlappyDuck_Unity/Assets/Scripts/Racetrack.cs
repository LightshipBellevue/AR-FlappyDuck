using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racetrack : MonoBehaviour
{
    //These are the race track points in 3D space
    public List<Vector3> PointList = new List<Vector3>();
    public string Filepath = "MyTrack.json";

    //the total distance of the race track
    float totalDist = 0;

    //the total amount the duck has travelled
    float duckTravelledDistance = 0;

    //how fast the duck is moving along the track
    public float duckSpeed = 2;

    // Start is called before the first frame update
    void Start()
    {
        //duckSpeed = 2;
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

    public void UpdateDuck()
    {
        duckTravelledDistance += duckSpeed * Time.deltaTime;

    }

    public float DistanceLeft()
    {
        return totalDist - duckTravelledDistance;
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
        totalDist = CalcTotalDistance();
        return true;

        //by default, we fail to load.
        //return false;
    }

    void LoadFakeData()
    {
        if (PointList == null)
            PointList = new List<Vector3>();

        PointList.Clear();

        for(int a=0; a<10; a++)
        {
            PointList.Add(new Vector3(a*3, 0, 0));
        }
    }

    //Finds the interpolated distance along the spline
    public Vector3 GetPositionAtDistance()
    {
        Vector3 ret = Vector3.zero;
        if (PointList == null) return ret;

        float curDist = 0;
        int index = 0;
        float lastDist = 0;

        while(curDist < duckTravelledDistance)
        {
            //early out since we exceeded the number of elements in the point list
            if (index + 1 >= PointList.Count) return ret;

            Vector3 cur = PointList[index];
            Vector3 next = PointList[index + 1];
            float dist = (cur - next).magnitude;


            //did we exceed our desired distance?
            if (dist + curDist > duckTravelledDistance)
            {
                //todo: then we do linear interpolation
                
                //get a normalized alpha value for distance travelled between points
                float alpha = (duckTravelledDistance - lastDist) / dist;
                return Vector3.Lerp(cur, next, alpha);
                
            }
            else
            {
                curDist += dist;
            }
            lastDist = curDist;
            index++;
        }

        return ret;
    }

    float CalcTotalDistance()
    {
        float total = 0;
        if (PointList != null)
        {
            //Calculate the total distance of the spline points (very ugly)
            for (int a = 0; a < PointList.Count; a++)
            {
                if (a + 1 < PointList.Count)
                {
                    total += (PointList[a] - PointList[a + 1]).magnitude;
                }
                else return total;
            }
        }

        return total;
    }
}
