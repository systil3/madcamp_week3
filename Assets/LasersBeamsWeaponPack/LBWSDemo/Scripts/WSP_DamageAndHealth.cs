using UnityEngine;
using System.Collections;

public class WSP_DamageAndHealth : MonoBehaviour {

	public bool Alive = true;
	public int TargetLevel = 1;
	public float Health = 0;
	public float HealthMAX = 100;

	public GameObject ExplosionPrefab;
	public GameObject Explosion2Prefab;

	// Use this for initialization
	void Start () {
		TargetLevel = (int)WSP_DemoController.GlobalAccess.DemoTargetLevel;

		Alive = true;
		HealthMAX = WSP_DemoController.GlobalAccess.DemoTargetHealthMAX + (5 * TargetLevel);
		Health = HealthMAX;
	}

	void Damage(float damageRecieved) {
		Health -= damageRecieved;
		if (Health <= 0) {
			Alive = false;
		}
	}

	// Update is called once per frame
	void Update () {
		if (Alive) {
		}
		else {
			// Destroyed
			DoDestructionEvents();
		}
	}

	private void SpawnExplosion() {
		GameObject newExplosion = GameObject.Instantiate(ExplosionPrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
		newExplosion.name = gameObject.name + "_EXPLO";
	}

	private void SpawnExplosion2() {
		GameObject newExplosion = GameObject.Instantiate(Explosion2Prefab, gameObject.transform.position, Quaternion.identity) as GameObject;
		newExplosion.name = gameObject.name + "_EXPLO2";
	}

	private void DoDestructionEvents() {
		if (ExplosionPrefab != null) {
			SpawnExplosion();
		}
		if (Explosion2Prefab != null) {
			SpawnExplosion2();
		}
		Destroy(gameObject);
	}
}
