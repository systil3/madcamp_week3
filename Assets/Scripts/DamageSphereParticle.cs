using UnityEngine;

public class DamageSphereParticle : MonoBehaviour
{
    public GameManager GameManager;
    public float Damage;

    void OnParticleTrigger()
    {
        Debug.Log("Sparkle!");
        GameManager.DamageToPlayer(Damage);
    }
}
