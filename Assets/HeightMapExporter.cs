using UnityEngine;
using UnityEditor;
using System.IO;

public class ExportHeightmapToRaw
{
    [MenuItem("Terrain/Export Heightmap to RAW")]
    static void ExportToRaw()
    {
        GameObject selectedObject = Selection.activeGameObject;
        TerrainData terrain = null;

        // 씬에서 Terrain 오브젝트를 선택한 경우
        if (selectedObject != null)
        {
            Terrain terrainComponent = selectedObject.GetComponent<Terrain>();
            if (terrainComponent != null)
            {
                terrain = terrainComponent.terrainData; // Terrain에서 TerrainData 가져오기
            }
        }

        // Assets에서 TerrainData를 직접 선택한 경우
        if (terrain == null)
        {
            terrain = Selection.activeObject as TerrainData;
        }

        // TerrainData가 없으면 에러 메시지
        if (terrain == null)
        {
            Debug.LogError("Terrain이나 TerrainData를 선택하세요!");
            return;
        }

        int res = terrain.heightmapResolution;
        if (res != 513)
        {
            Debug.LogWarning($"Heightmap 해상도가 {res}x{res}입니다. 513x513이 아닌 경우 주의하세요.");
        }
        float[,] heights = terrain.GetHeights(0, 0, res, res);

        string name = string.Copy(terrain.name).Replace(" ", "_") + "Heightmap";

        string filePath = EditorUtility.SaveFilePanel("Save RAW Heightmap", "Assets", name + ".raw", "raw");
        if (string.IsNullOrEmpty(filePath)) return;

        using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            for (int y = 0; y < res; y++)
            {
                for (int x = 0; x < res; x++)
                {
                    float value = heights[y, x];
                    writer.Write(value);
                }
            }
        }

        Debug.Log($"RAW 파일 저장 완료: {filePath} ({res}x{res})");
        AssetDatabase.Refresh();
    }
}