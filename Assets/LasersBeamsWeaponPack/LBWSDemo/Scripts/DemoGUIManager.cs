using UnityEngine;
using System.Collections;

public class DemoGUIManager : MonoBehaviour {
	public static DemoGUIManager GlobalAccess;
	void Awake () {
		GlobalAccess = this;
	}

	public bool GUIHidden = false;
	public GameObject GUIGameObject;
	public string TurretLabel;
	public string TurretDamageLabel;

	// Use this for initialization
	void Start () {
		// Update GUI At Start
		UpdateGUI = true;
		GUIHidden = false;
		if (GUIGameObject != null)
			GUIGameObject.SetActive(true);
	}

	public bool UpdateGUI = false;

	// Update is called once per frame
	void Update () {
		// Hide/Show GUI
		if (GUIGameObject != null) {
			if (Input.GetKeyUp(KeyCode.F1)) {
				if (GUIHidden) {
					// Show GUI
					GUIGameObject.SetActive(true);
					GUIHidden = false;
				}
				else {
					// Hide GUI
					GUIGameObject.SetActive(false);
					GUIHidden = true;
				}
			}
		}

		// Update GUI
		if (UpdateGUI) {
			UpdateGUINow();
			UpdateGUI = false;
		}
	}

	private void UpdateGUINow() {
		if (WSP_DemoController.GlobalAccess.CurrentlySelectedTurret != null) {
			if (TurretLabel != null) {
				int laserBeamLevel = WSP_DemoController.GlobalAccess.CurrentlySelectedTurret.LevelController.laserBeamLevelNum + 1;
				int turretNum = WSP_DemoController.GlobalAccess.CurrentlySelectedTurret.TurretNumber;
				TurretLabel = "#" + turretNum.ToString() + ") Laser Beam Turret - (Level " + laserBeamLevel.ToString() + "):";
			}
			if (TurretDamageLabel != null) {
				int damageLevel = (int)WSP_DemoController.GlobalAccess.CurrentlySelectedTurret.LaserBeamWeaponSystem.LaserDamage;
				TurretDamageLabel = "Damage: " + damageLevel.ToString("###,##0");
			}
		}
	}
}
