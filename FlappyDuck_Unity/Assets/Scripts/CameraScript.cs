using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraScript : MonoBehaviour
{
    public Camera _camera;
    public GameState _gameState;
    public Racetrack track;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_gameState._gameMode == GameMode.RecordingTrack)
        {
            RaycastHit hit;

            //todo: grab the center of the screen instead of the mouse position
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                track.PointList.Add(objectHit.position);
            }

            //todo: When the user taps the screen, we stop recording

            
        }

        if(_gameState._gameMode == GameMode.RecordingStopped)
        {
            track.Save();
            _gameState._gameMode = GameMode.MainMenu;
        }
    }
}
