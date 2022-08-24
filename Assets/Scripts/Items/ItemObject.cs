using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for adapting the Item (non-monobehaviour) data to the scene.
/// - Loads the ItemIDs mesh, material, etc
/// - Responsible for how the item sits on the ground
/// </summary>
public class ItemObject : NetworkBehaviour {

	private Item item;

	public void setItem(Item item) {
		this.item = item;
		reloadPhysical();
	}

	/// <summary>
	/// Reloads the mesh, material, and other things
	/// </summary>
	private void reloadPhysical() { 
	
	}

}
