using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 This keeps track of the race track spline, whether the user has reached the race starting point, whether the users duck actor has reached
the finish line, and any other state info.
We also keep track of the VPS anchor tracking state and location.
 */

public enum GameMode
{
    MainMenu,
    FindingVPSAnchor,       //the user is finding a VPS anchor location
    RecordingTrack,         //the user is recording track transforms every frame
    RecordingStopped,       //The user stopped the recording and we're saving the data to a json file
    LoadingTrack,           //the user has found a VPS anchor and we're loading a series of recorded transforms
    MovingToStart,          //the user has loaded the recorded transforms and moving to the race starting point
    WaitingToRace,          //the user has reached the starting point (and other users?) and we're ready to start the race countdown
    Racing,                 //The race is in progress
    RaceFinished,           //The player has reached the finish line
    ScoreScreen             //presenting the score screen to the user
}

public class GameState : MonoBehaviour
{
    public GameMode _gameMode;

    public GameMode _desiredMode;       //The user will always start by finding a VPS anchor, but after the anchor has been located, we switch to this mode

    //The race count down timer in seconds
    public float CountDown = 5;

    //The amount of time the player has been racing
    public float RaceTime = 0;

    //The race track we're either creating or following
    public Racetrack _raceTrack;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartNewGame()
    {
        _gameMode = GameMode.FindingVPSAnchor;
        _desiredMode = GameMode.LoadingTrack;

        //check to make sure that we actually loaded a track successfully
        if (_raceTrack.Load())
        {
            _gameMode = GameMode.MovingToStart;
        }
        else
        {
            //todo: show error that no racetracks exist
        }
    }

    public void StartNewRecording()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(_gameMode == GameMode.FindingVPSAnchor)
        {
            //todo: figure out when we've localized to a VPS anchor

            bool VPSFound = false;

            if (VPSFound)
            {
                //We are either starting a new game or starting a racetrack recording session.
                if(_desiredMode == GameMode.LoadingTrack)
                {
                    _desiredMode = GameMode.MovingToStart;
                }

                //after the VPS anchor has been found, we set the game mode to the desired mode
                _gameMode = _desiredMode;
            }
        }
    }
}
