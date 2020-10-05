using System.Collections;
using System.Collections.Generic;

public static class MethodExtensions
{
   private static readonly Dictionary<CharacterState, string> _characterStateDict = new Dictionary<CharacterState, string>();
   
   public static string GetCacheString(this CharacterState characterState)
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
