using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeDataManager {

    public static int TEMPERATURE_COUNT = 3;
    public static int WETNESS_COUNT = 3;

    private Dictionary<BiomeType, BiomeData> data;

    public BiomeDataManager() {

        data = new Dictionary<BiomeType, BiomeData>();

        BiomeData plains = new BiomeData(new Color32(68, 166, 38, 255), new Vector2[] {
            new Vector2(1, 80),
            new Vector2(3, 120),
            new Vector2(12, 200),
        }); 
        data.Add(BiomeType.PLAINS, plains);

        BiomeData mountains = new BiomeData(new Color32(22, 122, 72, 255), new Vector2[] {
            new Vector2(1, 30),
            new Vector2(3, 45),
            new Vector2(12, 80),
            new Vector2(50, 200)
        });
        data.Add(BiomeType.MOUNTAINS, mountains);

        BiomeData forest = new BiomeData(new Color32(17, 94, 25, 255), new Vector2[] {
            new Vector2(3, 30),
            new Vector2(12, 60),
            new Vector2(50, 100)
        });
        data.Add(BiomeType.FOREST, forest);

        // old desert color new Color32(122, 119, 16, 255)
        BiomeData desert = new BiomeData(Color.red, new Vector2[] {
            new Vector2(1, 80),
            new Vector2(3, 120),
            new Vector2(12, 200),
        });
        data.Add(BiomeType.DESERT, desert);

        BiomeData jungle = new BiomeData(new Color32(40, 166, 12, 255), new Vector2[] {
            new Vector2(3, 30),
            new Vector2(12, 60),
            new Vector2(50, 100)
        });
        data.Add(BiomeType.JUNGLE, jungle);

        BiomeData tundra = new BiomeData(new Color32(166, 247, 197, 255), new Vector2[] {
            new Vector2(1, 80),
            new Vector2(3, 120),
            new Vector2(12, 200),
        });
        data.Add(BiomeType.TUNDRA, tundra);

    }

    public BiomeData getData(BiomeType type) {
        return data[type];
    }

    public BiomeType[,] getBiomeTable() {
        return new BiomeType[,] {
            // COLD                  NORMAL                  HOT        
            {BiomeType.TUNDRA,       BiomeType.DESERT,       BiomeType.DESERT}, // DRY
            {BiomeType.TUNDRA,       BiomeType.PLAINS,       BiomeType.JUNGLE}, // NORMAL   x axis
            {BiomeType.MOUNTAINS,    BiomeType.PLAINS,       BiomeType.JUNGLE}  // WET
        };
        /**return new BiomeType[,] {
            // COLD                  NORMAL                  HOT        
            {BiomeType.PLAINS,    BiomeType.PLAINS,       BiomeType.PLAINS}, // WET
            {BiomeType.DESERT,    BiomeType.DESERT,       BiomeType.DESERT}, // NORMAL
            {BiomeType.DESERT,       BiomeType.DESERT,       BiomeType.DESERT}  // DRY
        };*/
    }

    public void printTable() {
        BiomeType[,] table = getBiomeTable();
        for (int y = 0; y < TEMPERATURE_COUNT; y++) {
            String row = "";
            for (int x = 0; x < WETNESS_COUNT; x++) {
                row += "(" + x + ", " + y + ") " + table[x, y] + "       ";
            }
            Debug.Log(row);
        }
    }

}
public class BiomeData {

    private Vector2[] octaves;
    private Color baseColor;

    public BiomeData(Color baseColor, Vector2[] octaves) {
        this.octaves = octaves;
        this.baseColor = baseColor;
    }

    public Vector2[] getOctaves() {
        return octaves;
    }

    public Color getColor() {
        return baseColor;
    }

}

public enum BiomeType {
    PLAINS, 
    MOUNTAINS, 
    FOREST, 
    DESERT,
    JUNGLE,
    TUNDRA
}