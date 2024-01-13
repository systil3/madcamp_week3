using UnityEngine;
using System.Collections;

public class WSP_LookAtTarget : MonoBehaviour {

	private Transform myTransform;
	public Transform TargetTransform;

	// Use this for initialization
	void Start () {
		myTransform = gameObject.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (TargetTransform != null) {
			myTransform.LookAt(TargetTransform.position);
		}
	}
}
