using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureLoader : MonoBehaviour {

	public List<Texture2D> itemTextures;
	private Dictionary<ItemID, Texture2D> textures;
	private bool loadedTextures;

	private void Awake() {
		loadedTextures = false;
	}

	public void loadTextures() {
		textures = new Dictionary<ItemID, Texture2D>();
		foreach (Texture2D texture in itemTextures) {
			try {
				ItemID type = (ItemID) Enum.Parse(typeof(ItemID), texture.name);
				textures.Add(type, texture);
			} catch (Exception) {
				Debug.LogWarning("Failed to sort " + texture.name);
			}
		}
		loadedTextures = true;
	}

	public Texture2D getTexture(ItemID id) {
		if (!loadedTextures) loadTextures();
		if (!textures.ContainsKey(id)) {
			Debug.LogWarning("No texture exists for " + id.ToString());
			return null;
		}
		return textures[id];
	}

}
