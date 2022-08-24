using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ChunkLoader : NetworkBehaviour {

    // Manages the chunks for the player

	public int renderDistance = 5;
	public float updateTime = 0.5f;

	Dictionary<Vector2, int> lifetime;

	public void Awake() { // Awake in ChunkLoader
	}

    public override void OnStartClient() {
        base.OnStartClient();
        //updateLoadedChunks();
        lifetime = new Dictionary<Vector2, int>();
        StartCoroutine(updateClock());
    }

    void Update() {
        
    }

	// For every chunk in the render distance, assign it a lifetime. If it's outside of the render distance, count down the lifetime
	public void updateLoadedChunks() {

        //if (terrainManager == null) {
        //    lifetime = new Dictionary<Vector2, int>();
        //    Debug.Log("isServer: " + base.IsServer + ", " + base.LocalConnection.ClientId);
        //    Debug.Log("  simple test: " + (GameObject.FindGameObjectWithTag("TerrainManager") != null));
        //    GameObject terrainManagerGO = GameObject.FindGameObjectWithTag("TerrainManager");
        //    terrainManager = terrainManagerGO.GetComponent<TerrainManager>();
        //    Debug.Log("  Found terrainManager: " + (terrainManager != null));
        //}

        TerrainManager terrainManager = TerrainManager.Instance;

        int defaultLifetime = 3;

		Vector3 playerPos = transform.position;
		Vector2 currentChunk = terrainManager.toChunkCoordinates(playerPos.x, playerPos.z);

		for (int xco = -renderDistance; xco < renderDistance; xco++) {
			for (int zco = -renderDistance; zco < renderDistance; zco++) {

				int chunkX = (int)currentChunk.x - xco;
				int chunkZ = (int)currentChunk.y - zco;

				ChunkGenerator chunkGenerator = terrainManager.getChunk(chunkX, chunkZ).GetComponent<ChunkGenerator>();
				if (chunkGenerator.getChunkData() == null) {
					ChunkData data = TerrainManager.Instance.getChunkData(new Vector2(chunkX, chunkZ));
					chunkGenerator.setChunkData(data);
				}

				if (!lifetime.ContainsKey(new Vector2(chunkX, chunkZ))) {
					terrainManager.loadChunk(chunkX, chunkZ);
					lifetime.Add(new Vector2(chunkX, chunkZ), defaultLifetime + 1); // Automatically add 1 because it will be counted down in the next loop
				} else {
					lifetime[new Vector2(chunkX, chunkZ)] = defaultLifetime + 1;
				}

			}
		}

	    List<Vector2> toRemove = new List<Vector2>();
		foreach (Vector2 key in lifetime.Keys.ToList()) {
			lifetime[key] -= 1;
			if (lifetime[key] == 0) {
				toRemove.Add(key);
				//terrainManager.unloadChunk((int)key.x, (int)key.y);
			}
		}

		foreach (Vector2 key in toRemove) {
			lifetime.Remove(key);
		}

	}
	IEnumerator updateClock() {
		for (; ; ) {
            yield return new WaitForSeconds(updateTime);
            updateLoadedChunks();
		}
	}

}
