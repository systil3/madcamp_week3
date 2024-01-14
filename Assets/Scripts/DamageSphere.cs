using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSphere: MonoBehaviour
{   
    public GameManager GameManager;
    private float Damage = 20f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
        {
            GameManager.DamageToPlayer(Damage);
            GameManager.SlowDownPlayerSpeed();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player"))
        {
            GameManager.ReturnPlayerSpeed();
        }
    }
}
