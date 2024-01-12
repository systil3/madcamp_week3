using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float RemoveDistance = 200.0f;

    GameObject player;
    new Renderer renderer;
    ParticleSystem exp;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        renderer = GetComponent<Renderer>();
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
            renderer.enabled = false;
            Destroy(gameObject, exp.main.duration);
        }
    }
}
