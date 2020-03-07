using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;

public class ImportAnimatorRunConfig
{
    public const string CONFIG_PATH = "Assets/JSONS/Editor-AnimatorRunConfig.json";
    public const string SAVE_PATH = "Assets/Res/Config/AnimatorRunConfig.asset";
    
    [MenuItem("Import/AnimatorRunConfig")]
    public static void MenuItem_ImportAnimatorRunConfig()
    {
        Import();
    }

    public static void Import()
    {
        string content = File.ReadAllText(CONFIG_PATH);

        var jsonArray = JsonMapper.ToObject(content);

        int length = jsonArray.Count;
        
        List<AnimatorRunConfig> list = new List<AnimatorRunConfig>(length);
        
        for (int i = 0; i < length; ++i)
        {
            var json = jsonArray[i];

            Character character = EditorUtils.ParseEnum<Character>((string) json["Character"]);
            bool singleRun = (bool) json["singleRun"];

            string animatorUp = (string) json["AnimatorUp"];
            Vector3 scaleUp = ParseScale(json["ScaleUp"]);
            
            string animatorRight = (string) json["AnimatorRight"];
            Vector3 scaleRight = ParseScale(json["ScaleRight"]);
            
            string animatorDown = (string) json["AnimatorDown"];
            Vector3 scaleDown = ParseScale(json["ScaleDown"]);
            
            string animatorLeft = (string) json["AnimatorLeft"];
            Vector3 scaleLeft = ParseScale(json["ScaleLeft"]);
            
            list.Add(new AnimatorRunConfig()
            {
                character = character,
                
                singleRun = singleRun,
                
                animatorUp = animatorUp,
                scaleUp =  scaleUp,
                
                animatorRight = animatorRight,
                scaleRight =  scaleRight,
                
                animatorDown = animatorDown,
                scaleDown =  scaleDown,
                
                animatorLeft = animatorLeft,
                scaleLeft =  scaleLeft,
                
            });
        }

        var configData = ScriptableObject.CreateInstance<AnimatorRunConfigData>();
        configData.list = list;

        AssetDatabase.CreateAsset(configData, SAVE_PATH);
        AssetDatabase.Refresh();
    }

    private static Vector3 ParseScale(JsonData jsonData)
    {
        Assert.IsTrue(jsonData.Count == 3);

        Vector3 scale = Vector3.one;
        
        scale.x = Convert.ToSingle(jsonData[0].IsDouble ? (double) jsonData[0] : (int)jsonData[0]);
        scale.y = Convert.ToSingle(jsonData[0].IsDouble ? (double) jsonData[1] : (int)jsonData[1]);
        scale.z = Convert.ToSingle(jsonData[0].IsDouble ? (double) jsonData[2] : (int)jsonData[2]);

        return scale;
    }
}
