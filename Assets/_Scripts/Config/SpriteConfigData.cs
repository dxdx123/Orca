using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteConfigData : ScriptableObject
{
   public List<SpriteConfig> list;
   
   private Dictionary<Character, Tuple<string, string>> _dict;

   public void Initialize()
   {
      int length = list.Count;
      _dict = new Dictionary<Character, Tuple<string, string>>(length);

      for (int i = 0; i < length; ++i)
      {
         SpriteConfig config = list[i];

         var character = config.character;
         var tuple = Tuple.Create(config.spritePath, config.animationPath);
         
         _dict.Add(character, tuple);
      }
   }
   
   public Tuple<string, string> GetSpriteConfig(Character character)
   {
      Tuple<string, string> tuple;

      if (_dict.TryGetValue(character, out tuple))
      {
         return tuple;
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
   public string spritePath;
   public string animationPath;
}
