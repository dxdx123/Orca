//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class InputEntity {

    public InputMoveComponent inputMove { get { return (InputMoveComponent)GetComponent(InputComponentsLookup.InputMove); } }
    public bool hasInputMove { get { return HasComponent(InputComponentsLookup.InputMove); } }

    public void AddInputMove(float newX, float newY) {
        var index = InputComponentsLookup.InputMove;
        var component = (InputMoveComponent)CreateComponent(index, typeof(InputMoveComponent));
        component.x = newX;
        component.y = newY;
        AddComponent(index, component);
    }

    public void ReplaceInputMove(float newX, float newY) {
        var index = InputComponentsLookup.InputMove;
        var component = (InputMoveComponent)CreateComponent(index, typeof(InputMoveComponent));
        component.x = newX;
        component.y = newY;
        ReplaceComponent(index, component);
    }

    public void RemoveInputMove() {
        RemoveComponent(InputComponentsLookup.InputMove);
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

    static Entitas.IMatcher<InputEntity> _matcherInputMove;

    public static Entitas.IMatcher<InputEntity> InputMove {
        get {
            if (_matcherInputMove == null) {
                var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.InputMove);
                matcher.componentNames = InputComponentsLookup.componentNames;
                _matcherInputMove = matcher;
            }

            return _matcherInputMove;
        }
    }
}