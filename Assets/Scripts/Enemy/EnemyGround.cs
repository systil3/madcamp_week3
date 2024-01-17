using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    public float elapsedTime = 100;

    private int shotCount = 0;
    private float shotElapsedTime = 0;
    public float bulletForce = 20;
    public float numberOfBullets = 8;
    private bool isShooting = false;

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

        if (elapsedTime > TimeBetweenPatterns)
        {
            elapsedTime = 0;
            isShooting = true;
        }
        else
        {
            elapsedTime += Time.deltaTime;
        }
    }

    public void Shot()
    {
        // 1/5의 확률로 파괴가 불가능한 탄 생성
        // 4/5의 확률로 파괴가 가능한 탄 생성
        print(shotCount);
        if (shotCount < NumberOfShots)
        {

            if (shotElapsedTime > TimeBetweenShots)
            {
                for (int i = 0; i < numberOfBullets; i++)
                {
                    float angle = math.PI * 2 * i / numberOfBullets;
                    //Vector3 bulletDirection = new Vector3(math.cos(angle), 0, math.sin(angle));
                    float bulletRandomValue = UnityEngine.Random.Range(0f, 1f);
                    GameObject bullet;

                    if (bulletRandomValue < 1 / 5f)
                    {
                        print(BulletWeak);
                        bullet = Instantiate(BulletWeak, MuzzleTransform.position, transform.rotation);
                    }
                    else
                    {
                        print(BulletStrong);
                        bullet = Instantiate(BulletStrong, MuzzleTransform.position, transform.rotation);
                    }

                    bullet.transform.RotateAround(bullet.transform.position, Vector3.up, angle);
                    bullet.transform.forward = new Vector3(math.cos(angle), 0, math.sin(angle));

                    Rigidbody brb = bullet.GetComponent<Rigidbody>();
                    brb.AddForce(brb.transform.forward * bulletForce, ForceMode.Impulse);
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
