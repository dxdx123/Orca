//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public ViewEventTriggerComponent viewEventTrigger { get { return (ViewEventTriggerComponent)GetComponent(GameComponentsLookup.ViewEventTrigger); } }
    public bool hasViewEventTrigger { get { return HasComponent(GameComponentsLookup.ViewEventTrigger); } }

    public void AddViewEventTrigger(string newEventName, string newEventInfo, float newEventFloat, int newEventInt) {
        var index = GameComponentsLookup.ViewEventTrigger;
        var component = (ViewEventTriggerComponent)CreateComponent(index, typeof(ViewEventTriggerComponent));
        component.eventName = newEventName;
        component.eventInfo = newEventInfo;
        component.eventFloat = newEventFloat;
        component.eventInt = newEventInt;
        AddComponent(index, component);
    }

    public void ReplaceViewEventTrigger(string newEventName, string newEventInfo, float newEventFloat, int newEventInt) {
        var index = GameComponentsLookup.ViewEventTrigger;
        var component = (ViewEventTriggerComponent)CreateComponent(index, typeof(ViewEventTriggerComponent));
        component.eventName = newEventName;
        component.eventInfo = newEventInfo;
        component.eventFloat = newEventFloat;
        component.eventInt = newEventInt;
        ReplaceComponent(index, component);
    }

    public void RemoveViewEventTrigger() {
        RemoveComponent(GameComponentsLookup.ViewEventTrigger);
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

    static Entitas.IMatcher<GameEntity> _matcherViewEventTrigger;

    public static Entitas.IMatcher<GameEntity> ViewEventTrigger {
        get {
            if (_matcherViewEventTrigger == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.ViewEventTrigger);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherViewEventTrigger = matcher;
            }

            return _matcherViewEventTrigger;
        }
    }
}