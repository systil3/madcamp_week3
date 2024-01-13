using UnityEngine;
using System.Collections;

public enum DemoSceneTypes {
	MainMenu,
	TowerDefenseDemo,
	LaserBeamsDemo
}

public class LBWSP_DemoGUI : MonoBehaviour {

	public DemoSceneTypes DemoSceneType = DemoSceneTypes.MainMenu;

	// Use this for initialization
	void Start () {
	
	}

	public string packageNameLabel = "Laser Beams - Weapon Systems Pack ";
	public string versionLabel = " v1.25";
	public string packageCreatorLabel = "Created by, Daniel Kole Productions";
	
	void OnGUI () {
		GUI.Label(new Rect(10, 10, 250, 50), packageNameLabel + versionLabel + "\n" + packageCreatorLabel);
		if (DemoSceneType == DemoSceneTypes.TowerDefenseDemo) {
			GUI.Label(new Rect(10, 60, 250, 20), DemoGUIManager.GlobalAccess.TurretLabel);
			GUI.Label(new Rect(10, 70, 250, 20), DemoGUIManager.GlobalAccess.TurretDamageLabel);
		}

		if (DemoSceneType == DemoSceneTypes.MainMenu) {
			if (GUI.Button(new Rect(10, 90, 180, 30), "Laser Beams Demo")) {
				// Load Laser Beams Demo Scene
				UnityEngine.SceneManagement.SceneManager.LoadScene(1);
			}
			if (GUI.Button(new Rect(10, 120, 180, 30), "Tower Defense Demo")) {
				// Load Tower Defense Demo Scene
				UnityEngine.SceneManagement.SceneManager.LoadScene(2);
			}
		}
		else if (DemoSceneType == DemoSceneTypes.TowerDefenseDemo) {
			if (GUI.Button(new Rect(10, 90, 180, 30), "Change Camera")) {
				// Change to Next Camera
				WSP_DemoController.GlobalAccess.ChangeCamera();
			}
			if (GUI.Button(new Rect(10, 120, 180, 30), "Change Turret")) {
				// Change to Next Camera
				WSP_DemoController.GlobalAccess.ChangeTurret();
			}
			if (GUI.Button(new Rect(10, 150, 180, 30), "Upgrade Turret")) {
				// Upgrade Turret
				WSP_DemoController.GlobalAccess.UpgradeTurret();
			}
			if (GUI.Button(new Rect(10, 180, 180, 30), "Main Menu")) {
				// Load Main Menu Scene
				UnityEngine.SceneManagement.SceneManager.LoadScene(0);
			}
		}
		else if (DemoSceneType == DemoSceneTypes.LaserBeamsDemo) {
			if (GUI.Button(new Rect(10, 60, 180, 30), "Toggle AutoFiring")) {
				// Toggle Auto Firing in Laser Beams Demo
				WSP_LaserBeamsDemoMng.GlobalAccess.ToggleAutofiring();
			}
			if (GUI.Button(new Rect(10, 90, 180, 30), "Main Menu")) {
				// Load Main Menu Scene
				UnityEngine.SceneManagement.SceneManager.LoadScene(0);
			}
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
