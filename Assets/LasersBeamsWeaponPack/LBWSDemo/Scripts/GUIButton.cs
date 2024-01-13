using UnityEngine;
using System.Collections;

public enum GUIButtonTypes {
	NotSet,
	UpgradeTurret,
	ChangeCamera,
	ChangeTurret,
	ToggleAutofiring,
	LaserBeamsDemo,
	TowerDefenseDemo,
	MainMenu
}

public class GUIButton : MonoBehaviour {

	public GUIButtonTypes ButtonType = GUIButtonTypes.NotSet;

	public bool CanClick = false;
	private float clickTimer = 0;
	private float clickTimerFreq = 1.0f;

	void Start () {
		CanClick = true;
	}

	void Update() {
		if (!CanClick) {
			if (clickTimer < clickTimerFreq) {
				clickTimer += Time.deltaTime;
			}
			else {
				clickTimer = 0;
				CanClick = true;
			}
		}

		if (LastPress) {
			if (CanClick) {
				if (ButtonType == GUIButtonTypes.NotSet) {
					// Do Nothing
				}
				else if (ButtonType == GUIButtonTypes.UpgradeTurret) {
					// Upgrade Turret
					WSP_DemoController.GlobalAccess.UpgradeTurret();
				}
				else if (ButtonType == GUIButtonTypes.ChangeCamera) {
					// Change to Next Camera
					WSP_DemoController.GlobalAccess.ChangeCamera();
				}
				else if (ButtonType == GUIButtonTypes.ChangeTurret) {
					// Change to Next Camera
					WSP_DemoController.GlobalAccess.ChangeTurret();
				}
				else if (ButtonType == GUIButtonTypes.ToggleAutofiring) {
					// Toggle Auto Firing in Laser Beams Demo
					WSP_LaserBeamsDemoMng.GlobalAccess.ToggleAutofiring();
				}
				else if (ButtonType == GUIButtonTypes.LaserBeamsDemo) {
					// Load Laser Beams Demo Scene
					UnityEngine.SceneManagement.SceneManager.LoadScene(1);
				}
				else if (ButtonType == GUIButtonTypes.TowerDefenseDemo) {
					// Load Tower Defense Demo Scene
					UnityEngine.SceneManagement.SceneManager.LoadScene(2);
				}
				else if (ButtonType == GUIButtonTypes.MainMenu) {
					// Load Main Menu Scene
					UnityEngine.SceneManagement.SceneManager.LoadScene(0);
				}
				LastPress = false;
				clickTimer = 0;
				CanClick = false;
			}
		}
	}

	public bool LastPress = false;

	void OnClick()
	{
		LastPress = true;
	}
}
