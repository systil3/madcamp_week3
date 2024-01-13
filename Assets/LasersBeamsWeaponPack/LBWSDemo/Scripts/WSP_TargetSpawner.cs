using UnityEngine;
using System.Collections;

public enum DemoTargetPrefabTypes {
	CapsuleTarget01
}

public class WSP_TargetSpawner : MonoBehaviour {
	public static WSP_TargetSpawner GlobalAccess;
	void Awake () {
		GlobalAccess = this;
	}

	public bool UseSpawner = true;
	public bool TargetSpawnerActive = false;
	public GameObject[] DemoTargetPrefabs;
	public int NumberOfDemoTargetsSpawned = 0;
	public int MaxTargetsAtOnce = 8;
	public int ActiveTargets = 0;

	// Use this for initialization
	void Start () {
		if (UseSpawner)
			TargetSpawnerActive = true;
	}

	private float spawningTimer = 0;
	private float SpawningTimerFreq = 2;

	void Update() {
		// Update SpawningTimerFreq Based on Number of Active Demo Targets
		if (ActiveTargets < 3) {
			SpawningTimerFreq = 0.5f;
		}
		else if (ActiveTargets >= 3 && ActiveTargets < 5) {
			SpawningTimerFreq = 1.0f;
		}
		else if (ActiveTargets >= 5) {
			SpawningTimerFreq = 1.5f;
		}

		if (TargetSpawnerActive) {
			if (spawningTimer < SpawningTimerFreq) {
				spawningTimer += Time.deltaTime;
			}
			else {
				if (ActiveTargets < MaxTargetsAtOnce) {
					// Spawn New Target
					SpawnDemoTarget(DemoTargetPrefabTypes.CapsuleTarget01);
					// Change Demo Level of Targets After Spawn
					int combinedLevelOfTurrets = WSP_DemoController.GlobalAccess.GetCombinedTurretLevel();
					WSP_DemoController.GlobalAccess.DemoTargetLevel = (int)Random.Range(1, combinedLevelOfTurrets);
				}
				spawningTimer = 0;
				SpawningTimerFreq = (int)Random.Range(2, 4);
			}
		}
	}

	public void SpawnDemoTarget(DemoTargetPrefabTypes targetType) {
		NumberOfDemoTargetsSpawned++;
		float randomXLoc = Random.Range(-10, 10);
		float randomYLoc = Random.Range(2, 8);
		float randomZLoc = Random.Range(25, 30);
		Vector3 startingPosition = new Vector3(randomXLoc, randomYLoc, randomZLoc);
		GameObject newDemoTarget = GameObject.Instantiate(DemoTargetPrefabs[(int)targetType], startingPosition, Quaternion.identity) as GameObject;
		newDemoTarget.name = targetType.ToString() + "_" + NumberOfDemoTargetsSpawned.ToString();
	}

}
