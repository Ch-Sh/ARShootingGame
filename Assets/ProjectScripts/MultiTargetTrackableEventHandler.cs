using UnityEngine;

namespace Vuforia{

	public class MultiTargetTrackableEventHandler : MonoBehaviour, 
													ITrackableEventHandler
	{

		public event System.Action MultiTargetFound;


		private TrackableBehaviour mTrackableBehaviour;
//		private bool m_TrackableDetectedForFirstTime;

		// Use this for initialization
		void Start () {
			mTrackableBehaviour = GetComponent<TrackableBehaviour>();
			if (mTrackableBehaviour)
				mTrackableBehaviour.RegisterTrackableEventHandler (this);
		}


		void OnDisable(){
			if (mTrackableBehaviour)
				mTrackableBehaviour.UnregisterTrackableEventHandler (this);
			mTrackableBehaviour.enabled = false;
		}

		#region PUBLIC_METHODS

		/// <summary>
		/// Implementation of the ITrackableEventHandler function called when the
		/// tracking state changes.
		/// </summary>
		public void OnTrackableStateChanged(
			TrackableBehaviour.Status previousStatus,
			TrackableBehaviour.Status newStatus)
		{
			if (newStatus == TrackableBehaviour.Status.DETECTED ||
				newStatus == TrackableBehaviour.Status.TRACKED ||
				newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
			{
				OnTrackingFound();
			}
			else
			{
				OnTrackingLost();
			}
		}

		#endregion // PUBLIC_METHODS



		#region PRIVATE_METHODS


		private void OnTrackingFound()
		{
			Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
			Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

			//added Animator
			Animator animator = GetComponentInChildren<Animator>();

			// Enable rendering:
			foreach (Renderer component in rendererComponents)
			{
				component.enabled = true;
			}

			// Enable colliders:
			foreach (Collider component in colliderComponents)
			{
				component.enabled = true;
			}

			if(animator != null)
			{
				animator.SetTrigger("ShowBlinking");
			}

//			if (!m_TrackableDetectedForFirstTime)
//			{
				if (this.MultiTargetFound != null)
				{
					this.MultiTargetFound();
					Debug.Log("Box Trackable Found at [" + Time.time + "]");
				}
//				m_TrackableDetectedForFirstTime = true;
//			}

			Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
		}


		private void OnTrackingLost()
		{
			Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
			Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

			// Disable rendering:
			foreach (Renderer component in rendererComponents)
			{
				component.enabled = false;
			}

			// Disable colliders:
			foreach (Collider component in colliderComponents)
			{
				component.enabled = false;
			}

//			transform.position = Vector3.zero;
//			transform.rotation = Quaternion.identity;

			Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
		}

		#endregion // PRIVATE_METHODS
	}

}


