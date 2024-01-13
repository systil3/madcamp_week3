using UnityEngine;
using System.Collections;

public class LaserMMBlockerScript : MonoBehaviour {
	
	private Transform myTransform;
	public float BlockerRotationSpeed = 2;

	// Use this for initialization
	void Start () {
		myTransform = gameObject.transform;
	}
	
	// Update is called once per frame
	void Update () {
		myTransform.RotateAround(myTransform.position, myTransform.up, BlockerRotationSpeed * Time.deltaTime);
	}
}
