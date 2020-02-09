using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;
using UnityEngine.Assertions;

public class CharacterViewSystem : ReactiveSystem<GameEntity>
{
   private GameContext _gameContext;
   
   public CharacterViewSystem(Contexts contexts)
      : base(contexts.game)
   {
      _gameContext = contexts.game;
   }

   protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
   {
      return context.CreateCollector(GameMatcher.Character.Added());
   }

   protected override bool Filter(GameEntity entity)
   {
      return entity.hasCharacter && entity.hasPosition;
   }

   protected override void Execute(List<GameEntity> entities)
   {
      foreach (var e in entities)
      {
         var character = e.character.character;
         var position = e.position;
         
         CreateCharacter(e, character, position.x, position.y);
      }
   }

   private void CreateCharacter(GameEntity e, Character character, float x, float y)
   {
      GameObject gameObject = AssetPoolManager.Instance.SpawnCharacter();
      gameObject.transform.position = new Vector3(x, y, 0);
      
      tk2dSprite sprite = gameObject.GetComponent<tk2dSprite>();
      tk2dSpriteAnimator tk2dAnimator = gameObject.GetComponent<tk2dSpriteAnimator>();
      
      var spriteTuple = _gameContext.config.spriteConfig.GetSpriteConfig(character);
      string collectionDataPath = spriteTuple.Item1;
      ResourceManager.Instance.GetAsset<GameObject>(collectionDataPath, this)
         .Then(spriteCollectionGo =>
         {
            var collectionData = spriteCollectionGo.GetComponent<tk2dSpriteCollectionData>();
            sprite.Collection = collectionData;

            string animPath = spriteTuple.Item2;
            return ResourceManager.Instance.GetAsset<GameObject>(animPath, this);
         })
         .Then(spriteAnimationGo =>
         {
            tk2dSpriteAnimation spriteAnimation = spriteAnimationGo.GetComponent<tk2dSpriteAnimation>();
            tk2dAnimator.Library = spriteAnimation;

            // Add Components
            e.AddTransform(gameObject.transform);

            CharacterViewController characterViewController = gameObject.GetComponent<CharacterViewController>();
            characterViewController.Initialize(e);

            e.AddView(characterViewController);
            e.AddFSM(characterViewController);
         })
         .Catch(ex => Debug.LogException(ex))
         ;

   }
   
}
