using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WSP_TargetManager : MonoBehaviour {
	public static WSP_TargetManager GlobalAccess;
	void Awake () {
		GlobalAccess = this;
	}

	public List<WSP_TargetObject> TargetObjectsList;

	// Use this for initialization
	void Start () {
		TargetObjectsList = new List<WSP_TargetObject>();
		TargetingList = new List<Transform>();
	}

	public void AddTargetToActiveList(WSP_TargetObject targetScript) {
		TargetObjectsList.Add(targetScript);
	}

	private List<Transform> TargetingList;

	public Transform GetRandomTargetFromList(Vector3 gettingObjectPosition, float minimumDistanceToTarget) {
		TargetingList.Clear();
		if (TargetObjectsList.Count > 0) {
			foreach (WSP_TargetObject target in TargetObjectsList) {
				float distanceToTarget = Vector3.Distance(gettingObjectPosition, target.transform.position);
				if (distanceToTarget > minimumDistanceToTarget) {
					TargetingList.Add(target.transform);
				}
			}
		}
		if (TargetingList.Count > 0) {
			float randomTarget = Random.Range(0, TargetingList.Count - 1);
			int targetNum = (int)randomTarget;
			return TargetingList[targetNum];
		}
		else {
			return null;
		}
	}

	public void RemoveTargetFromActiveList(WSP_TargetObject targetScript) {
		TargetObjectsList.Remove(targetScript);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
