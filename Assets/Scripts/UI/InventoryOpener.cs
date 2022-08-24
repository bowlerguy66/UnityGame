using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryOpener : MonoBehaviour {

	public GameObject gameplayCanvas;
	public GameObject inventoryCanvas;
	public GameObject pauseCanvas;

	private void Update() {

		if (Input.GetKeyDown(KeyCode.Tab)) {
			bool invShowing = inventoryCanvas.gameObject.activeSelf;
			invShowing = !invShowing;
			if (invShowing) {
				showInventory();
			} else {
				showGameplay();
			}

		}

		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (inventoryCanvas.gameObject.activeSelf || pauseCanvas.gameObject.activeSelf) {
				showGameplay();
			} else {
				UIManager.Instance.showUI(pauseCanvas);
				UIManager.Instance.setUIMode(true);
			}
		}

	}

	private void showGameplay() {
		UIManager.Instance.showUI(gameplayCanvas);
		UIManager.Instance.setUIMode(false);
		gameplayCanvas.GetComponent<GameplayInventoryManager>().SyncInventory();
	}

	private void showInventory() {
		UIManager.Instance.showUI(inventoryCanvas);
		UIManager.Instance.setUIMode(true);
		inventoryCanvas.GetComponent<InventoryManager>().SyncInventory();
	}

}
