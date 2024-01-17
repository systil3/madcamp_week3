using UnityEngine;

public abstract class Ammo : MonoBehaviour
{
    public float Damage;
    public AudioClip GunShotSound;
    public GameObject Player;

    protected Rigidbody body;
    protected ParticleSystem exp;
    protected AudioSource audioSource;
    protected bool alreadyDamaged = false;

    public virtual void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        body = GetComponent<Rigidbody>();
        exp = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(GunShotSound);
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
        if (!alreadyDamaged && !other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("PlayerBullet"))
        {
            OnCollision(other);
            exp.Play();
            body.velocity = Vector3.zero;
            alreadyDamaged = true;
            Destroy(gameObject, exp.main.duration);
        }
    }
}