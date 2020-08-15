using System;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class CellItem : EnhancedScrollerCellView
{
    private RawImage _rawImage;

    private string _path;
    
    private void Awake()
    {
        _rawImage = GetComponent<RawImage>();
    }

    public void SetTexture(string path)
    {
        Assert.IsNull(_path);
        Assert.IsNotNull(path);
        
        _path = path;

        _rawImage.enabled = false;
        ResourceManagerAsset.Instance.GetAssetAsset<Texture2D>(AssetWebRequestTextureLoader.Instance, path, this)
            .Then(texture =>
            {
                _rawImage.texture = texture;
                _rawImage.enabled = true;
            })
            .Catch(ex => Debug.LogException(ex));
    }

    public void DestroyTexture()
    {
        Assert.IsNotNull(_path);
        
        ResourceManagerAsset.Instance.DestroyAsset(_path, this);
        
        _rawImage.texture = null;
        _path = null;
    }
}
