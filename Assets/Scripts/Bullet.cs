using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Damage;
    public float Radius = 0.0f;
    public float RemoveDistance = 200.0f;

    GameObject player;
    Rigidbody body;
    ParticleSystem exp;
    bool alreadyDamaged = false;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        body = GetComponent<Rigidbody>();
        exp = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > RemoveDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (!alreadyDamaged && !other.gameObject.CompareTag("Player"))
        {
            if (Radius == 0)
            {
                if (other.gameObject.CompareTag("Enemy"))
                {
                    EnemyBase enemy = other.gameObject.GetComponent<EnemyBase>();
                    enemy.TakeDamage(Damage);
                }
            }
            else
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, Radius);

                foreach (Collider collider in colliders)
                {
                    if (!collider.CompareTag("Enemy")) continue;

                    float distance = Vector3.Distance(collider.ClosestPoint(transform.position), transform.position);
                    EnemyBase enemy = collider.gameObject.GetComponent<EnemyBase>();
                    enemy.TakeDamage((float)Math.Round((decimal)(Radius - distance) / (decimal)Radius * (decimal)Damage, 1));
                }
            }

            exp.Play();
            transform.Find("default").GetComponent<Renderer>().enabled = false;
            body.velocity = Vector3.zero;
            alreadyDamaged = true;
            Destroy(gameObject, exp.main.duration);
        }
    }
}
