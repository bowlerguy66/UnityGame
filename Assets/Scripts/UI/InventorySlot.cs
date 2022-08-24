using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler {

	private InventoryManager inventoryManager;

	private void Awake() {
		inventoryManager = GetComponentInParent<InventoryManager>();
	}

	public void OnDrop(PointerEventData eventData) {
		
		GameObject pdrag = eventData.pointerDrag;
		ItemInventory draggedScript = pdrag.GetComponent<ItemInventory>();
		int slotNum = InventoryManager.getSlotNumberFromObject(gameObject);

		PlayerInventory inv = inventoryManager.getInventory();
		if (inv.hasItem(slotNum)) {
			GameObject itemObjAtSlot = inventoryManager.getItemObjectFromSlotNumber(slotNum);
			if (itemObjAtSlot != null) itemObjAtSlot.GetComponent<ItemInventory>().handleDropEvent(pdrag);
			return;
		}

		PlayerInventory inventory = inventoryManager.getInventory();
		InventoryActionResult result = inventory.handleAction(draggedScript.getItem(), null, draggedScript.getSlotNumber(), slotNum);

		if (result == InventoryActionResult.DROP) {

			inventoryManager.moveItemObject(draggedScript.getSlotNumber(), slotNum, pdrag);
			draggedScript.setSlotNumber(slotNum);
			draggedScript.setDroppedOnSlot(true);

			float xOff = (GetComponent<RectTransform>().rect.width / 2) - (pdrag.GetComponent<RectTransform>().rect.width / 2);
			float yOff = (GetComponent<RectTransform>().rect.height / 2) - (pdrag.GetComponent<RectTransform>().rect.height / 2);
			pdrag.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position + new Vector3(xOff, -yOff, 0);

		}

		draggedScript.refreshItem();

	}

}
