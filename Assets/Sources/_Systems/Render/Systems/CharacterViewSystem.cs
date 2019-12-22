using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

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
      return true;
   }

   protected override void Execute(List<GameEntity> entities)
   {
      foreach (var e in entities)
      {
         var character = e.character.character;
         CreateCharacter(e, character);
      }
   }

   private void CreateCharacter(GameEntity e, Character character)
   {
      GameObject template = Resources.Load<GameObject>("Character_Template");
      GameObject gameObject = Object.Instantiate(template);
      tk2dSprite sprite = gameObject.GetComponent<tk2dSprite>();
      tk2dSpriteAnimator tk2dAnimator = gameObject.GetComponent<tk2dSpriteAnimator>();
      
      // Add Components
      e.AddTransform(gameObject.transform);

      ViewController viewController = gameObject.GetComponent<ViewController>();
      viewController.Initialize(e);
      
      e.AddView(viewController);
      e.AddFSM(viewController);

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
            
            tk2dAnimator.Play(CharacterState.Idle.GetCacheString());
         })
         .Catch(ex => Debug.LogException(ex))
         ;

   }
   
}
