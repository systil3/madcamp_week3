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
    public GameObject ShockFieldSignPrefab;
    public GameObject ShockFieldParticlesPrefab;
    public GameObject ShockFieldSign;
    public GameObject ShockFieldParticles;

    public float SphereSpeed = 10f;
    public float SphereScaleRate = 20f;
    public float ShockFieldDetectionRange = 10;
    public float TimeBetweenShock = 3;
    public float ShockFieldElapsedTime = 0;
    public float ShockFieldDamage = 30;
    public float ShockFieldDelay = 2;
    private bool isShockFieldEmitted = false;
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

        //충격파
        //shockFieldSignObject = Instantiate(ShockFieldSign, muzzleTransform.position, Quaternion.identity);    
        ShockFieldSign = Instantiate(ShockFieldSignPrefab, muzzleTransform.position, Quaternion.identity);
        ShockFieldParticles = Instantiate(ShockFieldParticlesPrefab, muzzleTransform.position, Quaternion.identity);
        ShockFieldSign.transform.parent = transform;
        ShockFieldParticles.transform.parent = transform;

        //인식 범위대로 사전 설정
        ParticleSystem shock = ShockFieldSign.GetComponent<ParticleSystem>();
        ParticleSystem glow = ShockFieldSign.transform.Find("Glow").GetComponent<ParticleSystem>();
        ParticleSystem.MainModule glowMain = glow.main;
        ParticleSystem.MainModule shockMain = shock.main;
        glowMain.startLifetime = TimeBetweenShock;
        shockMain.startLifetime = TimeBetweenShock;
        projectiles.Add(ShockFieldSign);
        ShockFieldSign.SetActive(false);

        //충격파 인식 범위대로 효과 반지름 설정
        ParticleSystem.MainModule shockGlowMain = ShockFieldParticles.transform.Find("Glow").GetComponent<ParticleSystem>().main;
        shockGlowMain.startSize = ShockFieldDetectionRange += 0.5f;
    }

    public override void OnCombat(float distanceToPlayer)
    {
        // 플레이어가 어느정도 떨어져 있을 시, 일정 시간마다 구체 발사
        if (distanceToPlayer >= ShockFieldDetectionRange)
        {
            ShockFieldElapsedTime = 0;
            ShockFieldSign.SetActive(false);

            if (elapsedTime >= TimeBetweenShots)
            {
                ShootProjectile();
                elapsedTime = 0f;
            }
            elapsedTime += Time.deltaTime;
        }
        // 플레이어가 일정 이상 너무 가까이 있을 경우, 충격파 신호 후 발산 
        else
        {
            if (ShockFieldElapsedTime >= TimeBetweenShock + ShockFieldDelay)
            {
                ShockFieldElapsedTime = 0;
                isShockFieldEmitted = false;
            }
            else if (ShockFieldElapsedTime >= TimeBetweenShock)
            {
                if (!isShockFieldEmitted)
                {
                    EmitShockField();
                    isShockFieldEmitted = true;
                    ShockFieldSign.SetActive(false);
                }

                ShockFieldElapsedTime += Time.deltaTime;
            }
            else
            {
                ShockFieldSign.SetActive(true);
                ShockFieldElapsedTime += Time.deltaTime;
            }
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

    void EmitShockField()
    {
        // 플레이어 방향으로 충격파 발산, 회피할 수 없음
        ParticleSystem[] particles = ShockFieldParticles.GetComponentsInChildren<ParticleSystem>();
        foreach (var particle in particles)
        {
            particle.Play();
        }
        GameManager.DamageToPlayer(ShockFieldDamage);
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
