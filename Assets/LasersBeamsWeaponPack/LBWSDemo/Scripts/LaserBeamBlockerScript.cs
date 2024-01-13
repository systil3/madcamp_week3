using UnityEngine;
using System.Collections;

public class LaserBeamBlockerScript : MonoBehaviour {

	private Transform myTransform;
	public float BlockerMoveSpeed = 10;

	private bool movingUp = false;
	private Vector3 moveUpDirection = new Vector3(-1, 0, 0);
	private Vector3 moveDownDirection = new Vector3(1, 0, 0);

	// Use this for initialization
	void Start () {
		myTransform = gameObject.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (!movingUp) {
			if (myTransform.position.x < 4) {
				myTransform.Translate(moveDownDirection * BlockerMoveSpeed * Time.deltaTime);
			}
			else {
				movingUp = true;
			}
		}
		else {
			if (myTransform.position.x > -4) {
				myTransform.Translate(moveUpDirection * BlockerMoveSpeed * Time.deltaTime);	
			}
			else {
				movingUp = false;
			}
		}
	}
}
