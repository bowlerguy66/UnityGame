using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData {

    private Vector2 chunkLocation;

    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;
    private Color[] colors;

	private List<GameObject> populatedObjects;

    public ChunkData() { }
    public ChunkData(int chunkX, int chunkZ) {
        this.chunkLocation = new Vector2(chunkX, chunkZ);
		this.populatedObjects = new List<GameObject>();
    }

    public void setMeshData(Vector3[] vertices, int[] triangles, Vector2[] uvs, Color[] colors) {
        this.vertices = vertices;
        this.triangles = triangles;
        this.uvs = uvs;
        this.colors = colors;
    }

    public Vector2 getChunkLocation() {
        return chunkLocation;
    }

    public Vector3[] getVertices() {
        return vertices;
    }

    public int[] getTriangles() {
        return triangles;
    }
    public Vector2[] getUVs() {
        return uvs;
    }
    
    public Color[] getColors() {
        return colors;
    }

	public List<GameObject> getPopulatedObjects() {
		return populatedObjects;
	}

	public void addPopulatedObject(GameObject obj) {
		populatedObjects.Add(obj);
	}

	public void DestroyPopulatedObjects() {
		if (populatedObjects == null) return;
		foreach (GameObject obj in populatedObjects) {
			GameObject.Destroy(obj);
		}
	}

}
