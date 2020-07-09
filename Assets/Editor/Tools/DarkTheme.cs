using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DarkTheme
{
    [MenuItem("EditorTools/DarkTheme")]
    public static void MenuItem_DarkTheme()
    {
        EditorPrefs.DeleteAll();
    }
}
