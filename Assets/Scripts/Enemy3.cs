using System.Collections;
using UnityEngine;

public class Enemy3 : Enemy
{
    private float elapsedTime = 5f + 1f;
    public float timeBetweenShots = 8;
    public float sphereLifeTime = 6;
    public GameObject damageSphere;
    public float sphereSpeed = 10f;
    public float sphereScaleRate = 20f;
    private float sphereScale;
    private Transform muzzleTransform;

    //파티클 관련
    public ParticleSystem particleSystemPrefab;
    public int numberOfParticles = 100;

    private ParticleSystem particleSystemInstance;
    private Transform particleTransform;
    GameObject projectile;

    public override void Awake()
    {
        base.Awake();
        muzzleTransform = transform.Find("Muzzle");
        elapsedTime = timeBetweenShots;

        // 파티클 시스템 프리팹을 인스턴스화
        particleSystemInstance = Instantiate(particleSystemPrefab);
        particleTransform = particleSystemInstance.transform;
    }

    void FixedUpdate()
    {
        if (currentState == EnemyState.Combat)
        {
            // 적이 일정 시간마다 발사
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= timeBetweenShots)
            {
                ShootProjectile();
                elapsedTime = 0f;
            }
        }
    }

    void ShootProjectile()
    {
        // 플레이어 방향으로 구체 발사
        Vector3 direction = (Player.position - muzzleTransform.position).normalized;
        GameObject projectile = Instantiate(damageSphere, muzzleTransform.position, Quaternion.identity);
        // 발사된 구체의 크기를 점점 키우기
        StartCoroutine(ScaleProjectileOverTime(direction, projectile.transform));
    }

    IEnumerator ScaleProjectileOverTime(Vector3 direction, Transform projectileTransform)
    {
        float sphereElapsedTime = 0f;
        while (sphereElapsedTime < sphereLifeTime)
        {
            //속도 적용
            projectileTransform.Translate(direction * sphereSpeed * Time.deltaTime, Space.World);

            //스케일 적용
            sphereScale = Mathf.Lerp(1f, 1f + sphereScaleRate, sphereElapsedTime / sphereLifeTime);
            Transform sphereTransformSurface = projectileTransform.Find("Surface");
            sphereTransformSurface.localScale = new Vector3(sphereScale, sphereScale, sphereScale);

            //파티클 스케일 일정하게 유지
            Transform sphereTransformParticle = projectileTransform.Find("Electricity");
            ParticleSystem sphereParticleSystem = sphereTransformParticle.GetComponent<ParticleSystem>();
            var mainModule = sphereParticleSystem.main;
            mainModule.startSize = 0.1f;
            mainModule.maxParticles += (int)(sphereScale * 20f);
            var startSpeed = mainModule.startSpeed;
            startSpeed.constant += sphereScale * 0.07f;
            sphereElapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(projectileTransform.gameObject);
    }
}
