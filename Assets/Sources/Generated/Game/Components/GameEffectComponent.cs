//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public EffectComponent effect { get { return (EffectComponent)GetComponent(GameComponentsLookup.Effect); } }
    public bool hasEffect { get { return HasComponent(GameComponentsLookup.Effect); } }

    public void AddEffect(string newEffectName) {
        var index = GameComponentsLookup.Effect;
        var component = (EffectComponent)CreateComponent(index, typeof(EffectComponent));
        component.effectName = newEffectName;
        AddComponent(index, component);
    }

    public void ReplaceEffect(string newEffectName) {
        var index = GameComponentsLookup.Effect;
        var component = (EffectComponent)CreateComponent(index, typeof(EffectComponent));
        component.effectName = newEffectName;
        ReplaceComponent(index, component);
    }

    public void RemoveEffect() {
        RemoveComponent(GameComponentsLookup.Effect);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherEffect;

    public static Entitas.IMatcher<GameEntity> Effect {
        get {
            if (_matcherEffect == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Effect);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherEffect = matcher;
            }

            return _matcherEffect;
        }
    }
}
