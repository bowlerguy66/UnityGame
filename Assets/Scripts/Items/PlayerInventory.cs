using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {

	// 24 inv slots and 3 equipment slots
	public static int SLOT_COUNT = 27;

	public static event Action<PlayerInventory> InventoryChange;

	private Item[] slots;

	private void Start() {
		
		slots = new Item[SLOT_COUNT];

		//TODO temp code, remove
		setItem(0, new Item(ItemID.WOOD, 1));
		setItem(24, new Item(ItemID.ROCK, 1));
		setItem(4, new Item(ItemID.WOOD, 1));

	}

	/// <summary>
	/// Handle and inventory action between two objects and adjust inventory accordingly
	/// </summary>
	/// <param name="draggedItem">The item being dragged and then dropped onto the target item</param>
	/// <param name="targetItem">The item that was dropped on to</param>
	/// <param name="originSlot">The slot that the drag originated from</param>
	/// <param name="targetSlot">The slot that was dropped on to</param>
	public InventoryActionResult handleAction(Item draggedItem, Item targetItem, int originSlot, int targetSlot) {

		if (targetItem == null) { // Results in a drop
			setItem(targetSlot, draggedItem);
			clearSlot(originSlot);
			return InventoryActionResult.DROP;
		} else if (draggedItem.isSimilar(targetItem) && draggedItem.isStackable()) { // Results in a stack
			targetItem.addCount(draggedItem.getCount());
			clearSlot(originSlot);
			return InventoryActionResult.STACK;
		} else if(draggedItem != null && targetItem != null) { // Results in a swap 
			setItem(originSlot, targetItem);
			setItem(targetSlot, draggedItem);
			return InventoryActionResult.SWAP;
		}

		return InventoryActionResult.NONE;

	}

	public Item getItem(int slot) {
		return slots[slot];
	}

	public void setItem(int slot, Item item) {
		InventoryChange?.Invoke(this);
		slots[slot] = item;
	}

	public void clearSlot(int slot) {
		setItem(slot, null);
	}

	/// <summary>
	/// Will add item in first available slot, stacking on top of others if possible
	/// </summary>
	/// <returns>
	/// <para>TRUE if the item was successfully added</para>
	/// <para>FALSE if the item wansn't added</para>
	/// </returns>
	public bool addItem(Item item) {
		for (int i = 0; i < slots.Length; i++) {

			// Found empty slot
			if (slots[i] == null){
				slots[i] = item;
				InventoryChange?.Invoke(this);
				return true;
			}

			// Found unstackable slot, can't add here
			if (!slots[i].isStackable()) continue;

			// Found a similar item, add the amounts and break
			if (slots[i].isSimilar(item)) {
				slots[i].addCount(item.getCount());
				InventoryChange?.Invoke(this);
				return true;
			}

		}
		return false;
	}

	public bool hasItem(int slot) {
		try{
			return slots[slot] != null;
		} catch (Exception) {
			return false;
		}
	}

}