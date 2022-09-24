using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraScript : MonoBehaviour
{
    public Camera camera;
    public GameState _gameState;
    public Racetrack track;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_gameState._gameMode == RecordingTrack)
        {
            RaycastHit hit;
            Ray ray = Camera.ScreenPointToRay(0.5, 0.5);
        }
    }
}
