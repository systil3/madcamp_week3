using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum LaserBeamLevelTypes {
	Level1,
	Level2,
	Level3,
	Level4,
	Level5,
	Level6,
	Level7,
	Level8,
	Level9,
	Level10
}

public class LaserLevelController : MonoBehaviour {

	public LaserBeamLevelTypes CurrentLaserBeamLevel = LaserBeamLevelTypes.Level1;
	private LaserBeamLevelTypes activeLaserBeamLevel = LaserBeamLevelTypes.Level1;
	public int laserBeamLevelNum = 0;

	private WSP_LaserBeamWS LaserBeamWSScript;
	private AudioSource LaserBeamFiringSFX;
	private bool HasLaserBeamWS = false;

	public Color[] LaserBeamLevelColors;
	public Material[] LBStartPartMaterials;
	public Material[] LBTargetHitMaterials;
	public float[] LBTargetHitSizes;
	public Material[] OuterLaserBeamMaterials;
	public Material[] InnerLaserBeamMaterials;
	public AudioClip[] LaserFireSoundFXs;

	// Use this for initialization
	void Start () {
		LaserBeamWSScript = gameObject.GetComponent<WSP_LaserBeamWS>();
		LaserBeamFiringSFX = gameObject.GetComponent<AudioSource>();
		if (LaserBeamWSScript != null) {
			HasLaserBeamWS = true;
		}
		if (HasLaserBeamWS) {
			UpdateLaserBeamLevel();
		}
	}

	void Update () {
		if (activeLaserBeamLevel != CurrentLaserBeamLevel) {
			activeLaserBeamLevel = CurrentLaserBeamLevel;
			laserBeamLevelNum = (int)activeLaserBeamLevel;
			UpdateLaserBeamLevel();
			if (DemoGUIManager.GlobalAccess != null)
				DemoGUIManager.GlobalAccess.UpdateGUI = true;
		}
	}

	public void CycleLevel() {
		Debug.Log("Cycling Level!");
		if (CurrentLaserBeamLevel == LaserBeamLevelTypes.Level1)
			CurrentLaserBeamLevel = LaserBeamLevelTypes.Level2;
		else if (CurrentLaserBeamLevel == LaserBeamLevelTypes.Level2)
			CurrentLaserBeamLevel = LaserBeamLevelTypes.Level3;
		else if (CurrentLaserBeamLevel == LaserBeamLevelTypes.Level3)
			CurrentLaserBeamLevel = LaserBeamLevelTypes.Level4;
		else if (CurrentLaserBeamLevel == LaserBeamLevelTypes.Level4)
			CurrentLaserBeamLevel = LaserBeamLevelTypes.Level5;
		else if (CurrentLaserBeamLevel == LaserBeamLevelTypes.Level5)
			CurrentLaserBeamLevel = LaserBeamLevelTypes.Level6;	
		else if (CurrentLaserBeamLevel == LaserBeamLevelTypes.Level6)
			CurrentLaserBeamLevel = LaserBeamLevelTypes.Level7;
		else if (CurrentLaserBeamLevel == LaserBeamLevelTypes.Level7)
			CurrentLaserBeamLevel = LaserBeamLevelTypes.Level8;
		else if (CurrentLaserBeamLevel == LaserBeamLevelTypes.Level8)
			CurrentLaserBeamLevel = LaserBeamLevelTypes.Level9;
		else if (CurrentLaserBeamLevel == LaserBeamLevelTypes.Level9)
			CurrentLaserBeamLevel = LaserBeamLevelTypes.Level10;
		else if (CurrentLaserBeamLevel == LaserBeamLevelTypes.Level10)
			CurrentLaserBeamLevel = LaserBeamLevelTypes.Level1;
	}

	public void UpdateLaserBeamLevel() {
		if (HasLaserBeamWS) {
			// Update Laser Firing SoundFX
			if (LaserBeamFiringSFX != null) {
				LaserBeamFiringSFX.clip = LaserFireSoundFXs[laserBeamLevelNum];
			}

			// Set the Outer Laser Beam Materials
			LaserBeamWSScript.OuterLaserLineRender.material = OuterLaserBeamMaterials[laserBeamLevelNum];

			// Set the Inner Laser Beam Materials
			LaserBeamWSScript.InnerLaserLineRender.material = InnerLaserBeamMaterials[laserBeamLevelNum];

			// Set Laser Beam Grow Speed
			float growSpeed = 0.75f + (0.25f * laserBeamLevelNum);
			LaserBeamWSScript.LaserGrowSpeed = growSpeed;

			// Set Inner and Outer Tile Amounts
			float innerTileAmount = 1 + laserBeamLevelNum;
			LaserBeamWSScript.innerLaserTileAmount = innerTileAmount;
			float outerTileAmount = 1 + laserBeamLevelNum;
			LaserBeamWSScript.outerLaserTileAmount = outerTileAmount;
			LaserBeamWSScript.UpdateLaserBeamTiling();

			// Update Laser Beam Recharge Speed by Level Number
			LaserBeamWSScript.LaserRechargeTime = 1.0f - (0.025f * laserBeamLevelNum);

			// Update Laser Beam Color Based on Level Number
			LaserBeamWSScript.LaserBeamColor = LaserBeamLevelColors[laserBeamLevelNum];
			if (LaserBeamWSScript.TurretColorRender != null)
				LaserBeamWSScript.TurretColorRender.material.color = LaserBeamLevelColors[laserBeamLevelNum];

			// Update Firing and Hit Particle Size Based On Level
			float firingParticleSize = 1.25f + (0.1f * laserBeamLevelNum);
			LaserBeamWSScript.FiringParticleSize = firingParticleSize;
			float hitParticleSize = 2.0f + LBTargetHitSizes[laserBeamLevelNum];
			LaserBeamWSScript.HitParticleSize = hitParticleSize;

			// Update Level Color Indicators
			LaserBeamWSScript.UpdateLevelIndicators(laserBeamLevelNum, LaserBeamLevelColors[laserBeamLevelNum]);

			// Update Firing Particle Materials Based on Laser Beam Level
			LaserBeamWSScript.LaserFireCenterEmitter.GetComponent<Renderer>().material = LBStartPartMaterials[laserBeamLevelNum];
			ParticleSystem.MainModule mainModule = LaserBeamWSScript.LaserFireCenterEmitter.main;
			mainModule.startSpeed = 1 + (0.25f * laserBeamLevelNum);

			// Update Laser Hit Particle Materials Based on Laser Beam Level
			LaserBeamWSScript.TargetHitEmitter.GetComponent<Renderer>().material = LBTargetHitMaterials[laserBeamLevelNum];
			ParticleSystem.MainModule hitEmitterModule = LaserBeamWSScript.TargetHitEmitter.main;
			hitEmitterModule.startSize = 2 + (0.5f * laserBeamLevelNum);

			// Update Beam Width Based on Laser Level Num
			float outerWidth = 0.65f;// + (0.025f * laserBeamLevelNum);
			float innerWidth = 0.5f + (0.05f * laserBeamLevelNum);
			LaserBeamWSScript.OuterLaserStartWidth = outerWidth;
			LaserBeamWSScript.OuterLaserEndWidth = outerWidth;
			LaserBeamWSScript.InnerLaserStartWidth = innerWidth;
			LaserBeamWSScript.InnerLaserEndWidth = innerWidth;

			// Update Beam Damage Based on Laser Level Num
			float laserBeamDamage = 5 + (5 * laserBeamLevelNum);
			float laserBeamDamageOverTime = 2 + (2 * laserBeamLevelNum);
			LaserBeamWSScript.LaserDamage = laserBeamDamage;
			LaserBeamWSScript.LaserDamagePerSecond = laserBeamDamageOverTime;

			if (CurrentLaserBeamLevel == LaserBeamLevelTypes.Level1) {
				// Change Laser Beam Graphics


				// Change Laser Beam Stats
				LaserBeamWSScript.LaserDamage = 10;
			}
		}
	}
}
