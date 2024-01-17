using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGround : EnemyBase
{
    public GameObject BulletWeak;
    public GameObject BulletStrong;
    public Transform MuzzleTransform;

    public float TimeBetweenPatterns = 5;
    public float TimeBetweenShots = 0.2f;
    public float NumberOfShots = 10;
    public float BulletLifeTime = 5;
    public float ElapsedTime = 100;
    public float BulletForce = 20;
    public float NumberOfBullets = 8;

    int shotCount = 0;
    float shotElapsedTime = 0;
    bool isShooting = false;

    public override void Awake()
    {
        base.Awake();
        MuzzleTransform = transform.Find("Cylinder");
    }

    public override void OnCombat(float distanceToPlayer)
    {
        if (isShooting)
        {
            Shot();
        }

        if (ElapsedTime > TimeBetweenPatterns)
        {
            ElapsedTime = 0;
            isShooting = true;
        }
        else
        {
            ElapsedTime += Time.deltaTime;
        }
    }

    public void Shot()
    {
        // 1/5의 확률로 파괴가 불가능한 탄 생성
        // 4/5의 확률로 파괴가 가능한 탄 생성
        if (shotCount < NumberOfShots)
        {
            if (shotElapsedTime > TimeBetweenShots)
            {
                for (int i = 0; i < NumberOfBullets; i++)
                {
                    float angle = Mathf.PI * 2 * i / NumberOfBullets;
                    float bulletRandomValue = Random.Range(0f, 1f);

                    GameObject bullet = Instantiate(bulletRandomValue > 0.2f ? BulletWeak : BulletStrong, MuzzleTransform.position + Vector3.up * 0.5f, transform.rotation);
                    bullet.transform.RotateAround(bullet.transform.position, Vector3.up, angle);
                    bullet.transform.forward = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

                    Rigidbody brb = bullet.GetComponent<Rigidbody>();
                    brb.AddForce(brb.transform.forward * BulletForce, ForceMode.Impulse);
                }
                shotElapsedTime = 0;
                shotCount++;
            }
            else
            {
                shotElapsedTime += Time.deltaTime;
            }
        }
        else
        {
            isShooting = false;
            shotCount = 0;
        }
    }

    public override void OnDormant() { }

    // 파티클이 트리거에 들어갈 때 호출되는 함수
    private void OnParticleCollision(GameObject other)
    {
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleTrigger()
    {
        StartCoroutine(PlayerForceCoroutine(Player.GetComponent<Player>()));
        GameManager.DamageToPlayer(Damage);
    }

    IEnumerator PlayerForceCoroutine(Player player)
    {
        player.IsFreeze = true;
        yield return new WaitForSeconds(DamageDuration);
        player.IsFreeze = false;
    }
}
