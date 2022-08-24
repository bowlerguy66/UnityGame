using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkDataGenerator : MonoBehaviour {

    private BiomeManager biomeManager;

    public static int size = 16;
    public static float scale = 1.5f;

    public void Awake() {
        biomeManager = GetComponent<TerrainManager>().getBiomeManager();
    }

    public ChunkData generateChunkData(int chunkX, int chunkZ) {

        // Size + 1 because there needs to be an extra column of vertices to make size amount of squares
        Vector3[] vertices = new Vector3[(size + 1) * (size + 1) + (size) * (size)];
        int[] triangles = new int[3 * 4 * (size * size)]; // 4x^2 because there's 4 triangles per square, 3* because there's three points to a triangle
        Vector2[] uvs = new Vector2[vertices.Length];
        Color[] colors = new Color[vertices.Length];

        int ct = 0;
        for (int z = 0; z < size + 1; z++) {

            for (int x = 0; x < size + 1; x++) { // Main coordinate points

                float localX = x * scale;
                float localZ = z * scale;
                float worldX = (chunkX * size + x) * scale;
                float worldZ = (chunkZ * size + z) * scale;

                float y = biomeManager.pollHeight(worldX, worldZ);
                vertices[ct] = new Vector3(localX, y * scale, localZ);

                uvs[ct] = new Vector2(worldX / size, worldZ / size);
                colors[ct] = getColorAt(worldX, worldZ);
                
                ct++;

            }

            if (z == size) continue; // Don't want to generate center coordinates, these would be below the bottom row
            for (int x = 0; x < size; x++) { // Center coordinate points

                float localX = (x + 0.5f) * scale;
                float localZ = (z + 0.5f) * scale;
                float worldX = (chunkX * size + x + 0.5f) * scale;
                float worldZ = (chunkZ * size + z + 0.5f) * scale;

                float y = biomeManager.pollHeight(worldX, worldZ);                
                vertices[ct] = new Vector3(localX, y * scale, localZ);

                uvs[ct] = new Vector2(worldX / size, worldZ / size);
                colors[ct] = getColorAt(worldX, worldZ);
                
                ct++;

            }

        }

        int tri = 0;
        for (int x = 0; x < size; x++) {
            for (int z = 0; z < size; z++) {

                // Math for these coordinates at https://imgur.com/a/izuv6gk or in OneNote code notebook
                int baseCoord = z * (2 * size + 1) + x;

                int upperLeft = baseCoord;
                int upperRight = baseCoord + 1;
                int lowerLeft = baseCoord + 2 * size + 1;
                int lowerRight = baseCoord + 2 * size + 2;
                int center = baseCoord + size + 1;

                triangles[tri + 2] = upperLeft;
                triangles[tri + 1] = upperRight;
                triangles[tri + 0] = center;
                //Debug.Log(tri);
                tri += 3;

                triangles[tri + 2] = upperRight;
                triangles[tri + 1] = lowerRight;
                triangles[tri + 0] = center;
                //Debug.Log(tri);
                tri += 3;

                triangles[tri + 2] = lowerRight;
                triangles[tri + 1] = lowerLeft;
                triangles[tri + 0] = center;
                //Debug.Log(tri);
                tri += 3;

                triangles[tri + 2] = lowerLeft;
                triangles[tri + 1] = upperLeft;
                triangles[tri + 0] = center;
                //Debug.Log(tri);
                tri += 3;

            }
        }

        ChunkData data = new ChunkData(chunkX, chunkZ);
        data.setMeshData(vertices, triangles, uvs, colors);
        return data;

    }

    private Color getColorAt(float x, float z) {

        Dictionary<BiomeType, float> blending = biomeManager.getBiomeBlending(x, z, false);

        if (blending.Count == 0) {
            return biomeManager.getBiomeData(x, z).getColor();
        }

        float r = 0;
        float g = 0;
        float b = 0;

        foreach (BiomeType type in blending.Keys) {
            Color c = biomeManager.getBiomeDataManager().getData(type).getColor();
            r += (float)c.r * blending[type];
            g += (float)c.g * blending[type];
            b += (float)c.b * blending[type];
        }

        return new Color(r, g, b);

    }

}
