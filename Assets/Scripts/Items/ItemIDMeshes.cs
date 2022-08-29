using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemIDMeshes : MonoBehaviour {

	[Serializable]
	public struct ItemIDGameObject {
		public ItemID id;
		public GameObject gameObject;
	}

	public GameObject[] objects;

	public GameObject getObject(ItemID id) {
		return objects[(int) id];
	}

}
