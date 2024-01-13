using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CameraTypes {
	IsometricCamera,
	RearCamera,
	FrontCamera,
	TurretCamera
}

public class WSP_DemoController : MonoBehaviour {
	public static WSP_DemoController GlobalAccess;
	void Awake () {
		GlobalAccess = this;
		LaserBeamTurrets = new List<WSP_TurretController>();
	}

	public CameraTypes CurrentCameraType = CameraTypes.RearCamera;
	public Camera IsometricCamera;
	public Camera RearCamera;
	public Camera FrontCamera;
	public Camera TurretCamera;

	public List<WSP_TurretController> LaserBeamTurrets;
	public int AddLaserBeamTurretToList(WSP_TurretController laserBeamSystem) {
		Debug.Log("Adding Turret To List.");
		LaserBeamTurrets.Add(laserBeamSystem);
		return LaserBeamTurrets.Count;
	}

	public bool DemoStarted = false;

	// Demo Target Variables
	public float DemoTargetLevel = 1;
	public float DemoTargetSpeed = 2.0f;
	public float DemoTargetHealthMAX = 10;

	// Use this for initialization
	void Start () {
		DemoStarted = true;
		GoToCamera(CameraTypes.RearCamera);
	}

	public WSP_TurretController CurrentlySelectedTurret;

	public void UpgradeTurret() {
		Debug.Log("Upgrading Turret!");
		if (CurrentlySelectedTurret != null) {
			CurrentlySelectedTurret.LevelController.CycleLevel();
		}
	}

	public void GoToCamera(CameraTypes cameraToChangeTo) {
		// Change To Specific Camera
		if (cameraToChangeTo == CameraTypes.RearCamera) {
			RearCamera.GetComponent<Camera>().enabled = true;
			IsometricCamera.GetComponent<Camera>().enabled = false;
			FrontCamera.GetComponent<Camera>().enabled = false;
			CurrentCameraType = CameraTypes.RearCamera;
		}
		else if (cameraToChangeTo == CameraTypes.IsometricCamera) {
			IsometricCamera.GetComponent<Camera>().enabled = true;
			RearCamera.GetComponent<Camera>().enabled = false;
			FrontCamera.GetComponent<Camera>().enabled = false;
			CurrentCameraType = CameraTypes.IsometricCamera;
		}
		else if (cameraToChangeTo == CameraTypes.FrontCamera) {
			IsometricCamera.GetComponent<Camera>().enabled = false;
			RearCamera.GetComponent<Camera>().enabled = false;
			FrontCamera.GetComponent<Camera>().enabled = true;
			CurrentCameraType = CameraTypes.FrontCamera;
		}
		else if (cameraToChangeTo == CameraTypes.TurretCamera) {
			IsometricCamera.GetComponent<Camera>().enabled = false;
			RearCamera.GetComponent<Camera>().enabled = false;
			FrontCamera.GetComponent<Camera>().enabled = false;
			TurretCamera.GetComponent<Camera>().enabled = true;
			CurrentCameraType = CameraTypes.TurretCamera;
		}
	}

	public void ChangeCamera() {
		// Cycle Through Cameras
		Debug.Log("Changing Camera!");
		if (CurrentCameraType == CameraTypes.RearCamera) {
			GoToCamera(CameraTypes.IsometricCamera);
		}
		else if (CurrentCameraType == CameraTypes.IsometricCamera) {
			GoToCamera(CameraTypes.FrontCamera);
		}
		else if (CurrentCameraType == CameraTypes.FrontCamera) {
			GoToCamera(CameraTypes.TurretCamera);
		}
		else if (CurrentCameraType == CameraTypes.TurretCamera) {
			GoToCamera(CameraTypes.RearCamera);
		}
	}

	public void ChangeTurret() {
		if (TurretIndex + 1 < LaserBeamTurrets.Count) {
			TurretIndex++;
		}
		else {
			TurretIndex = 0;
		}
		CurrentlySelectedTurret = LaserBeamTurrets[TurretIndex];
		CurrentlySelectedTurret.Select();
		foreach (WSP_TurretController turret in LaserBeamTurrets) {
			if (turret != CurrentlySelectedTurret) {
				turret.Deselect();
			}
		}
		DemoGUIManager.GlobalAccess.UpdateGUI = true;
	}

	public int TurretIndex = 0;

	public int GetCombinedTurretLevel() {
		int combinedTurretLevel = 1;

		if (LaserBeamTurrets.Count > 0) {
			combinedTurretLevel = 0;
			foreach (WSP_TurretController turret in LaserBeamTurrets) {
				combinedTurretLevel += ((int)turret.LevelController.CurrentLaserBeamLevel + 1);
			}
		}

		return combinedTurretLevel;
	}

	// Update is called once per frame
	void Update () {
		if (LaserBeamTurrets.Count > 0) {
			if (CurrentlySelectedTurret == null) {
				CurrentlySelectedTurret = LaserBeamTurrets[TurretIndex];
				CurrentlySelectedTurret.Select();
				DemoGUIManager.GlobalAccess.UpdateGUI = true;
			}
		}

		if (Input.GetKeyUp(KeyCode.UpArrow)) {
			foreach(WSP_TurretController control in LaserBeamTurrets) {
				control.LevelController.CycleLevel();
			}
		}
		if (Input.GetKeyUp(KeyCode.Alpha1)) {
			GoToCamera(CameraTypes.RearCamera);
		}
		if (Input.GetKeyUp(KeyCode.Alpha2)) {
			GoToCamera(CameraTypes.IsometricCamera);
		}
		if (Input.GetKeyUp(KeyCode.Alpha3)) {
			GoToCamera(CameraTypes.FrontCamera);
		}
		if (Input.GetKeyUp(KeyCode.Alpha4)) {
			GoToCamera(CameraTypes.TurretCamera);
		}
	}
}
