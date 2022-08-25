using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayInventoryManager : MonoBehaviour {

	public static int SHOWN_SLOTS = 3;
	public static int STARTING_SLOT = 24;
	public static string[] SHOWN_SLOT_NAMES = {"Left Hand", "Right Hand", "Back"};

	public GameObject[] shownSlots;
	public GameObject itemObjectPrefab;

	private PlayerInventory inventory;
	private GameObject[] itemObjects;

	private void Awake() {
		loadItemObjects();
		FirstObjectNotifier.OnFirstObjectSpawned += FirstObjectNotifier_OnFirstObjectSpawned;
	}

	private void OnDestroy() {
		FirstObjectNotifier.OnFirstObjectSpawned -= FirstObjectNotifier_OnFirstObjectSpawned;
	}

	private void FirstObjectNotifier_OnFirstObjectSpawned(GameObject player) {
		UpdatePlayer(player.GetComponent<PlayerInventory>());
	}

	public void UpdatePlayer(PlayerInventory inventory) {
		this.inventory = inventory;
		SyncInventory();
	}

	public void loadItemObjects() {
		itemObjects = new GameObject[SHOWN_SLOTS];
		for (int i = 0; i < SHOWN_SLOTS; i++) {
			itemObjects[i] = createItemObj(SHOWN_SLOT_NAMES[i], STARTING_SLOT + i, shownSlots[i]);
		}
	}

	// Called by InventoryOpener every time the gameplay canvas is shown
	public void SyncInventory() {
		if (inventory == null) return;
		for (int i = 0; i < SHOWN_SLOTS; i++) {
			GameObject itemObject = itemObjects[i];
			int invSlotNumber = STARTING_SLOT + i;
			ItemInventory script = itemObject.GetComponent<ItemInventory>();
			script.setItem(inventory.getItem(invSlotNumber));
		}
	}

	private GameObject createItemObj(string slotName, int slotNumber, GameObject hostSlot) {

		GameObject itemObj = Instantiate(itemObjectPrefab, gameObject.transform);
		itemObj.name = "Slot Item " + slotName;

		Image img = hostSlot.GetComponent<Image>();
		RectTransform transform = itemObj.GetComponent<RectTransform>();
		transform.sizeDelta = new Vector2(img.rectTransform.rect.width * 0.8f, img.rectTransform.rect.height * 0.8f);
		transform.pivot = new Vector3(0, 1, 0);

		float xOff = (hostSlot.GetComponent<RectTransform>().rect.width / 2) - (itemObj.GetComponent<RectTransform>().rect.width / 2);
		float yOff = (hostSlot.GetComponent<RectTransform>().rect.height / 2) - (itemObj.GetComponent<RectTransform>().rect.height / 2);
		itemObj.GetComponent<RectTransform>().position = hostSlot.GetComponent<RectTransform>().position + new Vector3(xOff, -yOff, 0);

		ItemInventory script = itemObj.GetComponent<ItemInventory>();
		script.setSlotNumber(slotNumber);
		
		return itemObj;
	}

}
