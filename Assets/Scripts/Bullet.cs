using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Damage = 10.0f;
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
