using System.Collections;
using System.Collections.Generic;
using RSG;
using UnityEngine;

public abstract class SimpleMenu<T> : Menu<T> where T : SimpleMenu<T>
{
    public static void Show()
    {
        Open(null);
    }

    public static IPromise<T> ShowWithPromise()
    {
        var promise = new Promise<T>();

        Open(promise);
        
        return promise;
    }

    public static void Hide()
    {
        Close();
    }
}
