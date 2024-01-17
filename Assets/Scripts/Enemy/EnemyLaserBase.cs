using UnityEngine;
using UnityEditor;
using System.Collections;

public abstract class EnemyLaserBase : EnemyBase
{
    public float LaserPeriod = 2f; // 레이저 발사 주기
    public float LaserAimingDelay = 1f; // 레이저 조준 시간
    public float LaserFiringTime = 0.5f; // 레이저 발사 시간
    public float LaserTime = 2.5f + 10.0f; // 시간 측정

    protected bool isAimingLaser = false; // 레이저를 조준하고 있는지
    protected bool isFiringLaser = false; // 레이저를 발사하고 있는지

    //레이저 파티클
    public ParticleSystem LaserFireSignParticleSystem;
    public ParticleSystem.EmissionModule LaserFireSignEmissionModule;
    public ParticleSystem LaserFireParticleSystem;
    public Transform LaserFireParticleTransform;
    public ParticleSystem.EmissionModule LaserFireEmissionModule;
    public float FiringParticleSize = 1;

    public Transform TargetHitTransform;
    public ParticleSystem TargetHitEmitter;

    // 레이저 라인렌더러
    public LineRenderer LaserLine;
    public float InnerLaserTileAmount = 1.0f;
    public float OuterLaserTileAmount = 1.0f;
    public Material LaserYBeamMaterial;

    protected Transform laserTransform;
    protected float innerLaserStartWidth = 1.0f;
    protected float innerLaserEndWidth = 1.0f;
    protected float outerLaserStartWidth = 1.0f;
    protected float outerLaserEndWidth = 1.0f;
    protected Vector3 laserStartPoint = Vector3.zero;
    protected Vector3 laserEndPoint = Vector3.zero;

    //레이캐스트 관련
    public float MaxRayLength = 200f;
    protected Vector3 hitPosition;

    public AudioClip EnemyLaserShotSound;
    protected AudioSource audioSource; // AudioSource 컴포넌트

    public override void Awake()
    {
        base.Awake();
        // 파티클 시스템 컴포넌트 가져오기
        LaserFireSignParticleSystem = transform.Find("LaserSignParticle").GetComponent<ParticleSystem>();
        LaserFireSignEmissionModule = LaserFireSignParticleSystem.emission;
        LaserFireSignEmissionModule.enabled = true;
        LaserFireParticleTransform = transform.Find("LaserParticle");
        LaserFireParticleSystem = LaserFireParticleTransform.GetComponent<ParticleSystem>();
        LaserFireEmissionModule = LaserFireParticleSystem.emission;
        LaserFireEmissionModule.enabled = false;

        //레이저(라인렌더러) 찾기
        laserTransform = transform.Find("Laser");
        //Debug.Log(laserTransform);
        if (laserTransform != null)
        {
            LaserLine = laserTransform.GetComponent<LineRenderer>();
            LaserLine.useWorldSpace = true;
        }

        // AudioSource 컴포넌트 초기화
        audioSource = GetComponent<AudioSource>();
    }

    public abstract RaycastHit MakeLaser();

    public override void OnCombat(float distanceToPlayer)
    {
        if (LaserTime > LaserPeriod + LaserFiringTime)
        {
            OnDormant();
        }

        if (LaserTime > LaserPeriod)
        {
            //플레이어 위치를 기억해놨다 딜레이 후 레이저를 발사
            isAimingLaser = false;

            if (!isFiringLaser)
            {
                laserStartPoint = transform.position + new Vector3(-2, 2, 0);
                isFiringLaser = true;
                LaserLine.enabled = true;
                LaserLine.startColor = new Color(1f, 0, 0);
                LaserLine.endColor = new Color(0.5f, 0, 0);
                LaserFireSignEmissionModule.enabled = false;
            }

            RaycastHit hit = MakeLaser();

            LaserFireParticleTransform.position = hitPosition;
            LaserFireEmissionModule.enabled = true;

            if (hit.collider != null && hit.collider.gameObject.CompareTag("Player"))
            {
                Player player = Player.GetComponent<Player>();
                StartCoroutine(Shake(player));
                GameManager.DamageToPlayer(Damage);
            }
        }
        else
        {
            if (LaserTime > LaserPeriod - LaserAimingDelay && !isAimingLaser)
            {
                laserEndPoint = Player.position;
                isAimingLaser = true; //레이저 조준 상태 (조준 딜레이 존재)
                                      //파티클 활성화로 곧 쏘겠다는 신호 보내기
                LaserFireSignEmissionModule.enabled = true;
                audioSource.PlayOneShot(EnemyLaserShotSound);
                //Debug.Log("laser fire soon");
            }
        }

        LaserTime += Time.deltaTime;
    }

    public override void OnDormant()
    {
        // 발사 종료
        LaserTime = 0;
        isFiringLaser = false;
        LaserLine.enabled = false;
        LaserFireEmissionModule.enabled = false;
    }

    public override void Die()
    {
        OnDormant();
    }

    IEnumerator Shake(Player player)
    {
        float halfDuration = DamageDuration / 2;
        float elapsed = 0f;
        float tick = Random.Range(0f, 1000f);

        while (elapsed < DamageDuration)
        {
            Vector3 noise = new Vector3(Mathf.PerlinNoise(100, tick), Mathf.PerlinNoise(200, tick), Mathf.PerlinNoise(300, tick)) - 0.5f * Vector3.one;
            Camera.main.transform.localPosition = player.CameraOffset + noise * 2.0f * Mathf.PingPong(elapsed, halfDuration);
            tick += Time.deltaTime * 10.0f;
            elapsed += Time.deltaTime / halfDuration;
            yield return null;
        }

        Camera.main.transform.localPosition = player.CameraOffset;
    }
}
