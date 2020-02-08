using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Xml.Linq;
using LitJson;
using UnityEngine.Assertions;

using Object = UnityEngine.Object;

public class EditorMapPacker : EditorWindow
{
    public const string PYTHON_SLICER = "Assets/Editor/Tools/MapSlicer.py";

    public const string TK2D_MAP_SHADER = "tk2d/Map";

    public const int MAP_WIDTH = 512;
    public const int MAP_HEIGHT = 512;

    [MenuItem("EditorTools/Map/Map Packer")]
    private static void MenuItem_MapPacker()
    {
        var window = GetWindow<EditorMapPacker>("Map Packer");
        window.Show();
    }

    private string _mapPath = "Assets/Temp/Map/";
    private string _slicePath = "Assets/Temp/Map_Slicer/";
    private string _collectionPath = "Assets/Res/_Map/";

    private void OnGUI()
    {
        string mapPath = EditorGUILayout.TextField("map path: ", _mapPath);
        if(mapPath != _mapPath)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture>(mapPath);
            if(texture)
            {
                _mapPath = mapPath;
            }
        }

        string slicePath = EditorGUILayout.TextField("slice path: ", _slicePath);
        if(slicePath != _slicePath)
        {
            _slicePath = slicePath.EndsWith("/") ? slicePath : slicePath + "/";
        }

        string collectionPath = EditorGUILayout.TextField("collection path: ", _collectionPath);
        if(collectionPath != _collectionPath)
        {
            _collectionPath = collectionPath.EndsWith("/") ? collectionPath : collectionPath + "/";
        }

        if(GUILayout.Button("Slice Map"))
        {
            SliceMap();

            AssetDatabase.Refresh();
        }

        if(GUILayout.Button("Gen Collection"))
        {
            GenCollection();

            AssetDatabase.Refresh();
        }

        this.Repaint();
    }

    private void GenCollection()
    {
        if (Directory.Exists(_collectionPath))
        {
            Directory.Delete(_collectionPath, true);
        }
        Directory.CreateDirectory(_collectionPath);
        AssetDatabase.Refresh();

        var directoryInfo = new DirectoryInfo(_slicePath);
        foreach (var directory in directoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly))
        {
            string mapName = directory.Name;

            GenCollectionInternal(_slicePath + mapName + "/", _collectionPath + mapName + "/");
        }
    }

    private void SliceMap()
    {
        Texture2D[] textures = EditorUtils.GetAtPath<Texture2D>(_mapPath);

        foreach (var texture in textures)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            string assetPath = EditorUtils.TrimAssetPath(path);

            SliceMapInternal(assetPath, MAP_WIDTH, MAP_HEIGHT, _slicePath + texture.name + "/");
        }
    }

    private void GenCollectionInternal(string slicePath, string collectionPath)
    {
        if (!Directory.Exists(collectionPath))
        {
            Directory.CreateDirectory(collectionPath);
        }

        Texture2D[] textures = EditorUtils.GetAtPath<Texture2D>(slicePath);
        foreach (var texture in textures)
        {
            GenCollectionInternal(collectionPath, texture);
        }
    }

    
    private tk2dSpriteCollection GenCollectionInternal(string collectionPath, Texture2D texture)
    {
        // 生成 tk2d prefab
        var collection = GenerateTk2dPrefab(collectionPath, texture);

        // 生成图集
        PackTextureToCollection(collection, texture);

        // 更改shader
        ChangeShader(collection);

        return collection;
    }

    private void ChangeShader(tk2dSpriteCollection collection)
    {
        string fullPath = AssetDatabase.GetAssetPath(collection);

        string parentDir = Path.GetDirectoryName(fullPath);
        string nameWidthoutExt = Path.GetFileNameWithoutExtension(fullPath);

        string materialPath = string.Format("{0}/{1} Data/atlas0 material.mat", parentDir, nameWidthoutExt);
        EditorUtils.ChangeShader(materialPath, TK2D_MAP_SHADER);

        AssetDatabase.Refresh();
    }

    private void PackTextureToCollection(tk2dSpriteCollection collection, Texture2D texture)
    {
        var gen = collection;
        gen.padAmount = 0;
        
        tk2dSpriteCollectionEditorPopup v = ScriptableObject.CreateInstance<tk2dSpriteCollectionEditorPopup>();
        v.SetGenerator(gen);
        
        Object[] objs = new Object[1] { texture };
        
        v.PackMap(objs);
        
        AssetDatabase.Refresh();
    }

    private tk2dSpriteCollection GenerateTk2dPrefab(string collectionPath, Texture2D texture)
    {
        string basePath = collectionPath.EndsWith("/") ? collectionPath.Substring(0, collectionPath.Length - 1) : collectionPath;

        return tk2dSpriteCollectionEditor.CreateSpriteCollection(basePath, texture.name);
    }

    private void SliceMapInternal(string mapPath, int mapWidth, int mapHeight, string slicePath)
    {
        Assert.IsNotNull(mapPath);
        Assert.IsNotNull(slicePath);

        Assert.IsTrue(File.Exists(mapPath));

        if(Directory.Exists(slicePath))
        {
            Directory.Delete(slicePath, true);
        }
        Directory.CreateDirectory(slicePath);
        AssetDatabase.Refresh();

        string commandFormat = "python3 {0} {1} {2} {3} {4}";
        string command = string.Format(commandFormat, PYTHON_SLICER, mapPath, mapWidth, mapHeight, slicePath);
        
        EditorUtils.ExecuteCommandSync(command);
    }
}

public class TexturePackerSprite
{
    public string n;

    public int x;
    public int y;
    public int w;
    public int h;

    public float pX;
    public float pY;

    // only available if trimmed
    public int oX;
    public int oY;
    public int oW;
    public int oH;

    // only set if sprite is rotated
    public bool r;

    // with polygon mode enabled
    public int[] vertices;
    public int[] verticesUV;
    public int[] triangles;

    public TexturePackerSprite(XElement element)
    {
        try
        {
            this.n = element.Attribute("n").Value;

            this.x = Convert.ToInt32(element.Attribute("x").Value);
            this.y = Convert.ToInt32(element.Attribute("y").Value);
            this.w = Convert.ToInt32(element.Attribute("w").Value);
            this.h = Convert.ToInt32(element.Attribute("h").Value);

            this.pX = Convert.ToSingle(element.Attribute("pX").Value);
            this.pY = Convert.ToSingle(element.Attribute("pY").Value);

            var oXElement = element.Attribute("oX");
            this.oX = oXElement != null ? Convert.ToInt32(oXElement.Value) : 0;

            var oYElement = element.Attribute("oY");
            this.oY = oYElement != null ? Convert.ToInt32(oYElement.Value) : 0;

            var oWElement = element.Attribute("oW");
            this.oW = oWElement != null ? Convert.ToInt32(oWElement.Value) : 0;

            var oHElement = element.Attribute("oH");
            this.oH = oHElement != null ? Convert.ToInt32(oHElement.Value) : 0;

            var rElement = element.Attribute("r");
            this.r = rElement != null && rElement.Value == "y";

            var verticesElement = element.Element("vertices");
            if (verticesElement != null)
            {
                string[] param = verticesElement.Value.Split(' ');

                this.vertices = new int[param.Length];
                for (int i = 0, length = param.Length; i < length; ++i)
                {
                    this.vertices[i] = Convert.ToInt32(param[i]);
                }
            }

            var verticesUVElement = element.Element("verticesUV");
            if (verticesUVElement != null)
            {
                string[] param = verticesUVElement.Value.Split(' ');

                this.verticesUV = new int[param.Length];
                for (int i = 0, length = param.Length; i < length; ++i)
                {
                    this.verticesUV[i] = Convert.ToInt32(param[i]);
                }
            }

            var trianglesElement = element.Element("triangles");
            if (trianglesElement != null)
            {
                string[] param = trianglesElement.Value.Split(' ');

                this.triangles = new int[param.Length];
                for (int i = 0, length = param.Length; i < length; ++i)
                {
                    this.triangles[i] = Convert.ToInt32(param[i]);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}

public class TexturePackerAtlas
{
    public string imagePath;
    public int width;
    public int height;

    public Dictionary<string, TexturePackerSprite> dicts = new Dictionary<string, TexturePackerSprite>();

    public TexturePackerAtlas(string path)
    {
        try
        {
            var document = XElement.Load(path);

            var atlasElement = document;

            this.imagePath = atlasElement.Attribute("imagePath").Value;
            this.width = Convert.ToInt32(atlasElement.Attribute("width").Value);
            this.height = Convert.ToInt32(atlasElement.Attribute("height").Value);

            var spriteElemnts = atlasElement.Elements("sprite");

            foreach (var spriteElemnt in spriteElemnts)
            {
                var texturePackerSprite = new TexturePackerSprite(spriteElemnt);

                dicts.Add(Path.GetFileNameWithoutExtension(texturePackerSprite.n), texturePackerSprite);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
