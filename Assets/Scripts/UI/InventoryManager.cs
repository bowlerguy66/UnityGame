using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {

	public WorldManager worldManager;
	public GameObject itemObjectPrefab;
	public List<GameObject> itemSlots;
	public GameObject itemSlotsParent;

	private GameObject player;
	private PlayerInventory inventory;

	private Dictionary<int, GameObject> itemObjects;
	private float itemWidth = 104;
	private float itemHeight = 104;

	private bool syncFlag;

	private void Update() {

		if (syncFlag) { 
			SyncInventoryHelper();
			syncFlag = false;
		}

		if (Input.GetKeyDown(KeyCode.G) && EventSystem.current.IsPointerOverGameObject()) {
			PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
			eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
			foreach (RaycastResult result in results) {
				ItemInventory script = result.gameObject.GetComponent<ItemInventory>();
				if (script == null) continue;
				Item item = script.getItem();
				Item toDrop = script.getItem().clone();
				int slotNum = script.getSlotNumber();
				if (item.getCount() > 1) {
					inventory.getItem(slotNum).addCount(-1);
				} else {
					deleteItemObject(script.getSlotNumber());
					inventory.setItem(slotNum, null);
				}
				Debug.Log("sending over: " + toDrop.ToString());
				worldManager.DropItem(toDrop, player.transform.position + new Vector3(0, 1, 0));
			}
		}

	}

	private void Awake() {
		FirstObjectNotifier.OnFirstObjectSpawned += FirstObjectNotifier_OnFirstObjectSpawned;
		PlayerInventory.InventoryChange += PlayerInventory_InventoryChange;
	}

	private void OnDestroy() {
		FirstObjectNotifier.OnFirstObjectSpawned -= FirstObjectNotifier_OnFirstObjectSpawned;
		PlayerInventory.InventoryChange -= PlayerInventory_InventoryChange;
	}

	private void FirstObjectNotifier_OnFirstObjectSpawned(GameObject player) {
		this.player = player;
		UpdatePlayer(player.GetComponent<PlayerInventory>());
	}

	private void PlayerInventory_InventoryChange(PlayerInventory obj) {
		if (inventory == null) return;
		if (!obj.Equals(inventory)) return;
		SyncInventory();
	}

	public void UpdatePlayer(PlayerInventory inventory) {
		this.inventory = inventory;
		itemObjects = new Dictionary<int, GameObject>();
		SyncInventory();
	}

	/// <summary>
	/// Syncs the inventory UI with the currently viewed playerInventory
	/// </summary>
	// Called by InventoryOpener on every inventory open
	public void SyncInventory() {
		if(!syncFlag) syncFlag = true;
	}

	private void SyncInventoryHelper() {

		for (int slot = 0; slot < PlayerInventory.SLOT_COUNT; slot++) {

			// Means inventory doesn't have item in slot but there might be an object so delete it
			if (!inventory.hasItem(slot)) {
				if (itemObjects.ContainsKey(slot)) deleteItemObject(slot);
				continue;
			}

			GameObject itemObject;

			// Means the inventory has item in slot but there's not itemObject to go with. Create one
			if (!itemObjects.ContainsKey(slot)) {
				GameObject hostSlot = getObjectFromSlotNumber(slot);
				itemObject = createItemObj(slot, hostSlot);
			} else {
				itemObject = itemObjects[slot];
			}

			ItemInventory script = itemObject.GetComponent<ItemInventory>();
			Item item = script.getItem();

			// Items don't match, sync them
			if (item == null || !item.Equals(inventory.getItem(slot))) {
				script.setItem(inventory.getItem(slot), false);
			}

			script.updateItem();

		}

	}

	public void deleteItemObject(int slot) {
		if (!itemObjects.ContainsKey(slot)) return;
		Destroy(itemObjects[slot]);
		itemObjects.Remove(slot);
	}

	private void setSlot(int slot, GameObject itemObject) {
		if (itemObjects.ContainsKey(slot)) {
			itemObjects[slot] = itemObject;
		} else {
			itemObjects.Add(slot, itemObject);
		}
	}

	public void moveItemObject(int prevSlot, int newSlot, GameObject itemObject) {
		if (itemObjects.ContainsKey(prevSlot)) itemObjects.Remove(prevSlot);
		setSlot(newSlot, itemObject);
	}

	public void swapItemObject(int slotA, GameObject objectA, int slotB, GameObject objectB) {
		setSlot(slotA, objectA);
		setSlot(slotB, objectB);
	}

	private GameObject createItemObj(int slotNumber, GameObject hostSlot) {

		GameObject itemObj = Instantiate(itemObjectPrefab, itemSlotsParent.transform);

		RectTransform transform = itemObj.GetComponent<RectTransform>();
		transform.sizeDelta = new Vector2(itemWidth, itemHeight);
		transform.pivot = new Vector3(0, 1, 0);

		ItemInventory script = itemObj.GetComponent<ItemInventory>();
		script.setSlotNumber(slotNumber);

		itemObjects.Add(slotNumber, itemObj);

		float xOff = (hostSlot.GetComponent<RectTransform>().rect.width / 2) - (itemObj.GetComponent<RectTransform>().rect.width / 2);
		float yOff = (hostSlot.GetComponent<RectTransform>().rect.height / 2) - (itemObj.GetComponent<RectTransform>().rect.height / 2);
		itemObj.GetComponent<RectTransform>().position = hostSlot.GetComponent<RectTransform>().position + new Vector3(xOff, -yOff, 0);

		return itemObj;

	}

	public static int getSlotNumberFromObject(GameObject obj) {
		if (!obj.name.Substring(0, 5).Equals("Slot ")) {
			Debug.LogError("Error in creating itemObjects, inventory slot name has changed from format: Slot x (x is slot number)");
			Debug.LogError("  Read: " + obj.name);
			return -1;
		}
		String slotString = obj.name.Replace("Slot ", "");
		try {
			return Int32.Parse(slotString);
		} catch (Exception) {
			return -1;
		}
	}

	public GameObject getObjectFromSlotNumber(int slotNum) {
		Image[] slots = itemSlotsParent.GetComponentsInChildren<Image>();
		foreach (Image img in slots) {
			GameObject obj = img.gameObject;
			if (obj.name.Equals("InventorySlots")) continue; // GetComponentsInChildren includes the parent for some reason
			int slotNumber = getSlotNumberFromObject(obj);
			if (slotNumber == slotNum) return obj;
		}
		return null;
	}

	public GameObject getItemObjectFromSlotNumber(int slot) {
		if (itemObjects.ContainsKey(slot)) return itemObjects[slot];
		return null;
	}

	public PlayerInventory getInventory() {
		return inventory;
	}

}
