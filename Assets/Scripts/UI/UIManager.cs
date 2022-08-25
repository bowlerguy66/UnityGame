using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour {

	public GameObject gameplayCanvas;

    public static UIManager Instance { get; private set; }
    public static event Action<bool> UIModeChange;

    private Canvas[] uiCanvases;
	private GameObject[] mainCanvases;
   // Controls if the mouse is controlling the game or not
    private bool uiMode;
	private GameObject shownCanvas;

    private void Awake() {

        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }

        uiMode = true;
        uiCanvases = FindObjectsOfType<Canvas>();
		mainCanvases = GameObject.FindGameObjectsWithTag("MainCanvas");

		foreach (GameObject obj in mainCanvases) {
			obj.GetComponent<Canvas>().enabled = true;
			if (obj.name == "Canvas_Pause") continue;
			obj.SetActive(false);
		}
	}

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            setUIMode(true);
        }
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
			showUI(gameplayCanvas);
			setUIMode(false);
        }
	}

	public void showUI(GameObject obj) {
		if(obj.tag != "MainCanvas") {
			Debug.LogError($"Can't use this canvas ({obj.name}) in showUI because it's not a MainCanvas.");
			return;
		}
		shownCanvas = obj;
		obj.SetActive(true);
		foreach (GameObject targObj in mainCanvases) {
			Canvas canv = targObj.GetComponent<Canvas>();
			if (obj.Equals(targObj)) continue;
			targObj.SetActive(false);
		}
	}

    /// <summary>
    /// Controls if the mouse is controlling the game or using UI
    /// </summary>
    public void setUIMode(bool uiMode) { 
        this.uiMode = uiMode;
        foreach (Canvas canv in uiCanvases) {
			CanvasGroup group = canv.GetComponent<CanvasGroup>();
			if (group == null) group = canv.GetComponentInParent<CanvasGroup>();
			if (group == null) {
                Debug.LogWarning("Found a canvas without a group (name: " + canv.name + ")");
                continue;
            }
            group.interactable = uiMode;
        }
        UIModeChange?.Invoke(uiMode);
    }

    public bool inUIMode() {
        return uiMode;    
    }

	public GameObject getShownCanvas() {
		return shownCanvas;
	}

}
