using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace JustAssets.TerrainUtility
{
    public sealed partial class SplitTerrain
    {
#if DEBUG_ALPHAMAPS
        [UnityEditor.MenuItem("Tools/Terrain/Store Alphamaps")]
        public static void StoreAssetAsImage()
        {
            var selectedObject = UnityEditor.Selection.activeObject;
            if (selectedObject is TerrainData terrainData)
            {
                var alphamaps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapResolution,
                    terrainData.alphamapResolution);
                for (int i = 0; i < terrainData.alphamapLayers; i++)
                {
                    StoreLayer(alphamaps, selectedObject.name, i);
                }
            }
        }
#endif
        
        [Conditional("DEBUG_ALPHAMAPS")]
        private static void StoreLayer(float[,,] targetControlTextures, string name, int layerIndex)
        {
            var width = targetControlTextures.GetLength(0);
            var height = targetControlTextures.GetLength(1);
            var tex = new Texture2D(width, height,
                GraphicsFormat.R8G8B8A8_UNorm, TextureCreationFlags.None);

            Color[] colors = new Color[width * height];
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var intensity = targetControlTextures[x, y, layerIndex];
                    colors[y * width + x] = new Color(intensity, 0, 0, 1);
                }
            }
            
            tex.SetPixels(colors);
            var pngData = tex.EncodeToPNG();

            var folder = $"{Application.dataPath}\\PNG\\";

            if (!Directory.Exists(folder)) 
                Directory.CreateDirectory(folder);

            File.WriteAllBytes($"{folder}layer_{name}_{layerIndex}.png", pngData);
        }

    }
}