using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class will manage the biome data for the world, the chunk generator will look here for height, color data, etc
public class BiomeManager {

    private int seed;

    public int biomeScale = 2000;
    BiomeDataManager biomeDataManager;
    BiomeType[,] biomeTable;

    public BiomeManager(int seed) {
        this.seed = seed;

        biomeDataManager = new BiomeDataManager();
        biomeTable = biomeDataManager.getBiomeTable();
    }

    public float pollTemp(float x, float z) {
        return Mathf.PerlinNoise((x + seed) / biomeScale, (z + seed) / biomeScale);
    }

    public float pollWet(float x, float z) {
        return Mathf.PerlinNoise((x - seed * 2) / biomeScale, (z - seed * 2) / biomeScale);
    }

    public BiomeType getBiome(float x, float z) {
        int temp = convertFromRaw(pollTemp(x, z));
        int wet = convertFromRaw(pollWet(x, z));
        return biomeTable[wet, temp];
    }

    public Dictionary<BiomeType, float> getBiomeBlending(float x, float z, bool log) {
        return new BiomeBlending(this, biomeDataManager, x, z, log).getBiomeBlending();
    }

    public BiomeData getBiomeData(float x, float z) {
        return biomeDataManager.getData(getBiome(x, z));
    }

    public float pollHeight(float x, float z) {

        Dictionary<BiomeType, float> blending = getBiomeBlending(x, z, false);
        float final = 0;

        if (blending.Count > 0) {
            foreach (BiomeType type in blending.Keys) {
                float current = 0;
                Vector2[] octaves = biomeDataManager.getData(type).getOctaves();
                for (int i = 0; i < octaves.Length; i++) {

                    float height = octaves[i].x;
                    float scale = octaves[i].y;

                    float mapX = (x + seed) * (1 / scale) + 5000;
                    float mapZ = (z + seed) * (1 / scale) + 5000;

                    current += height * Mathf.PerlinNoise(mapX, mapZ);

                }
                final += current * blending[type];
            }
        } else {
            Vector2[] octaves = getBiomeData(x, z).getOctaves();
            for (int i = 0; i < octaves.Length; i++) {

                float height = octaves[i].x;
                float scale = octaves[i].y;

                float mapX = (x + seed) * (1 / scale) + 5000;
                float mapZ = (z + seed) * (1 / scale) + 5000;

                final += height * Mathf.PerlinNoise(mapX, mapZ);

            }
        }

        return final;
    }

    public float pollSlope(float x, float z) {

        float offset = 1;

        float poX = pollHeight(x + offset, z);
        float neX = pollHeight(x - offset, z);
        float poZ = pollHeight(x, z + offset);
        float neZ = pollHeight(x, z - offset);

        float xSlope = Mathf.Abs(poX - neX) / (2 * offset);
        float zSlope = Mathf.Abs(poZ - neZ) / (2 * offset);

        //return Mathf.Max(xSlope, zSlope);
        return (xSlope + zSlope) / 2;

    }

    // Converts the raw temp/wetness value into their 0-2 value
    public int convertFromRaw(float raw) {
        if (BiomeDataManager.TEMPERATURE_COUNT > 3 || BiomeDataManager.WETNESS_COUNT > 3) {
            Debug.LogError("PROBLEM WITH BiomeManager.convertFromRaw(), needs to be adjusted to account for new temp/wetness counts");
        }
        if (raw <= 0.3333) return 0;
        if (raw > 0.3333 && raw < 0.66666) return 1;
        if (raw >= 0.6666) return 2;
        return 0;
    }

    public int convertRawTemp(float rawTemp) {
        return 1;
    }

    public BiomeDataManager getBiomeDataManager() {
        return biomeDataManager;
    }

}