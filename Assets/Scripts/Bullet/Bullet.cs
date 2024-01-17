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
        else if (other.gameObject.CompareTag("EnemyBullet"))
        {
            EnemyBullet enemyBullet = other.gameObject.GetComponent<EnemyBullet>();
            enemyBullet.TakeDamage(Damage);
        }

        transform.Find("default").GetComponent<Renderer>().enabled = false;
    }
}
