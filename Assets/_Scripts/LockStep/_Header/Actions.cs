using System.Collections;
using System.Collections.Generic;
using MessagePack;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_IOS
using UnityEngine;
#endif

public interface IAction
{
    void ProcessAction(bool logEnable);
}

[MessagePackObject]
public class NoAction : IAction
{
    public void ProcessAction(bool logEnable)
    {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_IOS
        if(logEnable)
            Debug.LogWarning("NoAction::ProcessAction");
#endif
    }
}

[MessagePackObject]
public class InputMoveAction : IAction
{
    [Key(0)]
    public float moveX;
    
    [Key(1)]
    public float moveY;
    
    public void ProcessAction(bool logEnable)
    {
        InputContext inputContext = Contexts.sharedInstance.input;
        InputEntity inputEntity = inputContext.CreateEntity();
        
        inputEntity.AddInputMove(moveX, moveY);
    }
}

[MessagePackObject]
public class InputActionAction : IAction
{
    [Key(0)]
    public CharacterAction characterAction;

    public void ProcessAction(bool logEnable)
    {
        InputContext inputContext = Contexts.sharedInstance.input;
        InputEntity inputEntity = inputContext.CreateEntity();
        
        inputEntity.AddInputAction(characterAction);
    }
}

// [MessagePackObject]
// public class DummyAction : IAction
// {
//     [Key(0)]
//     public string ActionString;
//     
//     public void ProcessAction(bool logEnable)
//     {
// #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_IOS
//         Debug.Log($"Dummy::ProcessAction {ActionString}");
// #endif
//     }
// }
