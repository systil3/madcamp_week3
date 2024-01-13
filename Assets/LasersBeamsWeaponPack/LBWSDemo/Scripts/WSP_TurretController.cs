using UnityEngine;
using System.Collections;

public class WSP_TurretController : MonoBehaviour {

	public int TurretNumber = 0;
	public bool Selected = false;
	public Renderer SelectedRenderer;
	public Material IsSelectedMaterial;
	public Material NotSelectedMaterial;
	public void Select() {
		if (SelectedRenderer != null) {
			SelectedRenderer.material = IsSelectedMaterial;
		}
		Selected = true;
	}
	public void Deselect() {
		if (SelectedRenderer != null) {
			SelectedRenderer.material = NotSelectedMaterial;
		}
		Selected = false;
	}

	public LaserLevelController LevelController;
	public float MinimumTargetDistance = 10;

	public bool CanRotate = false;
	public Transform YRotationTransform;
	public Transform XRotationTransform;

	public float TargetAngle = 0;
	public float DirectionAngle = 0;

	public float TargetXAngle = 0;
	public float CurrentXAngle = 0;
	public float DifferenceToTargetAngle = 0;
	private Vector3 XRotationEulers = Vector3.zero;
	public float DifferenceFromTargetXAngle = 0;

	public bool AimingAtTarget = false;
	public float TurretRotationSpeed = 25;
	public Transform CurrentTargetTransform;

	public WSP_LaserBeamWS LaserBeamWeaponSystem;
	public bool HasLaserBeamWeapon = false;

	void Awake () {
		Deselect();
	}

	// Use this for initialization
	void Start () {

		// Check Laser Beam Weapon System
		LaserBeamWeaponSystem = gameObject.GetComponent<WSP_LaserBeamWS>();
		if (LaserBeamWeaponSystem != null) {
			HasLaserBeamWeapon = true;
		}
		else {
			HasLaserBeamWeapon = false;
		}

		// Laser Level Controller
		LevelController = gameObject.GetComponent<LaserLevelController>();

		// Add To Weapon System Turret List
		TurretNumber = WSP_DemoController.GlobalAccess.AddLaserBeamTurretToList(this);
	}
	
	// Update is called once per frame
	void Update () {
		if (HasLaserBeamWeapon) {
			if (LaserBeamWeaponSystem.LaserBeamActive) {
				CanRotate = false;
			}
			else {
				CanRotate = true;
			}
		}

		if (CurrentTargetTransform != null) {
			float distanceToTarget = Vector3.Distance(CurrentTargetTransform.position, YRotationTransform.position);
			if (distanceToTarget < MinimumTargetDistance) {
				CurrentTargetTransform = null;
				if (HasLaserBeamWeapon) {
					if (LaserBeamWeaponSystem.LaserFiring)
						LaserBeamWeaponSystem.StopLaserFire();
					LaserBeamWeaponSystem.CurrentTarget = null;
				}
			}
			else {
				if (CanRotate) {
					TrackPosition(CurrentTargetTransform.position);
					// Rotate Turret
					XRotationEulers = new Vector3(TargetXAngle, YRotationTransform.rotation.eulerAngles.y, 0);
					XRotationTransform.rotation = Quaternion.Euler(XRotationEulers);
				}

				// Fire Laser Beam
				if (AimingAtTarget) {
					if (HasLaserBeamWeapon) {
						if (LaserBeamWeaponSystem.CurrentTarget != CurrentTargetTransform)
							LaserBeamWeaponSystem.CurrentTarget = CurrentTargetTransform;
						LaserBeamWeaponSystem.FireLaser();
					}
				}
			}
		}
		else {
			// Get Random Target
			Transform newTargetTransform = WSP_TargetManager.GlobalAccess.GetRandomTargetFromList(YRotationTransform.position, MinimumTargetDistance * 2);
			if (newTargetTransform != null) {
				CurrentTargetTransform = newTargetTransform;
			}
		}
	}

	private void TrackPosition(Vector3 positionToTrack) {
		// Track Target
		Vector3 targetPosition = positionToTrack;				
		Vector3 v = targetPosition - YRotationTransform.forward;
		float DirectionAngle = Mathf.Atan2(v.x, v.z) * Mathf.Rad2Deg;
//		float DirectionXAngle = Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg;
		
		// Difference From Target Angle
		Vector3 angleToTarget = targetPosition - YRotationTransform.position;
		float DifferenceFromTargetAngle = Vector3.Angle(YRotationTransform.forward, angleToTarget);
		Vector3 cross = Vector3.Cross(YRotationTransform.forward, angleToTarget);
		if (cross.y < 0) DifferenceFromTargetAngle = -DifferenceFromTargetAngle;
		
//		Quaternion lookRot = Quaternion.LookRotation(angleToTarget);
//		LookDirection = lookRot.eulerAngles;
		
		DirectionAngle = Vector3.Angle(angleToTarget, YRotationTransform.forward);
		cross = Vector3.Cross(angleToTarget, YRotationTransform.forward);
		if (cross.y < 0) DirectionAngle = -DirectionAngle;
		
		TargetAngle = Mathf.Lerp(TargetAngle, DirectionAngle, (TurretRotationSpeed / 2) * Time.deltaTime);

		DifferenceToTargetAngle = Mathf.Abs(TargetAngle) - Mathf.Abs(DirectionAngle);

		if (TargetAngle < -6) {
			YRotationTransform.RotateAround(YRotationTransform.position, YRotationTransform.up, TurretRotationSpeed * Time.deltaTime);
			AimingAtTarget = false;
		}
		if (TargetAngle > 6) {
			YRotationTransform.RotateAround(YRotationTransform.position, YRotationTransform.up, -TurretRotationSpeed * Time.deltaTime);
			AimingAtTarget = false;
		}
		if (TargetAngle > -6 && TargetAngle < 6) {
			AimingAtTarget = true;
		}
		
		float DifferenceFromTargetXAngle = Vector3.Angle(angleToTarget, YRotationTransform.up);
		cross = Vector3.Cross(angleToTarget, YRotationTransform.up);
		if (cross.y < 0) {
			DifferenceFromTargetXAngle = -DifferenceFromTargetXAngle;
		}

		float fixedXAngle = DifferenceFromTargetXAngle - 180;
		DifferenceFromTargetXAngle = Mathf.Abs(fixedXAngle) - Mathf.Abs(TargetXAngle);
		TargetXAngle = Mathf.LerpAngle(TargetXAngle, fixedXAngle, (TurretRotationSpeed / 4) * Time.deltaTime);

		CurrentXAngle = fixedXAngle;
	}
}
