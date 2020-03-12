using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEditor;
using UnityEngine;


public class ImportEffectConfig
{
    public const string CONFIG_PATH = "Assets/JSONS/Editor-EffectConfig.json";
    public const string SAVE_PATH = "Assets/Res/Config/EffectConfig.asset";
    
    [MenuItem("Import/EffectConfig")]
    public static void MenuItem_ImportEffectConfig()
    {
        Import();
    }

    public static void Import()
    {
        string content = File.ReadAllText(CONFIG_PATH);

        var jsonArray = JsonMapper.ToObject(content);

        int length = jsonArray.Count;
        
        List<EffectConfig> list = new List<EffectConfig>(length);
        
        for (int i = 0; i < length; ++i)
        {
            var json = jsonArray[i];

            string effectName = (string) json["EffectName"];
            string animationPath = (string) json["Animation"];
            
            list.Add(new EffectConfig()
            {
                effectName = effectName,
                animationPath = animationPath,
            });
        }

        var configData = ScriptableObject.CreateInstance<EffectConfigData>();
        configData.list = list;

        AssetDatabase.CreateAsset(configData, SAVE_PATH);
        AssetDatabase.Refresh();
    }
}