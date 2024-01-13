using UnityEngine;
using UnityEditor;
using System.Collections;
using Unity.Mathematics;

//레이저 원형 회전 패턴

public class EnemyLaser2 : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;
    private EnemyState currentState;

    public float detectionRange = 50f;
    public float combatRange = 20f;
    public float moveSpeed = 3f; // 적의 이동 속도
    public float laserPeriod = 2f; // 레이저 발사 주기
    public float laserAimingDelay = 1f; // 레이저 조준 시간 
    public float laserFiringTime = 0.5f; // 레이저 발사 시간 
    private bool isAimingLaser = false; // 레이저를 조준하고 있는지
    private bool isFiringLaser = false; // 레이저를 발사하고 있는지
    public float laserTime = 1.5f+10.0f; // 시간 측정

    //-----------------------------------------------------
    public float laserAngularSpeed = 0; // 각속도
    public float laserPlaneRadius = 5.0f; // 동심원 반지름
    public float laserAngle = 0;

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
    public LineRenderer laserLine;
    private Transform laserTransform;
	public float innerLaserTileAmount = 1.0f;
	private float innerLaserStartWidth = 1.0f;
	private float innerLaserEndWidth = 1.0f;
	public float outerLaserTileAmount = 1.0f;
	private float outerLaserStartWidth = 1.0f;
	private float outerLaserEndWidth = 1.0f;
	public Material LaserYBeamMaterial;
	private Vector3 laserStartPoint = Vector3.zero;
	private Vector3 laserEndPoint = Vector3.zero;

    //레이캐스트 관련
    public float maxRayLength = 200f;
    private Vector3 hitPosition;
    
    public Transform player;

    AudioSource audioSource; // AudioSource 컴포넌트
    public AudioClip EnemyLaserShotSound;

    private Rigidbody rb;
    private enum EnemyState
    {
        Dormant,
        Combat,
        Dead
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentState = EnemyState.Dormant;
        rb = GetComponent<Rigidbody>();
        combatRange = 30f;

        // 플레이어를 이름으로 찾아서 할당
        player = GameObject.Find("Player").transform;

        if (player == null)
        {
            Debug.LogError("Player not found. Make sure the Player object has the correct name.");
        }

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
        print(laserTransform);
        if (laserTransform != null)
        {
            laserLine = laserTransform.GetComponent<LineRenderer>();
            laserLine.useWorldSpace = true;
        }

        // AudioSource 컴포넌트 초기화
        audioSource = GetComponent<AudioSource>();
        #if UNITY_EDITOR
            EnemyLaserShotSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Custom_Yung/EnemyLaserShot.wav");
        #endif
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        switch (currentState)
        {
            case EnemyState.Dormant:
                if (distanceToPlayer < detectionRange)
                {
                    currentState = EnemyState.Combat;
                    Debug.Log("Combat mode activated!");
                }
                break;

            case EnemyState.Combat:
                if (distanceToPlayer > detectionRange)
                {
                    currentState = EnemyState.Dormant;
                    Debug.Log("Returning to dormant state.");
                }
                else
                {   
                    Vector3 direction = player.position - transform.position;
                    // 플레이어를 향해 회전
                    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

                    // 플레이어 쪽으로 Rigidbody의 velocity를 조절하여 이동
                    if(distanceToPlayer > combatRange) {
                        rb.AddForce(direction * moveSpeed, ForceMode.VelocityChange);
                    } else {
                        rb.velocity *= 0.8f;
                    }

                    if(laserTime > laserPeriod + laserFiringTime) {
                        // 발사 종료
                        laserTime = 0;
                        isFiringLaser = false;
                        laserLine.enabled = false;
                        LaserFireEmissionModule.enabled = false;
                    }

                    if(laserTime > laserPeriod) {

                        //플레이어 위치를 기억해놨다 딜레이 후 레이저를 발사
                        //앞 패턴과 달리 기억해놓은 플레이어 위치 중심으로 동심원 회전 
                        isAimingLaser = false; 
                        if(!isFiringLaser) {
                            //LaserFireSignEmissionModule.enabled = false;
                            laserStartPoint = rb.position + new Vector3(-2,2,0);
                            isFiringLaser = true;
                            laserLine.enabled = true;
                            laserLine.startColor = new Color(1f, 0, 0);
                            laserLine.endColor = new Color(0.5f, 0, 0);
                            LaserFireSignEmissionModule.enabled = false;
                        } 

                        // 플레이어의 x좌표, z좌표를 감지하여 반지름 r의 동심원 형성
                        laserAngle = laserTime * laserAngularSpeed;
                        Vector3 radEndPoint = new Vector3(laserEndPoint.x + laserPlaneRadius*math.cos(laserAngle),
                                                    laserEndPoint.y,
                                                    laserEndPoint.z + laserPlaneRadius*math.sin(laserAngle));
                        

                        Vector3 rayDirection = (radEndPoint - laserStartPoint).normalized;
                        Ray ray = new Ray(laserStartPoint, rayDirection);

                        bool cast = Physics.Raycast(ray, out RaycastHit hit, maxRayLength);
                        // 충돌 시 위치 계산 (레이캐스트로)
                        // 다른 물체에 닿을 시 거기서 멈추기
                       
                        laserLine.SetPosition(0, laserStartPoint);
                        if(!cast) {
                            hitPosition = radEndPoint + (radEndPoint - laserStartPoint) * 10;}
                        else {
                            hitPosition = hit.point;}
                        laserLine.SetPosition(1, hitPosition);
                        ActivateLaserHitParticles();
                        
                        //0.5초 후 disable
                        laserTime += Time.deltaTime;
                        
                    } else {
                        if(laserTime > laserPeriod - laserAimingDelay
                        && !isAimingLaser) {
                            laserEndPoint = player.position;
                            isAimingLaser = true; //레이저 조준 상태 (조준 딜레이 존재)
                            //파티클 활성화로 곧 쏘겠다는 신호 보내기
                            LaserFireSignEmissionModule.enabled = true;
                            audioSource.PlayOneShot(EnemyLaserShotSound);
                            print("laser fire soon");
                        }
                        laserTime += Time.deltaTime;
                    }
                        
                }
                break;

            case EnemyState.Dead:
                break;
        }
    }

    void ActivateLaserHitParticles()
    {
        LaserFireParticleTransform.position = hitPosition;
        LaserFireEmissionModule.enabled = true;
    }

    public void TakeDamage(float damage)
    {
        if (!isDead)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        currentState = EnemyState.Dead;
        isDead = true;

        // 여기에는 사망 시 수행할 동작을 추가
        Destroy(gameObject, 3f);
    }
}
