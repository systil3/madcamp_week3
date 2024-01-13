using UnityEngine;
using System.Collections;

public class WSP_DestroyAfterTime : MonoBehaviour {

	private float lifeTimer = 0;
	public float LifeSpan = 4.0f;

	// Use this for initialization
	void Start () {
		lifeTimer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (lifeTimer < LifeSpan) {
			lifeTimer += Time.deltaTime;
		}
		else {
			Destroy(gameObject);
		}
	}
}
