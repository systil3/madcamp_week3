using System;
using UnityEngine;

public class Bullet : Ammo
{
    void Awake() {
        Damage = 10.0f;
        Radius = 0f;
        RemoveDistance = 200.0f;

        player = GameObject.Find("Player");
        body = GetComponent<Rigidbody>();
        //exp = GetComponent<ParticleSystem>();
    }
    override public void OnCollisionEnter(Collision other)
    {
        if (!alreadyDamaged && !other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Bullet"))
        {

            if (other.gameObject.CompareTag("Enemy"))
            {
                EnemyBase enemy = other.gameObject.GetComponent<EnemyBase>();
                enemy.TakeDamage(Damage);
            } 

            exp.Play();
            transform.Find("default").GetComponent<Renderer>().enabled = false;
            body.velocity = Vector3.zero;
            alreadyDamaged = true;
            Destroy(gameObject, exp.main.duration);
        }
    }
}
