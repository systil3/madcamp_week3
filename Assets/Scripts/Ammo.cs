using System;
using UnityEngine;
public abstract class Ammo : MonoBehaviour 
{

    public GameManager GameManager;
    public float Damage;
    public float Radius = 0.0f;
    public float RemoveDistance = 200.0f;
    public GameObject player;
    public Rigidbody body;
    public ParticleSystem exp;
    public bool alreadyDamaged = false;

    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > RemoveDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy() {
        exp.Stop();
    }

    public abstract void OnCollisionEnter(Collision other);

}