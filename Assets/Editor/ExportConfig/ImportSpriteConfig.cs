using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEditor;
using UnityEngine;


public class ImportSpriteConfig
{
    public const string CONFIG_PATH = "Assets/JSONS/Editor-SpriteConfig.json";
    public const string SAVE_PATH = "Assets/Res/Config/SpriteConfig.asset";
    
    [MenuItem("Import/SpriteConfig")]
    public static void MenuItem_ImportSpriteConfig()
    {
        Import();
    }

    public static void Import()
    {
        string content = File.ReadAllText(CONFIG_PATH);

        var jsonArray = JsonMapper.ToObject(content);

        int length = jsonArray.Count;
        
        List<SpriteConfig> list = new List<SpriteConfig>(length);
        
        for (int i = 0; i < length; ++i)
        {
            var json = jsonArray[i];

            Character character = EditorUtils.ParseEnum<Character>((string) json["Character"]);
            string spritePath = (string) json["Sprite"];
            string animationPath = (string) json["Animation"];
            
            list.Add(new SpriteConfig()
            {
                character = character,
                spritePath = spritePath,
                animationPath = animationPath,
            });
        }

        var configData = ScriptableObject.CreateInstance<SpriteConfigData>();
        configData.list = list;

        AssetDatabase.CreateAsset(configData, SAVE_PATH);
        AssetDatabase.Refresh();
    }
}