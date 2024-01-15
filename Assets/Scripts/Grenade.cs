using System;
using UnityEngine;

public class Grenade : Ammo
{
    public float Radius = 6.0f;

    public override void OnCollision(Collision other)
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
        //transform.Find("default").GetComponent<Renderer>().enabled = false;
    }
}
