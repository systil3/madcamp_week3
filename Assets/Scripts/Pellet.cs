using UnityEngine;

public class Pellet : Ammo
{
    public override void OnCollision(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyBase enemy = other.gameObject.GetComponent<EnemyBase>();
            enemy.TakeDamage(Damage);
        }

        //transform.Find("default").GetComponent<Renderer>().enabled = false;
    }
}