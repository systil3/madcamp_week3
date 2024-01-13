using UnityEngine;
using System.Collections;

public class WSP_LaserBeamWS : MonoBehaviour {

	public bool LaserBeamActive = false;
	public bool AutoFireForDemo = false;
	public bool UseExtendedLength = false;

	public bool UseLineOfSight = true;
	private float damageOverTimeTimer = 0;
	private float damageOverTimeFreq = 0.25f;
	private bool canDamageTarget = false;

	private AudioSource LaserFireSoundFX;
	private bool LaserSoundPlayed = false;
		
	// Level Indicators
	public ParticleSystem LevelUpEffect;

	public Renderer[] LevelIndicatorRenderers;
	public void UpdateLevelIndicators(int levelIn, Color levelColorIn) {
		if (LevelUpEffect != null) {
			ParticleSystem.MainModule levelUpModule = LevelUpEffect.main;
			levelUpModule.startColor = levelColorIn;
			LevelUpEffect.Play();
			if (LevelIndicatorRenderers.Length > 0) {
				for (int i = 0; i < LevelIndicatorRenderers.Length; i++) {
					if (i <= levelIn) {
						// Enable if Lower or Equal To Level
						if (!LevelIndicatorRenderers[i].gameObject.activeSelf)
							LevelIndicatorRenderers[i].gameObject.SetActive(true);
					}
					else {
						// Disable if Higher Than Level
						if (LevelIndicatorRenderers[i].gameObject.activeSelf)
							LevelIndicatorRenderers[i].gameObject.SetActive(false);
					}
					LevelIndicatorRenderers[i].material.color = levelColorIn;
				}
			}
		}
	}

	public Renderer TurretColorRender;

	public Transform LaserFireEmitPointTrans;
	public ParticleSystem LaserFireCenterEmitter;
	public float FiringParticleSize = 1;
	
	public Transform TargetHitTransform;
	public ParticleSystem TargetHitEmitter;
	public float HitParticleSize = 1;
	
	public LineRenderer InnerLaserLineRender;
	public LineRenderer OuterLaserLineRender;
	public Color LaserBeamColor;
	public Material LaserYBeamMaterial;
	private Vector3 laserStartPoint = Vector3.zero;
	private Vector3 laserEndPoint = Vector3.zero;

	public bool LaserCanFire = false;
	public bool LaserFiring = false;
	public float LaserDamage = 10;
	public float LaserDamagePerSecond = 2;

	public float InnerLaserStartWidth = 1.0f;
	public float InnerLaserEndWidth = 1.0f;
	public float OuterLaserStartWidth = 1.0f;
	public float OuterLaserEndWidth = 1.0f;

	public float LaserGrowSpeed = 1;
	public float innerLaserTileAmount = 1.0f;
	private float innerLaserStartWidth = 1.0f;
	private float innerLaserEndWidth = 1.0f;
	public float outerLaserTileAmount = 1.0f;
	private float outerLaserStartWidth = 1.0f;
	private float outerLaserEndWidth = 1.0f;

	public float FireSpeed = 20.0f;

	public bool OffsetMaterialTexture = true;
	public float ScrollSpeed = 2.5F;

	private float laserLifeTimer = 0;
	public float LaserFireTime = 1.0f;

	private float laserRechargeTimer = 0;
	public float LaserRechargeTime = 2.5f;

	public Transform CurrentTarget;
	public void AssignNewTarget(Transform target) {
		if (CurrentTarget == null) {
			CurrentTarget = target;
			FireLaser();
		}
	}
	
	public void FireLaser() {
		if (LaserCanFire) {
			// Set Colors
			InnerLaserLineRender.startColor = LaserBeamColor;
			InnerLaserLineRender.endColor = LaserBeamColor;
			ParticleSystem.MainModule emitterModule = LaserFireCenterEmitter.main;
			emitterModule.startColor = LaserBeamColor;
			ParticleSystem.MainModule emitter2Module = TargetHitEmitter.main;
			emitter2Module.startColor = LaserBeamColor;

			innerLaserStartWidth = 0f;
			innerLaserEndWidth = 0f;
			outerLaserStartWidth = 0f;
			outerLaserEndWidth = 0f;
			laserLifeTimer = 0;
			if (!LaserSoundPlayed) {
				LaserFireSoundFX.Play();
				LaserSoundPlayed = true;
			}
			LaserFiring = true;
			laserRechargeTimer = 0;
			LaserCanFire = false;
		}
	}
	
	public void StopLaserFire() {
		ParticleSystem.EmissionModule emissionModule = LaserFireCenterEmitter.emission;
		emissionModule.rateOverTime = 0;
		LaserFiring = false;
		LaserSoundPlayed = false;
	}

//	private float laserOldLife = 1.0f;

	// Use this for initialization
	void Start () {

		LaserFireSoundFX = gameObject.GetComponent<AudioSource>();
		if (!AutoFireForDemo) {
			CurrentTarget = null;
		}
				
		if (LaserFireCenterEmitter != null) {
			ParticleSystem.EmissionModule emissionModule = LaserFireCenterEmitter.emission;
			emissionModule.rateOverTime = 0;
		}
		if (TargetHitEmitter != null) {
			ParticleSystem.EmissionModule emissionModule = TargetHitEmitter.emission;
			emissionModule.rateOverTime = 0;
		}
		if (InnerLaserLineRender != null) {
			InnerLaserLineRender.enabled = false;
		}
		if (OuterLaserLineRender != null) {
			OuterLaserLineRender.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		// If Test Laser Beam Firing
		if (AutoFireForDemo) {
			FireLaser();
		}
		if (UseExtendedLength) {
			LaserFireTime = 5.0f;
		}

		// Check to See if Laser Beam is Active Still
		if (OuterLaserLineRender.enabled || InnerLaserLineRender.enabled) {
			LaserBeamActive = true;
		}
		else {
			LaserBeamActive = false;
		}

		// Stop Beam if target Destroyed
		if (CurrentTarget == null) {
			if (LaserFiring) {
				StopLaserFire();
			}
		}

		if (LaserFiring) {
			if (UseLineOfSight) {
				// Handle Damage Over Time
				if (!canDamageTarget) {
					if (damageOverTimeTimer < damageOverTimeFreq) {
						damageOverTimeTimer += Time.deltaTime;
					}
					else {
						damageOverTimeTimer = 0;
						canDamageTarget = true;
					}
				}
			}

			if (laserLifeTimer < LaserFireTime) {
				laserLifeTimer += Time.deltaTime;
				if (!LaserSoundPlayed) {
					LaserFireSoundFX.Play();
					LaserSoundPlayed = true;
				}
			}
			else {
				if (CurrentTarget != null) {
					CurrentTarget.gameObject.SendMessage("Damage", LaserDamage, SendMessageOptions.DontRequireReceiver);
				}
				laserLifeTimer = 0;
				StopLaserFire();
			}
		}
		else {
			if (!LaserCanFire) {
				if (laserRechargeTimer < LaserRechargeTime) {
					laserRechargeTimer += Time.deltaTime;
				}
				else {
					LaserCanFire = true;
				}
			}
		}
		
		UpdateLaserBeam();
	}

	public void UpdateLaserBeamTiling() {
		// Set Laser Beam Line Material Tile Amount
		InnerLaserLineRender.material.mainTextureScale = new Vector2(innerLaserTileAmount, 1);
		OuterLaserLineRender.material.mainTextureScale = new Vector2(outerLaserTileAmount, 1);
	}

	private void UpdateLaserBeam() {
		// Update Firing and Hitting Particles
		ParticleSystem.MainModule mainModule1 = LaserFireCenterEmitter.main;
		mainModule1.startSize = FiringParticleSize;
		ParticleSystem.MainModule mainModule2 = TargetHitEmitter.main;
		mainModule2.startSize = HitParticleSize;

		// Always Have Laser Start at Center Emit Point
		laserStartPoint = LaserFireEmitPointTrans.position;
		if (CurrentTarget != null && LaserFiring) {
			ParticleSystem.EmissionModule emissionModule = LaserFireCenterEmitter.emission;
			emissionModule.rateOverTime = 10;

			// Grow LaserBeam
			if (innerLaserStartWidth < InnerLaserStartWidth)
				innerLaserStartWidth += LaserGrowSpeed * Time.deltaTime;
			if (innerLaserEndWidth < InnerLaserEndWidth)
				innerLaserEndWidth += LaserGrowSpeed * Time.deltaTime;
			if (outerLaserStartWidth < OuterLaserStartWidth)
				outerLaserStartWidth += LaserGrowSpeed * Time.deltaTime;
			if (outerLaserEndWidth < OuterLaserEndWidth)
				outerLaserEndWidth += LaserGrowSpeed * Time.deltaTime;

			// Set Laser Beam Line Start and End Width
			InnerLaserLineRender.startWidth = innerLaserStartWidth;
			InnerLaserLineRender.endWidth = innerLaserEndWidth;
			OuterLaserLineRender.startWidth = outerLaserStartWidth;
			OuterLaserLineRender.endWidth = outerLaserEndWidth;

			if (!InnerLaserLineRender.enabled)
				InnerLaserLineRender.enabled = true;
			if (!OuterLaserLineRender.enabled)
				OuterLaserLineRender.enabled = true;

			Vector3 targetHitPosition = CurrentTarget.position;
			// If Line of Sight Enables Use LOS Hit Check
			if (UseLineOfSight) {
				targetHitPosition = CheckLOSOnTarget();
			}

			float distanceToTarget = Vector3.Distance(laserEndPoint, targetHitPosition);
			if (distanceToTarget > 1.0f) {
				laserEndPoint = Vector3.Lerp(laserEndPoint, targetHitPosition, FireSpeed * Time.deltaTime);
				ParticleSystem.EmissionModule emission3Module = TargetHitEmitter.emission;
				emission3Module.rateOverTime = 0;
			}
			else {
				laserEndPoint = targetHitPosition;
				TargetHitTransform.position = targetHitPosition;
				ParticleSystem.EmissionModule emission4module = TargetHitEmitter.emission;
				emission4module.rateOverTime = 10;
			}			
			if (OffsetMaterialTexture) {
				float offset = Time.time * ScrollSpeed;
//				InnerLaserLineRender.material.mainTextureScale = new Vector2(-offset, innerLaserTileAmount);
				InnerLaserLineRender.material.SetTextureOffset("_MainTex", new Vector2(-offset, 0));
			}
		}
		else {
			if (InnerLaserLineRender.enabled)
				InnerLaserLineRender.enabled = false;
			if (OuterLaserLineRender.enabled)
				OuterLaserLineRender.enabled = false;
			ParticleSystem.EmissionModule emission5module = LaserFireCenterEmitter.emission;
			emission5module.rateOverTime = 0;
			ParticleSystem.EmissionModule emission6module = TargetHitEmitter.emission;
			emission6module.rateOverTime = 0;
			laserEndPoint = laserStartPoint;
		}
		
		if (InnerLaserLineRender != null) {
			//if (LaserLineRender.po
			InnerLaserLineRender.SetPosition(0, laserStartPoint);
			InnerLaserLineRender.SetPosition(1, laserEndPoint);
		}
		if (OuterLaserLineRender != null) {
			//if (LaserLineRender.po
			laserStartPoint.y += 0.1f;
			laserEndPoint.y += 0.1f;
			OuterLaserLineRender.SetPosition(0, laserStartPoint);
			OuterLaserLineRender.SetPosition(1, laserEndPoint);
		}
	}
	
	private RaycastHit myhit = new RaycastHit();	
	private Ray myray = new Ray();

	private Vector3 CheckLOSOnTarget() {
		Vector3 hitPoint = Vector3.zero;

		Vector3 rayDirection = CurrentTarget.position - LaserFireEmitPointTrans.position;
		myray = new Ray(LaserFireEmitPointTrans.position, rayDirection);
		if (Physics.Raycast(myray, out myhit, 1000.0f)) {
//			print(myhit.collider.name);
			if (canDamageTarget) {
				myhit.collider.gameObject.transform.root.SendMessage("Damage", LaserDamagePerSecond, SendMessageOptions.DontRequireReceiver);
				canDamageTarget = false;
			}
			hitPoint = myhit.point;
		}

		return hitPoint;
	}

}
