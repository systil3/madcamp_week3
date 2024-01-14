using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSphereParticle: MonoBehaviour
{   
    public GameManager GameManager;
    private float Damage = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) {
        print(other);
        if (other.CompareTag("Player"))
        {
            GameManager.DamageToPlayer(Damage);
        }
    }

}
