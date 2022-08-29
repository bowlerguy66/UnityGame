using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
public class Item {

	private ItemID id;
	private int count;
	private bool stackable;

	public Item(ItemID id, int count, bool stackable) {
		this.id = id;
		this.count = count;
		this.stackable = stackable;
	}

	public Item(ItemID id, int count) : this(id, count, true) {}
	public Item() { }

	/// <summary>
	/// Checks if the item has the same id, ignores count and stackability
	/// </summary>
	public bool isSimilar(Item toCompare) {
		return id == toCompare.id;
	}

	public override bool Equals(object obj) {
		try {
			Item toCompare = (Item)obj;
			return id == toCompare.getID() && count == toCompare.getCount() && stackable == toCompare.isStackable();
		} catch (Exception) { }
		return false;
	}

	public override string ToString() {
		return $"{id.ToString()}, {count}";
	}

	public Item clone() {
		return new Item(id, count, stackable);
	}

	public ItemID getID() {
		return id;
	}

	public int getCount(){
		return count;
	}

	public void setCount(int count) {
		this.count = count;
	}

	public void addCount(int addAmt) {
		this.count += addAmt;
	}

	public bool isStackable() {
		return stackable;
	}

	public void setStackable(bool stackable) {
		this.stackable = stackable;
	}

}
