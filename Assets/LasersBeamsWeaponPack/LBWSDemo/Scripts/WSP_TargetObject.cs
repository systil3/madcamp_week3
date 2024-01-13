using UnityEngine;
using System.Collections;

public class WSP_TargetObject : MonoBehaviour {

	private Transform myTransform;
	public int TargetLevel = 1;
	public float MoveSpeed = 10;

	// Use this for initialization
	void Start () {
		WSP_TargetManager.GlobalAccess.AddTargetToActiveList(this);
		WSP_TargetSpawner.GlobalAccess.ActiveTargets++;

		myTransform = gameObject.transform;
		TargetLevel = (int)WSP_DemoController.GlobalAccess.DemoTargetLevel;
		float targetScale = 1.0f + (0.1f * TargetLevel);
		Vector3 localScale = new Vector3(targetScale, targetScale, targetScale);
		myTransform.localScale = localScale;
		MoveSpeed = WSP_DemoController.GlobalAccess.DemoTargetSpeed + (0.25f * TargetLevel);
	}
	
	// Update is called once per frame
	void Update () {
		// Basic Movement
		myTransform.Translate(-(myTransform.forward * MoveSpeed * Time.deltaTime));
		if (myTransform.position.z < -28) {
			Destroy(gameObject);
		}
	}

	void OnDestroy() {
		// Remove From TargetObjectsList in Target Manager
		WSP_TargetManager.GlobalAccess.RemoveTargetFromActiveList(this);
		WSP_TargetSpawner.GlobalAccess.ActiveTargets--;
	}
}
