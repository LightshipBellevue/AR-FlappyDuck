using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBob : MonoBehaviour
{
    [SerializeField]
    public GameObject arrow;

    [SerializeField]
    [Range (0,2)]
    public float maxMovement = 1;
    
    [SerializeField]
    [Range (0,4)]
    public float speed = 2;

    private float yStart; 
    // Start is called before the first frame update
    void Start()
    {
        yStart = arrow.transform.localPosition.y; 
    }

    // Update is called once per frame
    void Update()
    {
        arrow.transform.localPosition = new Vector3(
            arrow.transform.localPosition.x,
            maxMovement*Mathf.Sin(speed*Time.time) + yStart, 
            arrow.transform.localPosition.z); 
        
    }
}
