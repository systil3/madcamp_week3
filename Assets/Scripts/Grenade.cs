using System;
using UnityEngine;

public class Grenade : Ammo
{
    void Awake() {
        Damage = 20.0f;
        Radius = 10.0f;
        RemoveDistance = 200.0f;

        player = GameObject.Find("Player");
        body = GetComponent<Rigidbody>();
        //exp = GetComponent<ParticleSystem>();
    }

    override public void OnCollisionEnter(Collision other)
    {
        if (!alreadyDamaged && !other.gameObject.CompareTag("Player"))
        {   
            print("granade");

            Collider[] colliders = Physics.OverlapSphere(transform.position, Radius);
            foreach (Collider collider in colliders)
            {
                if (!collider.CompareTag("Enemy")) continue;

                float distance = Vector3.Distance(collider.ClosestPoint(transform.position), transform.position);
                EnemyBase enemy = collider.gameObject.GetComponent<EnemyBase>();
                enemy.TakeDamage((float)Math.Round((decimal)(Radius - distance) / (decimal)Radius * (decimal)Damage, 1));
            }

            print("exp : " + exp);
            exp.Play();
            //transform.Find("default").GetComponent<Renderer>().enabled = false;
            body.velocity = Vector3.zero;
            alreadyDamaged = true;
            Destroy(gameObject, exp.main.duration);
        }
    }
}
