using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkPopulator : MonoBehaviour {

	public List<GameObject> objectsToPopulate;

	public int count;
	public bool randomRotationX;
	public bool randomRotationY;
	public bool randomRotationZ;

	/// <summary>
	/// Populate the chunk with game objects
	/// </summary>
	/// <returns>
	///	true if we successfully populated
	///	false if a RayCastHit returned null
	/// </returns>
	public bool populate(GameObject chunk) {

		TerrainManager tm = GetComponent<TerrainManager>();
		Vector3 chunkPos = chunk.transform.position;
		int seed = TerrainManager.Instance.getSeed();
		Random.InitState(seed + (int) (chunkPos.x * chunkPos.z) + 10000);

		for (int i = 0; i < count; i++) {

			float locX = Random.Range(0, ChunkDataGenerator.size * ChunkDataGenerator.scale);
			float locZ = Random.Range(0, ChunkDataGenerator.size * ChunkDataGenerator.scale);

			int layerMask = 1 << 6;
			RaycastHit hit;
			Vector3 origin = new Vector3(chunkPos.x + locX, 999, chunkPos.z + locZ);
			Physics.Raycast(origin, Vector3.down, out hit, Mathf.Infinity, layerMask);

			// Failed to hit a mesh when we should, return false so we can try to populate again later
			if (hit.collider == null) return false;

			GameObject obj = GameObject.Instantiate(objectsToPopulate[Random.Range(0, this.objectsToPopulate.Count)], chunk.transform);
			Transform transform = obj.transform;
			transform.position = hit.point;
			if (randomRotationX) transform.Rotate(Vector3.forward, Random.Range(0, 360));
			if (randomRotationY) transform.Rotate(Vector3.up, Random.Range(0, 360));
			if (randomRotationZ) transform.Rotate(Vector3.right, Random.Range(0, 360));

			Vector2 chunkCoordinates = tm.toChunkCoordinates(chunkPos.x, chunkPos.z);
			ChunkData chunkData = tm.getChunkData(chunkCoordinates);
			chunkData.addPopulatedObject(obj);

		}

		return true;

	}

}
