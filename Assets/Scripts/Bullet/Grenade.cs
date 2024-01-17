using System;
using UnityEngine;

public class Grenade : Ammo
{
    public float Radius = 6.0f;
    public AudioClip ExplosionSound;

    public override void Awake()
    {
        base.Awake();
        exp = transform.Find("Meteor hit").GetComponent<ParticleSystem>();
    }

    public override void OnCollision(Collision other)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, Radius);
        foreach (Collider collider in colliders)
        {
            if (!collider.CompareTag("Enemy")) continue;

            float distance = Vector3.Distance(collider.ClosestPoint(transform.position), transform.position);
            EnemyBase enemy = collider.gameObject.GetComponent<EnemyBase>();
            enemy.TakeDamage((float)Math.Round((decimal)(Radius - distance) / (decimal)Radius * (decimal)Damage, 1));
        }

        if (colliders.Length > 0)
        {
            GameManager.NumHit++;
        }

        audioSource.PlayOneShot(ExplosionSound);
        transform.Find("Trace").GetComponent<ParticleSystem>().Stop();
        transform.Find("Circle.012").GetComponent<Renderer>().enabled = false;
    }
}
