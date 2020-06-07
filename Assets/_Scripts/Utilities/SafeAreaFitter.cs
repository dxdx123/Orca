using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SafeAreaFitter : MonoBehaviour
{
    private RectTransform _rectTransform;

    private int _screenWidth;
    private int _screenHeight;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (_screenWidth != Screen.width || _screenHeight != Screen.height)
        {
            AdjustSafeArea();

            _screenWidth = Screen.width;
            _screenHeight = Screen.height;
        }
        else
        {
            // nothing
        }
    }

    private void AdjustSafeArea()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        Rect safeArea = Screen.safeArea;

        float left = safeArea.xMin / screenWidth;
        float bottom = safeArea.yMin / screenHeight;
        float right = safeArea.xMax / screenWidth;
        float top = safeArea.yMax / screenHeight;

        var min = new Vector2(left, bottom);
        var max = new Vector2(right, top);

        _rectTransform.anchorMin = min;
        _rectTransform.anchorMax = max;

        _rectTransform.offsetMin = Vector2.zero;
        _rectTransform.offsetMax = Vector2.zero;
    }
}
