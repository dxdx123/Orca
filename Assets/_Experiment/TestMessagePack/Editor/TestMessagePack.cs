using System.Collections;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Resolvers;
using UnityEngine;
using UnityEditor;

public class TestMessagePack
{
    [MenuItem("Test/MessagePack")]
    public static void MenuItem_TestMessagePack()
    {
        Initialize();
        
        // InputMoveAction action = new InputMoveAction()
        // {
        //     moveX = 1.0f,
        //     moveY = 2.0f,
        // };
        //
        // var bytes = MessagePackSerializer.Serialize(action);
        //
        // InputMoveAction newObj = MessagePackSerializer.Deserialize<InputMoveAction>(bytes);
        //
        // Debug.Log($"{newObj.moveX} {newObj.moveY}");
        
        // InputActionAction action = new InputActionAction()
        // {
        //     characterAction = CharacterAction.LevelUp,
        // };
        // var bytes = MessagePackSerializer.Serialize(action);
        //
        // InputActionAction newObj = MessagePackSerializer.Deserialize<InputActionAction>(bytes);
        //
        // Debug.Log($"{newObj.characterAction}");
        
        IAction action = new NoAction();
        var bytes = MessagePackSerializer.Serialize(action as NoAction);
        
        Debug.Log(bytes.Length);
    }
    
    private static bool serializerRegistered = false;
    
    private static void Initialize()
    {
        if (!serializerRegistered)
        {
            StaticCompositeResolver.Instance.Register(
                MessagePack.Resolvers.GeneratedResolver.Instance,
                MessagePack.Resolvers.StandardResolver.Instance
            );

            var option = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);

            MessagePackSerializer.DefaultOptions = option;
            serializerRegistered = true;
        }
    }
    
}
