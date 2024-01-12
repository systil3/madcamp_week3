using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float RemoveDistance = 200.0f;

    GameObject player;
    Rigidbody body;
    Renderer modelRenderer;
    ParticleSystem exp;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        body = GetComponent<Rigidbody>();
        modelRenderer = transform.Find("default").GetComponent<Renderer>();
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
        if (!other.gameObject.CompareTag("Player"))
        {
            exp.Play();
            modelRenderer.enabled = false;
            body.velocity = Vector3.zero;
            Destroy(gameObject, exp.main.duration);
        }
    }
}
