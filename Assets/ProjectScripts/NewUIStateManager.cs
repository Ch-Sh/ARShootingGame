using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Vuforia;
using UnityEngine.UI;
using CompleteProject;

public class NewUIStateManager : MonoBehaviour
{

    public enum UIStates
    {
        OVERLAY_OUTLINE, INIT_ANIMATION, SCANNING, GAME_RENDERING, GAME_PLAY, NONE
    }

    public DualTouchPlayerController playerController;
    public GameObject leftcontroller;
    public GameObject rightcontroller;
    public RawImage InstructionsImage;
    public RawImage ScanningOutline;
    public Button doneButton;
	public Button resetButton;
    public Canvas HUDImage;
//	public GameObject player;

	private bool isPlaying;
    private UIStates mState;
    private SurfaceBehaviour mSmartSurface;
    private ReconstructionBehaviour mReconstructionBehaviour;
    private SmartTerrainEventHandler mSTEventHandler; //smartTerrain EventHandler
    private SmartTerrainTrackableEventHandler mSTTrackableHandler;  //Primary Surface Event Handler 
    private DefaultTrackableEventHandler imageTrackableHandler;   //base image event handler
    private CompleteProject.EnemyManager enemyManager;

    // Use this for initialization
    void Start()
    {
		isPlaying = false;
        // Assign all references
        mReconstructionBehaviour = FindObjectOfType<ReconstructionBehaviour>();
        imageTrackableHandler = FindObjectOfType<DefaultTrackableEventHandler>();
        mSTEventHandler = GameObject.FindObjectOfType<SmartTerrainEventHandler>();
        mSmartSurface = GameObject.FindObjectOfType<SurfaceBehaviour>();
        mSTTrackableHandler = GameObject.FindObjectOfType<SmartTerrainTrackableEventHandler>();
        enemyManager = FindObjectOfType<CompleteProject.EnemyManager>();

        // Register to events
        imageTrackableHandler.ImageTrackableFoundFirstTime += OnImageTrackableFoundFirstTime;
    }

    // Update is called once per frame
    void Update()
    {
        bool showOutline = false;
        bool showDoneButton = false;
		bool showRestButton = false;

        switch (mState)
        {
            //Detection phase
            case UIStates.OVERLAY_OUTLINE:
				mSmartSurface.GetComponent<Renderer>().enabled = true;
                showOutline = true;
                break;

            // image rahme scanning blinking 
            case UIStates.INIT_ANIMATION:

                mSmartSurface.GetComponent<Renderer>().enabled = mSTTrackableHandler.m_trackablesFound;
                mState = UIStates.SCANNING;

                break;

            // Scanning phase
			case UIStates.SCANNING:
				mSmartSurface.GetComponent<Renderer> ().enabled = mSTTrackableHandler.m_trackablesFound;
				InstructionsImage.enabled = true;
				showDoneButton = true;
				showRestButton = true;

                //add scanning shader to primary plane 

                break;

            // Icebergs rendering phase - user taps on [DONE] button 
			case UIStates.GAME_RENDERING:

				if ((mReconstructionBehaviour != null) && (mReconstructionBehaviour.Reconstruction != null)) {
					mSTEventHandler.ShowPropClones ();
					mSTEventHandler.SwitchSurfaceMaterial ();
					mSTEventHandler.BuildNavMesh ();
					mReconstructionBehaviour.Reconstruction.Stop ();
					mState = UIStates.GAME_PLAY;
				}

				InstructionsImage.enabled = false;
				rightcontroller.SetActive (true);
				leftcontroller.SetActive (true);
				HUDImage.enabled = true;
				showDoneButton = false;
				showRestButton = false;

                break;

			case UIStates.GAME_PLAY:
				if (mSTEventHandler.propsCloned && playerController)
	 			{
					playerController.gameObject.SetActive (true);
					enemyManager.StartSpawnEnemy ();
						
				if (playerController && !isPlaying) {
					playerController.GetComponent<CompleteProject.PlayerHealth> ().Playing = true;
						mSTEventHandler.EnablePlayerFallDown ();
						isPlaying = true;
					}
				}				
                break;

            // Just a placeholder state, to make sure that the previous state runs for just one frame.
            case UIStates.NONE:
                break;
        }


        if (ScanningOutline != null && ScanningOutline.enabled != showOutline)
            ScanningOutline.enabled = showOutline;

        if (doneButton != null && doneButton.enabled != showDoneButton)
        {
            doneButton.enabled = showDoneButton;
            doneButton.image.enabled = showDoneButton;
            doneButton.gameObject.SetActive(showDoneButton);
        }

		if (resetButton != null && resetButton.enabled != showRestButton) {
			resetButton.enabled = showRestButton;
			resetButton.image.enabled = showRestButton;
			resetButton.gameObject.SetActive (showRestButton);
		}
    }

    public void TerrainDone()
    {
        mState = UIStates.GAME_RENDERING;
    }

    private void OnImageTrackableFoundFirstTime()
    {
        mState = UIStates.INIT_ANIMATION;
    }
}
