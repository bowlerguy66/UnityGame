using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : NetworkBehaviour {

	private GameObject playerCamera;

    void Update() {

		if (Input.GetKeyDown(KeyCode.F)) {

			RaycastHit hit;
			Physics.Raycast(playerCamera.transform.position + (playerCamera.transform.forward.normalized), playerCamera.transform.forward, out hit);
			GameObject objHit = hit.transform.gameObject;
			if (objHit.gameObject.name.StartsWith("Tree")) {
				GetComponent<PlayerInventory>().addItem(new Item(ItemID.WOOD, 1));
			}
			ItemGround groundItem = objHit.GetComponent<ItemGround>();
			if (groundItem != null) {
				pickupItem(objHit);
			}

		}

    }

	[ServerRpc(RequireOwnership = false)]
	public void pickupItem(GameObject groundObject) {

		ItemGround groundItem = groundObject.GetComponent<ItemGround>();
		PlayerInventory inventory = GetComponent<PlayerInventory>();
		bool itemAdded = inventory.addItem(groundItem.getItem());
		if (itemAdded) {
			ServerManager.Despawn(groundObject);
		}

	}

	public void setCamera(GameObject camera) { 
		this.playerCamera = camera;
	}

}
