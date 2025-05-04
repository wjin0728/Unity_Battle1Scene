using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditorInternal;
using Unity.VisualScripting;
using UnityEditor.TerrainTools;


public static class MeshRaycast
{
    public static bool RaycastMesh(Mesh mesh, Transform meshTransform, Ray ray, out RaycastHit hitInfo, float maxDist)
    {
        hitInfo = new RaycastHit();
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        float closestDist = float.MaxValue;
        bool hit = false;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            // 월드 좌표로 변환
            Vector3 v0 = meshTransform.TransformPoint(vertices[triangles[i]]);
            Vector3 v1 = meshTransform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 v2 = meshTransform.TransformPoint(vertices[triangles[i + 2]]);

            if (RayTriangleIntersect(ray, v0, v1, v2, out Vector3 hitPoint, out float dist))
            {
                if (dist < closestDist && dist <= maxDist)
                {
                    closestDist = dist;
                    hit = true;
                    hitInfo.point = hitPoint;
                    hitInfo.distance = dist;
                    hitInfo.normal = Vector3.Cross(v1 - v0, v2 - v0).normalized;
                }
            }
        }

        return hit;
    }

    private static bool RayTriangleIntersect(Ray ray, Vector3 v0, Vector3 v1, Vector3 v2, out Vector3 hitPoint, out float distance)
    {
        hitPoint = Vector3.zero;
        distance = 0f;

        const float EPSILON = 0.0000001f;
        Vector3 edge1 = v1 - v0;
        Vector3 edge2 = v2 - v0;
        Vector3 h = Vector3.Cross(ray.direction, edge2);
        float a = Vector3.Dot(edge1, h);
        if (a > -EPSILON && a < EPSILON) return false; // Ray 평행

        float f = 1.0f / a;
        Vector3 s = ray.origin - v0;
        float u = f * Vector3.Dot(s, h);
        if (u < 0.0 || u > 1.0) return false;

        Vector3 q = Vector3.Cross(s, edge1);
        float v = f * Vector3.Dot(ray.direction, q);
        if (v < 0.0 || u + v > 1.0) return false;

        float t = f * Vector3.Dot(edge2, q);
        if (t > EPSILON) // 충돌
        {
            hitPoint = ray.origin + ray.direction * t;
            distance = t;
            return true;
        }
        else return false;
    }
}


public class NavMaskGenerator : EditorWindow
{
    private Terrain terrain;
    private int resolution = 512;
    private LayerMask obstacleLayer;
    private Texture2D previewTexture;
    float brushSize = 5f;
    bool eraseMode = false;

    [MenuItem("Tools/NavMask Generator")]
    public static void ShowWindow()
    {
        GetWindow<NavMaskGenerator>("NavMask Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("NavMask 생성기", EditorStyles.boldLabel);
        terrain = (Terrain)EditorGUILayout.ObjectField("Terrain", terrain, typeof(Terrain), true);
        obstacleLayer = EditorGUILayout.MaskField("Obstacle Layer", obstacleLayer.value, InternalEditorUtility.layers);
        //obstacleLayer.value = Mathf.Clamp(obstacleLayer.value, 0, ~0); // 마스크 범위 제한
        brushSize = EditorGUILayout.Slider("Brush Size", brushSize, 1f, 50f);
        eraseMode = EditorGUILayout.Toggle("Erase Mode", eraseMode);

        if (GUILayout.Button("NavMask 생성"))
        {
            if (terrain != null)
                GenerateNavMask();
            else
                Debug.LogWarning("Terrain을 먼저 선택하세요.");
        }   

        if (previewTexture != null)
        {
            GUILayout.Label("미리보기:");
            GUILayout.Label(previewTexture, GUILayout.Width(256), GUILayout.Height(256));
        }

        if (previewTexture != null)
        {
            if (GUILayout.Button("PNG 저장"))
            {
                SaveAsPNG();
            }
            if (GUILayout.Button("RAW 저장"))
            {
                SaveAsRaw();
            }
        }
    }

    void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (previewTexture == null || terrain == null) return;

        Event e = Event.current;
        if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
        {
            if (e.button == 0 && Tools.current == Tool.None)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
                {
                    Vector3 localPos = hit.point - terrain.transform.position;
                    TerrainData data = terrain.terrainData;
                    Vector3 size = data.size;

                    int px = Mathf.FloorToInt(localPos.x / size.x * resolution);
                    int pz = Mathf.FloorToInt(localPos.z / size.z * resolution);

                    DrawCircleOnTexture(px, pz, brushSize, eraseMode ? (byte)255 : (byte)0);
                    e.Use();
                }
            }
        }

        SceneView.RepaintAll();
    }

    void DrawCircleOnTexture(int cx, int cz, float radius, byte value)
    {
        int r = Mathf.FloorToInt(radius / terrain.terrainData.size.x * resolution);

        for (int z = -r; z <= r; z++)
        {
            for (int x = -r; x <= r; x++)
            {
                int px = cx + x;
                int pz = cz + z;
                if (px >= 0 && px < resolution && pz >= 0 && pz < resolution)
                {
                    float dist = Mathf.Sqrt(x * x + z * z);
                    if (dist <= r)
                    {
                        previewTexture.SetPixel(px, pz, new Color32(value, value, value, 255));
                    }
                }
            }
        }

        previewTexture.Apply();
    }


    void GenerateNavMask()
    {
        TerrainData data = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;
        Vector3 terrainSize = data.size;
        resolution = data.heightmapResolution * 2;

        previewTexture = new Texture2D(resolution, resolution, TextureFormat.R8, false);
        previewTexture.wrapMode = TextureWrapMode.Clamp;

        float cellSizeX = terrainSize.x / resolution;
        float cellSizeZ = terrainSize.z / resolution;

        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float worldX = terrainPos.x + x * cellSizeX;
                float worldZ = terrainPos.z + z * cellSizeZ;
                float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));

                float rayY = worldY;

                Vector3 rayOriginDown = new Vector3(worldX, rayY + 10f, worldZ);
                Vector3 rayOriginUp = new Vector3(worldX, rayY - 2f, worldZ);
                Ray rayUp = new Ray(rayOriginUp, Vector3.up);
                Ray rayDown = new Ray(rayOriginDown, Vector3.down);
                bool blocked = false;

                bool blockedUp = Physics.Raycast(rayUp, out RaycastHit _, 6f, obstacleLayer);
                bool blockedDown = Physics.Raycast(rayDown, out RaycastHit _, 11f, obstacleLayer);
                blocked = blockedUp || blockedDown;

                byte value = (byte)(blocked ? 0 : 255);
                previewTexture.SetPixel(x, z, new Color32(value, value, value, 255));
            }
        }

        previewTexture.Apply();
        Debug.Log("NavMask 생성 완료!");
    }

    void SaveAsPNG()
    {
        string path = EditorUtility.SaveFilePanel("NavMask PNG 저장", "", "navMask.png", "png");
        if (!string.IsNullOrEmpty(path))
        {
            byte[] bytes = previewTexture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();
            Debug.Log("PNG 저장됨: " + path);
        }
    }

    void SaveAsRaw()
    {
        TerrainData data = terrain.terrainData;
        string name = data.name;
        string path = EditorUtility.SaveFilePanel("NavMask RAW 저장", "", name + "NavMask.raw", "raw");
        if (!string.IsNullOrEmpty(path))
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                for (int y = 0; y < previewTexture.height; y++)
                {
                    for (int x = 0; x < previewTexture.width; x++)
                    {
                        Color pixel = previewTexture.GetPixel(x, y);
                        byte value = (byte)(pixel.r * 255f);
                        writer.Write(value);
                    }
                }
            }

            AssetDatabase.Refresh();
            Debug.Log("RAW 저장됨: " + path);
        }
    }
}
