using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteConfigData : ScriptableObject
{
   public List<SpriteConfig> list;
   
   private Dictionary<Character, string> _dict;

   public void Initialize()
   {
      int length = list.Count;
      _dict = new Dictionary<Character, string>(length);

      for (int i = 0; i < length; ++i)
      {
         SpriteConfig config = list[i];

         var character = config.character;
         
         _dict.Add(character, config.animationPath);
      }
   }
   
   public string GetSpriteConfig(Character character)
   {
      string animationPath;

      if (_dict.TryGetValue(character, out animationPath))
      {
         return animationPath;
      }
      else
      {
         Debug.LogError($"Not found SpriteConfig: {character}");
         return null;
      }
   }
}

[Serializable]
public class SpriteConfig
{
   public Character character;
   public string animationPath;
}
