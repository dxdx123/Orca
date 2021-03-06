//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class InputEntity {

    public NetworkActionComponent networkAction { get { return (NetworkActionComponent)GetComponent(InputComponentsLookup.NetworkAction); } }
    public bool hasNetworkAction { get { return HasComponent(InputComponentsLookup.NetworkAction); } }

    public void AddNetworkAction(CharacterAction newCharacterAction) {
        var index = InputComponentsLookup.NetworkAction;
        var component = (NetworkActionComponent)CreateComponent(index, typeof(NetworkActionComponent));
        component.characterAction = newCharacterAction;
        AddComponent(index, component);
    }

    public void ReplaceNetworkAction(CharacterAction newCharacterAction) {
        var index = InputComponentsLookup.NetworkAction;
        var component = (NetworkActionComponent)CreateComponent(index, typeof(NetworkActionComponent));
        component.characterAction = newCharacterAction;
        ReplaceComponent(index, component);
    }

    public void RemoveNetworkAction() {
        RemoveComponent(InputComponentsLookup.NetworkAction);
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
public sealed partial class InputMatcher {

    static Entitas.IMatcher<InputEntity> _matcherNetworkAction;

    public static Entitas.IMatcher<InputEntity> NetworkAction {
        get {
            if (_matcherNetworkAction == null) {
                var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.NetworkAction);
                matcher.componentNames = InputComponentsLookup.componentNames;
                _matcherNetworkAction = matcher;
            }

            return _matcherNetworkAction;
        }
    }
}
