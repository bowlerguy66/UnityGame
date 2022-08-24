using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// The item that is a part of the inventory UI
/// </summary>
public class ItemInventory : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler {

	private Item item;
	private RectTransform rectTransform;
	private CanvasGroup canvasGroup;
	private InventoryManager inventoryManager;

	[SerializeField]
	private int slotNumber;
	
	private bool droppedOnSlot;
	private Vector3 originalLoc;

	private int oldSiblingIndex;

	private void Awake() {
		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();
		inventoryManager = GetComponentInParent<InventoryManager>();
	}

	public void OnBeginDrag(PointerEventData eventData) {
		canvasGroup.blocksRaycasts = false;
		canvasGroup.alpha = 0.8f;
		droppedOnSlot = false;
		originalLoc = rectTransform.position;
		oldSiblingIndex = transform.GetSiblingIndex();
		transform.SetAsLastSibling();
	}

	public void OnDrag(PointerEventData eventData) {
		rectTransform.anchoredPosition += eventData.delta;
	}

	public void OnEndDrag(PointerEventData eventData) {
		canvasGroup.blocksRaycasts = true;
		canvasGroup.alpha = 1f;
		if (!droppedOnSlot) {
			rectTransform.position = originalLoc;
		}
		transform.SetSiblingIndex(oldSiblingIndex);
	}

	public void OnPointerDown(PointerEventData eventData) {

	}

	// Handles when an item is dropped on top of the other
	// Inventory slot handles when an empty slot happens
	public void OnDrop(PointerEventData eventData) {
		handleDropEvent(eventData.pointerDrag);
	}

	// Separate method for inventoryslots to use
	public void handleDropEvent(GameObject objDragged) {

		ItemInventory draggedScript = objDragged.GetComponent<ItemInventory>();
		ItemInventory currentScript = GetComponent<ItemInventory>();

		PlayerInventory inventory = inventoryManager.getInventory();
		InventoryActionResult result = inventory.handleAction(draggedScript.getItem(), currentScript.getItem(), draggedScript.getSlotNumber(), currentScript.getSlotNumber());

		if (result == InventoryActionResult.STACK) {
			// Can stack items
			inventoryManager.deleteItemObject(draggedScript.getSlotNumber());
		} else if (result == InventoryActionResult.SWAP) {
			// Swap items
			int tempSlot = draggedScript.getSlotNumber();
			draggedScript.setSlotNumber(tempSlot);
			inventoryManager.swapItemObject(draggedScript.getSlotNumber(), objDragged, getSlotNumber(), gameObject);
		}

		draggedScript.refreshItem();
		currentScript.refreshItem();

	}

	public Item getItem() {
		return item;
	}

	public void setItem(Item item) { setItem(item, true); }
	public void setItem(Item item, bool update) {
		this.item = item;
		if (update) updateItem();
	}

	public void refreshItem() {
		setItem(inventoryManager.getInventory().getItem(slotNumber));
	}

	public void updateItem() {
		Image img = GetComponent<Image>();
		if (item != null) {
			TextureLoader tl = GetComponentInParent<TextureLoader>();
			if (tl == null) {
				Debug.LogError("Failed to find TextureLoader");
				return;
			}
			Texture2D texture = tl.getTexture(item.getID());
			img.sprite = Sprite.Create(texture, new Rect(new Vector2(0, 0), new Vector2(texture.width, texture.height)), new Vector2(0, 1));
			img.color = Color.white;
			name = item.getID().ToString() + ", " + item.getCount();
		} else {
			img.color = Color.clear;
		}

		TMP_Text countText = GetComponentInChildren<TMP_Text>();
		if (item != null && item.getCount() > 1) {
			countText.text = item.getCount() + "";
		} else {
			countText.text = "";
		}

	}

	public int getSlotNumber() {
		return slotNumber;
	}

	public void setSlotNumber(int slotNumber) {
		this.slotNumber = slotNumber;
	}

	public void setDroppedOnSlot(bool droppedOnSlot) {
		this.droppedOnSlot = droppedOnSlot;
	}

	public Vector3 getOriginalLoc() {
		return originalLoc;
	}

}
