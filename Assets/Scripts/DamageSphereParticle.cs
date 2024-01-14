using UnityEngine;

public class DamageSphereParticle : MonoBehaviour
{
    public GameManager GameManager;
    public float Damage = 5.0f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.DamageToPlayer(Damage);
        }
    }
}
