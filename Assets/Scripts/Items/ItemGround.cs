using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGround : MonoBehaviour {

	private ItemIDMeshes itemManager;
	private Item item;

	private void Awake() {
		itemManager = GameObject.Find("ItemManager").GetComponent<ItemIDMeshes>();
	}

	public Item getItem() {
		return item;
	}

	public void setItem(Item item) { 
		this.item = item;
		updateItem();
	}

	public void updateItem() {
		GameObject targetObj = itemManager.getObject(item.getID());
		GetComponent<Transform>().localScale = targetObj.GetComponent<Transform>().localScale;
		GetComponent<MeshFilter>().sharedMesh = targetObj.GetComponent<MeshFilter>().sharedMesh;
		MeshCollider collider = GetComponent<MeshCollider>();
		collider.sharedMesh = targetObj.GetComponent<MeshCollider>().sharedMesh;
		collider.convex = true;
		GetComponent<MeshRenderer>().sharedMaterial = targetObj.GetComponent<MeshRenderer>().sharedMaterial;
	}

}
