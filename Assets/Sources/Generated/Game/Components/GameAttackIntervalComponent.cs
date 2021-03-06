//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public AttackIntervalComponent attackInterval { get { return (AttackIntervalComponent)GetComponent(GameComponentsLookup.AttackInterval); } }
    public bool hasAttackInterval { get { return HasComponent(GameComponentsLookup.AttackInterval); } }

    public void AddAttackInterval(float newInterval) {
        var index = GameComponentsLookup.AttackInterval;
        var component = (AttackIntervalComponent)CreateComponent(index, typeof(AttackIntervalComponent));
        component.interval = newInterval;
        AddComponent(index, component);
    }

    public void ReplaceAttackInterval(float newInterval) {
        var index = GameComponentsLookup.AttackInterval;
        var component = (AttackIntervalComponent)CreateComponent(index, typeof(AttackIntervalComponent));
        component.interval = newInterval;
        ReplaceComponent(index, component);
    }

    public void RemoveAttackInterval() {
        RemoveComponent(GameComponentsLookup.AttackInterval);
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

    static Entitas.IMatcher<GameEntity> _matcherAttackInterval;

    public static Entitas.IMatcher<GameEntity> AttackInterval {
        get {
            if (_matcherAttackInterval == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.AttackInterval);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherAttackInterval = matcher;
            }

            return _matcherAttackInterval;
        }
    }
}
