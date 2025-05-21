using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using UnityEditor.SearchService;
using Unity.VisualScripting;
using Unity.Mathematics;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UI;

public class MyBinaryWriter
{
    private BinaryWriter writer = null;

    public MyBinaryWriter(string filePath)
    {
        writer = new BinaryWriter(File.Open(filePath, FileMode.OpenOrCreate));
    }

    ~MyBinaryWriter()
    {
    }

    public void Close()
    {
        writer.Close();
    }

    public void WriteObjectName(Object obj)
    {
        writer.Write((obj) ? string.Copy(obj.name).Replace(" ", "_") : "null");
    }

    public void WriteObjectName(string strHeader, Object obj)
    {
        writer.Write(strHeader);
        writer.Write((obj) ? string.Copy(obj.name).Replace(" ", "_") : "null");
    }

    public void WriteString(string strToWrite)
    {
        writer.Write(strToWrite);
    }

    public void WriteString(string strToWrite, int i)
    {
        writer.Write(strToWrite);
        writer.Write(i);
    }

    public void WriteTextureName(string strHeader, Texture texture)
    {
        writer.Write(strHeader);
        if (texture)
        {
            writer.Write(string.Copy(texture.name).Replace(" ", "_"));
        }
        else
        {
            writer.Write("null");
        }
    }
    public void WriteTextureName(Texture texture)
    {
        if (texture)
        {
            writer.Write(string.Copy(texture.name).Replace(" ", "_"));
        }
        else
        {
            writer.Write("null");
        }
    }

    public void WriteInteger(int i)
    {
        writer.Write(i);
    }

    public void WriteInteger(string strHeader, int i)
    {
        writer.Write(strHeader);
        writer.Write(i);
    }

    public void WriteFloat(string strHeader, float f)
    {
        writer.Write(strHeader);
        writer.Write(f);
    }
    public void WriteFloat(float f)
    {
        writer.Write(f);
    }

    public void WriteVector(Vector2 v)
    {
        writer.Write(v.x);
        writer.Write(v.y);
    }

    public void WriteVector(Vector3 v)
    {
        writer.Write(v.x);
        writer.Write(v.y);
        writer.Write(v.z);
    }
    public void WriteVector(string strHeader, Vector3 v)
    {
        writer.Write(strHeader);
        writer.Write(v.x);
        writer.Write(v.y);
        writer.Write(v.z);
    }
    public void WriteVector(Vector4 v)
    {
        writer.Write(v.x);
        writer.Write(v.y);
        writer.Write(v.z);
        writer.Write(v.w);
    }
    public void WriteVector(Quaternion q)
    {
        writer.Write(q.x);
        writer.Write(q.y);
        writer.Write(q.z);
        writer.Write(q.w);
    }
    public void WriteColor(Color c)
    {
        writer.Write(c.r);
        writer.Write(c.g);
        writer.Write(c.b);
        writer.Write(c.a);
    }

    public void WriteColor(string strHeader, Color c)
    {
        writer.Write(strHeader);
        WriteColor(c);
    }

    public void WriteTextureCoord(Vector2 uv)
    {
        writer.Write(uv.x);
        writer.Write(1.0f - uv.y);
    }
    public void WriteVectors(string strHeader, Vector3[] vectors)
    {
        writer.Write(strHeader);
        writer.Write(vectors.Length);
        if (vectors.Length > 0) foreach (Vector3 v in vectors) WriteVector(v);
    }
    public void WriteColors(string strHeader, Color[] colors)
    {
        writer.Write(strHeader);
        writer.Write(colors.Length);
        if (colors.Length > 0) foreach (Color c in colors) WriteColor(c);
    }

    public void WriteTextureCoords(string strHeader, Vector2[] uvs)
    {
        writer.Write(strHeader);
        writer.Write(uvs.Length);
        if (uvs.Length > 0) foreach (Vector2 uv in uvs) WriteTextureCoord(uv);
    }
    public void WriteIntegers(string strHeader, int n, int[] pIntegers)
    {
        writer.Write(strHeader);
        writer.Write(n);
        writer.Write(pIntegers.Length);
        if (pIntegers.Length > 0) foreach (int i in pIntegers) writer.Write(i);
    }

    public void WriteBoundingBox(string strHeader, Bounds bounds)
    {
        writer.Write(strHeader);
        WriteVector(bounds.center);
        WriteVector(bounds.extents);
    }

    public void WriteMatrix(Matrix4x4 matrix)
    {
        writer.Write(matrix.m00);
        writer.Write(matrix.m10);
        writer.Write(matrix.m20);
        writer.Write(matrix.m30);
        writer.Write(matrix.m01);
        writer.Write(matrix.m11);
        writer.Write(matrix.m21);
        writer.Write(matrix.m31);
        writer.Write(matrix.m02);
        writer.Write(matrix.m12);
        writer.Write(matrix.m22);
        writer.Write(matrix.m32);
        writer.Write(matrix.m03);
        writer.Write(matrix.m13);
        writer.Write(matrix.m23);
        writer.Write(matrix.m33);
    }


    public void WriteVector(string strHeader, Quaternion q)
    {
        writer.Write(strHeader);
        WriteVector(q);
    }

    public void WriteTransform(string strHeader, Transform current)
    {
        writer.Write(strHeader);
        WriteVector(current.localPosition);
        WriteVector(current.localEulerAngles);
        WriteVector(current.localScale);
        WriteVector(current.localRotation);

        Matrix4x4 matrix = Matrix4x4.identity;
        matrix.SetTRS(current.localPosition, current.localRotation, current.localScale);
        WriteMatrix(matrix);
    }
    public void WriteTransform(Transform current)
    {
        WriteVector(current.localPosition);
        WriteVector(current.localEulerAngles);
        WriteVector(current.localScale);
        WriteVector(current.localRotation);

        Matrix4x4 matrix = Matrix4x4.identity;
        matrix.SetTRS(current.localPosition, current.localRotation, current.localScale);
        WriteMatrix(matrix);
    }


    public void WriteMeshInfo(Mesh mesh)
    {
        WriteInteger(mesh.vertexCount);
        WriteBoundingBox("<Bounds>:", mesh.bounds);

        if ((mesh.vertices != null) && (mesh.vertices.Length > 0)) WriteVectors("<Positions>:", mesh.vertices);
        if ((mesh.colors != null) && (mesh.vertices.Length > 0)) WriteColors("<Colors>:", mesh.colors);
        if ((mesh.uv != null) && (mesh.uv.Length > 0)) WriteTextureCoords("<TextureCoords0>:", mesh.uv);
        if ((mesh.uv2 != null) && (mesh.uv2.Length > 0)) WriteTextureCoords("<TextureCoords1>:", mesh.uv2);
        if ((mesh.normals != null) && (mesh.normals.Length > 0)) WriteVectors("<Normals>:", mesh.normals);

        if ((mesh.normals.Length > 0) && (mesh.tangents.Length > 0))
        {
            Vector3[] tangents = new Vector3[mesh.tangents.Length];
            Vector3[] biTangents = new Vector3[mesh.tangents.Length];
            for (int i = 0; i < mesh.tangents.Length; i++)
            {
                tangents[i] = new Vector3(mesh.tangents[i].x, mesh.tangents[i].y, mesh.tangents[i].z);
            }

            WriteVectors("<Tangents>:", tangents);
        }

        WriteInteger("<SubMeshes>:", mesh.subMeshCount);
        if (mesh.subMeshCount > 0)
        {
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                int[] subindicies = mesh.GetTriangles(i);
                WriteIntegers("<SubMesh>:", i, subindicies);
            }
        }

        WriteString("</Mesh>");
    }

    public void WriteSkinnedMeshInfo(Mesh skinnedMesh)
    {
        WriteMeshInfo(skinnedMesh);

        // 바인드 포즈 추출
        Matrix4x4[] bindPoses = skinnedMesh.bindposes;
        WriteInteger("<BindPoses>:", bindPoses.Length);
        Debug.Log($"바인드 포즈 개수 : {bindPoses.Length}");
        foreach (Matrix4x4 bp in bindPoses)
        {
            WriteMatrix(bp);
        }

        // 가중치 정보 추출
        BoneWeight[] boneWeights = skinnedMesh.boneWeights;
        WriteInteger("<BoneWeights>:", boneWeights.Length);
        foreach (BoneWeight bw in boneWeights)
        {
            WriteInteger(bw.boneIndex0); // 뼈대 인덱스 0
            WriteFloat(bw.weight0);      // 가중치 0
            WriteInteger(bw.boneIndex1); // 뼈대 인덱스 1
            WriteFloat(bw.weight1);      // 가중치 1
            WriteInteger(bw.boneIndex2); // 뼈대 인덱스 2
            WriteFloat(bw.weight2);      // 가중치 2
            WriteInteger(bw.boneIndex3); // 뼈대 인덱스 3
            WriteFloat(bw.weight3);      // 가중치 3
        }

        WriteString("</SkinnedMesh>");
    }

    public void WriteMaterial(Material material)
    {
        WriteObjectName(material);

        string shaderName = material.shader.name;

        WriteObjectName(material.shader);

        switch (shaderName)
        {
            case "SyntyStudios/Basic_LOD_Shader":

                WriteTextureName("<AlbedoMap>:", material.GetTexture("_Albedo"));
                WriteColor("<AlbedoColor>:", material.GetColor("_AlbedoColour"));

                WriteFloat("<Smoothness>:", material.GetFloat("_Smoothness"));

                WriteFloat("<Metallic>:", material.GetFloat("_Metallic"));

                WriteTextureName("<NormalMap>:", material.GetTexture("_NormalMap"));

                break;
            case "SyntyStudios/SkyboxUnlit":
                WriteColor("<TopColor>:", material.GetColor("_ColorTop"));
                WriteColor("<BottomColor>:", material.GetColor("_ColorBottom"));
                WriteFloat("<Offset>:", material.GetFloat("_Offset"));
                WriteFloat("<Distance>:", material.GetFloat("_Distance"));

                WriteFloat("<Falloff>:", material.GetFloat("_Falloff"));

                break;
            case "SyntyStudios/Triplanar01":
            case "SyntyStudios/TriplanarBasic":

                WriteTextureName("<SidesMap>:", material.GetTexture("_Sides"));
                WriteTextureName("<SidesNormalMap>:", material.GetTexture("_SidesNormal"));

                WriteTextureName("<TopMap>:", material.GetTexture("_Top"));
                WriteTextureName("<TopNormalMap>:", material.GetTexture("_TopNormal"));

                WriteFloat("<FallOff>:", material.GetFloat("_FallOff"));
                WriteFloat("<Tiling>:", material.GetFloat("_Tiling"));

                break;
            case "SyntyStudios/VegitationShader":
            case "SyntyStudios/VegitationShader_Basic":

                WriteTextureName("<LeafAlbedoMap>:", material.GetTexture("_LeafTex"));
                WriteTextureName("<LeafNormalMap>:", material.GetTexture("_LeafNormalMap"));
                WriteFloat("<LeafNormalScale>:", material.GetFloat("_LeafNormalAmount"));
                WriteColor("<LeafAlbedoColor>:", material.GetColor("_LeafBaseColour"));
                WriteFloat("<LeafSmoothness>:", material.GetFloat("_LeafSmoothness"));
                WriteFloat("<LeafMetallic>:", material.GetFloat("_LeafMetallic"));

                WriteTextureName("<TrunkAlbedoMap>:", material.GetTexture("_TunkTex"));
                WriteTextureName("<TrunkNormalMap>:", material.GetTexture("_TrunkNormalMap"));
                WriteFloat("<TrunkNormalScale>:", material.GetFloat("_TrunkNormalAmount"));
                WriteColor("<TrunkAlbedoColor>:", material.GetColor("_TrunkBaseColour"));
                WriteFloat("<TrunkSmoothness>:", material.GetFloat("_TrunkSmoothness"));
                WriteFloat("<TrunkMetallic>:", material.GetFloat("_TrunkMetallic"));

                break;
            case "Universal Render Pipeline/Lit":
                WriteFloat("<RenderMode>:", material.GetFloat("_Surface"));
                WriteTextureName("<AlbedoMap>:", material.GetTexture("_BaseMap"));
                WriteColor("<AlbedoColor>:", material.GetColor("_BaseColor"));

                WriteFloat("<Smoothness>:", material.GetFloat("_Smoothness"));
                WriteFloat("<Metallic>:", material.GetFloat("_Metallic"));

                WriteTextureName("<NormalMap>:", material.GetTexture("_BumpMap"));
                WriteTextureName("<EmissionMap>:", material.GetTexture("_EmissionMap"));
                WriteColor("<EmissionColor>:", material.GetColor("_EmissionColor"));

                break;
            case "SyntyStudios/WaterShader":
                WriteColor("<ShallowColour>:", material.GetColor("_ShallowColour"));
                WriteColor("<DeepColour>:", material.GetColor("_DeepColour"));
                WriteColor("<VeryDeepColour>:", material.GetColor("_VeryDeepColour"));
                WriteColor("<FoamColor>:", material.GetColor("_FoamColor"));
                WriteFloat("<Opacity>:", material.GetFloat("_Opacity"));
                WriteFloat("<Smoothness>:", material.GetFloat("_Smoothness"));
                WriteFloat("<FoamSmoothness>:", material.GetFloat("_FoamSmoothness"));
                WriteFloat("<FoamShoreline>:", material.GetFloat("_FoamShoreline"));
                WriteFloat("<FoamFalloff>:", material.GetFloat("_FoamFalloff"));
                WriteFloat("<FoamSpread>:", material.GetFloat("_FoamSpread"));
                WriteFloat("<OpacityFalloff>:", material.GetFloat("_OpacityFalloff"));
                WriteFloat("<OpacityMin>:", material.GetFloat("_OpacityMin"));
                WriteFloat("<ReflectionPower>:", material.GetFloat("_ReflectionPower"));
                WriteFloat("<Depth>:", material.GetFloat("_Depth"));
                WriteFloat("<NormalScale>:", material.GetFloat("_NormalScale"));
                WriteFloat("<NormalTiling>:", material.GetFloat("_NormalTiling"));
                WriteFloat("<NormalTiling2>:", material.GetFloat("_NormalTiling2"));
                WriteFloat("<RippleSpeed>:", material.GetFloat("_RippleSpeed"));
                WriteFloat("<ShallowFalloff>:", material.GetFloat("_ShallowFalloff"));
                WriteFloat("<OverallFalloff>:", material.GetFloat("_OverallFalloff"));

                WriteFloat("<WaveDirection>:", material.GetFloat("_WaveDirection"));
                WriteFloat("<WaveWavelength>:", material.GetFloat("_WaveWavelength"));
                WriteFloat("<WaveAmplitude>:", material.GetFloat("_WaveAmplitude"));
                WriteFloat("<WaveSpeed>:", material.GetFloat("_WaveSpeed"));
                WriteFloat("<WaveFoamOpacity>:", material.GetFloat("_WaveFoamOpacity"));
                WriteFloat("<WaveNoiseAmount>:", material.GetFloat("_WaveNoiseAmount"));
                WriteFloat("<WaveNoiseScale>:", material.GetFloat("_WaveNoiseScale"));

                WriteTextureName("<RipplesNormal>:", material.GetTexture("_RipplesNormal"));
                WriteTextureName("<RipplesNormal2>:", material.GetTexture("_RipplesNormal2"));
                WriteTextureName("<WaveMask>:", material.GetTexture("_WaveMask"));
                WriteTextureName("<FoamMask>:", material.GetTexture("_FoamMask"));


                break;
            default:
                return;
        }

        WriteString("</Material>");
    }

    public void WriteTerrainData(TerrainData terrainData)
    {
        WriteObjectName(terrainData);

        WriteInteger(terrainData.heightmapResolution);
        WriteVector(terrainData.size);

        int alphaMapCnt = terrainData.alphamapTextureCount;
        WriteInteger(alphaMapCnt);

        Texture2D[] alphaMaps = terrainData.alphamapTextures;

        int i = 0;
        foreach (Texture2D alphaMap in alphaMaps)
        {
            string name = terrainData.name + $"_splatmap_{i++}";
            WriteString(name);
        }
        TerrainLayer[] terrainLayers = terrainData.terrainLayers;
        int cnt = 0;
        WriteInteger(terrainLayers.Length);
        foreach (TerrainLayer terrainLayer in terrainLayers)
        {
            WriteTextureName(terrainLayer.diffuseTexture);
            WriteTextureName(terrainLayer.normalMapTexture);
            WriteFloat(terrainLayer.metallic);
            WriteFloat(terrainLayer.smoothness);
            cnt++;
        }
    }

    public Bounds WriteFrameInfo(Transform current)
    {
        string tag = current.tag;
        WriteObjectName("<Frame>:", current.gameObject);

        WriteString("<Tag>:");
        WriteString(tag);

        WriteTransform("<Transform>:", current);

        Bounds bounds = new Bounds();

        Renderer renderer = current.gameObject.GetComponent<Renderer>();
        if (renderer)
        {
            WriteString("<Renderer>:");
            MeshRenderer meshRenderer = current.gameObject.GetComponent<MeshRenderer>();
            MeshFilter meshFilter = current.gameObject.GetComponent<MeshFilter>();
            SkinnedMeshRenderer skinnedMeshRenderer = current.gameObject.GetComponent<SkinnedMeshRenderer>();
            CanvasRenderer canvasRenderer = current.gameObject.GetComponent<CanvasRenderer>();

            if (skinnedMeshRenderer)
            {
                WriteObjectName("<SkinnedMesh>:", skinnedMeshRenderer.sharedMesh);

                Transform[] bones = skinnedMeshRenderer.bones;

                WriteInteger(bones.Length);

                Debug.Log($"뼈 개수 : {bones.Length}");
                if (bones.Length > 0)
                {
                    foreach (Transform bone in bones)
                    {
                        WriteObjectName(bone);
                    }
                }
            }
            else if (meshRenderer && meshFilter)
            {
                WriteObjectName("<Mesh>:", meshFilter.sharedMesh);
            }
            bounds = renderer.bounds;

            Material[] materials = renderer.sharedMaterials;
            WriteInteger(materials.Length);
            if (materials.Length > 0)
            {
                foreach (Material mat in materials)
                {
                    WriteObjectName(mat);

                }
            }
        }

        Terrain terrain = current.GetComponent<Terrain>();
        if (terrain)
        {
            WriteString("<Terrain>:");
            WriteTerrain(terrain);
        }

        if (current.GetComponent<Animator>())
        {
            WriteString("<Animation>:");
            WriteAnimationInfo(current);
        }

        BoxCollider collider = new();
        if (current.TryGetComponent<BoxCollider>(out collider))
        {
            WriteString("<Collider>:");
            WriteVector(collider.center);
            WriteVector(collider.size);
        }

        Light light = new();
        if (current.TryGetComponent<Light>(out light))
        {
            WriteString("<Light>:");
            switch (light.type)
            {
                case LightType.Directional:
                    WriteInteger(0);
                    break;
                case LightType.Point:
                    WriteInteger(1);
                    break;
                case LightType.Spot:
                    WriteInteger(2);
                    break;   
            }
            WriteColor(light.color);
            WriteFloat(light.range);
            WriteFloat(light.intensity);
            WriteFloat(light.innerSpotAngle);
            WriteFloat(light.spotAngle);
        }
        Image image;
        if (current.TryGetComponent<Image>(out image))
        {
            WriteString("<Image>:");
            WriteTextureName(image.mainTexture);
            WriteColor(image.color);
            Vector2 canvasSize = new Vector2(1920, 1080);
            GameObject root = current.root.gameObject;
            Bounds rectBound = RectTransformUtility.CalculateRelativeRectTransformBounds(root.transform, current);
            WriteVector(new Vector2(rectBound.size.x, rectBound.size.y));
            WriteVector(rectBound.center/ (canvasSize / 2));
        }

        return bounds;
    }

    public Bounds WriteFrameHierarchyInfo(Transform child, bool isPrefabable)
    {
        Bounds bounds = new Bounds();
        if(child.gameObject.name == "Armature")
        {
            int total = child.GetComponentsInChildren<Transform>().Length - 1;
            Debug.Log($"뼈 개수 : {total}");
        }

        if (isPrefabable && PrefabUtility.IsOutermostPrefabInstanceRoot(child.gameObject))
        {
            GameObject prefabSource = (GameObject)PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject);
            WriteObjectName("<Prefab>:", prefabSource);
            WriteTransform(child);
            WriteString(child.tag);
        }
        else
        {
            bounds = WriteFrameInfo(child);
            int childCount = 0;
            for (int k = 0; k < child.childCount; k++)
            {
                if (child.GetChild(k).gameObject.activeSelf) childCount++;
            }
            WriteInteger("<Children>:", childCount);
            if (childCount > 0)
            {
                for (int k = 0; k < child.childCount; k++)
                {
                    if (!child.GetChild(k).gameObject.activeSelf) continue;
                    bounds.Encapsulate(WriteFrameHierarchyInfo(child.GetChild(k), isPrefabable));
                }
            }
            Camera camera = child.GetComponent<Camera>();
            WriteString("</Frame>");
        }

        return bounds;
    }

    public void WriteTerrain(Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;
        WriteTerrainData(terrainData);
        WriteVector(terrain.gameObject.transform.position);
    }

    public void WriteAnimationInfo(Transform current)
    {

        Animator animator = current.GetComponent<Animator>();
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                writer.Write(clip.name);
                break;
            }
        }
        else
        {
            Animation animation = current.GetComponent<Animation>();
            if (animation != null)
            {
                foreach (AnimationState state in animation)
                {
                    writer.Write(state.clip.name);
                    break;
                }
            }
        }
    }

    public void WriteObject(GameObject gameObject, bool isPrefabable)
    {
        Transform transform = gameObject.transform;
        Bounds combinedBounds = WriteFrameHierarchyInfo(transform, isPrefabable);

        Vector3 sphereCenter = transform.InverseTransformPoint(combinedBounds.center);
        float radius = combinedBounds.extents.magnitude;
        float maxScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
        radius /= maxScale;


        WriteVector(sphereCenter);
        WriteFloat(radius);

        float isStatic = 0;
        if (gameObject.isStatic)
        {
            isStatic = 1;
        }
        WriteFloat(isStatic);
    }
}

public class SingleObjectExporter
{
    [MenuItem("Scene/Export Single Object")]
    static void ExportSingleObject()
    {
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject == null)
        {
            Debug.LogError("No object selected.");
            return;

        }
        string filePath = EditorUtility.SaveFilePanel("Save Object bin", "Assets", selectedObject.name + ".bin", "bin");
        if (string.IsNullOrEmpty(filePath)) return;
        MyBinaryWriter binaryWriter = new MyBinaryWriter(filePath);
        Renderer[] renderers = selectedObject.GetComponentsInChildren<Renderer>();
        MeshRenderer[] meshRenderers = selectedObject.GetComponentsInChildren<MeshRenderer>();
        SkinnedMeshRenderer[] skinnedMeshRenderers = selectedObject.GetComponentsInChildren<SkinnedMeshRenderer>();

        List<Mesh> Meshes = new List<Mesh>();
        List<Mesh> SkinnedMeshes = new List<Mesh>();
        List<Material> materials = new List<Material>();

        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            Mesh mesh = meshRenderer.GetComponent<MeshFilter>().sharedMesh;
            if (!Meshes.Contains(mesh))
            {
                Meshes.Add(mesh);
            }
        }
        foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
        {
            Mesh mesh = skinnedMeshRenderer.sharedMesh;
            if (!SkinnedMeshes.Contains(mesh))
            {
                SkinnedMeshes.Add(mesh);
            }
        }

        foreach (Renderer renderer in renderers)
        {
            Material[] sharedMaterials = renderer.sharedMaterials;
            foreach (Material material in sharedMaterials)
            {
                if (!materials.Contains(material))
                {
                    materials.Add(material);
                }
            }
        }
        binaryWriter.WriteInteger(Meshes.Count);
        foreach (Mesh mesh in Meshes)
        {
            binaryWriter.WriteObjectName(mesh);
        }
        binaryWriter.WriteInteger(SkinnedMeshes.Count);
        foreach (Mesh mesh in SkinnedMeshes)
        {
            binaryWriter.WriteObjectName(mesh);
        }
        binaryWriter.WriteInteger(materials.Count);
        foreach (Material material in materials)
        {
            binaryWriter.WriteMaterial(material);
        }
        binaryWriter.WriteObject(selectedObject, false);
        binaryWriter.Close();
        Debug.Log($"Single object export completed: {selectedObject.name}");
    }
}

public class SceneObjectExporter
{
    

    [MenuItem("Scene/Export Scene")]
    static void ExportAllObjects()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        Dictionary<string, GameObject> prefabSources = new Dictionary<string, GameObject>();

        Terrain[] terrains = GameObject.FindObjectsOfType<Terrain>();

        string filePath = EditorUtility.SaveFilePanel("Save Scene bin", "Assets", "Scene.bin", "bin");
        if (string.IsNullOrEmpty(filePath)) return;

        MyBinaryWriter binaryWriter = new MyBinaryWriter(filePath);

        foreach (GameObject obj in allObjects)
        {
            if (!obj.activeSelf) continue;
            if (PrefabUtility.IsOutermostPrefabInstanceRoot(obj))
            {
                GameObject prefabSource = (GameObject)PrefabUtility.GetCorrespondingObjectFromSource(obj);
                if (!prefabSources.ContainsKey(prefabSource.name))
                {
                    prefabSources[prefabSource.name] = prefabSource;
                }
            }
        }
        foreach (Terrain terrain in terrains)
        {
            if (terrain.gameObject.activeSelf)
            {
                TerrainData terrainData = terrain.terrainData;
                if (terrainData != null)
                {
                    TreePrototype[] treePrototypes = terrainData.treePrototypes;
                    foreach (TreePrototype treePrototype in treePrototypes)
                    {
                        string terrainName = treePrototype.prefab.name;
                        if (!prefabSources.ContainsKey(terrainName))
                        {
                            prefabSources[terrainName] = treePrototype.prefab;  
                        }
                    }
                }
            }
        }

        binaryWriter.WriteInteger(prefabSources.Count);
        foreach (GameObject obj in prefabSources.Values)
        {
            binaryWriter.WriteObject(obj, false);
        }

        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        int activeRootCount = 0;
        foreach (GameObject root in rootObjects)
        {
            if (root.activeInHierarchy)
            {
                activeRootCount++;
            }
        }
        foreach (Terrain terrain in terrains)
        {
            if (terrain.gameObject.activeSelf)
            {
                TerrainData terrainData = terrain.terrainData;
                if (terrainData != null)
                {
                    activeRootCount += terrainData.treeInstanceCount;
                }
            }
        }

        binaryWriter.WriteInteger(activeRootCount);
        foreach (GameObject obj in rootObjects)
        {
            if (!obj.activeSelf) continue;
            binaryWriter.WriteObject(obj, true);
        }
        foreach (Terrain terrain in terrains)
        {
            if (terrain.gameObject.activeSelf)
            {
                TerrainData terrainData = terrain.terrainData;
                Vector3 terrainScale = terrainData.size;
                if (terrainData != null)
                {
                    int treeInstanceCnt = terrainData.treeInstanceCount;
                    if (treeInstanceCnt > 0)
                    {
                        TreeInstance[] treeInstances = terrainData.treeInstances;
                        foreach (TreeInstance treeInstance in treeInstances)
                        {
                            Vector3 position = Vector3.Scale(treeInstance.position, terrainScale);
                            GameObject prefab = terrainData.treePrototypes[treeInstance.prototypeIndex].prefab;
                            GameObject obj = GameObject.Instantiate(prefab, position, Quaternion.identity);
                            obj.transform.localScale = new Vector3(treeInstance.widthScale, treeInstance.heightScale, treeInstance.widthScale);
                            obj.transform.localRotation = Quaternion.Euler(0, treeInstance.rotation, 0);
                            obj.transform.localEulerAngles = new Vector3(0, treeInstance.rotation, 0);
                            if (obj != null)
                            {
                                binaryWriter.WriteObject(obj, true);
                            }
                            else
                            {
                                Debug.LogError("Prefab is null");
                            }
                            GameObject.DestroyImmediate(obj);
                        }
                    }
                }
            }
        }

        binaryWriter.Close();
        Debug.Log($"루트 오브젝트 개수 : {activeRootCount}");

        Debug.Log($"씬 내 모든 오브젝트 추출 완료 : {filePath}");

        Debug.Log($"씬 추출 완료");
    }

    [MenuItem("Scene/Export Objects Mesh")]
    static void ExportObjectsMesh()
    {
        // 현재 씬의 모든 GameObject 추출
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        Dictionary<string, Mesh> sceneMeshes = new Dictionary<string, Mesh>();
        Dictionary<string, Mesh> sceneSkinnedMeshes = new Dictionary<string, Mesh>();

        foreach (GameObject obj in allObjects)
        {
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                string meshName = meshFilter.sharedMesh.name;
                if (!sceneMeshes.ContainsKey(meshName))
                {
                    sceneMeshes[meshName] = meshFilter.sharedMesh;
                }
            }

            SkinnedMeshRenderer skinnedRenderer = obj.GetComponent<SkinnedMeshRenderer>();
            if (skinnedRenderer != null && skinnedRenderer.sharedMesh != null)
            {
                string meshName = skinnedRenderer.sharedMesh.name;
                if (!sceneSkinnedMeshes.ContainsKey(meshName))
                {
                    sceneSkinnedMeshes[meshName] = skinnedRenderer.sharedMesh;
                }
            }

            Terrain terrain = obj.GetComponent<Terrain>();
            if (terrain)
            {
                TerrainData terrainData = terrain.terrainData;
                int treeInstanceCnt = terrainData.treeInstanceCount;
                if (treeInstanceCnt > 0)
                {
                    TreePrototype[] treePrototypes = terrainData.treePrototypes;
                    foreach (TreePrototype treePrototype in treePrototypes)
                    {
                        string treeName = treePrototype.prefab.name;
                        if (!sceneMeshes.ContainsKey(treeName))
                        {
                            MeshFilter[] meshFilters = treePrototype.prefab.GetComponentsInChildren<MeshFilter>();
                            foreach (MeshFilter filter in meshFilters)
                            {
                                if (filter != null && filter.sharedMesh != null)
                                {
                                    string meshName = filter.sharedMesh.name;
                                    if (!sceneMeshes.ContainsKey(meshName))
                                    {
                                        sceneMeshes[meshName] = filter.sharedMesh;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        int total = sceneMeshes.Count;
        int current = 0;
        foreach (Mesh mesh in sceneMeshes.Values)
        {
            current++;

            string filePath = "./Meshes/" + string.Copy(mesh.name).Replace(" ", "_") + ".bin";
            if (!string.IsNullOrEmpty(filePath) && !File.Exists(filePath))
            {
                MyBinaryWriter writer = new(filePath);
                writer.WriteMeshInfo(mesh);

                writer.Close();
            }
            EditorUtility.DisplayProgressBar("Exporting Meshes", $"Processing {current}/{total}", (float)current / total);

        }
        EditorUtility.ClearProgressBar();

        total = sceneSkinnedMeshes.Count;
        current = 0;
        foreach (Mesh mesh in sceneSkinnedMeshes.Values)
        {
            current++;
            string filePath = "./Meshes/" + string.Copy(mesh.name).Replace(" ", "_") + ".bin";
            if (!string.IsNullOrEmpty(filePath) && !File.Exists(filePath))
            {
                MyBinaryWriter writer = new(filePath);
                writer.WriteSkinnedMeshInfo(mesh);
                writer.Close();
            }
            EditorUtility.DisplayProgressBar("Exporting Skinned Meshes", $"Processing {current}/{total}", (float)current / total);
        }
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("Scene/Export Scene Resources")]
    static void ExportSceneResources()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        string filePath = EditorUtility.SaveFilePanel("Save Scene Resources bin", "Assets", "SceneResources.bin", "bin");
        if (string.IsNullOrEmpty(filePath)) return;
        MyBinaryWriter binaryWriter = new MyBinaryWriter(filePath);
        ExportResources(allObjects, binaryWriter);
        binaryWriter.Close();
    }

    static void ExportResources(GameObject[] allObjects, MyBinaryWriter writer)
    {
        Dictionary<string, Mesh> sceneMeshes = new Dictionary<string, Mesh>();
        Dictionary<string, Material> sceneMaterials = new Dictionary<string, Material>();
        Dictionary<string, Mesh> sceneSkinnedMeshes = new Dictionary<string, Mesh>();
        List<string> images = new List<string>();

        //씬 내 공유 리소스 탐색
        foreach (GameObject obj in allObjects)
        {
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                string meshName = meshFilter.sharedMesh.name;
                if (!sceneMeshes.ContainsKey(meshName))
                {
                    sceneMeshes[meshName] = meshFilter.sharedMesh;
                }
            }
            SkinnedMeshRenderer skinnedRenderer = obj.GetComponent<SkinnedMeshRenderer>();
            if (skinnedRenderer != null && skinnedRenderer.sharedMesh != null)
            {
                string meshName = skinnedRenderer.sharedMesh.name;
                if (!sceneSkinnedMeshes.ContainsKey(meshName))
                {
                    sceneSkinnedMeshes[meshName] = skinnedRenderer.sharedMesh;
                }
            }

            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer)
            {
                Material[] materials = renderer.sharedMaterials;
                foreach (Material material in materials)
                {
                    string matName = material.name;
                    if (!sceneMaterials.ContainsKey(matName))
                    {
                        sceneMaterials[matName] = material;
                    }
                }
            }

            Terrain terrain = obj.GetComponent<Terrain>();
            if (terrain)
            {
                TerrainData terrainData = terrain.terrainData;
                int treeInstanceCnt = terrainData.treeInstanceCount;
                if (treeInstanceCnt > 0)
                {
                    TreePrototype[] treePrototypes = terrainData.treePrototypes;
                    foreach (TreePrototype treePrototype in treePrototypes)
                    {
                        string treeName = treePrototype.prefab.name;
                        MeshFilter[] meshFilters = treePrototype.prefab.GetComponentsInChildren<MeshFilter>();
                        foreach (MeshFilter filter in meshFilters)
                        {
                            if (filter != null && filter.sharedMesh != null)
                            {
                                string meshName = filter.sharedMesh.name;
                                if (!sceneMeshes.ContainsKey(meshName))
                                {
                                    sceneMeshes[meshName] = filter.sharedMesh;
                                }
                            }
                        }
                        Renderer[] renderers = treePrototype.prefab.GetComponentsInChildren<Renderer>();
                        foreach (Renderer childRenderer in renderers)
                        {
                            Material[] materials = childRenderer.sharedMaterials;
                            foreach (Material material in materials)
                            {
                                string matName = material.name;
                                if (!sceneMaterials.ContainsKey(matName))
                                {
                                    sceneMaterials[matName] = material;
                                }
                            }
                        }
                    }
                }
            }

            Image image = obj.GetComponent<Image>();
            if (image)
            {
                Texture texture = image.mainTexture;
                if (texture != null)
                {
                    string texName = texture.name;
                    if (!images.Contains(texName))
                    {
                        images.Add(texName);
                    }
                }
            }
        }
        //리소스 추출
        writer.WriteInteger(sceneMeshes.Count);
        foreach (Mesh mesh in sceneMeshes.Values)
        {
            writer.WriteObjectName(mesh);
        }
        writer.WriteInteger(sceneSkinnedMeshes.Count);
        foreach (Mesh mesh in sceneSkinnedMeshes.Values)
        {
            writer.WriteObjectName(mesh);
        }

        writer.WriteInteger(sceneMaterials.Count);
        foreach (Material material in sceneMaterials.Values)
        {
            writer.WriteMaterial(material);
        }

        writer.WriteInteger(images.Count);
        foreach (string image in images)
        {
            writer.WriteString(image);
        }


        Debug.Log($"리소스 추출 완료");
    }

    [MenuItem("Scene/Export Canvas")]
    static void ExportSelectedCanvas()
    {
        GameObject selectedObject = Selection.activeGameObject;
        Canvas canvas = selectedObject.GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No UI Object selected.");
            return;
        }
        string filePath = EditorUtility.SaveFilePanel("Save Canvas bin", "Assets", selectedObject.name + ".bin", "bin");
        if (string.IsNullOrEmpty(filePath)) return;
        MyBinaryWriter binaryWriter = new MyBinaryWriter(filePath);

        binaryWriter.WriteObjectName(canvas);
        binaryWriter.WriteObject(selectedObject, false);

        Debug.Log($"UI 추출 완료");
    }
}