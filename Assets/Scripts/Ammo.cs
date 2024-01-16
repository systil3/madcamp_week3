using UnityEngine;

public abstract class Ammo : MonoBehaviour
{
    public float Damage;
    public GameObject Player;

    protected Rigidbody body;
    protected ParticleSystem exp;
    protected bool alreadyDamaged = false;

    public virtual void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        body = GetComponent<Rigidbody>();
        exp = GetComponent<ParticleSystem>();
    }

    public virtual void Update()
    {
        if (Vector3.Distance(transform.position, Player.transform.position) > 200.0f)
        {
            Destroy(gameObject);
        }
    }

    public virtual void OnDestroy()
    {
        exp.Stop();
    }

    public abstract void OnCollision(Collision other);

    public virtual void OnCollisionEnter(Collision other)
    {
        if (!alreadyDamaged && !other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Bullet"))
        {
            OnCollision(other);
            exp.Play();
            body.velocity = Vector3.zero;
            alreadyDamaged = true;
            Destroy(gameObject, exp.main.duration);
        }
    }
}