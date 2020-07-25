using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ResourcesVisualWindow : EditorWindow
{
    [MenuItem("Tools/Resources Visual Window")]
    public static void MenuItem_ShowResourcesVisualWindow()
    {
        ResourcesVisualWindow window = GetWindow<ResourcesVisualWindow>("Resources Visual");
        window.Show();
    }

    private bool[] _showRefs;
    private Dictionary<string, List<object>> _rawDict;
    private Dictionary<string, bool> _diffDict;

    private Vector2 _scrollViewPos;

    private GUIStyle _normalStyle;
    private GUIStyle _redStyle;
    
    private void Awake()
    {
        _normalStyle = new GUIStyle();
        _normalStyle.normal.textColor = Color.white;
        _normalStyle.hover.textColor = Color.white;
        
        _redStyle = new GUIStyle();
        _redStyle.normal.textColor = Color.white;
        _redStyle.hover.textColor = Color.white;
        
        SetupData();
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        
        Events.instance.AddListener<ResourceRefIncreaseEvent>(OnResourceRefIncreaseEvent);
        Events.instance.AddListener<ResourceRefDecreaseEvent>(OnResourceRefDecreaseEvent);
    }

    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        ResetData();
        
        Repaint();
    }

    private void OnResourceRefIncreaseEvent(ResourceRefIncreaseEvent e)
    {
        SetupData();
        Repaint();
    }

    private void OnResourceRefDecreaseEvent(ResourceRefDecreaseEvent e)
    {
        SetupData();
        Repaint();
    }

    private void OnGUI()
    {
        _scrollViewPos = EditorGUILayout.BeginScrollView(_scrollViewPos);
        
        EditorGUILayout.LabelField("AssetBundles:");
        EditorGUI.indentLevel = 1;

        DrawContent();

        EditorGUILayout.EndScrollView();
    }

    // TODO:: Refractory me
    private void DrawContent()
    {
        if (_rawDict == null)
            return;
            
        int index = 0;
        foreach (KeyValuePair<string, List<object>> item in _rawDict)
        {
            string bundleName = item.Key;
            List<object> refList = item.Value;

            string abTitle = $"{bundleName} - ({refList.Count})";

            // ugly
            bool diff = _diffDict[bundleName];
            var oldColor = GUI.color;

            if (diff)
            {
                GUI.color = Color.red;
            }
            else
            {
                GUI.color = oldColor;
            }

            _showRefs[index] = EditorGUILayout.Foldout(_showRefs[index], abTitle);
            
            GUI.color = oldColor;

            if (_showRefs[index])
            {
                EditorGUI.indentLevel++;

                foreach (var obj in refList)
                {
                    EditorGUILayout.LabelField($"* {obj.ToString()}");
                }

                EditorGUI.indentLevel--;
            }

            ++index;
        }
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        
        Events.instance.RemoveListener<ResourceRefIncreaseEvent>(OnResourceRefIncreaseEvent);
        Events.instance.RemoveListener<ResourceRefDecreaseEvent>(OnResourceRefDecreaseEvent);
    }

    private void SetupData()
    {
        Dictionary<string, List<object>> newDict = ResourceManagerAssetBundle.Instance.GetReferenceDict();

        _diffDict = GenerateDiffDict(_rawDict, newDict);
        
        int length = newDict.Count;
        _showRefs = new bool[length];

        _rawDict = newDict;
    }

    private Dictionary<string, bool> GenerateDiffDict(Dictionary<string, List<object>> oldDict, Dictionary<string, List<object>> newDict)
    {
        var dict = new Dictionary<string, bool>(newDict.Count);

        if (oldDict == null)
        {
            foreach (var item in newDict)
            {
                string assetBundleName = item.Key;
                dict.Add(assetBundleName, true);
            }
        }
        else
        {
            foreach (var item in newDict)
            {
                string assetBundleName = item.Key;
                List<object> newList = item.Value;

                List<object> oldList;
                if (oldDict.TryGetValue(assetBundleName, out oldList))
                {
                    bool same = IsSameList(oldList, newList);
                    
                    dict.Add(assetBundleName, !same);
                }
                else
                {
                    dict.Add(assetBundleName, true);
                }
            }
                 
        }

        return dict;
    }

    private bool IsSameList(List<object> oldList, List<object> newList)
    {
        if (oldList != null && newList != null && oldList.Count == newList.Count)
        {
            int length = oldList.Count;

            for (int i = 0; i < length; ++i)
            {
                object oldObj = oldList[i];
                object newObj = newList[i];

                if (!oldObj.Equals(newObj))
                {
                    return false;
                }
                else
                {
                    // test for next
                }
            }

            return true;
        }
        else
        {
            if (oldList == null && newList == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private void ResetData()
    {
        _rawDict = null;
        _showRefs = null;
    }

    private void OnDestroy()
    {
    }
}
