using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGround : MonoBehaviour {

	private Item item;

	public Item getItem() {
		return item;
	}

	public void setItem(Item item) { 
		this.item = item;
		updateItem();
	}

	public void updateItem() { 
		
	}

}
