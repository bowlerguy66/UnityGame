using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeBlending {

    private static float blendingAmount = 0.02f;

    private BiomeManager biomeManager;
    private BiomeDataManager biomeDataManager;
    private float x, z;
    private float wet, temp;
    private bool log;

    private BiomeType[,] biomeTable;
    private int TCOUNT, WCOUNT;
    private int tempTarg, wetTarg; // The border that the value is approaching (0-TCOUNT or 0-WCOUNT)
    private float tempSeam, wetSeam; // The border that the value is approaching IN BIOME SPACE (0-1)
    private float tempVal, wetVal; // The amount the value is in the blending space (0-1)

    private Dictionary<BiomeType, float> blending;

    public BiomeBlending(BiomeManager biomeManager, BiomeDataManager biomeDataManager, float x, float z, bool logMessages) {

        this.x = x;
        this.z = z;

        this.wet = biomeManager.pollWet(x, z);
        this.temp = biomeManager.pollTemp(x, z);

        this.biomeManager = biomeManager;
        this.biomeDataManager = biomeDataManager;
        this.log = logMessages;

        this.biomeTable = biomeDataManager.getBiomeTable();
        this.TCOUNT = BiomeDataManager.TEMPERATURE_COUNT;
        this.WCOUNT = BiomeDataManager.WETNESS_COUNT;

        // Find the border that the temperature is approaching
        for (int tempInc = 1; tempInc < TCOUNT; tempInc++) {
            tempTarg = tempInc;
            //if (temp >= (tempInc - 0.5f / (float)TCOUNT) && temp < (tempInc + 0.5f / (float)TCOUNT)) break;
            if (Mathf.Abs(temp - (tempInc / (float)TCOUNT)) < 0.5f / (float)TCOUNT) break;
        }

        for (int wetInc = 1; wetInc < WCOUNT; wetInc++) {
            wetTarg = wetInc;
            //if (wet >= (wetInc - 0.5f / (float)WCOUNT) && wet < (wetInc + 0.5f / (float)WCOUNT)) break;
            if (Mathf.Abs(wet - (wetInc / (float)WCOUNT)) < 0.5f / (float)WCOUNT) break;
        }

        this.tempSeam = (float)tempTarg / (float)TCOUNT;
        this.wetSeam = (float)wetTarg / (float)WCOUNT;
        float tempBlendingStart = tempSeam - blendingAmount;
        float wetBlendingStart = wetSeam - blendingAmount;

        this.tempVal = (temp - tempBlendingStart) / (blendingAmount * 2);
        this.wetVal = (wet - wetBlendingStart) / (blendingAmount * 2);

    }

    public Dictionary<BiomeType, float> getBiomeBlending() {

        this.blending = new Dictionary<BiomeType, float>();

        if (log) {
            for (int i = 0; i < 15; i++) { Debug.Log(""); }
            //biomeDataManager.printTable();
            Debug.Log("Finding biome blending at: (" + x + ", " + z + "), True biome: " + biomeManager.getBiome(x, z));
            Debug.Log("  Polled wet/temp as: " + wet + ", " + temp);
            Debug.Log("  Found wet/temp targets as: " + wetTarg + ", " + tempTarg);
            Debug.Log("  Calculated wet/temp values as: " + wetVal + ", " + tempVal);
        }

        // Sides mode, no corner math
        if (isTempBlending() && !isWetBlending()) {
            return getTempEdgeBlending();
        } else if (isWetBlending() && !isTempBlending()) {
            return getWetEdgeBlending();
        } else if (isTempBlending() && isWetBlending()) {
            return getCornerBlending();
        }

        return blending;

    }

    public Dictionary<BiomeType, float> getCornerBlending() {

        int tempDir = (int)Mathf.Sign(tempSeam - temp);
        int wetDir = (int)Mathf.Sign(wetSeam - wet);

        BiomeType[] array = new BiomeType[4] {
            biomeTable[wetTarg - 1, tempTarg - 1], // Upper left
            biomeTable[wetTarg, tempTarg - 1],     // Upper right
            biomeTable[wetTarg - 1, tempTarg],     // Lower left
            biomeTable[wetTarg, tempTarg]          // Lower right
        };

        //float wetVal = applyWeight(this.wetVal);
        //float tempVal = applyWeight(this.tempVal);

        float lowerRightVal = (1 - wetVal) * (1 - tempVal);
        float lowerLeftVal = (wetVal) * (1 - tempVal);
        float upperRightVal = (1 - wetVal) * (tempVal);
        float upperLeftVal = (wetVal) * (tempVal);

        safeAdd(array[0], lowerRightVal);
        safeAdd(array[1], lowerLeftVal);
        safeAdd(array[2], upperRightVal);
        safeAdd(array[3], upperLeftVal);

        normalizeBlending();

        if (log) {
            Debug.Log(blending[array[0]] + " | " + blending[array[1]]);
            Debug.Log(blending[array[2]] + " | " + blending[array[3]]);
        }

        return blending;

    }

    public Dictionary<BiomeType, float> getTempEdgeBlending() {
        //float tempVal = applyWeight(this.tempVal);
        return edgeBlending(biomeTable[biomeManager.convertFromRaw(wet), tempTarg - 1],
                            biomeTable[biomeManager.convertFromRaw(wet), tempTarg],
                            tempVal);
    }

    public Dictionary<BiomeType, float> getWetEdgeBlending() {
        //float wetVal = applyWeight(this.wetVal);
        return edgeBlending(biomeTable[wetTarg - 1, biomeManager.convertFromRaw(temp)],
                            biomeTable[wetTarg, biomeManager.convertFromRaw(temp)],
                            wetVal);
    }

    private Dictionary<BiomeType, float> edgeBlending(BiomeType lowerBiome, BiomeType upperBiome, float val) {

        Dictionary<BiomeType, float> blending = new Dictionary<BiomeType, float>();

        if (lowerBiome != upperBiome) {
            blending.Add(lowerBiome, 1 - val);
            blending.Add(upperBiome, val);
        }

        return blending;

    }

    public bool isTempBlending() {
        return temp >= (tempTarg / (float)TCOUNT) - blendingAmount && temp <= (tempTarg / (float)TCOUNT) + blendingAmount;
    }

    public bool isWetBlending() {
        return wet >= (wetTarg / (float)WCOUNT) - blendingAmount && wet <= (wetTarg / (float)WCOUNT) + blendingAmount;
    }

    /** Normalizes all of the values in the blending so that the total = 1 */
    public void normalizeBlending() {
        float total = 0;
        foreach (BiomeType type in blending.Keys) {
            total += blending[type];
        }
        Dictionary<BiomeType, float> normalizedBlending = new Dictionary<BiomeType, float>();
        foreach (BiomeType type in blending.Keys) {
            normalizedBlending.Add(type, blending[type] / total);
        }
        this.blending = normalizedBlending;
    }

    public void safeAdd(BiomeType type, float value) {
        if (blending.ContainsKey(type)) {
            blending[type] += value;
        } else {
            blending.Add(type, value);
        }
    }

    private float applyWeight(float val) {
        if (val > 1) return 1;
        if (val < 0) return 0;
        // return (-(1-val) + 1) / (4 * (1-val) + 1); Simplified below:
        return val / (5 - 4 * val);
    }

    private float applyBell(float x) {
        return Mathf.Pow(2.07f/Mathf.Pow((float)(2.5 * (x * x) - 2.5 * x + 1.625), 3/2) - 1,2);
    }

}
