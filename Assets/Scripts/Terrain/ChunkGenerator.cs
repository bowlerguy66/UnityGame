using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
public class ChunkGenerator : MonoBehaviour {

    // ChunkGenerator attaches to the chunk and is responsible for applying ChunkData
    private Vector2 chunkLocation;
    private ChunkData chunkData;

    private Mesh mesh;

	private int chunkPopulatorTimer;

    void Awake() {
        mesh = new Mesh();
		chunkPopulatorTimer = -1;
    }

    void FixedUpdate() {

		if (chunkPopulatorTimer == 0) {
			bool success = true;
			chunkData.DestroyPopulatedObjects();
			foreach (ChunkPopulator pop in TerrainManager.Instance.GetComponents<ChunkPopulator>()) {
				// Populate the chunk and check if it was successfull
				if (!pop.populate(gameObject)) {
					success = false;
					break;
				}
			}
			if (success) chunkPopulatorTimer = -1;
		} else { 
			chunkPopulatorTimer--;
		}

	}

	/// <summary>
	/// Call when the chunk data changes
	/// </summary>
	private void UpdatedChunkData() {

		this.chunkLocation = chunkData.getChunkLocation();

		GetComponent<MeshFilter>().mesh = mesh;
		UpdateMesh();
		GetComponent<MeshCollider>().sharedMesh = mesh;

		Vector2 worldCoordinates = TerrainManager.Instance.toWorldCoordinates(chunkLocation.x, chunkLocation.y);
		transform.position = new Vector3(worldCoordinates.x, 0, worldCoordinates.y);

		chunkPopulatorTimer = 5;

	}

	public void UpdateMesh() {
        if (chunkData == null) return;
        mesh.Clear();
        mesh.vertices = chunkData.getVertices();
        mesh.triangles = chunkData.getTriangles();
        mesh.uv = chunkData.getUVs();
        mesh.colors = chunkData.getColors();
        mesh.RecalculateNormals();
    }

    public ChunkData getChunkData() { 
        return chunkData;
    }

    public void setChunkData(ChunkData chunkData) {
        this.chunkData = chunkData;
		UpdatedChunkData();
    }

}
