using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorRunConfigData : ScriptableObject
{
    public List<AnimatorRunConfig> list;
   
    private Dictionary<Character, AnimatorRunConfig> _dict;

    public void Initialize()
    {
        int length = list.Count;
        _dict = new Dictionary<Character, AnimatorRunConfig>(length);

        for (int i = 0; i < length; ++i)
        {
            AnimatorRunConfig config = list[i];

            var character = config.character;
         
            _dict.Add(character, config);
        }
    }
   
    public AnimatorRunConfig GetAnimatorRunConfig(Character character)
    {
        AnimatorRunConfig config;

        if (_dict.TryGetValue(character, out config))
        {
            return config;
        }
        else
        {
            Debug.LogError($"Not found GetAnimatorRunConfig: {character}");
            return null;
        }
    }
}

[Serializable]
public class AnimatorRunConfig
{
    public Character character;

    public bool singleRun;
    
    public string animatorUp;
    public Vector3 scaleUp;

    public string animatorRight;
    public Vector3 scaleRight;

    public string animatorDown;
    public Vector3 scaleDown;

    public string animatorLeft;
    public Vector3 scaleLeft;
}
