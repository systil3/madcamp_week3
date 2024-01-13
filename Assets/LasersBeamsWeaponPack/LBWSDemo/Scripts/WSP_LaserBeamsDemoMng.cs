using UnityEngine;
using System.Collections;

public class WSP_LaserBeamsDemoMng : MonoBehaviour {
	public static WSP_LaserBeamsDemoMng GlobalAccess;
	void Awake () {
		GlobalAccess = this;
		ToggleAutofiring();
	}

	public bool AutoFireLasers = false;
	public bool HoldLaserFire = false;
	public WSP_LaserBeamWS[] LaserBeams;

	public string KeyboardFireLabel;
	public string AutofiringButtonLabel;
	public string AutofiringLabel;

	public GameObject[] Blockers;

	// Use this for initialization
	void Start () {
	}

	public void ToggleAutofiring() {
		AutoFireLasers = !AutoFireLasers;
		foreach (WSP_LaserBeamWS laserBeam in LaserBeams) {
			laserBeam.AutoFireForDemo = AutoFireLasers;
		}
	}

	private void SetHoldLaserFire() {
		foreach (WSP_LaserBeamWS laserBeam in LaserBeams) {
			laserBeam.UseExtendedLength = true;
		}
		foreach (GameObject blocker in Blockers) {
			blocker.SetActive(false);
		}
	}

	// Update is called once per frame
	void Update () {
		if (HoldLaserFire) {
			SetHoldLaserFire();
			HoldLaserFire = false;
		}

//		if (KeyboardFireLabel != null) {
//			if (AutoFireLasers) {
//				if (KeyboardFireLabel.enabled)
//					KeyboardFireLabel.enabled = false;
//			}
//			else {
//				if (!KeyboardFireLabel.enabled)
//					KeyboardFireLabel.enabled = true;
//			}
//		}
		if (AutofiringLabel != null) {
			if (AutoFireLasers) {
				if (AutofiringLabel != "Auto Firing: On")
					AutofiringLabel = "Auto Firing: On";
			}
			else {
				if (AutofiringLabel != "Auto Firing: Off")
					AutofiringLabel = "Auto Firing: Off";
			}
		}
		if (AutofiringButtonLabel != null) {
			if (AutoFireLasers) {
				if (AutofiringButtonLabel != "Auto Firing: On")
					AutofiringButtonLabel = "Auto Firing: On";
			}
			else {
				if (AutofiringButtonLabel != "Auto Firing: Off")
					AutofiringButtonLabel = "Auto Firing: Off";
			}
		}

		if (!AutoFireLasers) {
			if (Input.GetKeyUp(KeyCode.Alpha1)) {
				LaserBeams[0].FireLaser();
			}
			if (Input.GetKeyUp(KeyCode.Alpha2)) {
				LaserBeams[1].FireLaser();
			}
			if (Input.GetKeyUp(KeyCode.Alpha3)) {
				LaserBeams[2].FireLaser();
			}
			if (Input.GetKeyUp(KeyCode.Alpha4)) {
				LaserBeams[3].FireLaser();
			}
			if (Input.GetKeyUp(KeyCode.Alpha5)) {
				LaserBeams[4].FireLaser();
			}
			if (Input.GetKeyUp(KeyCode.Alpha6)) {
				LaserBeams[5].FireLaser();
			}
			if (Input.GetKeyUp(KeyCode.Alpha7)) {
				LaserBeams[6].FireLaser();
			}
			if (Input.GetKeyUp(KeyCode.Alpha8)) {
				LaserBeams[7].FireLaser();
			}
			if (Input.GetKeyUp(KeyCode.Alpha9)) {
				LaserBeams[8].FireLaser();
			}
			if (Input.GetKeyUp(KeyCode.Alpha0)) {
				LaserBeams[9].FireLaser();
			}
		}
	}
}
