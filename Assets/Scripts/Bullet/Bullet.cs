using UnityEngine;

public class Bullet : Ammo
{
    public override void OnCollision(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyBase enemy = other.gameObject.GetComponent<EnemyBase>();
            enemy.TakeDamage(Damage);
            GameManager.NumHit++;
        }

        transform.Find("default").GetComponent<Renderer>().enabled = false;
    }
}
