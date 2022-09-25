using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Niantic.ARDK;
using Niantic.ARDK.AR;
using Niantic.ARDK.AR.ARSessionEventArgs;
using Niantic.ARDK.AR.HitTest;
using Niantic.ARDK.AR.WayspotAnchors;
using Niantic.ARDK.Extensions;
using Niantic.ARDK.LocationService;
using Niantic.ARDK.Utilities;
using Niantic.ARDK.Utilities.Input.Legacy;

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
    RaceCountdown,          //All players are ready, we're counting down to begin!
    Racing,                 //The race is in progress
    RaceFinished,           //The player has reached the finish line
    ScoreScreen             //presenting the score screen to the user
}

public class GameState : MonoBehaviour
{
    public GameMode _gameMode;

    public GameMode _desiredMode;       //The user will always start by finding a VPS anchor, but after the anchor has been located, we switch to this mode

    //The race count down timer in seconds
    public float CountDown = 3;

    //The amount of time the player has been racing
    public float RaceTime = 0;

    //The race track we're either creating or following
    public Racetrack _raceTrack;

    //the duck which moves along the race track
    public GameObject _duckActor;

    //This is the camera/phone being used by the user
    public GameObject _ARCamera;

    // Objects to access Niantic services
    private IARSession _arSession;
    public WayspotAnchorService WayspotAnchorService;
    private IWayspotAnchorsConfiguration _config;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
        ARSessionFactory.SessionInitialized += HandleSessionInitialized;
    }

    private void OnDestroy()
    {
      if (WayspotAnchorService != null)
      {
        // WayspotAnchorService.LocalizationStateUpdated -= LocalizationStateUpdated;
        WayspotAnchorService.Dispose();
      }
    }
//////////////////
// Start of section to integrate Niantic */
//////////////////

    // Listen for AR session creation...
    private void HandleSessionInitialized(AnyARSessionInitializedArgs args)
    { 
      // Debug.Log("Rajeev says: ARSession initialized");
      _arSession = args.Session;
      _arSession.Ran += HandleSessionRan;
    }

    // Listen for AR session running...
    private void HandleSessionRan(ARSessionRanArgs args)
    {
      // Debug.Log("Rajeev says: ARSession ran");
      _arSession.Ran -= HandleSessionRan;
      WayspotAnchorService = CreateWayspotAnchorService();
      // WayspotAnchorService.LocalizationStateUpdated += OnLocalizationStateUpdated;
    }

    private WayspotAnchorService CreateWayspotAnchorService()
    {
      var locationService = LocationServiceFactory.Create(_arSession.RuntimeEnvironment);
      locationService.Start();

      if (_config == null)
        _config = WayspotAnchorsConfigurationFactory.Create();

      var wayspotAnchorService =
        new WayspotAnchorService
        (
          _arSession,
          locationService,
          _config
        );

      // wayspotAnchorService.LocalizationStateUpdated += LocalizationStateUpdated;

      return wayspotAnchorService;
    }

//////////////////
// End of section integrating Niantic
//////////////////


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
            //todo: show UI error that no racetracks exist
        }
    }

    public void StartNewRecording()
    {
        _gameMode = GameMode.FindingVPSAnchor;
        _desiredMode = GameMode.RecordingTrack;
    }

    // Update is called once per frame
    void Update()
    {
        if(_gameMode == GameMode.FindingVPSAnchor)
        {
            //todo: test if we've localized to a VPS anchor
            if (WayspotAnchorService != null && WayspotAnchorService.LocalizationState == LocalizationState.Localized)
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

        if(_gameMode == GameMode.MovingToStart)
        {
            //todo: Figure out when the user is close enough to the race start point
            float PlayerDistance = 100;


            //todo: find the first transform in the race track and measure the camera distance to that start point.
            //when the distance is within 5 meters, we can switch over to the "race start" mode.
            Vector3 camPos = _ARCamera.transform.position;
            Vector3 startPos = _raceTrack.GetStartLocation();

            if(PlayerDistance <= 5.0)
            {
                _gameMode = GameMode.WaitingToRace;
            }
        }

        if(_gameMode == GameMode.WaitingToRace)
        {
            //for now, we have a single player so we just immediately move to the count down
            _gameMode = GameMode.RaceCountdown;
            CountDown = 3;
            
        }

        if(_gameMode == GameMode.RaceCountdown)
        {
            CountDown -= Time.deltaTime;

            //Todo: Display the current count down time on the UI

            if(CountDown <= 0)
            {
                _gameMode = GameMode.Racing;
            }
        }

        if(_gameMode == GameMode.Racing)
        {
            RaceTime += Time.deltaTime;
            float DistanceFromFinish = 100;
            //todo: find the distance from the current position on the track and look at the distance from the last position on the track.
            //when the distance from the finish line is nearly 0, then we "finish" the race.

            if(DistanceFromFinish <= 0)
            {
                _gameMode = GameMode.RaceFinished;
            }
        }

        if (_gameMode == GameMode.RaceFinished)
        {
            _desiredMode = GameMode.MainMenu;

            //todo: Display the score UI

            //todo: When the user taps the screen, we move to the main menu
            //If we detect a new touch, call our 'TouchBegan' function
            var touch = PlatformAgnosticInput.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _gameMode = _desiredMode;

                //hide the score screen UI
            }
        }
    }
}
