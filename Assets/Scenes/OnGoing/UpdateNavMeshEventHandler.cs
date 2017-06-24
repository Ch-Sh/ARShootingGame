using System;
using System.Diagnostics;
using Pathfinding;
using UnityEngine;
using Vuforia;
using Debug = UnityEngine.Debug;

///TITLE: Update Nav Mesh Event Handler script
/// <summary>
///  A custom handler that implements the ITrackerEventHandler interface.
/// </summary>
public class UpdateNavMeshEventHandler : MonoBehaviour//, ISmartTerrainEventHandler
{
    #region PUBLIC_MEMBERS

    public PropBehaviour PropTemplate;
    // how often the nav mesh should be updated (in seconds)
    // set to 0 to immediately update when new mesh is available
    //public float NavMeshUpdateIntervalSec;

	public Material scanningMaterial;

    //Modified by SH 
    public SurfaceBehaviour SurfaceTemplate;

    public Iceberg IcePrefab;
    public bool propsCloned
    {
        get
        {
            return m_propsCloned;
        }
    }

    #endregion // PUBLIC_MEMBERS

    #region PRIVATE_MEMBERS

    //private NavMeshWireframeRenderer mNavMeshWireframeRenderer;
	private SurfaceAbstractBehaviour trackedSurfaceBehaviour;

//    private AstarPath mAstar;
//    private Mesh mNavMeshToUpdate;
//    private DateTime mLastNavMeshUpdate;
    //private long mLastMeshBuildingTime = -1;

//	private uint scanCounts;
    //Modified by SH
    private bool m_propsCloned;
    //equivalent to SmartterrrainBehaviour
    private ReconstructionBehaviour mReconstructionBehaviour;

    #endregion // PRIVATE_MEMBERS

    #region UNTIY_MONOBEHAVIOUR_METHODS

    void Start()
    {
        //SmartTerrainBehaviour behaviour = GetComponent<SmartTerrainBehaviour>();
        //if (behaviour)
        //{
        //    behaviour.RegisterSmartTerrainEventHandler(this);
        //}
        mReconstructionBehaviour = GetComponent<ReconstructionBehaviour>();
        if (mReconstructionBehaviour)
        {
            mReconstructionBehaviour.RegisterInitializedCallback(OnInitialized);
            mReconstructionBehaviour.RegisterPropCreatedCallback(OnPropCreated);
            mReconstructionBehaviour.RegisterSurfaceCreatedCallback(OnSurfaceCreated);
            mReconstructionBehaviour.RegisterSurfaceUpdatedCallback(OnSurfaceUpdated);
        }


        //mNavMeshWireframeRenderer = GetComponentInChildren<NavMeshWireframeRenderer>();

//        mAstar = FindObjectOfType(typeof(AstarPath)) as AstarPath;
//        if (mAstar != null)
//            mAstar.gameObject.SetActive(false);
//
//        mLastNavMeshUpdate = DateTime.Now;
//
//		scanCounts = 0;
    }


    void OnDestroy()
    {
        if (mReconstructionBehaviour)
        {
            mReconstructionBehaviour.UnregisterInitializedCallback(OnInitialized);
            mReconstructionBehaviour.UnregisterPropCreatedCallback(OnPropCreated);
            mReconstructionBehaviour.UnregisterSurfaceCreatedCallback(OnSurfaceCreated);
            mReconstructionBehaviour.UnregisterSurfaceUpdatedCallback(OnSurfaceUpdated);
        }
    }
		

//    void Update()
//    {
//        if (mNavMeshToUpdate != null)
//        {
//            // check if it's time to update the nav mesh
//            if ((DateTime.Now - mLastNavMeshUpdate).Seconds > NavMeshUpdateIntervalSec)
//                UpdateNavMesh();
//        }
//    }

    #endregion // UNTIY_MONOBEHAVIOUR_METHODS



    #region PUBLIC_METHODS

    /// <summary>
        /// Implementation of the ITrackerEventHandler function called after
        /// Vuforia has been initialized completely
        /// </summary>
    public void OnInitialized(SmartTerrainInitializationInfo initializationInfo)
    {
        Debug.Log("Finished initializing");
    }


    /// <summary>
    /// from SmartTerrainEventHandler
    /// </summary>
    /// <param name="surface"></param>
    public void OnSurfaceCreated(Surface surface)
    {
        Debug.Log("---Created Surface ID" + surface.ID);

        //shows an example of how you could get a handle on the surface game objects to perform different game logic
        if (mReconstructionBehaviour)
        {
            mReconstructionBehaviour.AssociateSurface(SurfaceTemplate, surface);
            //SurfaceAbstractBehaviour behaviour;
//            if (mReconstructionBehaviour.TryGetSurfaceBehaviour(surface, out behaviour))
//            {
//                behaviour.gameObject.name = "Surface " + surface.ID;
//            }
			if (mReconstructionBehaviour.TryGetSurfaceBehaviour (surface, out trackedSurfaceBehaviour)) {
				trackedSurfaceBehaviour.name = "Surface " + surface.ID;
//				trackedSurfaceBehaviour.gameObject.GetComponent<MeshRenderer>().material = scanningMaterial;
			}
        }
    }

    public void OnSurfaceUpdated(Surface surface)
    {
		Debug.Log ("Updated Surface:" + surface.ID);
//		Debug.Log ("Associated Surface" + trackedSurfaceBehaviour.name);
        // remember the nav mesh 
//        mNavMeshToUpdate = surface.GetNavMesh();
        // only update immediately if the interval is set to 0
//        if (NavMeshUpdateIntervalSec <= 0f)
//            UpdateNavMesh();
    }

    public void OnPropCreated(Prop prop)
    {
        //Debug.Log("---Created Prop");
        //var manager = TrackerManager.Instance.GetStateManager().GetSmartTerrainManager();

        //manager.AssociateProp(PropTemplate, prop);

        Debug.Log("---Created Prop ID: " + prop.ID);

        //shows an example of how you could get a handle on the prop game objects to perform different game logic
//        if (mReconstructionBehaviour)
//        {
//            mReconstructionBehaviour.AssociateProp(PropTemplate, prop);
//            PropAbstractBehaviour behaviour;
//            if (mReconstructionBehaviour.TryGetPropBehaviour(prop, out behaviour))
//            {
//                behaviour.gameObject.name = "Prop " + prop.ID;
//            }
//        }
    }

    public void OnPropUpdated(Prop prop)
    {
        Debug.Log("---Updated Prop");
    }

    public void OnPropDeleted(Prop prop)
    {
        Debug.Log("---Deleted Prop");
    }

    #endregion // PUBLIC_METHODS // ISmartTerrainEventHandler_Implementations


    /// <summary>
    /// Iceberg Animation with Props called by UIManager
    /// </summary>
    public void ShowPropClones()
    {

        if (!m_propsCloned)
        {
            PropAbstractBehaviour[] props = GameObject.FindObjectsOfType(typeof(PropAbstractBehaviour)) as PropAbstractBehaviour[];

            foreach (PropAbstractBehaviour prop in props)
            {
                Transform BoundingBox = prop.transform.Find("BoundingBoxCollider");
                BoxCollider collider = BoundingBox.GetComponent<BoxCollider>();
                collider.isTrigger = false;

                prop.SetAutomaticUpdatesDisabled(true);
                Renderer propRenderer = prop.GetComponent<MeshRenderer>();
                if (propRenderer != null)
                {
                    Destroy(propRenderer);
                }

                Iceberg effect = Instantiate(IcePrefab) as Iceberg;
                effect.name = "Ice";
                effect.transform.parent = BoundingBox;
                effect.transform.localPosition = new Vector3(0f, 0.032f, 0f);
                effect.transform.localScale = new Vector3(100, 50, 100);
                effect.transform.localRotation = Quaternion.identity;

            }

            m_propsCloned = true;
        }
    }

    #region PRIVATE_METHODS

//    public void UpdateNavMesh()
//    {
//        if (mNavMeshToUpdate != null)
//        {
//
//
//            if (mAstar != null)
//            {
//                // turn astar on now
//                if (!mAstar.gameObject.activeSelf)
//                    mAstar.gameObject.SetActive(true);
//
//                foreach (NavGraph navGraph in mAstar.graphs)
//                {
//                    if (navGraph is NavMeshGraph)
//                    {
//                        NavMeshGraph navMeshGraph = navGraph as NavMeshGraph;
//                        // set the source mesh
//                        navMeshGraph.sourceMesh = mNavMeshToUpdate;
//
//                        Debug.Log("Nodes = " + navMeshGraph.CountNodes());
//                    }
//                }
////                 tell astar to rescan
//                mAstar.Scan();
//				scanCounts++;
//				Debug.Log ("Scan Counts:" + scanCounts);
////				mAstar.ScanAsync ();
//            }
//
//            //// update the wireframerenderer
//            //if (mNavMeshWireframeRenderer != null)
//            //    mNavMeshWireframeRenderer.SetMeshToRender(mNavMeshToUpdate);
//
//
//            mNavMeshToUpdate = null;
//            mLastNavMeshUpdate = DateTime.Now;
//        }
//    }

//    void ISmartTerrainEventHandler.OnInitialized(SmartTerrainInitializationInfo initializationInfo)
//    {
//        throw new NotImplementedException();
//    }
//
//    void ISmartTerrainEventHandler.OnPropCreated(Prop prop)
//    {
//        throw new NotImplementedException();
//    }
//
//    void ISmartTerrainEventHandler.OnPropUpdated(Prop prop)
//    {
//        throw new NotImplementedException();
//    }
//
//    void ISmartTerrainEventHandler.OnPropDeleted(Prop prop)
//    {
//        throw new NotImplementedException();
//    }
//
//    void ISmartTerrainEventHandler.OnSurfaceCreated(Surface surface)
//    {
//        throw new NotImplementedException();
//    }
//
//	void ISmartTerrainEventHandler.OnSurfaceUpdated(Surface surface)
//    {
//        throw new NotImplementedException();
//    }
//
    #endregion // PRIVATE_METHODS
}