/*============================================================================== 
 * Copyright (c) 2015 Qualcomm Connected Experiences, Inc. All Rights Reserved. 
 * ==============================================================================*/
using UnityEngine;
using System.Collections;

public class PenguinTapHandler : TapHandler
{
    #region PRIVATE_MEMBERS
    private Penguin mPenguin = null;
    #endregion //PRIVATE_MEMBERS


    #region PROTECTED_METHODS
    protected override void OnSingleTapConfirmed()
    {
        if (mPenguin == null)
            mPenguin = FindObjectOfType<Penguin>();
        
        mPenguin.OnSingleTapped();
    }

    protected override void OnDoubleTap()
    {
        // do nothing
    }
    #endregion //PROTECTED_METHODS
}
