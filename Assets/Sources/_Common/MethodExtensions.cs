using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MethodExtensions
{
   private static readonly Dictionary<ChacaterState, string> _characterStateDict = new Dictionary<ChacaterState, string>();
   
   public static string GetCacheString(this ChacaterState characterState)
   {
      string cacheString;

      if (_characterStateDict.TryGetValue(characterState, out cacheString))
      {
         return cacheString;
      }
      else
      {
         cacheString = characterState.ToString();
         _characterStateDict.Add(characterState, cacheString);

         return cacheString;
      }
   }
}
