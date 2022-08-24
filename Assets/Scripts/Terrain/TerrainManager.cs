using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : NetworkBehaviour {

	public static TerrainManager Instance;
	public GameObject terrainObject;
	public GameObject chunkPrefab;

	// We have two dictionaries:
	//   1. Local Dictionary<Vector2, GameObject> holds the local chunk prefab and all that
	//   2. Server Dictionary<Vector2, ChunkData> holds the data for each chunk, we apply the data from this to the local gameobject one
	private Dictionary<Vector2, GameObject> chunks = new Dictionary<Vector2, GameObject>();
	[SyncObject]
	private readonly SyncIDictionary<Vector2, ChunkData> chunkData = new SyncDictionary<Vector2, ChunkData>();

	private int seed;
	private BiomeManager biomeManager;

	private void Awake() {

		if (Instance != null && Instance != this) {
			Destroy(this);
		} else {
			Instance = this;
		}

		//seed = generateSeed();
		seed = 1320303;
		biomeManager = new BiomeManager(seed);
	}

	public override void OnStartServer() {
		base.OnStartServer();
		chunkData.OnChange += ChunkData_OnChange;
	}

	private void ChunkData_OnChange(SyncDictionaryOperation op, Vector2 key, ChunkData value, bool asServer) {
		if (value == null) return;
		if (asServer) return;
		Vector2 chunkLocation = value.getChunkLocation();
		GameObject chunk = getChunk((int)chunkLocation.x, (int)chunkLocation.y);
		chunk.GetComponent<ChunkGenerator>().setChunkData(value);
	}

	public GameObject getChunk(Vector2 chunkCoordinates) {
		if (!chunks.ContainsKey(chunkCoordinates)) { // Load chunk
			GameObject c = Instantiate(chunkPrefab, terrainObject.GetComponent<Transform>());
			c.name = "Chunk (" + chunkCoordinates.x + ", " + chunkCoordinates.y + ")";
			chunks.Add(chunkCoordinates, c);
			ChunkData data = getChunkData(chunkCoordinates);
			if (data != null) c.GetComponent<ChunkGenerator>().setChunkData(data);
		}

		return chunks[chunkCoordinates];
	}
	public GameObject getChunk(int chunkX, int chunkZ) { return getChunk(new Vector2(chunkX, chunkZ)); }

	public void loadChunk(Vector2 chunkCoordinates) { loadChunk((int)chunkCoordinates.x, (int)chunkCoordinates.y); }
	public void loadChunk(int chunkX, int chunkZ) {
		GameObject c = getChunk(chunkX, chunkZ);
		c.SetActive(true);
		//c.GetComponent<MeshRenderer>().enabled = true;
	}

	public void unloadChunk(Vector2 chunkCoordinates) { unloadChunk((int)chunkCoordinates.x, (int)chunkCoordinates.y); }
	public void unloadChunk(int chunkX, int chunkZ) {
		GameObject c = getChunk(chunkX, chunkZ);
		c.SetActive(false);
		//c.GetComponent<MeshRenderer>().enabled = false;
	}

	public ChunkData getChunkData(Vector2 chunkCoordinates) {
		if (chunkData.ContainsKey(chunkCoordinates)) {
			return chunkData[chunkCoordinates];
		} else {
			ChunkData newData = GetComponent<ChunkDataGenerator>().generateChunkData((int)chunkCoordinates.x, (int)chunkCoordinates.y);
			addChunkData(chunkCoordinates, newData);
			return newData;
		}
	}

	/// <summary>
	/// Converts world coordinates to chunk coordinates
	/// </summary>
	public Vector2 toChunkCoordinates(float x, float z) {
		float size = ChunkDataGenerator.size;
		float scale = ChunkDataGenerator.scale;
		float chunkX = x / (size * scale);
		float chunkZ = z / (size * scale);
		return new Vector2(Mathf.FloorToInt(chunkX), Mathf.FloorToInt(chunkZ));
	}

	/// <summary>
	/// Converts chunk coordinates to world coordinates
	/// </summary>
	public Vector2 toWorldCoordinates(float chunkX, float chunkZ) {
		float conversionFactor = ChunkDataGenerator.size * ChunkDataGenerator.scale;
		return new Vector2(chunkX * conversionFactor, chunkZ * conversionFactor);
	}
	public int getSeed() {
		return seed;
	}

	public BiomeManager getBiomeManager() {
		return biomeManager;
	}

	public int generateSeed() {
		int seedLength = 7;
		int seed = 0;
		for (int i = 0; i < seedLength; i++) {
			seed += Random.Range(i==seedLength-1 ? 1 : 0, 9) * (int)Mathf.Pow(10, i);
		}
		return seed;
	}

	public void addChunkData(Vector2 chunkLocation, ChunkData data) {
		if (chunkData.ContainsKey(chunkLocation)) {
			chunkData[chunkLocation] = data;
		} else {
			chunkData.Add(chunkLocation, data);
		}
	}

}
