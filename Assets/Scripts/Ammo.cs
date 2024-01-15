using UnityEngine;

public abstract class Ammo : MonoBehaviour
{
    public GameObject Player;
    public float Damage;
    public float RemoveDistance = 200.0f;

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
        if (Vector3.Distance(transform.position, Player.transform.position) > RemoveDistance)
        {
            Destroy(gameObject);
        }
    }

    public virtual void OnDestroy()
    {
        exp.Stop();
    }

    void OnCollisionEnter(Collision other)
    {
        if (!alreadyDamaged && !other.gameObject.CompareTag("Player"))
        {
            OnCollision(other);
            exp.Play();
            body.velocity = Vector3.zero;
            alreadyDamaged = true;
            Destroy(gameObject, exp.main.duration);
        }
    }

    public abstract void OnCollision(Collision other);
}