using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ResourcesVisualWindow : EditorWindow
{
    [MenuItem("EditorTools/Resources Visual Window")]
    public static void MenuItem_ShowResourcesVisualWindow()
    {
        ResourcesVisualWindow window = GetWindow<ResourcesVisualWindow>("Resources Visual");
        window.Show();
    }

    private bool[] _showRefs;
    private HashSet<string> _foldOutSet = new HashSet<string>(); // which path is foldout open
    private Dictionary<string, List<Tuple<object, int>>> _rawDict;
    private Dictionary<string, bool> _diffDict;

    private Vector2 _scrollViewPos;
    
    private void Awake()
    {
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

    private void DrawContent()
    {
        if (_rawDict == null)
            return;
        
        _foldOutSet.Clear();
            
        int index = 0;
        foreach (KeyValuePair<string, List<Tuple<object, int>>> item in _rawDict)
        {
            string bundleName = item.Key;
            List<Tuple<object, int>> refList = item.Value;

            string abTitle = $"{bundleName} - ({refList.Count})";
            bool diff = _diffDict[bundleName];
            
            _showRefs[index] = EditorGUILayout.Foldout(_showRefs[index], abTitle, diff ? GenerateGUIStyle(Color.red) : GenerateGUIStyle(Color.white));

            if (_showRefs[index])
            {
                EditorGUI.indentLevel++;

                foreach (var obj in refList)
                {
                    EditorGUILayout.LabelField($"{obj.Item2.ToString()} - {obj.Item1.ToString()}");
                }

                EditorGUI.indentLevel--;

                _foldOutSet.Add(bundleName);
            }
            else
            {
                // nothing
            }

            ++index;
        }
    }
    
    private GUIStyle GenerateGUIStyle(Color color)
    {
        GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
        
        myFoldoutStyle.fontStyle = FontStyle.Bold;
        myFoldoutStyle.fontSize = 14;
        myFoldoutStyle.normal.textColor = color;
        myFoldoutStyle.onNormal.textColor = color;
        myFoldoutStyle.hover.textColor = color;
        myFoldoutStyle.onHover.textColor = color;
        myFoldoutStyle.focused.textColor = color;
        myFoldoutStyle.onFocused.textColor = color;
        myFoldoutStyle.active.textColor = color;
        myFoldoutStyle.onActive.textColor = color;

        return myFoldoutStyle;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        
        Events.instance.RemoveListener<ResourceRefIncreaseEvent>(OnResourceRefIncreaseEvent);
        Events.instance.RemoveListener<ResourceRefDecreaseEvent>(OnResourceRefDecreaseEvent);
    }

    private void SetupData()
    {
        Dictionary<string, List<System.Tuple<object, int>>> newDict = ResourceManagerAssetBundle.Instance.GetReferenceDict();

        _diffDict = GenerateDiffDict(_rawDict, newDict);
        
        _showRefs = GenerateShowRefs(newDict);

        _rawDict = newDict;
    }

    private bool[] GenerateShowRefs(Dictionary<string, List<Tuple<object, int>>> newDict)
    {
        int length = newDict.Count;
        bool[] showRefs = new bool[length];

        int index = 0;
        foreach (var item in newDict)
        {
            string bundleName = item.Key;

            showRefs[index] = _foldOutSet.Contains(bundleName);
            
            ++index;
        }

        return showRefs;
    }

    private Dictionary<string, bool> GenerateDiffDict(Dictionary<string, List<Tuple<object, int>>> oldDict, Dictionary<string, List<Tuple<object, int>>> newDict)
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
                List<Tuple<object, int>> newList = item.Value;

                List<Tuple<object, int>> oldList;
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

    private bool IsSameList(List<Tuple<object, int>> oldList, List<Tuple<object, int>> newList)
    {
        if (oldList != null && newList != null && oldList.Count == newList.Count)
        {
            int length = oldList.Count;

            for (int i = 0; i < length; ++i)
            {
                object oldObj = oldList[i].Item1;
                object newObj = newList[i].Item1;

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
