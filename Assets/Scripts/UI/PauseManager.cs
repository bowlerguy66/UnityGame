using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour {

	public Button resumeButton;

	public GameObject gameplayCanvas;

	private void Start() {
		resumeButton.onClick.AddListener(resumeClicked);
	}

	public void resumeClicked() {

		UIManager.Instance.showUI(gameplayCanvas);
		UIManager.Instance.setUIMode(false);

	}

}
