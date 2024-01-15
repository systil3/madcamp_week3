using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySphere : EnemyBase
{
    public float TimeBetweenShots = 8;
    public float SphereLifeTime = 6;
    float elapsedTime = 5f + 1f;

    public GameObject DamageSphere;
    public float SphereSpeed = 10f;
    public float SphereScaleRate = 20f;

    float sphereScale;
    Transform muzzleTransform;

    //파티클 관련
    public int NumberOfParticles = 100;

    ParticleSystem particleSystemPrefab;
    ParticleSystem particleSystemInstance;
    Transform particleTransform;

    List<GameObject> projectiles = new List<GameObject>();

    public override void Awake()
    {
        base.Awake();
        elapsedTime = TimeBetweenShots;
        muzzleTransform = transform.Find("Muzzle");

        DamageSphere damageSphere = DamageSphere.transform.Find("Surface").GetComponent<DamageSphere>();
        damageSphere.GameManager = GameManager;
        damageSphere.Damage = Damage;

        DamageSphereParticle damageSphereParticle = DamageSphere.transform.Find("Electricity").GetComponent<DamageSphereParticle>();
        damageSphereParticle.GameManager = GameManager;
        damageSphereParticle.Damage = Damage / 4.0f;

        particleSystemPrefab = damageSphereParticle.GetComponent<ParticleSystem>();
        particleSystemPrefab.trigger.SetCollider(0, Player);

        // 파티클 시스템 프리팹을 인스턴스화
        /*
        particleSystemInstance = Instantiate(particleSystemPrefab);
        particleTransform = particleSystemInstance.transform;
        */
    }

    public override void OnCombat()
    {
        // 적이 일정 시간마다 발사
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= TimeBetweenShots)
        {
            ShootProjectile();
            elapsedTime = 0f;
        }
    }

    public override void OnDormant() { }

    public override void Die()
    {
        projectiles.ForEach(Destroy);
        Destroy(gameObject, 3f);
    }

    void ShootProjectile()
    {
        // 플레이어 방향으로 구체 발사
        Vector3 direction = (Player.position - muzzleTransform.position).normalized;
        GameObject projectile = Instantiate(DamageSphere, muzzleTransform.position, Quaternion.identity);
        projectiles.Add(projectile);
        // 발사된 구체의 크기를 점점 키우기
        StartCoroutine(ScaleProjectileOverTime(direction, projectile));
    }

    IEnumerator ScaleProjectileOverTime(Vector3 direction, GameObject projectile)
    {
        float sphereElapsedTime = 0f;
        while (sphereElapsedTime < SphereLifeTime && !projectile.IsDestroyed())
        {
            //속도 적용
            projectile.transform.Translate(direction * SphereSpeed * Time.deltaTime, Space.World);

            //스케일 적용
            sphereScale = Mathf.Lerp(1f, 1f + SphereScaleRate, sphereElapsedTime / SphereLifeTime);
            Transform sphereTransformSurface = projectile.transform.Find("Surface");
            sphereTransformSurface.localScale = Vector3.one * sphereScale;

            //파티클 스케일 일정하게 유지
            Transform sphereTransformParticle = projectile.transform.Find("Electricity");
            ParticleSystem sphereParticleSystem = sphereTransformParticle.GetComponent<ParticleSystem>();

            var mainModule = sphereParticleSystem.main;
            mainModule.startSize = 0.1f;
            mainModule.maxParticles += (int)(sphereScale * 20f);

            var startSpeed = mainModule.startSpeed;
            startSpeed.constant += sphereScale * 0.07f;
            sphereElapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(projectile);
        projectiles.Remove(projectile);
    }
}
