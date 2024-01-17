using UnityEngine;

public class Pellet : Ammo
{
    public override void OnCollision(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyBase enemy = other.gameObject.GetComponent<EnemyBase>();
            enemy.TakeDamage(Damage);
            GameManager.NumHit++;
        }

        //transform.Find("trace").GetComponent<Renderer>().enabled = false;
    }
}
