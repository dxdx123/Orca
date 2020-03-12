using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectConfigData : ScriptableObject
{
   public List<EffectConfig> list;
   
   private Dictionary<string, string> _dict;

   public void Initialize()
   {
      int length = list.Count;
      _dict = new Dictionary<string, string>(length);

      for (int i = 0; i < length; ++i)
      {
         EffectConfig config = list[i];

         var effectName = config.effectName;
         
         _dict.Add(effectName, config.animationPath);
      }
   }
   
   public string GetEffectConfig(string effectName)
   {
      string animationPath;

      if (_dict.TryGetValue(effectName, out animationPath))
      {
         return animationPath;
      }
      else
      {
         Debug.LogError($"Not found EffectConfig: {effectName}");
         return null;
      }
   }
}

[Serializable]
public class EffectConfig
{
   public string effectName;
   public string animationPath;
}
