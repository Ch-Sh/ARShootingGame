using UnityEngine;
using Vuforia;

namespace CompleteProject
{
    public class EnemyManager : MonoBehaviour
    {
        public PlayerHealth playerHealth;       // Reference to the player's heatlh.
        public GameObject[] enemies;                // The enemy prefab to be spawned.
        public float spawnTime = 3f;            // How long between each spawn.
                                            

		
		public CylinderTrackableEventHandler sodaOriginTrackableHandler;
		public CylinderTrackableEventHandler sodaDietTrackableHandler;
		public Transform[] spawnPoints; // An array of the spawn points this enemy can spawn from.


        private bool spawnCylinderReady;
		private bool spawnBoxReady;
        private bool spawnStarted;
		private GameObject newEnemy;

        void Start()
        {
            spawnStarted = false;
			spawnCylinderReady = false;
			spawnBoxReady = false;

			spawnPoints = new Transform[2];

//			spawnPositions = new Vector3[2];

//            cylinderTrackableHandler = FindObjectOfType<CylinderTrackableEventHandler>();
			if (sodaOriginTrackableHandler)
				sodaOriginTrackableHandler.CylinderTrackableFound += 
														OnSodaOriginTrackableFound;


//			multiTargetTrackableHandler = FindObjectOfType<MultiTargetTrackableEventHandler>();
			if (sodaDietTrackableHandler)
				sodaDietTrackableHandler.CylinderTrackableFound += 
														OnSodaDietTrackableFound;
        }


        void Spawn()
        {
            // If the player has no health left...
            if (playerHealth.currentHealth <= 0f)
            {
                // ... exit the function.
                return;
            }

            // Find a random index between zero and one less than the number of spawn points.
            int spawnPointIndex = Random.Range (0, spawnPoints.Length);

//			int spawnPositionIndex = Random.Range(0,spawnPositions.Length);
			int spawnEnemyIndex = Random.Range (0,enemies.Length);

            // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
            //Instantiate (enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
			Vector3 spawnPos = new Vector3();
			spawnPos = spawnPoints [spawnPointIndex].position;
			spawnPos.y = 0f;
			newEnemy = Instantiate(enemies[spawnEnemyIndex], 
						spawnPos, 
						Quaternion.identity);

//			newEnemy = Instantiate (enemies[spawnEnemyIndex],
//									spawnPositions[spawnPositionIndex],
//									Quaternion.identity);
			
			newEnemy.GetComponent<AIPath> ().target = playerHealth.transform;
//			Debug.Log ("enemy position:" + newEnemy.transform.position.ToString());
			Debug.Log ("enemy TF position:" + spawnPoints[spawnPointIndex].position);
			Debug.Log ("player position:" + playerHealth.transform.position.ToString());

        }

		public void StartSpawnEnemy()
        {
			if (spawnCylinderReady && spawnBoxReady && !spawnStarted) {
				spawnStarted = true;
				InvokeRepeating ("Spawn", spawnTime, spawnTime);


//				cylinderTrackableHandler.gameObject.SetActive (false);
//				multiTargetTrackableHandler.gameObject.SetActive (false);

				Debug.Log ("start spawning"); 
			}

//				if(cylinderTrackableHandler.isActiveAndEnabled)
//					cylinderTrackableHandler.enabled = false;
//
//				if(multiTargetTrackableHandler.isActiveAndEnabled)
//					multiTargetTrackableHandler.enabled = false;

				//return true;

			//} //else
//				UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (0);
				//return false;
        }

        private void OnSodaOriginTrackableFound()
        {
			if (sodaOriginTrackableHandler)
            {
				spawnPoints [0] = sodaOriginTrackableHandler.transform;
//				spawnPositions[0] = cylinderTrackableHandler.transform.position;
//				spawnPositions [0].y = 0f;
                spawnCylinderReady = true;
            }
        }

		private void OnSodaDietTrackableFound()
		{
			if (sodaDietTrackableHandler) {
				spawnPoints [1] = sodaDietTrackableHandler.transform;
//                				spawnPositions[1] = multiTargetTrackableHandler.transform.position;
//                				spawnPositions [1].y = 0;
                spawnBoxReady = true;
			}
		}
    }
}