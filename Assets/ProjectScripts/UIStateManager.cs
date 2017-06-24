/*==============================================================================
Copyright (c) 2015 PTC Inc. All Rights Reserved.

Copyright (c) 2013-2015 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
==============================================================================*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Vuforia;

public enum UIStates
{
    OVERLAY_OUTLINE, INIT_ANIMATION, SCANNING, GAME_RENDERING, GAME_PLAY, RESET_ALL, NONE
}

/// <summary>
/// Manages all UI States in the app
/// </summary>
public class UIStateManager : MonoBehaviour
{
    #region PUBLIC_MEMBERS
    public Penguin penguin;
    public RawImage cylinderOutline;
    public RawImage instructionsImage;
    public Button doneButton;
    public Button resetButton;
    #endregion //PUBLIC_MEMBERS


    #region PRIVATE_MEMBERS
    private UIStates mState;
    private SurfaceBehaviour mSmartSurface;
    private ReconstructionBehaviour mReconstructionBehaviour;
    private SmartTerrainEventHandler mSTEventHandler;
	private DefaultTrackableEventHandler imageTrackableHandler;
    //private CylinderTrackableEventHandler mCylinderTrackableHandler;
    private SmartTerrainTrackableEventHandler mSTTrackableHandler;
    private Texture2D mPointDeviceTexture;
    private Texture2D mPullBackTexture;
    private Texture2D mTapIceTexture;
    #endregion //PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS
    void Start()
    {
        // Assign all references
        mReconstructionBehaviour = FindObjectOfType<ReconstructionBehaviour>();
		imageTrackableHandler = FindObjectOfType<DefaultTrackableEventHandler> ();
        //mCylinderTrackableHandler = FindObjectOfType<CylinderTrackableEventHandler>();
        mSTEventHandler = GameObject.FindObjectOfType<SmartTerrainEventHandler>();
        mSmartSurface = GameObject.FindObjectOfType<SurfaceBehaviour>();
        mSTTrackableHandler = GameObject.FindObjectOfType<SmartTerrainTrackableEventHandler>();
        
        // Register to events
		imageTrackableHandler.ImageTrackableFoundFirstTime += OnCylinderTrackableFoundFirstTime;
        //mCylinderTrackableHandler.CylinderTrackableFoundFirstTime += OnCylinderTrackableFoundFirstTime;


        // Load header textures for user instructions
        mPointDeviceTexture = Resources.Load<Texture2D>("UserInterface/header_pointdevice");
        mPullBackTexture = Resources.Load<Texture2D>("UserInterface/header_pullbackslowly");
        mTapIceTexture = Resources.Load<Texture2D>("UserInterface/header_tapice");
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.JoystickButton0))
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        bool showOutline = false;
        bool showDoneButton = false;
        bool showResetButton = false;

        switch (mState)
        {
            //Detection phase
            case UIStates.OVERLAY_OUTLINE:
                instructionsImage.texture = mPointDeviceTexture;
                mSmartSurface.GetComponent<Renderer>().enabled = false;
                showOutline = true;
                break;

            // Soda can ice animating phase
            case UIStates.INIT_ANIMATION:
                mSmartSurface.GetComponent<Renderer>().enabled = false;
                Ice anim = GameObject.FindObjectOfType<Ice>();
                anim.Play();
                if (anim.DidFinishAnimation)
                {
                    mSmartSurface.GetComponent<Renderer>().enabled = mSTTrackableHandler.m_trackablesFound;
                    mState = UIStates.SCANNING;
                }
                break;

            // Scanning phase
            case UIStates.SCANNING:
                mSmartSurface.GetComponent<Renderer>().enabled = mSTTrackableHandler.m_trackablesFound;
                instructionsImage.texture = mPullBackTexture;
                showDoneButton = true;
                break;

            // Icebergs rendering phase - user taps on [DONE] button 
            case UIStates.GAME_RENDERING:
                if ((mReconstructionBehaviour != null) && (mReconstructionBehaviour.Reconstruction != null))
                {
                    mSTEventHandler.ShowPropClones();
                    mReconstructionBehaviour.Reconstruction.Stop();
                    mState = UIStates.GAME_PLAY;
                }
                break;

            //Penguin appears and user taps on surface to move the penguin around
            case UIStates.GAME_PLAY:
                if (mSTEventHandler.propsCloned && !penguin.DidAppear)
                {
                    penguin.gameObject.SetActive(true);
                }

                if (penguin.DidAppear)
                {
                    // Update UI messaging
                    instructionsImage.texture = mTapIceTexture;
                    showResetButton = true;
                }
                break;

            //User taps on [RESET] button - Re-loads the level
            case UIStates.RESET_ALL:
                // Go back to loading scene
                int loadingScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1;
                if (loadingScene < 0) loadingScene = 0;
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(loadingScene);

                mState = UIStates.NONE;
                break;

            // Just a placeholder state, to make sure that the previous state runs for just one frame.
            case UIStates.NONE: break;
        }

        if (cylinderOutline != null && 
            showOutline != cylinderOutline.enabled)
        {
            cylinderOutline.enabled = showOutline;
        }

        if (doneButton != null && 
            showDoneButton != doneButton.enabled)
        {
            doneButton.enabled = showDoneButton;
            doneButton.image.enabled = showDoneButton;
            doneButton.gameObject.SetActive(showDoneButton);
        }

        if (resetButton != null && 
            showResetButton != resetButton.enabled)
        {
            resetButton.enabled = showResetButton;
            resetButton.image.enabled = showResetButton;
            resetButton.gameObject.SetActive(showResetButton);
        }
    }
    #endregion //MONOBEHAVIOUR_METHODS


    #region PUBLIC_METHODS
    public void TerrainDone()
    {
        mState = UIStates.GAME_RENDERING;
    }

    public void ResetAll()
    {
        mState = UIStates.RESET_ALL;
    }
    #endregion //PUBLIC_METHODS


    #region PRIVATE_METHODS
    private void OnCylinderTrackableFoundFirstTime()
    {
        mState = UIStates.INIT_ANIMATION;
    }
    #endregion //PRIVATE_METHODS
}
