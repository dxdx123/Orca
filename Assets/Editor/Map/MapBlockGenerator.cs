using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

public class MapBlockGenerator
{
    public const float Mesh_Unit = 0.01f;

    private static string _mapBlockPath = "Assets/Temp/Map";

    [MenuItem("EditorTools/Map/Generate Map Block")]
    private static void MenuItem_GenerateMapBlock()
    {
        foreach (var file in new DirectoryInfo(_mapBlockPath).GetFiles("*.xml", SearchOption.TopDirectoryOnly))
        {
            string xmlPath = EditorUtils.TrimAssetPath(file.FullName);
            GenerateMapBlock(xmlPath);
        }
    }

    private static void GenerateMapBlock(string xmlPath)
    {
        var go = new GameObject();
        go.AddComponent<MeshRenderer>();

        var mf = go.AddComponent<MeshFilter>();
        if (mf.sharedMesh == null)
        {
            mf.sharedMesh = new Mesh();
        }

        Mesh mesh = mf.sharedMesh;
        try
        {
            TexturePackerAtlas atlas = new TexturePackerAtlas(xmlPath);

            string xmlName = Path.GetFileNameWithoutExtension(xmlPath);
            GenerateMesh(mesh, atlas, xmlName);

            string directoryPath = Path.GetDirectoryName(xmlPath);

            string assetPath = directoryPath + "/" + xmlName + ".asset";
            SaveMesh(mesh, false, true, assetPath);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        finally
        {
            // clean up
            Object.DestroyImmediate(go);
            AssetDatabase.Refresh();
        }
    }

    private static Vector3[] GetVertices(TexturePackerAtlas atlas, string mapName)
    {
        TexturePackerSprite sprite;
        if (!atlas.dicts.TryGetValue(mapName, out sprite))
        {
            throw new Exception("!!! GetVertices mapName not found: " + mapName);
        }

        int length = sprite.vertices.Length / 2;
        Vector3[] vertices = new Vector3[length];

        int height = sprite.h;

        for (int i = 0; i < length; ++i)
        {
            int x = sprite.vertices[2 * i + 0];
            int y = sprite.vertices[2 * i + 1];

            vertices[i] = new Vector3(x * Mesh_Unit, 0, (height - y) * Mesh_Unit);
        }

        return vertices;
    }

    private static Vector3[] GetNormals(TexturePackerAtlas atlas, string mapName)
    {
        TexturePackerSprite sprite;
        if (!atlas.dicts.TryGetValue(mapName, out sprite))
        {
            throw new Exception("!!! GetNormals mapName not found: " + mapName);
        }

        int length = sprite.vertices.Length / 2;

        Vector3[] normals = new Vector3[length];
        for (int i = 0; i < length; ++i)
        {
           normals[i] = new Vector3(0, 1, 0);
        }

        return normals;
    }

    private static Vector2[] GetUVS(TexturePackerAtlas atlas, string mapName)
    {
        TexturePackerSprite sprite;
        if (!atlas.dicts.TryGetValue(mapName, out sprite))
        {
            throw new Exception("!!! GetUVS mapName not found: " + mapName);
        }

        int length = sprite.verticesUV.Length / 2;
        Vector2[] uvs = new Vector2[length];

        float width = sprite.w;
        float height = sprite.h;
        for (int i = 0; i < length; ++i)
        {
            int x = sprite.verticesUV[2 * i + 0];
            int y = sprite.verticesUV[2 * i + 1];

            float uvX = x / width;
            float uvY = y / height;

            uvs[i] = new Vector2(uvX, uvY);
        }

        return uvs;
    }

    private static int[] GetTriangleIndices(TexturePackerAtlas atlas, string mapName)
    {
        TexturePackerSprite sprite;
        if (!atlas.dicts.TryGetValue(mapName, out sprite))
        {
            throw new Exception("!!! GetTriangleIndices mapName not found: " + mapName);
        }

        int[] triangleIndices = sprite.triangles;

        return triangleIndices;
    }

    private static void GenerateMesh(Mesh mesh, TexturePackerAtlas atlas, string mapName)
    {
        Vector3[] vertices = GetVertices(atlas, mapName);

        Vector3[] normals = GetNormals(atlas, mapName);

        Vector2[] uvs = GetUVS(atlas, mapName);

        int[] triangleIndices = GetTriangleIndices(atlas, mapName);

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangleIndices;
    }

    private static void SaveMesh(Mesh mesh, bool makeNewInstance, bool optimizeMesh, string path)
    {
        if (string.IsNullOrEmpty(path)) return;

        Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;

        if (optimizeMesh)
            MeshUtility.Optimize(meshToSave);

        AssetDatabase.CreateAsset(meshToSave, path);
        AssetDatabase.SaveAssets();
    }
}
